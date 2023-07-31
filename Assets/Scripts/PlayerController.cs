using static EnumTypes;
using UnityEngine;
using System.Collections;

public struct PlayerStatus
{
    public float Hp;
    public int AttackPower;
    public float AttackRate;
    public float AttackRange;
    public float SkillAttackRange;
    public int RequireEXP4LvUp;
}

public class PlayerController : StateMachine
{
    [SerializeField] PlayerData myStatus;          // �÷��̾��� �⺻ ���� ����(Scriptable Object)
    [SerializeField] private PlayerStatus status;  // �⺻ ���������� ����ü�� �޾Ƶеڿ� ������ ������ ��ġ ����
    [SerializeField] private GameObject healEff;   // �� ����Ʈ
    [SerializeField] private GameObject skillEff;  // ��ų ����Ʈ
    [SerializeField] private GameObject hitEff;    // �ǰ� ����Ʈ
                                                    
    private AttackPattern curAttackPattern;        // ���� ���ݻ���
    private Animator myAnim;                       // �ִϸ����� ������Ʈ
    private WaitForSeconds attackRate;             // ���ݼӵ�
    private float curHp;                           // ���� ü��
    private float curExp;                          // ���� EXP
    private bool isAttacking;                      // ���� ������
    private bool isRotateDone;                     // ȸ���� ��������
    private int curLevel;                          // ���� ����
    private int monsterLayerMask;                  // ������ ���̾��ũ

    protected override void Start()
    {
        InitStatus();
        InitPlayer();
        UIManager.Instacne.UpdateLevel(curLevel);
        // 0.5�� �ֱ�� Ÿ���� ������Ʈ�Ѵ�
        InvokeRepeating(nameof(UpdateTarget), 0f, 0.5f);
        base.Start();
    }

    // ���� ����ü�� ��ũ���ͺ� ������Ʈ(�ʱ� ����)������ ����
    private void InitStatus()
    {
        status.Hp = myStatus.Hp;
        status.AttackPower = myStatus.AttackPower;
        status.AttackRate = myStatus.AttackRate;
        status.AttackRange = myStatus.AttackRange;
        status.SkillAttackRange = myStatus.SkillAttackRange;
        status.RequireEXP4LvUp = myStatus.RequireEXP4LvUp;
    }


    // �÷��̾� �ʱ�ȭ
    private void InitPlayer()
    {
        // ���� �ӵ��� ��ٸ��� ���� WaitForSeconds ��ü ����
        attackRate = new WaitForSeconds(1 / status.AttackRate);
        // �÷��̾��� �ִϸ����� ������Ʈ ã��
        myAnim = GetComponent<Animator>();
        // ��, ��ų, �ǰ� ����Ʈ ��Ȱ��ȭ
        healEff.SetActive(false);
        skillEff.SetActive(false);
        hitEff.SetActive(false);
        // �ʱ� ���� ������ �⺻ �������� ����
        curAttackPattern = AttackPattern.BASIC;
        // ���� ���� �ƴ����� �ʱ�ȭ
        isAttacking = false;
        // �ʱ� ������ 1�� ����
        curLevel = 1;
        // ���� ü���� �ִ� ü������ �ʱ�ȭ
        curHp = status.Hp;
        // ���� ����ġ �ʱ�ȭ
        curExp = 0;
        // "Monster" ���̾ ���� LayerMask ����
        monsterLayerMask = 1 << LayerMask.NameToLayer("Monster");
    }

    // IDLE ���� �ڷ�ƾ
    protected override IEnumerator State_IDLE()
    {
        while (state == State.IDLE)
        {
            // �ִϸ����Ϳ� "Idle" Ʈ���� ����
            myAnim.SetTrigger("Idle");
            // Ÿ���� ������ �̵� ���·� ��ȯ
            if (target != null)
            {
                TransferState(State.MOVE);
            }
            yield return null;
        }
    }

