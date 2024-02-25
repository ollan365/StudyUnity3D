using UnityEngine;
using static Constants;

[CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Object/Weapon", order = int.MaxValue)]
public class Weapon : ScriptableObject
{
    [SerializeField] private WeaponType weaponType;
    public WeaponType WeaponType { get => weaponType; }
    [SerializeField] private int minDamage;
    [SerializeField] private int maxDamage;

    [SerializeField] private Material objectMaterial; // 오브젝트의 이미지 등을 담는다 지금은 Material

    public Material ObjectMaterial { get => objectMaterial; }
    public int WeaponDamage
    {
        get => Random.Range(minDamage, maxDamage + 1);
    }
}
