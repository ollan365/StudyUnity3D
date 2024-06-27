using static Constants;
public class EventEffect
{
    private int[] effectTurn = new int[7]; // 효과들이 지속되는 턴
    private int[] effectAmount = new int[7]; // 효과의 크기
    
    
    public void EffectAdd(StatusEffect effect, int turn)
    {
        int index = Int(effect);
        effectTurn[index] += turn;
    }

    public int Int(StatusEffect effect)
    {
        switch (effect)
        {
            case StatusEffect.HP: return 0;
            case StatusEffect.HP_PERCENT: return 1;
            case StatusEffect.SLIENCE: return 2; // 침묵
            case StatusEffect.INVINCIBILITY: return 3; // 무적
            case StatusEffect.FATIGUE:return 4; // 피로 (약화)
            case StatusEffect.SUNSHINE: return 5; // 활력
            case StatusEffect.STIGMA: return 6; // 낙인 (취약)
            case StatusEffect.EVASION: return 7; // 회피

            default: return -1; // ALL
        }
    }
}
