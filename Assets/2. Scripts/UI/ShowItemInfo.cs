using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using static Constants;

public class ShowItemInfo : MonoBehaviour
{
    [SerializeField] Image itemImage;
    [SerializeField] TMP_Text itemName;
    [SerializeField] TMP_Text itemPrice;
    [SerializeField] TMP_Text itemAtk;
    [SerializeField] TMP_Text itemDesc;

    public void SetItemInfo(ItemObject item, Slot slot)
    {
        //item type�� NULL�� ���, ����â�� ����� �ʴ´�.
        if (item.ItemType == ItemType.NULL)
            return;

        //item info panel�� ��ġ�� �����Ѵ�.
        Vector3 pos = slot.transform.position;
        pos.y += 190;
        pos.x += 5;
        gameObject.transform.position = pos;

        //item�� �ް�, item�� Ÿ�Կ� ���� �ٿ�ĳ���� �Ͽ� �˸��� ������ �����Ѵ�.
        //Ÿ�԰� �����ϰ� ������ �� �ִ� ������, Ÿ�Կ� ���� �ٸ��� �����ؾ� �� ������ �����Ͽ� �����Ѵ�.
        itemImage.sprite = item.Icon;
        itemName.text = item.ItemName;
        itemDesc.text = item.description;

        switch (slot.STYPE)
        {
            case Slot.SlotType.INVENTORY:
                itemPrice.text = $"�ǸŰ�: {item.SellCost}G";
                break;
            case Slot.SlotType.STORE_INVENTORY:
                itemPrice.text = $"���Ű�: {item.BuyCost}G";
                break;
            default:
                itemPrice.text = $"";
                break;
        }

        switch (item.ItemType) 
        {
            case ItemType.WEAPON:
                itemAtk.text = $"�⺻ ���ݷ�: {((Weapon)item).MinDamage} ~ {((Weapon)item).MaxDamage}";
                break;
            case ItemType.PORTION:
                itemAtk.text = $"ȸ���� {((Portion)item).HealRate} ~ {((Portion)item).HealRate}";
                break;
            case ItemType.SCROLL:
                itemAtk.text = $"";
                break;
            case ItemType.NULL:
                itemAtk.text = $"";
                break;
            default:
                break;
        }

        gameObject.SetActive(true);
        
    }

}
