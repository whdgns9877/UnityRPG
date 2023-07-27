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
    [SerializeField] PlayerData myStatus;
    [SerializeField] private PlayerStatus status;
    [SerializeField] private GameObject healEff;
    [SerializeField] private GameObject skillEff;
    [SerializeField] private GameObject hitEff;
    private AttackPattern curAttackPattern;
    private Animator myAnim;
    private WaitForSeconds attackRate;
    private float curHp;
    private float curExp;
    private bool isAttacking;
    private bool isRotateDone;
    private int curLevel;
    private int monsterLayerMask;

    protected override void Start()
    {
        InitStatus();
        InitPlayer();
        UIManager.Instacne.UpdateLevel(curLevel);
        // 0.5초 주기로 타겟을 업데이트한다
        InvokeRepeating(nameof(UpdateTarget), 0f, 0.5f);
        base.Start();
    }

    private void InitStatus()
    {
        status.Hp = myStatus.Hp;
        status.AttackPower = myStatus.AttackPower;
        status.AttackRate = myStatus.AttackRate;
        status.AttackRange = myStatus.AttackRange;
        status.SkillAttackRange = myStatus.SkillAttackRange;
        status.RequireEXP4LvUp = myStatus.RequireEXP4LvUp;
    }

    private void InitPlayer()
    {
        attackRate = new WaitForSeconds(1 / status.AttackRate);
        myAnim = GetComponent<Animator>();
        healEff.SetActive(false);
        skillEff.SetActive(false);
        hitEff.SetActive(false);
        curAttackPattern = AttackPattern.BASIC;
        isAttacking = false;
        curLevel = 1;
        curHp = status.Hp;
        curExp = 0;
        monsterLayerMask = 1 << LayerMask.NameToLayer("Monster"); // "Monster" 레이어에 대한 LayerMask 생성
    }

    protected override IEnumerator State_IDLE()
    {
        while (state == State.IDLE)
        {
            myAnim.SetTrigger("Idle");
            if (target != null)
            {
                TransferState(State.MOVE);
            }
            yield return null;
        }
    }

    protected override IEnumerator State_MOVE()
    {
        myAnim.SetTrigger("Move");
        while (state == State.MOVE)
        {
            if (target == null)
            {
                TransferState(State.IDLE);
                yield break;
            }

            Move();

            if (IsTargetValidRange())
            {
                myAnim.ResetTrigger("Move");
                TransferState(State.ATK);
            }
            else if (target == null)
            {
                myAnim.ResetTrigger("Move");
                TransferState(State.IDLE);
            }

            yield return null;
        }
    }

    protected override IEnumerator State_ATK()
    {
        while (state == State.ATK)
        {
            // 타겟 방향을 확인하여 우선적으로 적을 향해 회전
            Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
            if (Quaternion.Angle(transform.rotation, targetRotation) > 5f)
            {
                // 타겟 방향을 향해 부드럽게 회전
                while (Quaternion.Angle(transform.rotation, targetRotation) > 5f)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
                    yield return null;
                }
            }

            if (IsTargetValidRange())
            {
                Attack();
            }
            else
            {
                TransferState(State.MOVE);
            }

            yield return new WaitUntil(() => isAttacking == false);
            myAnim.ResetTrigger("BasicAttack");
            myAnim.ResetTrigger("SkillAttack");
            myAnim.SetTrigger("Idle");
            yield return attackRate;
        }
    }

    protected override IEnumerator State_DEAD()
    {
        Dead();
        yield break;
    }

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


    // 플레이어가 죽지않으니 사용되지는 않는다...
    protected override void Dead()
    {
        myAnim.SetTrigger("Dead");
    }

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

    private bool IsTargetValidRange()
    {
        if (target == null) return false;
        return Vector3.Distance(transform.position, target.position) < status.AttackRange;
    }

    private void UpdateTarget() => target = FindNearestTarget();

    private Transform FindNearestTarget()
    {
        if (Global.Instacne.targets.Count == 0)
        {
            return null;
        }

        Transform nearestTarget = null;
        float nearestDistance = Mathf.Infinity;

        foreach (GameObject t in Global.Instacne.targets)
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

    public void AttackEnd() => isAttacking = false;

    public void OnAttack1Trigger()
    {
        if (target != null)
        {
            if (curAttackPattern == AttackPattern.BASIC)
            {
                target.GetComponent<MonsterController>().TransferDamage(status.AttackPower);
                curAttackPattern = AttackPattern.RANGEATTACK;
            }
            else
            {
                StartCoroutine(ActiveEff(healEff, 2f));
                curHp = Mathf.Min(curHp + status.AttackPower, 100f);
                UIManager.Instacne.UpdateHPBar(curHp / status.Hp);
                UIManager.Instacne.ShowDamageText(status.AttackPower, transform.position + Vector3.up * 1f, Color.green);
                curAttackPattern = AttackPattern.BASIC;
            }
        }
        else
        {
            Debug.LogError("Lost Target");
        }
    }

    private IEnumerator ActiveEff(GameObject eff, float time)
    {
        eff.SetActive(true);
        yield return new WaitForSeconds(time);
        eff.SetActive(false);
    }

    public void OnAttack2Trigger()
    {
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
        curAttackPattern = AttackPattern.HEAL;
    }

    public void TransferDamage(int attackPower)
    {
        if(!hitEff.activeInHierarchy)
            StartCoroutine(ActiveEff(hitEff, 0.5f));
        curHp = Mathf.Max(curHp - attackPower, 0f);
        UIManager.Instacne.UpdateHPBar(curHp / status.Hp);
        UIManager.Instacne.ShowDamageText(attackPower, transform.position + Vector3.up * 1f, Color.red);
        if (curHp <= 0)
        {
            curHp = status.Hp;
        }
    }

    public void GetExp(int addExpAmount)
    {
        curExp += addExpAmount;
        if(curExp >= status.RequireEXP4LvUp)
        {
            UIManager.Instacne.UpdateExpBar(curExp / status.RequireEXP4LvUp, true);
            LevelUp();
            curExp = 0;
        }
        else
        {
            UIManager.Instacne.UpdateExpBar(curExp / status.RequireEXP4LvUp);
        }
    }

    public void LevelUp()
    {
        status.Hp += 10;
        status.AttackPower += 10;
        status.AttackRate += 0.2f;
        status.RequireEXP4LvUp += 50;
        UIManager.Instacne.UpdateLevel(++curLevel);
    }
}
