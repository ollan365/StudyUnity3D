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
    [SerializeField] private GameObject weaponObject;

    public void ChangeWeaponType(bool dual)
    {
        if (dual)
        {
            if (weaponType == WeaponType.SWORD)
                weaponType = WeaponType.DUAL_SWORD;

            if (weaponType == WeaponType.STAFF)
                weaponType = WeaponType.DUAL_STAFF;
        }

        if (!dual)
        {
            if (weaponType == WeaponType.DUAL_SWORD)
                weaponType = WeaponType.SWORD;

            if (weaponType == WeaponType.DUAL_STAFF)
                weaponType = WeaponType.STAFF;
        }
    }
}

[System.Serializable]
public class WeaponLevel
{
    public int level;
    public int minDamage;
    public int maxDamage;
}
