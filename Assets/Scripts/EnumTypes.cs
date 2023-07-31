public static class EnumTypes
{
    // 몬스터와 플레이어의 상태(STATE)를 나타낼 EnumType
    public enum State
    {
        IDLE, 
        MOVE,
        ATK, 
        DEAD
    }

    // 플레이어의 공격상태를 나타낼 EnumType
    public enum AttackPattern
    {
        BASIC,
        RANGEATTACK,
        HEAL
    }
}
