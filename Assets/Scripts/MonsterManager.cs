using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    [SerializeField] private MonsterData monsterData; // ���� �ֱ⸦ �˱����� ���� ScriptableObject ����
    [SerializeField] private Transform[] spawnPoints; // �÷��̾� �ֺ��� Transform�� �迭��(�÷��̾� �ֺ� ����) 

    private void Start()
    {
        // 5�� ���Ŀ� SpawnTime �ֱ�� SpawnMonster() ȣ��
        InvokeRepeating(nameof(SpawnMonster), 5f, monsterData.SpawnTime);
    }

    private void SpawnMonster()
    {
        // ���� �ִ� �������� ���� �ʾ������� Ǯ���� ���� ������
        if(CheckMonsterSpawnMaxCount())
        {
            GameObject monsterObj = ObjectPool.Instacne.GetMonsterFromPool();
            monsterObj.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        }
    }

    // ���Ͱ� �ִ�� �����Ǿ��ִ����� �Ǻ��Ѵ�
    private bool CheckMonsterSpawnMaxCount()
    {
        return Global.Instacne.ActiveTargets.Count < monsterData.MaxMonsterCount;
    }
}
