using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    [SerializeField] private MonsterData monsterData;
    [SerializeField] private Transform[] spawnPoints;

    private void Start()
    {
        // 5�� ���Ŀ� SpawnTime �ֱ�� SpawnMonster() ȣ��
        InvokeRepeating(nameof(SpawnMonster), 5f, monsterData.SpawnTime);
    }

    private void SpawnMonster()
    {
        // ���� �ִ� �������� ���� �ʾ������� Ǯ���� ���� ������
        if(true == CheckMonsterSpawnMaxCount())
        {
            GameObject monsterObj = ObjectPool.Instacne.GetMonsterFromPool();
            monsterObj.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        }
    }

    private bool CheckMonsterSpawnMaxCount()
    {
        if (Global.Instacne.targets.Count >= monsterData.MaxMonsterCount)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
