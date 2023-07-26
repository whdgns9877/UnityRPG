using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour
{
    public List<GameObject> targets;
    // �ν��Ͻ��� ���� �ѹ��� �����ϰ� ���������� ���
    public static Global Inst { get; private set; }
    private void Awake()
    {
        targets = new List<GameObject> ();
        Inst = this;
    }

    public void AddTarget(GameObject target) => targets.Add(target);
    public void RemoveTarget(GameObject target) => targets.Remove(target);
}
