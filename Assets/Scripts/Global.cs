using System.Collections.Generic;
using UnityEngine;

public class Global : MonoSingleton<Global>
{
    // 플레이어가 사용할 활성화된 몬스터 리트스
    public List<GameObject> ActiveTargets;

    private void Awake()
    {
        ActiveTargets = new List<GameObject> ();
    }

    // 몬스터를 리스트에 추가
    public void AddTarget(GameObject target) => ActiveTargets.Add(target);
    // 몬스터를 리스트에서 제거
    public void RemoveTarget(GameObject target) => ActiveTargets.Remove(target);
}
