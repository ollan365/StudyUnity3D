using UnityEngine;
using static Constants;
public abstract class ItemObject : ScriptableObject
{
    [SerializeField] private int id;
    public int ID { get => id; set => id = value; }
    [SerializeField] private ItemType itemType;
    public ItemType ItemType { get => itemType; set => itemType = value; }
    [SerializeField] private int cost;
    public int Cost { get => cost; set => cost=value; }
    [TextArea(15, 20)] public string description;
    [SerializeField] private Color icon; // �������� ������ or ���̳� ���� ���� �ʻ�ȭ  �ϴ��� ������ ��ü
    public Color Icon { get => icon; set => icon = value; }
}