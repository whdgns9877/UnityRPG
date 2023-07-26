using System.Collections;
using UnityEngine;
using static EnumTypes;

public class MonsterController : StateMachine
{
    [SerializeField] MonsterData myStatus;
    private Animator myAnim;
    private WaitForSeconds attackRate;

    private void OnEnable()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        attackRate = new WaitForSeconds(1 / myStatus.AttackRate); // AttackRate�� ���������� ���� �ӵ��� ������
        myAnim = GetComponent<Animator>();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override IEnumerator State_IDLE()
    {
        // ���� State�� �̵����� �ٲ۴�.
        TransferState(State.MOVE);
        // TransferState �Լ����� ���� ������Ʈ�� �ڵ����� Stop���ִ� break�� �ƴ� null��ȯ
        yield return null;
    }

    protected override IEnumerator State_MOVE()
    {
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
                TransferState(State.MOVE);
            }

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
        Debug.Log("����");
        myAnim.SetTrigger("Attack");
    }

    protected override void Dead()
    {
        Debug.Log("���");
        myAnim.SetTrigger("Dead");
    }

    protected override void Move()
    {
        if (target == null)
        {
            Debug.LogError("Ÿ�� ã�� ����");
            target = GameObject.FindGameObjectWithTag("Player").transform;
            return;
        }
        Debug.Log("�̵�");


        // ���� ��ġ���� ��ǥ Ÿ���� �ٶ󺸴� ȸ���� ���
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);

        // Lerp �Լ��� ����Ͽ� ���� ȸ������ ��ǥ ȸ���� ���̸� �����Ͽ� �ε巯�� ȸ���� ����
        // rotationSpeed ������ �̿��Ͽ� ���� �ӵ� ���� ����
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.deltaTime);


        // ���͸� �ٶ󺸴� �������� �����δ�
        gameObject.transform.Translate(Vector3.forward * Time.deltaTime, Space.Self);
        myAnim.SetTrigger("Walk");
    }

    private bool IsTargetValidRange() => Vector3.Distance(gameObject.transform.position, target.transform.position) < myStatus.AttackRange;
}
