using System.Collections;
using UnityEngine;
using static EnumTypes;

public class MonsterController : StateMachine
{
    [SerializeField] MonsterData myStatus; // 몬스터의 기본스텟 정보(Scriptable Object)
                                            
    private PlayerController player;       // PlayerController 인스턴스 정보를 담아두기위함
    private Animator myAnim;               // 각 몬스터의 애니메이터 컴포넌트
    private WaitForSeconds attackRate;     // 주기적으로 사용할 attackRate를 매번 new로 생성하지 않고 한번 생성해두고 재사용
    private bool isAttacking;              // 공격중인지를 판별할 변수
    private bool isDeadAnimDone;           // 죽는 애니메이션이 끝났는지를 판별할 변수
    private bool initDone = false;         // 처음 생성단계인지 비활성화 -> 활성화 상태인지를 판별할 변수

    private float curHp; // 현재 HP
    public float CurHP { get { return curHp; } private set { } } // 현재 HP정보를 반환해주는 Property

    // 활성화 (처음 생성되거나 비활성화(죽음) -> 활성화 시에 실행)
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
        // 활성화 되어있는 타겟들을 List에 추가하기 위함
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

    // 해당 타겟이 공격가능 범위 안에 있는지를 뱉어준다
    private bool IsTargetValidRange()
    {
        if (target == null) return false;
        return Vector3.Distance(transform.position, target.position) < myStatus.AttackRange;
    }

    // 비활성화(죽음)되면 활성화 리스트에서 제거
    private void OnDisable()
    {
        Global.Instacne.RemoveTarget(gameObject);
    }

    // 공격 모션 애니메이션에 사용하여 공격이 끝난 상태로 전환
    public void AttackEnd() => isAttacking = false;

    // 플레이어에게 공격당하면 해당 함수 호출
    public void TransferDamage(int attackPower)
    {
        // 현재 체력을 attackPower만큼 깎고
        curHp -= attackPower;
        // UIManager에게 얼만큼의 데미지를 입었으니 표시해달라 요청(색은 푸른색으로)
        UIManager.Instacne.ShowDamageText(attackPower, transform.position + Vector3.up * 0.5f, Color.blue);
        // 현재 체력이 0이하라면 DEAD스테이트로 이동
        if (curHp <= 0)
        {
            TransferState(State.DEAD);
        }
    }

    // 몬스터의 공격애니메이션 특정 프레임에 이 함수가 호출
    public void OnAttack1Trigger()
    {
        // 미리 얻어놓은 player인스턴스에 피격사실 알림
        player.TransferDamage(myStatus.AttackPower);
    }

    public void EndDeadAnim()
    {
        isDeadAnimDone = true;
    }
}
