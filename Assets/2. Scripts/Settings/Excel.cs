public static class Excel
{
    public const string StageInfo = "CSV/StageInfo";
    public const string StageEnemy = "CSV/StageEnemy";
    public const string EnemyInfo = "CSV/EnemyInfo";
    public const string FriendInfo = "CSV/FriendInfo";

    // StageInfo
    public const int
        STAGE = 0,
        MIX = 2,
        MAX_HP = 3,
        REWARD = 4,
        WEAPON_CHANGE = 5,
        TREASURE_COUNT = 6,
        TREASURE_MIN = 7,
        TREASURE_MAX = 8,
        ENEMY_COUNT = 9,
        ROTATE_COUNT = 10,
        MOVE_COUNT = 11;

    // StageEnemy
    public const int
        STAGE_ENEMY_STAGE = 0,
        STAGE_ENEMY_ID = 1,
        STAGE_ENEMY_COUNT = 2;


    // EnemyInfo
    public const int
        ENEMY_ID = 0,
        ENEMY_NAME = 1,
        ENEMY_HP = 2,
        ENEMY_MIN = 3,
        ENEMY_MAX = 4,
        ENEMY_WEAPON_TYPE = 5;

    // FriendInfo
    public const int
        FRIEND_ID = 0,
        FRIEND_STAGE = 1,
        FRIEND_HP = 2,
        FRIEND_MIN = 3,
        FRIEND_MAX = 4,
        FRIEND_WEAPON_TYPE = 5;
}
