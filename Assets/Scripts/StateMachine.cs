using System.Collections;
using UnityEngine;
using static EnumTypes;

public abstract class StateMachine : MonoBehaviour
{
    protected State state;
    protected Transform target;

    // 활성화(처음 생성 or 오브젝트풀에서 다시 반환되어 나올때)
    protected virtual void Start()
    {
        state = State.IDLE;
        StartCoroutine("State_" + state);
    }

    protected void TransferState(State nextState)
    {
        // 현재 State의 코루틴을 중지시키고
        StopCoroutine("State_" + state);
        // State를 변경해준뒤
        state = nextState;
        // 해당 State의 코루틴을 실행시킨다.
        StartCoroutine("State_" + state);
    }

    protected abstract IEnumerator State_IDLE();
    protected abstract IEnumerator State_MOVE();
    protected abstract IEnumerator State_ATK();
    protected abstract IEnumerator State_DEAD();

    protected abstract void Attack();

    protected abstract void Dead();

    protected abstract void Move();
}
