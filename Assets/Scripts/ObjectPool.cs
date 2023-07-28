using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoSingleton<ObjectPool>
{
    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private Canvas canvas = null;
    [SerializeField] private int monsterInitPoolSize;
    [SerializeField] private int damageTextInitPoolSize;

    private Queue<GameObject> monsterPool;
    private Queue<GameObject> damageTextPool;

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

        // DamageText 오브젝트 풀 초기화
        damageTextPool = new Queue<GameObject>();
        for (int i = 0; i < damageTextInitPoolSize; i++)
        {
            GameObject damageText = Instantiate(damageTextPrefab, canvas.transform);
            damageText.SetActive(false);
            damageTextPool.Enqueue(damageText);
        }
    }

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
        return monster;
    }

    public void ReturnMonsterToPool(GameObject monster)
    {
        Global.Instacne.RemoveTarget(monster);
        monster.SetActive(false);
        monsterPool.Enqueue(monster);
    }

    public GameObject GetDamageTextFromPool()
    {
        if (damageTextPool.Count == 0)
        {
            // 풀이 비어있으면 새로운 오브젝트 생성
            GameObject newDamageText = Instantiate(damageTextPrefab, canvas.transform);
            newDamageText.SetActive(false);
            return newDamageText;
        }

        GameObject damageText = damageTextPool.Dequeue();
        damageText.SetActive(true);
        return damageText;
    }

    public void ReturnDamageTextToPool(GameObject damageText)
    {
        damageText.SetActive(false);
        damageTextPool.Enqueue(damageText);
    }
}
