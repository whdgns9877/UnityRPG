using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    [SerializeField] private MonsterData monsterData; // 스폰 주기를 알기위해 몬스터 ScriptableObject 참조
    [SerializeField] private Transform[] spawnPoints; // 플레이어 주변의 Transform을 배열로(플레이어 주변 생성) 

    private void Start()
    {
        // 5초 이후에 SpawnTime 주기로 SpawnMonster() 호출
        InvokeRepeating(nameof(SpawnMonster), 5f, monsterData.SpawnTime);
    }

    private void SpawnMonster()
    {
        // 아직 최대 스폰수가 되지 않았을때만 풀에서 몬스터 꺼내옴
        if(CheckMonsterSpawnMaxCount())
        {
            GameObject monsterObj = ObjectPool.Instacne.GetMonsterFromPool();
            monsterObj.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        }
    }

    // 몬스터가 최대로 스폰되어있는지를 판별한다
    private bool CheckMonsterSpawnMaxCount()
    {
        return Global.Instacne.ActiveTargets.Count < monsterData.MaxMonsterCount;
    }
}
