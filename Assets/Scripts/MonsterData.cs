using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Scriptable Object/MonsterData")]
public class MonsterData : ScriptableObject
{
    // 몬스터의 생성 주기(5초)
    // 몬스터의 동시 생성 가능한 최대 수(5)
    // 몬스터의 체력(100)
    // 몬스터의 공격력(10)
    // 몬스터의 초당 공격 횟수(1)
    // 몬스터의 공격 사거리(1)

    public float SpawnTime = 5f;
    public int MaxMonsterCount = 5;
    public int MaxHp = 100;
    public int AttackPower = 10;
    public float AttackRate = 1f;
    public float AttackRange = 1f;
}
