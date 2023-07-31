using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoSingleton<ObjectPool>
{
    [SerializeField] private GameObject monsterPrefab;     // 거미 몬스터 프리팹
    [SerializeField] private GameObject damageTextPrefab;  // 데미지 텍스트 프리팹
    [SerializeField] private int monsterInitPoolSize;      // 초기 몬스터풀 생성개수
    
    // 오브젝트풀을 큐 자료구조로 만든다
    private Queue<GameObject> monsterPool;           

    private void Start()
    {
        // Monster 오브젝트 풀 초기화
        monsterPool = new Queue<GameObject>();
        for (int i = 0; i < monsterInitPoolSize; i++)
        {
            GameObject monster = Instantiate(monsterPrefab, transform);
            // 처음 생성시 플레이어의위치(0,0,0) 과 가까이 생성되면 State가 Attack으로 시작하게되므로
            // 멀리서 생성 후에 풀에 넣는다.
            monster.transform.position = Vector3.one * 100;
            monster.SetActive(false);
            monsterPool.Enqueue(monster);
        }
    }

    // 요청하는 측에서 몬스터풀에서 하나의 몬스터를 꺼내달라 요청
    public GameObject GetMonsterFromPool()
    {
        if (monsterPool.Count == 0)
        {
            // 풀이 비어있으면 새로운 오브젝트 생성
            GameObject newMonster = Instantiate(monsterPrefab, transform);
            newMonster.transform.position = Vector3.one * 100;
            newMonster.SetActive(false);
            return newMonster;
        }

        GameObject monster = monsterPool.Dequeue();
        monster.SetActive(true);
        // 요청한 측에 해당 큐에서 꺼낸(Deque)몬스터 반환
        return monster;
    }

    // 사용이 끝난(죽은) 몬스터를 다시 풀에 집어넣는다
    public void ReturnMonsterToPool(GameObject monster)
    {
        Global.Instacne.RemoveTarget(monster);
        monster.SetActive(false);
        monsterPool.Enqueue(monster);
    }
}
