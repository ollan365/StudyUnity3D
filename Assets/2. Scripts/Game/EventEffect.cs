using System.Collections.Generic;
using static Constants;
public class EventEffect
{
    private int[] effectTurn = new int[7]; // ȿ������ ���ӵǴ� ��
    private int[] effectAmount = new int[7]; // ȿ���� ũ��
    
    // 
    public void EventAdd(int eventIndex)
    {

    }
    private void EffectAdd(StatusEffect effect, int turn, int amount)
    {
        int index = Int(effect);
        // effectTurn
    }

    public int Int(StatusEffect effect)
    {
        switch (effect)
        {
            case StatusEffect.HP: return 0;
            case StatusEffect.SLIENCE: return 1;
            case StatusEffect.FATIGUE:return 2;
            case StatusEffect.SUNSHINE: return 3;
            case StatusEffect.STIGMA: return 4;
            case StatusEffect.EVASION: return 5;
            case StatusEffect.NONVIOLENCE: return 6;
            case StatusEffect.BARRIER: return 7;

            default: return -1;
        }
    }
}

public class StatusEffectStatus
{
    private bool isInfinite;
    private List<int> amount;


}
