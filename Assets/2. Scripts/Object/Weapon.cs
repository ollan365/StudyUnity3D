using UnityEngine;
using static Constants;

[CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Object/Weapon", order = int.MaxValue)]
public class Weapon : ScriptableObject
{
    [SerializeField] private WeaponType weaponType;
    public WeaponType WeaponType { get => weaponType; }
    [SerializeField] private int minDamage;
    [SerializeField] private int maxDamage;
    public int WeaponDamage
    {
        get => Random.Range(minDamage, maxDamage + 1);
    }
}
