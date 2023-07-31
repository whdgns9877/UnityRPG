using System.Collections;
using UnityEngine;
using static EnumTypes;

public class MonsterController : StateMachine
{
    [SerializeField] MonsterData myStatus; // ������ �⺻���� ����(Scriptable Object)
                                            
    private PlayerController player;       // PlayerController �ν��Ͻ� ������ ��Ƶα�����
    private Animator myAnim;               // �� ������ �ִϸ����� ������Ʈ
    private WaitForSeconds attackRate;     // �ֱ������� ����� attackRate�� �Ź� new�� �������� �ʰ� �ѹ� �����صΰ� ����
    private bool isAttacking;              // ������������ �Ǻ��� ����
    private bool isDeadAnimDone;           // �״� �ִϸ��̼��� ���������� �Ǻ��� ����
    private bool initDone = false;         // ó�� �����ܰ����� ��Ȱ��ȭ -> Ȱ��ȭ ���������� �Ǻ��� ����

    private float curHp; // ���� HP
    public float CurHP { get { return curHp; } private set { } } // ���� HP������ ��ȯ���ִ� Property

    // Ȱ��ȭ (ó�� �����ǰų� ��Ȱ��ȭ(����) -> Ȱ��ȭ �ÿ� ����)
    private void OnEnable()
    {
        ActiveInit();
    }

    protected override void Start()
    {
        InitOnce();
        base.Start();
    }

    private void ActiveInit()
    {
        // Ȱ��ȭ �Ǿ��ִ� Ÿ�ٵ��� List�� �߰��ϱ� ����
        Global.Instacne.AddTarget(gameObject);
        curHp = myStatus.MaxHp;
        isAttacking = false;
        if (true == initDone)
        {
            InitState();
        }
    }

    private void InitOnce()
    {
        attackRate = new WaitForSeconds(1 / myStatus.AttackRate); // AttackRate�� ���������� ���� �ӵ��� ������
        myAnim = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        player = target.GetComponent<PlayerController>();
        initDone = true;
    }

    protected override IEnumerator State_IDLE()
    {
        // ���� State�� �̵����� �ٲ۴�.
        myAnim.ResetTrigger("Move");
        TransferState(State.MOVE);
        // TransferState �Լ����� ���� ������Ʈ�� �ڵ����� Stop���ִ� break�� �ƴ� null��ȯ
        yield return null;
    }

    protected override IEnumerator State_MOVE()
    {
        myAnim.SetTrigger("Move");
        while (state == State.MOVE)
        {
            // �̵��Ѵ�
            Move();

            // �÷��̾�� �ڱ��ڽ�(����)������ �Ÿ��� ������ ���� ���ɹ����� ���Ͽ� ����
            if (true == IsTargetValidRange())
            {
                TransferState(State.ATK);
            }

            yield return null;
        }
    }

    protected override IEnumerator State_ATK()
    {
        // ���� ������ �����϶� ���ѹݺ�
        while (state == State.ATK)
        {
            // ���ݰ��ɻ������� üũ
            if (true == IsTargetValidRange())
            {
                Attack();
            }
            // �������̸� Move State�� �̵�
            else
            {
                myAnim.ResetTrigger("Attack");
                TransferState(State.MOVE);
            }

            yield return new WaitUntil(() => isAttacking == false);
            myAnim.ResetTrigger("Attack");
            myAnim.SetTrigger("Idle");
            // ���� �ӵ� ��ŭ ���
            yield return attackRate;
        }
    }

    protected override IEnumerator State_DEAD()
    {
        Dead();
        yield return new WaitUntil(() => isDeadAnimDone == true);
        ObjectPool.Instacne.ReturnMonsterToPool(gameObject);
    }

    protected override void Attack()
    {
        myAnim.SetTrigger("Attack");
        isAttacking = true;
    }

    protected override void Dead()
    {
        myAnim.SetTrigger("Dead");
        isDeadAnimDone = false;
        player.GetExp(myStatus.GetExp);
        Global.Instacne.RemoveTarget(gameObject);
    }

    protected override void Move()
    {
        if (target == null)
        {
            Debug.LogError("Ÿ�� ã�� ����");
            target = GameObject.FindGameObjectWithTag("Player").transform;
            return;
        }

        // ���� ��ġ���� ��ǥ Ÿ���� �ٶ󺸴� ȸ���� ���
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);

        // Lerp �Լ��� ����Ͽ� ���� ȸ������ ��ǥ ȸ���� ���̸� �����Ͽ� �ε巯�� ȸ���� ����
        // rotationSpeed ������ �̿��Ͽ� ���� �ӵ� ���� ����
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.deltaTime);

        // ���͸� �ٶ󺸴� �������� �����δ�
        gameObject.transform.Translate(Vector3.forward * Time.deltaTime, Space.Self);
    }

    // �ش� Ÿ���� ���ݰ��� ���� �ȿ� �ִ����� ����ش�
    private bool IsTargetValidRange()
    {
        if (target == null) return false;
        return Vector3.Distance(transform.position, target.position) < myStatus.AttackRange;
    }

    // ��Ȱ��ȭ(����)�Ǹ� Ȱ��ȭ ����Ʈ���� ����
    private void OnDisable()
    {
        Global.Instacne.RemoveTarget(gameObject);
    }

    // ���� ��� �ִϸ��̼ǿ� ����Ͽ� ������ ���� ���·� ��ȯ
    public void AttackEnd() => isAttacking = false;

    // �÷��̾�� ���ݴ��ϸ� �ش� �Լ� ȣ��
    public void TransferDamage(int attackPower)
    {
        // ���� ü���� attackPower��ŭ ���
        curHp -= attackPower;
        // UIManager���� ��ŭ�� �������� �Ծ����� ǥ���ش޶� ��û(���� Ǫ��������)
        UIManager.Instacne.ShowDamageText(attackPower, transform.position + Vector3.up * 0.5f, Color.blue);
        // ���� ü���� 0���϶�� DEAD������Ʈ�� �̵�
        if (curHp <= 0)
        {
            TransferState(State.DEAD);
        }
    }

    // ������ ���ݾִϸ��̼� Ư�� �����ӿ� �� �Լ��� ȣ��
    public void OnAttack1Trigger()
    {
        // �̸� ������ player�ν��Ͻ��� �ǰݻ�� �˸�
        player.TransferDamage(myStatus.AttackPower);
    }

    public void EndDeadAnim()
    {
        isDeadAnimDone = true;
    }
}
