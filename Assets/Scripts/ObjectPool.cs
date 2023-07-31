using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoSingleton<ObjectPool>
{
    [SerializeField] private GameObject monsterPrefab;     // �Ź� ���� ������
    [SerializeField] private GameObject damageTextPrefab;  // ������ �ؽ�Ʈ ������
    [SerializeField] private int monsterInitPoolSize;      // �ʱ� ����Ǯ ��������
    
    // ������ƮǮ�� ť �ڷᱸ���� �����
    private Queue<GameObject> monsterPool;           

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
    }

    // ��û�ϴ� ������ ����Ǯ���� �ϳ��� ���͸� �����޶� ��û
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
        // ��û�� ���� �ش� ť���� ����(Deque)���� ��ȯ
        return monster;
    }

    // ����� ����(����) ���͸� �ٽ� Ǯ�� ����ִ´�
    public void ReturnMonsterToPool(GameObject monster)
    {
        Global.Instacne.RemoveTarget(monster);
        monster.SetActive(false);
        monsterPool.Enqueue(monster);
    }
}
