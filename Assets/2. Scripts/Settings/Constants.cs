public static class Constants
{
    public const int InventorySize = 16;
    public enum Colors { WHITE, RED, BLUE, GREEN, ORANGE, YELLOW, WY, RO, BG, NULL }
    public static int ToInt(this Colors color)
    {
        switch (color)
        {
            case Colors.WHITE: return 0;
            case Colors.RED: return 1;
            case Colors.BLUE: return 2;
            case Colors.GREEN: return 3;
            case Colors.ORANGE: return 4;
            case Colors.YELLOW: return 5;
            case Colors.WY: return 6;
            case Colors.RO: return 7;
            case Colors.BG: return 8;
            default: return -1;
        }
    }
    public const int WHITE = 0, RED = 1, BLUE = 2, GREEN = 3, ORANGE = 4, YELLOW = 5;
    public const int WY = 6, RO = 7, BG = 8;
    public static Colors ToColor(this int color)
    {
        switch (color)
        {
            case 0: return Colors.WHITE;
            case 1: return Colors.RED;
            case 2: return Colors.BLUE;
            case 3: return Colors.GREEN;
            case 4: return Colors.ORANGE;
            case 5: return Colors.YELLOW;
            case 6: return Colors.WY;
            case 7: return Colors.RO;
            case 8: return Colors.BG;
            default: return Colors.NULL;
        }
    }

    public enum ObjectType { PLAYER, FRIEND, ENEMY, TREASURE, MERCHANT, PORTAL, NULL }
    public enum StageStatus { INIT, PLAYER, FIGHT, END }
    public enum WeaponType { CAD, LAD, AP, NULL }
    public static WeaponType ToEnum(this string weaponType)
    {
        switch (weaponType)
        {
            case "CAD": return WeaponType.CAD;
            case "LAD": return WeaponType.LAD;
            case "AP": return WeaponType.AP;
            default: return WeaponType.NULL;
        }
    }
    public enum ItemType { WEAPON, PORTION, SCROLL, NULL }
    public enum StatusEffect { HP, SLIENCE, BLIND, RESCTRICTION, BETRAYAL, ALL }
    public enum StageText { MONSTER, MOVE, ROTATE, WEAPON_CHANGE, ALL }
    public enum BingoEffect { SILENCE, RECOVERY, ADD_REWEAPON, ADD_ROTATE, GOLD, ALL_KILL }
    public enum BingoStatus { DEFAULT, ONE_1, ONE_2, ONE_3, ALL_1, ALL_2, ALL_3 }
    public const int BINGO_DEFAULT = 0, BINGO_ONE = 1, BINGO_ALL = 2;

}
