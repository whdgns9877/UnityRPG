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
        // Monster ������Ʈ Ǯ �ʱ�ȭ
        monsterPool = new Queue<GameObject>();
        for (int i = 0; i < monsterInitPoolSize; i++)
        {
            GameObject monster = Instantiate(monsterPrefab, transform);
            // ó�� ������ �÷��̾�����ġ(0,0,0) �� ������ �����Ǹ� State�� Attack���� �����ϰԵǹǷ�
            // �ָ��� ���� �Ŀ� Ǯ�� �ִ´�.
            monster.transform.position = Vector3.one * 100;
            monster.SetActive(false);
            monsterPool.Enqueue(monster);
        }

        // DamageText ������Ʈ Ǯ �ʱ�ȭ
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
            // Ǯ�� ��������� ���ο� ������Ʈ ����
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
            // Ǯ�� ��������� ���ο� ������Ʈ ����
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
