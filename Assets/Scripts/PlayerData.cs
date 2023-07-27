using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Object/PlayerData")]
public class PlayerData : ScriptableObject
{
    // 플레이어의 체력(100)
    // 플레이어의 공격력(10)
    // 플레이어의 초당 공격 횟수(1)
    // 플레이어의 공격 사거리(1)
    // 플레이어의 스킬2 범위(2)
    public float Hp = 100f;
    public int AttackPower = 50;
    public float AttackRate = 1f;
    public float AttackRange = 1f;
    public float SkillAttackRange = 2f;
}