    // MOVE ���� �ڷ�ƾ
    protected override IEnumerator State_MOVE()
    {
        // "Move" �ִϸ��̼� Ʈ���� ����
        myAnim.SetTrigger("Move");
        while (state == State.MOVE)
        {
            // Ÿ���� ������ IDLE ���·� ��ȯ
            if (target == null)
            {
                TransferState(State.IDLE);
                yield break;
            }

            // �÷��̾� �̵�
            Move();

            // ���� ���� �Ÿ� ���� Ÿ���� ������ ATK ���·� ��ȯ
            if (IsTargetValidRange())
            {
                myAnim.ResetTrigger("Move");
                TransferState(State.ATK);
            }
            // Ÿ���� ������ IDLE ���·� ��ȯ
            else if (target == null)
            {
                myAnim.ResetTrigger("Move");
                TransferState(State.IDLE);
            }

            yield return null;
        }
    }

    // ATK ���� �ڷ�ƾ
    protected override IEnumerator State_ATK()
    {
        while (state == State.ATK)
        {
            // Ÿ���� ������ IDLE ���·� ��ȯ
            if (target == null)
            {
                TransferState(State.IDLE);
            }
            else
            {
                // Ÿ�� ������ Ȯ���Ͽ� �켱������ ���� ���� ȸ��
                Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
                if (Quaternion.Angle(transform.rotation, targetRotation) > 5f)
                {
                    // Ÿ�� ������ ���� �ε巴�� ȸ��
                    while (Quaternion.Angle(transform.rotation, targetRotation) > 5f)
                    {
                        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
                        yield return null;
                    }
                }
            }

            // ���� ���� �Ÿ� ���� Ÿ���� ������ ���� ����
            if (IsTargetValidRange())
            {
                Attack();
            }
            // ���� ���� �Ÿ� �ۿ� Ÿ���� ������ �̵� ���·� ��ȯ
            else
            {
                TransferState(State.MOVE);
            }

            // ���� �� ���� �ӵ���ŭ ���
            yield return new WaitUntil(() => isAttacking == false);
            myAnim.ResetTrigger("BasicAttack");
            myAnim.ResetTrigger("SkillAttack");
            myAnim.SetTrigger("Idle");
            yield return attackRate;
        }
    }

    // DEAD ���� �ڷ�ƾ
    protected override IEnumerator State_DEAD()
    {
        // �״� �ִϸ��̼� ���
        Dead();
        yield break;
    }


    // ���� ����
    protected override void Attack()
    {
        isAttacking = true;

        switch (curAttackPattern)
        {
            case AttackPattern.BASIC:
                myAnim.ResetTrigger("SkillAttack");
                myAnim.SetTrigger("BasicAttack");
                break;

            case AttackPattern.RANGEATTACK:
                myAnim.ResetTrigger("BasicAttack");
                myAnim.SetTrigger("SkillAttack");
                break;

            case AttackPattern.HEAL:
                myAnim.ResetTrigger("SkillAttack");
                myAnim.SetTrigger("BasicAttack");
                break;
        }
    }


    // �״� �ִϸ��̼� ����
    protected override void Dead()
    {
        myAnim.SetTrigger("Dead");
    }

    // �̵� ����
    protected override void Move()
    {
        if (target != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
            gameObject.transform.Translate(Vector3.forward * Time.deltaTime, Space.Self);
            myAnim.SetTrigger("Move");
        }
        else
        {
            TransferState(State.IDLE);
        }
    }


    // Ÿ�ٰ��� �Ÿ��� ���� ���� �Ÿ� �̳����� Ȯ��
    private bool IsTargetValidRange()
    {
        if (target == null) return false;
        return Vector3.Distance(transform.position, target.position) < status.AttackRange;
    }

    // �ֱ������� ���� ����� Ÿ�� ������Ʈ
    private void UpdateTarget() => target = FindNearestTarget();

    // ���� ����� Ÿ�� ã��
    private Transform FindNearestTarget()
    {
        // Ȱ��ȭ�� Ÿ���� ������ null ��ȯ
        if (Global.Instacne.ActiveTargets.Count == 0)
        {
            return null;
        }

        Transform nearestTarget = null;
        float nearestDistance = Mathf.Infinity;

        foreach (GameObject t in Global.Instacne.ActiveTargets)
        {
            MonsterController monster = t.GetComponent<MonsterController>();
            if (monster != null && monster.CurHP > 0)
            {
                float distance = Vector3.Distance(transform.position, t.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestTarget = t.transform;
                }
            }
        }

        return nearestTarget;
    }

