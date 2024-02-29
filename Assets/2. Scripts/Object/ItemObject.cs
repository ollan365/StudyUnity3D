using UnityEngine;
using static Constants;
public abstract class ItemObject : ScriptableObject
{
    public int Id;
    public ItemType itemType;
    [TextArea(15, 20)] public string description;
}