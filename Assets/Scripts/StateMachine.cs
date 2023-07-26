using System.Collections;
using UnityEngine;
using static EnumTypes;

public abstract class StateMachine : MonoBehaviour
{
    protected State state;
    protected Transform target;

    // Ȱ��ȭ(ó�� ���� or ������ƮǮ���� �ٽ� ��ȯ�Ǿ� ���ö�)
    protected virtual void Start()
    {
        state = State.IDLE;
        StartCoroutine("State_" + state);
    }

    protected void TransferState(State nextState)
    {
        // ���� State�� �ڷ�ƾ�� ������Ű��
        StopCoroutine("State_" + state);
        // State�� �������ص�
        state = nextState;
        // �ش� State�� �ڷ�ƾ�� �����Ų��.
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
