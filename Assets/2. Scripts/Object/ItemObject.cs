using UnityEngine;
using static Constants;
public abstract class ItemObject : ScriptableObject
{
    [SerializeField] private int id;
    public int ID { get => id; }
    [SerializeField] private ItemType itemType;
    public ItemType ItemType { get => itemType; }
    [SerializeField] private int cost;
    public int Cost { get => cost; }
    [TextArea(15, 20)] public string description;
    [SerializeField] private Color icon; // 아이템의 아이콘 or 적이나 동료 등의 초상화  일단은 색으로 대체
    public Color Icon { get => icon; }
}