using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour
{
    public List<GameObject> targets;
    // 인스턴스를 최초 한번만 생성하고 전역적으로 사용
    public static Global Inst { get; private set; }
    private void Awake()
    {
        targets = new List<GameObject> ();
        Inst = this;
    }

    public void AddTarget(GameObject target) => targets.Add(target);
    public void RemoveTarget(GameObject target) => targets.Remove(target);
}
