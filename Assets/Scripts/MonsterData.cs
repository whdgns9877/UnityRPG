using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Scriptable Object/MonsterData")]
public class MonsterData : ScriptableObject
{
    // ������ ���� �ֱ�(5��)
    // ������ ���� ���� ������ �ִ� ��(5)
    // ������ ü��(100)
    // ������ ���ݷ�(10)
    // ������ �ʴ� ���� Ƚ��(1)
    // ������ ���� ��Ÿ�(1)

    public float SpawnTime = 5f;
    public int MaxMonsterCount = 5;
    public int MaxHp = 100;
    public int AttackPower = 10;
    public float AttackRate = 1f;
    public float AttackRange = 1f;
}
