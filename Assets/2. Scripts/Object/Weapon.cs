using UnityEngine;
using static Constants;

[CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Object/Weapon", order = int.MaxValue)]
public class Weapon : ScriptableObject
{
    [SerializeField] private WeaponType weaponType;
    public WeaponType WeaponType { get => weaponType; }
    [SerializeField] private int damage;
    public int WeaponDamage { get => damage; }
}
