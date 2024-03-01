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
}