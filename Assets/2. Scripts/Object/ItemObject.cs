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

    [SerializeField] private int spawnMinStage;
    [SerializeField] private int spawnMaxStage;
    public int SpawnMinStage { get => spawnMinStage; set => spawnMinStage = value; }
    public int SpawnMaxStage { get => spawnMaxStage; set => spawnMaxStage = value; }


    [SerializeField] private Sprite icon; // 아이템의 아이콘 or 적이나 동료 등의 초상화  일단은 색으로 대체
    public Sprite Icon { get => icon; set => icon = value; }

    [TextArea(15, 20)] public string description;

}