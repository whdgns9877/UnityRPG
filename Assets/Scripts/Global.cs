using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour
{
    // 인스턴스를 최초 한번만 생성하고 전역적으로 사용
    public static Global Inst { get; private set; }
    private void Awake() => Inst = this;

    public List<Transform> targets;
    public void AddTarget(Transform target) => targets.Add(target);
    public void RemoveTarget(Transform target) => targets.Remove(target);
}
