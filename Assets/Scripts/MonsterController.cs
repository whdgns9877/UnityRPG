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
        attackRate = new WaitForSeconds(1 / myStatus.AttackRate); // AttackRate가 높아질수록 공격 속도가 빨라짐
        myAnim = GetComponent<Animator>();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override IEnumerator State_IDLE()
    {
        // 다음 State를 이동으로 바꾼다.
        TransferState(State.MOVE);
        // TransferState 함수에서 이전 스테이트를 자동으로 Stop해주니 break가 아닌 null반환
        yield return null;
    }

    protected override IEnumerator State_MOVE()
    {
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
                TransferState(State.MOVE);
            }

            // 공격 속도 만큼 대기
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
        Debug.Log("공격");
        myAnim.SetTrigger("Attack");
    }

    protected override void Dead()
    {
        Debug.Log("사망");
        myAnim.SetTrigger("Dead");
    }

    protected override void Move()
    {
        if (target == null)
        {
            Debug.LogError("타겟 찾지 못함");
            target = GameObject.FindGameObjectWithTag("Player").transform;
            return;
        }
        Debug.Log("이동");


        // 현재 위치에서 목표 타겟을 바라보는 회전값 계산
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);

        // Lerp 함수를 사용하여 현재 회전값과 목표 회전값 사이를 보간하여 부드러운 회전을 만듦
        // rotationSpeed 변수를 이용하여 보간 속도 조절 가능
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.deltaTime);


        // 몬스터를 바라보는 방향으로 움직인다
        gameObject.transform.Translate(Vector3.forward * Time.deltaTime, Space.Self);
        myAnim.SetTrigger("Walk");
    }

    private bool IsTargetValidRange() => Vector3.Distance(gameObject.transform.position, target.transform.position) < myStatus.AttackRange;
}
