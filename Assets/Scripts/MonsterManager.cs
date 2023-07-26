using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    [SerializeField] private MonsterData monsterData;
    [SerializeField] private Transform[] spawnPoints;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnMonster), 0f, monsterData.SpawnTime);
    }

    private void SpawnMonster()
    {
        // 아직 최대 스폰수가 되지 않았을때만 풀에서 몬스터 꺼내옴
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
