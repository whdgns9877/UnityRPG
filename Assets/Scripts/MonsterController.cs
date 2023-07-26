using System.Collections;
using UnityEngine;
using static EnumTypes;

public class MonsterController : StateMachine
{
    [SerializeField] MonsterData myStatus;

    private PlayerController player;
    private Animator myAnim;
    private WaitForSeconds attackRate;
    [SerializeField] private bool isAttacking;

    private float curHp;

    private void OnEnable()
    {
        curHp = myStatus.MaxHp;
        isAttacking = false;
        Global.Inst.AddTarget(transform);
    }

    protected override void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        player = target.GetComponent<PlayerController>();
        attackRate = new WaitForSeconds(1 / myStatus.AttackRate); // AttackRate�� ���������� ���� �ӵ��� ������
        myAnim = GetComponent<Animator>();
        base.Start();
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
        yield return null;
    }

    protected override void Attack()
    {
        Debug.Log("������ ����...");
        myAnim.SetTrigger("Attack");
        isAttacking = true;
    }

    protected override void Dead()
    {
        myAnim.SetTrigger("Dead");
        Global.Inst.RemoveTarget(transform);
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

    private bool IsTargetValidRange() => Vector3.Distance(gameObject.transform.position, target.transform.position) < myStatus.AttackRange;

    private void OnDisable()
    {
        Global.Inst.RemoveTarget(transform);
    }

    public void AttackEnd() => isAttacking = false;

    public void TransferDamage(int attackPower)
    {
        curHp -= attackPower;
        if (curHp <= 0)
        {
            TransferState(State.DEAD);
        }
    }

    public void OnAttack1Trigger()
    {
        player.TransferDamage(myStatus.AttackPower);
    }
}
