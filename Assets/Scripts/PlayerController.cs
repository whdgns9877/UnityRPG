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
    [SerializeField] PlayerData myStatus;          // 플레이어의 기본 스텟 정보(Scriptable Object)
    [SerializeField] private PlayerStatus status;  // 기본 스텟정보를 구조체로 받아둔뒤에 레벨업 등으로 수치 변경
    [SerializeField] private GameObject healEff;   // 힐 이펙트
    [SerializeField] private GameObject skillEff;  // 스킬 이펙트
    [SerializeField] private GameObject hitEff;    // 피격 이펙트
                                                    
    private AttackPattern curAttackPattern;        // 현재 공격상태
    private Animator myAnim;                       // 애니메이터 컴포넌트
    private WaitForSeconds attackRate;             // 공격속도
    private float curHp;                           // 현재 체력
    private float curExp;                          // 현재 EXP
    private bool isAttacking;                      // 공격 중인지
    private bool isRotateDone;                     // 회전이 끝났는지
    private int curLevel;                          // 현재 레벨
    private int monsterLayerMask;                  // 몬스터의 레이어마스크

    protected override void Start()
    {
        InitStatus();
        InitPlayer();
        UIManager.Instacne.UpdateLevel(curLevel);
        // 0.5초 주기로 타겟을 업데이트한다
        InvokeRepeating(nameof(UpdateTarget), 0f, 0.5f);
        base.Start();
    }

    // 스텟 구조체에 스크립터블 오브젝트(초기 스텟)정보를 대입
    private void InitStatus()
    {
        status.Hp = myStatus.Hp;
        status.AttackPower = myStatus.AttackPower;
        status.AttackRate = myStatus.AttackRate;
        status.AttackRange = myStatus.AttackRange;
        status.SkillAttackRange = myStatus.SkillAttackRange;
        status.RequireEXP4LvUp = myStatus.RequireEXP4LvUp;
    }


    // 플레이어 초기화
    private void InitPlayer()
    {
        // 공격 속도를 기다리기 위한 WaitForSeconds 객체 생성
        attackRate = new WaitForSeconds(1 / status.AttackRate);
        // 플레이어의 애니메이터 컴포넌트 찾기
        myAnim = GetComponent<Animator>();
        // 힐, 스킬, 피격 이펙트 비활성화
        healEff.SetActive(false);
        skillEff.SetActive(false);
        hitEff.SetActive(false);
        // 초기 공격 패턴은 기본 공격으로 설정
        curAttackPattern = AttackPattern.BASIC;
        // 공격 중이 아님으로 초기화
        isAttacking = false;
        // 초기 레벨은 1로 설정
        curLevel = 1;
        // 현재 체력을 최대 체력으로 초기화
        curHp = status.Hp;
        // 현재 경험치 초기화
        curExp = 0;
        // "Monster" 레이어에 대한 LayerMask 생성
        monsterLayerMask = 1 << LayerMask.NameToLayer("Monster");
    }

    // IDLE 상태 코루틴
    protected override IEnumerator State_IDLE()
    {
        while (state == State.IDLE)
        {
            // 애니메이터에 "Idle" 트리거 설정
            myAnim.SetTrigger("Idle");
            // 타겟이 있으면 이동 상태로 전환
            if (target != null)
            {
                TransferState(State.MOVE);
            }
            yield return null;
        }
    }

    // MOVE 상태 코루틴
    protected override IEnumerator State_MOVE()
    {
        // "Move" 애니메이션 트리거 설정
        myAnim.SetTrigger("Move");
        while (state == State.MOVE)
        {
            // 타겟이 없으면 IDLE 상태로 전환
            if (target == null)
            {
                TransferState(State.IDLE);
                yield break;
            }

            // 플레이어 이동
            Move();

            // 공격 사정 거리 내에 타겟이 있으면 ATK 상태로 전환
            if (IsTargetValidRange())
            {
                myAnim.ResetTrigger("Move");
                TransferState(State.ATK);
            }
            // 타겟이 없으면 IDLE 상태로 전환
            else if (target == null)
            {
                myAnim.ResetTrigger("Move");
                TransferState(State.IDLE);
            }

            yield return null;
        }
    }

    // ATK 상태 코루틴
    protected override IEnumerator State_ATK()
    {
        while (state == State.ATK)
        {
            // 타겟이 없으면 IDLE 상태로 전환
            if (target == null)
            {
                TransferState(State.IDLE);
            }
            else
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
            }

            // 공격 사정 거리 내에 타겟이 있으면 공격 수행
            if (IsTargetValidRange())
            {
                Attack();
            }
            // 공격 사정 거리 밖에 타겟이 있으면 이동 상태로 전환
            else
            {
                TransferState(State.MOVE);
            }

            // 공격 후 공격 속도만큼 대기
            yield return new WaitUntil(() => isAttacking == false);
            myAnim.ResetTrigger("BasicAttack");
            myAnim.ResetTrigger("SkillAttack");
            myAnim.SetTrigger("Idle");
            yield return attackRate;
        }
    }

    // DEAD 상태 코루틴
    protected override IEnumerator State_DEAD()
    {
        // 죽는 애니메이션 재생
        Dead();
        yield break;
    }


    // 공격 실행
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


    // 죽는 애니메이션 실행
    protected override void Dead()
    {
        myAnim.SetTrigger("Dead");
    }

    // 이동 실행
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


    // 타겟과의 거리가 공격 사정 거리 이내인지 확인
    private bool IsTargetValidRange()
    {
        if (target == null) return false;
        return Vector3.Distance(transform.position, target.position) < status.AttackRange;
    }

    // 주기적으로 가장 가까운 타겟 업데이트
    private void UpdateTarget() => target = FindNearestTarget();

    // 가장 가까운 타겟 찾기
    private Transform FindNearestTarget()
    {
        // 활성화된 타겟이 없으면 null 반환
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

    // 공격 종료 시 호출되는 메서드
    public void AttackEnd() => isAttacking = false;

    // 기본 공격과 힐을 수행하는 메서드
    public void OnAttack1Trigger()
    {
        if (target != null)
        {
            // 현재 공격 패턴이 기본 공격인 경우
            if (curAttackPattern == AttackPattern.BASIC)
            {
                // 타겟에게 기본 공격력만큼의 데미지를 전달
                target.GetComponent<MonsterController>().TransferDamage(status.AttackPower);
                // 다음 공격 패턴을 스킬 공격으로 변경
                curAttackPattern = AttackPattern.RANGEATTACK;
            }
            // 현재 공격 패턴이 힐인 경우
            else if (curAttackPattern == AttackPattern.HEAL)
            {
                // 힐 이펙트를 재생하고 체력을 증가시킨다.
                StartCoroutine(ActiveEff(healEff, 2f));
                curHp = Mathf.Min(curHp + status.AttackPower, 100f);
                // 체력 바 업데이트
                UIManager.Instacne.UpdateHPBar(curHp / status.Hp);
                // 피해 텍스트 표시
                UIManager.Instacne.ShowDamageText(status.AttackPower, transform.position + Vector3.up * 1f, Color.green);
                // 다음 공격 패턴을 기본 공격으로 변경
                curAttackPattern = AttackPattern.BASIC;
            }
        }
        else
        {
            Debug.LogError("Lost Target");
        }
    }

    // 이펙트를 지정된 시간동안 활성화한 뒤 비활성화하는 코루틴
    private IEnumerator ActiveEff(GameObject eff, float time)
    {
        eff.SetActive(true);
        yield return new WaitForSeconds(time);
        eff.SetActive(false);
    }

    // 스킬 공격을 수행하는 메서드
    public void OnAttack2Trigger()
    {
        // 스킬 이펙트를 재생하고 스킬 공격 사정 범위 내의 몬스터들에게 공격력만큼의 데미지를 전달
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
        // 다음 공격 패턴을 힐 공격으로 변경
        curAttackPattern = AttackPattern.HEAL;
    }

    // 피해를 받아 체력을 감소시키는 메서드
    public void TransferDamage(int attackPower)
    {
        if (!hitEff.activeInHierarchy)
        {
            StartCoroutine(ActiveEff(hitEff, 0.5f));
        }
        curHp = Mathf.Max(curHp - attackPower, 0f);
        // 체력 바 업데이트
        UIManager.Instacne.UpdateHPBar(curHp / status.Hp);
        // 피해 텍스트 표시
        UIManager.Instacne.ShowDamageText(attackPower, transform.position + Vector3.up * 1f, Color.red);
        if (curHp <= 0)
        {
            curHp = status.Hp;
        }
    }

    // 경험치를 얻어 레벨업을 처리하는 메서드
    public void GetExp(int addExpAmount)
    {
        curExp += addExpAmount;
        if (curExp >= status.RequireEXP4LvUp)
        {
            // 레벨업 시 UI 업데이트
            UIManager.Instacne.UpdateExpBar(curExp / status.RequireEXP4LvUp, true);
            LevelUp();
            curExp = 0;
        }
        else
        {
            // 경험치 바 업데이트
            UIManager.Instacne.UpdateExpBar(curExp / status.RequireEXP4LvUp);
        }
    }

    // 레벨업을 처리하는 메서드
    public void LevelUp()
    {
        // 스테이터스 증가
        status.Hp += 10;
        status.AttackPower += 10;
        status.AttackRate += 0.2f;
        status.RequireEXP4LvUp += 50;
        // 레벨 표시 UI 업데이트
        UIManager.Instacne.UpdateLevel(++curLevel);
    }
}
