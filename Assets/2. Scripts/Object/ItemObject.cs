using UnityEngine;
using static Constants;
public abstract class ItemObject : ScriptableObject
{
    [SerializeField] private string itemName;
    public string ItemName { get => itemName; set => itemName = value; }

    [SerializeField] private int id;
    public int ID { get => id; set => id = value; }
    [SerializeField] private ItemType itemType;
    public ItemType ItemType { get => itemType; set => itemType = value; }

    [SerializeField] private int sellCost;
    [SerializeField] private int buyCost;
    public int SellCost { get => sellCost; set => sellCost = value; }
    public int BuyCost { get => buyCost; set => buyCost = value; }

    [TextArea(15, 20)] public string description;
    [SerializeField] private Color icon; // �������� ������ or ���̳� ���� ���� �ʻ�ȭ  �ϴ��� ������ ��ü
    public Color Icon { get => icon; set => icon = value; }
}