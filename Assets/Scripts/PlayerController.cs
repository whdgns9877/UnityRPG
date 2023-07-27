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
    private bool isRotateDone;

    protected override void OnEnable()
    {
        isAttacking = false;
        curHp = myStatus.Hp;
        attackRate = new WaitForSeconds(1 / myStatus.AttackRate);
        myAnim = GetComponent<Animator>();
        curAttackPattern = AttackPattern.BASIC;
        // 0.5초 주기로 타겟을 업데이트한다
        InvokeRepeating(nameof(UpdateTarget), 0f, 0.5f);
        base.OnEnable();
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
        return Vector3.Distance(transform.position, target.position) < myStatus.AttackRange;
    }

    private void UpdateTarget() => target = FindNearestTarget();

    private Transform FindNearestTarget()
    {
        if (Global.Inst.targets.Count == 0)
        {
            return null;
        }

        Transform nearestTarget = null;
        float nearestDistance = Mathf.Infinity;

        foreach (GameObject t in Global.Inst.targets)
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
                target.GetComponent<MonsterController>().TransferDamage(myStatus.AttackPower);
                curAttackPattern = AttackPattern.RANGEATTACK;
            }
            else
            {
                myStatus.Hp = Mathf.Min(myStatus.Hp + myStatus.AttackPower, 100f);
                curAttackPattern = AttackPattern.BASIC;
            }
        }
        else
        {
            Debug.LogError("Lost Target");
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
        curAttackPattern = AttackPattern.HEAL;
    }

    public void TransferDamage(int attackPower)
    {
        curHp -= attackPower;
        if (curHp <= 0)
        {
            curHp = myStatus.Hp;
        }
    }
}
