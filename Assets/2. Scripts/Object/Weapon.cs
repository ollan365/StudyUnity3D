using UnityEngine;
using static Constants;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Items/Weapon", order = 0)]
public class Weapon : ItemObject
{
    [SerializeField] private WeaponType weaponType;
    public WeaponType WeaponType { get => weaponType; }
    [SerializeField] private float maxHP;
    public float MaxHP { get => maxHP; }
    [SerializeField] private int minDamage;
    [SerializeField] private int maxDamage;
    [SerializeField] private Material objectMaterial; // ������Ʈ�� �̹��� ���� ��´� ������ Material
    public Material ObjectMaterial { get => objectMaterial; }
    public int WeaponDamage
    {
        get => Random.Range(minDamage, maxDamage + 1);
    }
    public void SetDamage(int min, int max)
    {
        minDamage = min;
        maxDamage = max;
    }
}
