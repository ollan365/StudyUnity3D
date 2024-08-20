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
        //item type이 NULL인 경우, 정보창을 띄우지 않는다.
        if (item.ItemType == ItemType.NULL)
            return;

        //item info panel의 위치를 설정한다.
        Vector3 pos = slot.transform.position;
        pos.y += 190;
        pos.x += 5;
        gameObject.transform.position = pos;

        //item을 받고, item의 타입에 따라 다운캐스팅 하여 알맞은 정보를 설정한다.
        //타입과 무관하게 설정할 수 있는 정보와, 타입에 따라 다르게 설정해야 할 정보를 구분하여 설정한다.
        itemImage.sprite = item.Icon;
        itemName.text = item.ItemName;
        itemDesc.text = item.description;

        switch (slot.STYPE)
        {
            case Slot.SlotType.INVENTORY:
                itemPrice.text = $"판매가: {item.SellCost}G";
                break;
            case Slot.SlotType.STORE_INVENTORY:
                itemPrice.text = $"구매가: {item.BuyCost}G";
                break;
            default:
                itemPrice.text = $"";
                break;
        }

        switch (item.ItemType) 
        {
            case ItemType.WEAPON:
                itemAtk.text = $"기본 공격력: {((Weapon)item).MinDamage} ~ {((Weapon)item).MaxDamage}";
                break;
            case ItemType.PORTION:
                itemAtk.text = $"회복량 {((Portion)item).HealRate} ~ {((Portion)item).HealRate}";
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
