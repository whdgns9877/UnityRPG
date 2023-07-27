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
            GameObject monsterObj = ObjectPool.Inst.GetMonsterFromPool();
            monsterObj.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length - 1)].position;
        }
    }

    private bool CheckMonsterSpawnMaxCount()
    {
        if (Global.Inst.targets.Count >= monsterData.MaxMonsterCount)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}