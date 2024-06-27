using static Constants;
public class EventEffect
{
    private int[] effectTurn = new int[7]; // ȿ������ ���ӵǴ� ��
    private int[] effectAmount = new int[7]; // ȿ���� ũ��
    
    
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
            case StatusEffect.SLIENCE: return 2; // ħ��
            case StatusEffect.INVINCIBILITY: return 3; // ����
            case StatusEffect.FATIGUE:return 4; // �Ƿ� (��ȭ)
            case StatusEffect.SUNSHINE: return 5; // Ȱ��
            case StatusEffect.STIGMA: return 6; // ���� (���)
            case StatusEffect.EVASION: return 7; // ȸ��

            default: return -1; // ALL
        }
    }
}
