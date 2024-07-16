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

    public enum ObjectType { PLAYER, FRIEND, ENEMY, TRIGGER, MERCHANT, PORTAL, SOLDIER, NULL }
    public enum StageStatus { INIT, PLAYER, ENV, FIGHT, END }
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
    public enum StatusEffect { HP, HP_PERCENT, SLIENCE, POWERFUL, INVINCIBILITY, WEAKEN, BLESS, CURSE, ALL }
    public const int SLIENCE = 0, POWERFUL = 1, INVINCIBILITY = 2, WEAKEN = 3, BLESS = 4, CURSE = 5;
    public enum StageText { ALL_INIT, END,  MONSTER, MOVE, ROTATE, WEAPON_CHANGE, MONSTER_INIT, MOVE_INIT, ROTATE_INIT, WEAPON_CHANGE_INIT }
    public static int ToInt(this StageText text)
    {
        switch (text)
        {
            case StageText.MONSTER: return 0;
            case StageText.MOVE: return 1;
            case StageText.ROTATE: return 2;
            case StageText.WEAPON_CHANGE: return 3;
            case StageText.MONSTER_INIT: return 4;
            case StageText.MOVE_INIT: return 5;
            case StageText.ROTATE_INIT: return 6;
            case StageText.WEAPON_CHANGE_INIT: return 7;
            default: return -1;
        }
    }
    public enum BingoStatus { NONE, ONE, ALL }
}
