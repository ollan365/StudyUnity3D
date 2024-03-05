using UnityEngine;
using static Constants;

[CreateAssetMenu(fileName = "New Portion", menuName = "Items/Portion", order = 1)]
public class Portion : ItemObject
{
    [SerializeField] private StatusEffect statusEffect;
    public StatusEffect StatusEffectType { get => statusEffect; }
    [SerializeField] private int value;
    public int Value { get => value; }
}
