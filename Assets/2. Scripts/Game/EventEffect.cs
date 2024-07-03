using UnityEngine;
using static Constants;

[CreateAssetMenu(fileName = "New EventEffect", menuName = "ScriptableObject/EventEffect")]
public class EventEffect : ScriptableObject
{
    [SerializeField] private int[] effectTurn = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 }; // ȿ������ ���ӵǴ� ��
    public int[] EffectTurn { get => effectTurn; }
    public void Init()
    {
        for (int i = 0; i < effectTurn.Length; i++) effectTurn[i] = 0;
    }
    public void NextTurn()
    {
        for (int i = 0; i < effectTurn.Length; i++)
            if (effectTurn[i] > 0) effectTurn[i]--;
    }
    public float Dealt() // �긦 ���ϸ� �ִ� ������ ����
    {
        if (effectTurn[Int(StatusEffect.SLIENCE)] > 0) return 0;

        float rate = 1;

        if (effectTurn[Int(StatusEffect.POWERFUL)] > 0) rate += 0.25f;
        if (effectTurn[Int(StatusEffect.WEAKEN)] > 0) rate -= 0.5f;
        if (effectTurn[Int(StatusEffect.FATIGUE)] > 0) rate -= 0.25f;
        if (effectTurn[Int(StatusEffect.SUNSHINE)] > 0) rate += 0.5f;

        return rate;
    }
    public float Received() // �긦 ���ϸ� �޴� ������ ����
    {
        bool isInvincibility = false, isStigma = false;
        if (effectTurn[Int(StatusEffect.EVASION)] > 0)
        {
            int random = Random.Range(0, 100);
            if (random < 50) isInvincibility = true;
            else isStigma = true;
        }

        if (effectTurn[Int(StatusEffect.INVINCIBILITY)] > 0 || isInvincibility) return 0;

        float rate = 1;

        if (effectTurn[Int(StatusEffect.STIGMA)] > 0 || isStigma) rate += 0.25f;
        if (effectTurn[Int(StatusEffect.BARRIER)] > 0) rate -= 0.5f;

        return rate;
    }
    public void EffectAdd(StatusEffect effect, int turn)
    {
        int index = Int(effect);
        effectTurn[index] += turn;
    }

    public int Int(StatusEffect effect)
    {
        switch (effect)
        {
            case StatusEffect.SLIENCE: return 0; // ħ��
            case StatusEffect.POWERFUL: return 1; // ��ȭ
            case StatusEffect.WEAKEN: return 2; // ��ȭ
            case StatusEffect.INVINCIBILITY: return 3; // ����
            case StatusEffect.STIGMA: return 4; // ���
            case StatusEffect.BARRIER: return 5; // ���
            case StatusEffect.EVASION: return 6; // ȸ��
            case StatusEffect.FATIGUE:return 7; // �Ƿ�
            case StatusEffect.SUNSHINE: return 8; // ��Ȱ

            default: return -1; // ALL
        }
    }
}
