using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    [SerializeField] private MonsterData monsterData;
    [SerializeField] private Transform[] spawnPoints;

    private void Start()
    {
        // 5초 이후에 SpawnTime 주기로 SpawnMonster() 호출
        InvokeRepeating(nameof(SpawnMonster), 5f, monsterData.SpawnTime);
    }

    private void SpawnMonster()
    {
        // 아직 최대 스폰수가 되지 않았을때만 풀에서 몬스터 꺼내옴
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
