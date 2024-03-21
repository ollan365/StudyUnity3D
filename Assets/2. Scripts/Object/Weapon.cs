using UnityEngine;
using static Constants;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Items/Weapon", order = 0)]
public class Weapon : ItemObject
{
    [SerializeField] private WeaponType weaponType;
    public WeaponType WeaponType { get => weaponType; }
    [SerializeField] private int level;
    public int Level { get => level; set => level = value; }
    public int MinDamage { get => weaponLevel[Level].minDamage; }
    public int MaxDamage { get => weaponLevel[Level].maxDamage; }
    [SerializeField] private WeaponLevel[] weaponLevel;
}

[System.Serializable]
public class WeaponLevel
{
    public int level;
    public int minDamage;
    public int maxDamage;
}
