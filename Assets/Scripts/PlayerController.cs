using static EnumTypes;
using UnityEngine;
using System.Collections;

public class PlayerController : StateMachine
{
    [SerializeField] PlayerData myStatus;

    private AttackPattern curAttackPattern;
    private Animator myAnim;
    private WaitForSeconds attackRate;
    private float curHp;
    private bool isAttacking;

    protected override void Start()
    {
        isAttacking = false;
        curHp = myStatus.Hp;
        attackRate = new WaitForSeconds(1 / myStatus.AttackRate);
        myAnim = GetComponent<Animator>();
        curAttackPattern = AttackPattern.BASIC;
        base.Start();
    }

    protected override IEnumerator State_IDLE()
    {
        while (state == State.IDLE)
        {
            myAnim.SetTrigger("Idle");
            UpdateTarget();
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

            UpdateTarget();
            Move();

            if (IsTargetValidRange())
            {
                myAnim.ResetTrigger("Move");
                TransferState(State.ATK);
            }

            yield return null;
        }
    }

    protected override IEnumerator State_ATK()
    {
        while (state == State.ATK)
        {
            UpdateTarget();

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
                curAttackPattern = AttackPattern.RANGEATTACK;
                break;

            case AttackPattern.RANGEATTACK:
                myAnim.ResetTrigger("BasicAttack");
                myAnim.SetTrigger("SkillAttack");
                curAttackPattern = AttackPattern.HEAL;
                break;

            case AttackPattern.HEAL:
                myAnim.ResetTrigger("SkillAttack");
                myAnim.SetTrigger("BasicAttack");
                curAttackPattern = AttackPattern.BASIC;
                break;
        }
    }

    protected override void Dead()
    {
        myAnim.SetTrigger("Dead");
    }

    protected override void Move()
    {
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        gameObject.transform.Translate(Vector3.forward * Time.deltaTime, Space.Self);
        myAnim.SetTrigger("Move");
    }

    private bool IsTargetValidRange() => Vector3.Distance(transform.position, target.position) < myStatus.AttackRange;

    private void UpdateTarget() => target = FindNearestTarget();

    private Transform FindNearestTarget()
    {
        if (Global.Inst.targets.Count == 0)
        {
            Debug.LogError("타겟이 존재하지 않습니다.");
            return null;
        }

        Transform nearestTarget = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Transform t in Global.Inst.targets)
        {
            float distance = Vector3.Distance(transform.position, t.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = t;
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
                Debug.Log("몬스터를 공격했다...");
                target.GetComponent<MonsterController>().TransferDamage(myStatus.AttackPower);
            }
            else
            {
                Debug.Log("힐을 한다...");
                myStatus.Hp = Mathf.Min(myStatus.Hp + myStatus.AttackPower, 100f);
            }
        }
        else
        {
            Debug.LogError("공격 타겟을 잃었다..");
        }
    }

    public void OnAttack2Trigger()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, myStatus.SkillAttackRange);
        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag("Monster"))
            {
                MonsterController monster = col.GetComponent<MonsterController>();
                if (monster != null)
                {
                    monster.TransferDamage(myStatus.AttackPower);
                }
            }
        }
    }

    public void TransferDamage(int attackPower)
    {
        curHp -= attackPower;
        if (curHp <= 0)
        {
            TransferState(State.DEAD);
        }
    }
}
