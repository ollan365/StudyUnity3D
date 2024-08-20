[System.Serializable]
public class ItemSlot
{
    public ItemObject item;
    public int count;

    public ItemSlot (ItemObject item, int count)
    {
        this.item = item;
        this.count = count;
    }

    public void Init()
    {
        item = StaticManager.Instance.nullObject;
        count = 1;
    }
}
