using UnityEngine;
using static Constants;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Items/Weapon", order = int.MaxValue)]
public class Weapon : ItemObject
{
    [SerializeField] private WeaponType weaponType;
    public WeaponType WeaponType { get => weaponType; }
    [SerializeField] private int minDamage;
    [SerializeField] private int maxDamage;

    [SerializeField] private Material objectMaterial; // ������Ʈ�� �̹��� ���� ��´� ������ Material

    private void Awake()
    {
        itemType = ItemType.WEAPON;
    }
    public Material ObjectMaterial { get => objectMaterial; }
    public int WeaponDamage
    {
        get => Random.Range(minDamage, maxDamage + 1);
    }
}
