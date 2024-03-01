using UnityEngine;

[CreateAssetMenu(fileName = "New Portion", menuName = "Items/Portion", order = 1)]
public class Portion : ItemObject
{
    [SerializeField] private int value;
    [SerializeField] private Material objectMaterial; // ������Ʈ�� �̹��� ���� ��´� ������ Material
    public Material ObjectMaterial { get => objectMaterial; }

}
