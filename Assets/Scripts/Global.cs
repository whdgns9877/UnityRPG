using System.Collections.Generic;
using UnityEngine;

public class Global : MonoSingleton<Global>
{
    public List<GameObject> targets;
    private void Awake()
    {
        targets = new List<GameObject> ();
    }

    public void AddTarget(GameObject target) => targets.Add(target);
    public void RemoveTarget(GameObject target) => targets.Remove(target);
}
