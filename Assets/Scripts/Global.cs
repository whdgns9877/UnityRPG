using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour
{
    // �ν��Ͻ��� ���� �ѹ��� �����ϰ� ���������� ���
    public static Global Inst { get; private set; }
    private void Awake() => Inst = this;

    public List<Transform> targets;
    public void AddTarget(Transform target) => targets.Add(target);
    public void RemoveTarget(Transform target) => targets.Remove(target);
}
