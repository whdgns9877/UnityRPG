using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Inst { get; private set; }

    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private int monsterInitPoolSize;
    [SerializeField] private int damageTextInitPoolSize;

    private Queue<GameObject> monsterPool;
    private Queue<GameObject> damageTextPool;

    private void Awake()
    {
        Inst = this;
    }

    private void Start()
    {
        // Monster ������Ʈ Ǯ �ʱ�ȭ
        monsterPool = new Queue<GameObject>();
        for (int i = 0; i < monsterInitPoolSize; i++)
        {
            GameObject monster = Instantiate(monsterPrefab, transform);
            monster.SetActive(false);
            monsterPool.Enqueue(monster);
        }

        // DamageText ������Ʈ Ǯ �ʱ�ȭ
        damageTextPool = new Queue<GameObject>();
        for (int i = 0; i < damageTextInitPoolSize; i++)
        {
            GameObject damageText = Instantiate(damageTextPrefab, transform);
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
            newMonster.SetActive(false);
            return newMonster;
        }

        GameObject monster = monsterPool.Dequeue();
        monster.SetActive(true);
        Global.Inst.AddTarget(monster);
        return monster;
    }

    public void ReturnMonsterToPool(GameObject monster)
    {
        Global.Inst.RemoveTarget(monster);
        monster.SetActive(false);
        monsterPool.Enqueue(monster);
    }

    public GameObject GetDamageTextFromPool()
    {
        if (damageTextPool.Count == 0)
        {
            // Ǯ�� ��������� ���ο� ������Ʈ ����
            GameObject newDamageText = Instantiate(damageTextPrefab, transform);
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