    // ���� ���� �� ȣ��Ǵ� �޼���
    public void AttackEnd() => isAttacking = false;

    // �⺻ ���ݰ� ���� �����ϴ� �޼���
    public void OnAttack1Trigger()
    {
        if (target != null)
        {
            // ���� ���� ������ �⺻ ������ ���
            if (curAttackPattern == AttackPattern.BASIC)
            {
                // Ÿ�ٿ��� �⺻ ���ݷ¸�ŭ�� �������� ����
                target.GetComponent<MonsterController>().TransferDamage(status.AttackPower);
                // ���� ���� ������ ��ų �������� ����
                curAttackPattern = AttackPattern.RANGEATTACK;
            }
            // ���� ���� ������ ���� ���
            else if (curAttackPattern == AttackPattern.HEAL)
            {
                // �� ����Ʈ�� ����ϰ� ü���� ������Ų��.
                StartCoroutine(ActiveEff(healEff, 2f));
                curHp = Mathf.Min(curHp + status.AttackPower, 100f);
                // ü�� �� ������Ʈ
                UIManager.Instacne.UpdateHPBar(curHp / status.Hp);
                // ���� �ؽ�Ʈ ǥ��
                UIManager.Instacne.ShowDamageText(status.AttackPower, transform.position + Vector3.up * 1f, Color.green);
                // ���� ���� ������ �⺻ �������� ����
                curAttackPattern = AttackPattern.BASIC;
            }
        }
        else
        {
            Debug.LogError("Lost Target");
        }
    }

    // ����Ʈ�� ������ �ð����� Ȱ��ȭ�� �� ��Ȱ��ȭ�ϴ� �ڷ�ƾ
    private IEnumerator ActiveEff(GameObject eff, float time)
    {
        eff.SetActive(true);
        yield return new WaitForSeconds(time);
        eff.SetActive(false);
    }

    // ��ų ������ �����ϴ� �޼���
    public void OnAttack2Trigger()
    {
        // ��ų ����Ʈ�� ����ϰ� ��ų ���� ���� ���� ���� ���͵鿡�� ���ݷ¸�ŭ�� �������� ����
        StartCoroutine(ActiveEff(skillEff, 2f));
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, status.SkillAttackRange, monsterLayerMask);
        foreach (Collider col in hitColliders)
        {
            MonsterController monster = col.GetComponent<MonsterController>();
            if (monster != null)
            {
                monster.TransferDamage(status.AttackPower);
            }
        }
        // ���� ���� ������ �� �������� ����
        curAttackPattern = AttackPattern.HEAL;
    }

    // ���ظ� �޾� ü���� ���ҽ�Ű�� �޼���
    public void TransferDamage(int attackPower)
    {
        if (!hitEff.activeInHierarchy)
        {
            StartCoroutine(ActiveEff(hitEff, 0.5f));
        }
        curHp = Mathf.Max(curHp - attackPower, 0f);
        // ü�� �� ������Ʈ
        UIManager.Instacne.UpdateHPBar(curHp / status.Hp);
        // ���� �ؽ�Ʈ ǥ��
        UIManager.Instacne.ShowDamageText(attackPower, transform.position + Vector3.up * 1f, Color.red);
        if (curHp <= 0)
        {
            curHp = status.Hp;
        }
    }

    // ����ġ�� ��� �������� ó���ϴ� �޼���
    public void GetExp(int addExpAmount)
    {
        curExp += addExpAmount;
        if (curExp >= status.RequireEXP4LvUp)
        {
            // ������ �� UI ������Ʈ
            UIManager.Instacne.UpdateExpBar(curExp / status.RequireEXP4LvUp, true);
            LevelUp();
            curExp = 0;
        }
        else
        {
            // ����ġ �� ������Ʈ
            UIManager.Instacne.UpdateExpBar(curExp / status.RequireEXP4LvUp);
        }
    }

    // �������� ó���ϴ� �޼���
    public void LevelUp()
    {
        // �������ͽ� ����
        status.Hp += 10;
        status.AttackPower += 10;
        status.AttackRate += 0.2f;
        status.RequireEXP4LvUp += 50;
        // ���� ǥ�� UI ������Ʈ
        UIManager.Instacne.UpdateLevel(++curLevel);
    }
}
