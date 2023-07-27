using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Object/PlayerData")]
public class PlayerData : ScriptableObject
{
    // �÷��̾��� ü��(100)
    // �÷��̾��� ���ݷ�(10)
    // �÷��̾��� �ʴ� ���� Ƚ��(1)
    // �÷��̾��� ���� ��Ÿ�(1)
    // �÷��̾��� ��ų2 ����(2)
    public float Hp = 100f;
    public int AttackPower = 50;
    public float AttackRate = 1f;
    public float AttackRange = 1f;
    public float SkillAttackRange = 2f;
}
