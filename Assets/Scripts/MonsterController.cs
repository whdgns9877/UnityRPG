using System.Collections;
using UnityEngine;
using static EnumTypes;

public class MonsterController : StateMachine
{
    [SerializeField] MonsterData myStatus;

    private PlayerController player;
    private Animator myAnim;
    private WaitForSeconds attackRate;
    private bool isAttacking;
    private bool isDeadAnimDone;
    private bool initDone = false;

    private float curHp;
    public float CurHP { get { return curHp; } private set { } }

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
        Global.Instacne.AddTarget(gameObject);
        curHp = myStatus.MaxHp;
        isAttacking = false;
        if(true == initDone)
        {
            InitState();
        }
    }

    private void InitOnce()
    {
        attackRate = new WaitForSeconds(1 / myStatus.AttackRate); // AttackRate가 높아질수록 공격 속도가 빨라짐
        myAnim = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        player = target.GetComponent<PlayerController>();
        initDone = true;
    }

    protected override IEnumerator State_IDLE()
    {
        // 다음 State를 이동으로 바꾼다.
        myAnim.ResetTrigger("Move");
        TransferState(State.MOVE);
        // TransferState 함수에서 이전 스테이트를 자동으로 Stop해주니 break가 아닌 null반환
        yield return null;
    }

    protected override IEnumerator State_MOVE()
    {
        myAnim.SetTrigger("Move");
        while (state == State.MOVE)
        {
            // 이동한다
            Move();

            // 플레이어와 자기자신(몬스터)사이의 거리와 본인의 공격 가능범위를 비교하여 수행
            if (true == IsTargetValidRange())
            {
                TransferState(State.ATK);
            }

            yield return null;
        }
    }

    protected override IEnumerator State_ATK()
    {
        // 공격 가능한 상태일때 무한반복
        while (state == State.ATK)
        {
            // 공격가능상태인지 체크
            if (true == IsTargetValidRange())
            {
                Attack();
            }
            // 범위밖이면 Move State로 이동
            else
            {
                myAnim.ResetTrigger("Attack");
                TransferState(State.MOVE);
            }

            yield return new WaitUntil(() => isAttacking == false);
            myAnim.ResetTrigger("Attack");
            myAnim.SetTrigger("Idle");
            // 공격 속도 만큼 대기
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
            Debug.LogError("타겟 찾지 못함");
            target = GameObject.FindGameObjectWithTag("Player").transform;
            return;
        }

        // 현재 위치에서 목표 타겟을 바라보는 회전값 계산
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);

        // Lerp 함수를 사용하여 현재 회전값과 목표 회전값 사이를 보간하여 부드러운 회전을 만듦
        // rotationSpeed 변수를 이용하여 보간 속도 조절 가능
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.deltaTime);

        // 몬스터를 바라보는 방향으로 움직인다
        gameObject.transform.Translate(Vector3.forward * Time.deltaTime, Space.Self);
    }

    private bool IsTargetValidRange()
    {
        if (target == null) return false;
        return Vector3.Distance(transform.position, target.position) < myStatus.AttackRange;
    }

    private void OnDisable()
    {
        Global.Instacne.RemoveTarget(gameObject);
    }

    public void AttackEnd() => isAttacking = false;

    public void TransferDamage(int attackPower)
    {
        curHp -= attackPower;
        UIManager.Instacne.ShowDamageText(attackPower, transform.position + Vector3.up * 0.5f, Color.blue);
        if (curHp <= 0)
        {
            TransferState(State.DEAD);
        }
    }

    public void OnAttack1Trigger()
    {
        player.TransferDamage(myStatus.AttackPower);
    }

    public void EndDeadAnim()
    {
        isDeadAnimDone = true;
    }
}
