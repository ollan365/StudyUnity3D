using UnityEngine;

[CreateAssetMenu(fileName = "New Portion", menuName = "Items/ETC", order = 2)]
public class ETC : ItemObject
{
    [SerializeField] private int value;
    [SerializeField] private Material objectMaterial; // ������Ʈ�� �̹��� ���� ��´� ������ Material
    public Material ObjectMaterial { get => objectMaterial; }

}
