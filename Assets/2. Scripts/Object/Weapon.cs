using UnityEngine;
using static Constants;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Items/Weapon", order = 0)]
public class Weapon : ItemObject
{
    [SerializeField] private WeaponType weaponType;
    public WeaponType WeaponType { get => weaponType; }
    [SerializeField] private int minDamage;
    public int MinDamage { get => minDamage; }
    [SerializeField] private int maxDamage;
    public int MaxDamage { get => maxDamage; }
}
