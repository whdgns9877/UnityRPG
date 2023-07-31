using System.Collections;
using UnityEngine;
using static EnumTypes;

// 플레이어와 몬스터의 기본 스테이트머신 추상화 클래스
public abstract class StateMachine : MonoBehaviour
{
    protected State state;
    protected Transform target;
    
    protected virtual void Start()
    {
        InitState();
    }

    protected void InitState()
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

    // abstract 함수들을 자식들이 각자 구현
    protected abstract IEnumerator State_IDLE(); 
    protected abstract IEnumerator State_MOVE(); 
    protected abstract IEnumerator State_ATK();  
    protected abstract IEnumerator State_DEAD(); 
                                                 
    protected abstract void Attack();            
                                                 
    protected abstract void Dead();              
                                                 
    protected abstract void Move();              
}
