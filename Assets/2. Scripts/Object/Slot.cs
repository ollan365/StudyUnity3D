using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] int index;
    public enum SlotType { INVENTORY, STORE_INVENTORY, STORE }
    [SerializeField] private SlotType slotType;
    public SlotType STYPE { get => slotType; }

    [SerializeField] private GameObject item;
    [SerializeField] private GameObject itemImage;
    [SerializeField] private GameObject equip;
    [SerializeField] private GameObject count;
    [SerializeField] private GameObject itemInfoPanel;

    private void Start()
    {
        if (slotType == SlotType.INVENTORY)
        {
            itemImage.GetComponent<Button>().onClick.AddListener(() => ObjectManager.Instance.ClickInventoryBTN(index));
        }
        else if (slotType == SlotType.STORE)
        {
            itemImage.GetComponent<Button>().onClick.AddListener(() => ObjectManager.Instance.BuyItem(index));
        }
    }
    public void SetActive(bool on)
    {
        item.SetActive(on);
    }
    public void ChangeImage(Sprite sprite)
    {
        itemImage.GetComponent<Image>().sprite = sprite;
    }
    public void ChangeText(string text)
    {
        if (text == "use")
        {
            equip.SetActive(true);
            count.SetActive(false);
        }
        else if (text == "unuse")
        {
            equip.SetActive(false);
            count.SetActive(false);
        }
        else
        {
            equip.SetActive(false);
            count.SetActive(true);
            count.GetComponentInChildren<TMP_Text>().text = text;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {


        //슬롯 타입에 따라 다른 인벤토리를 참조하여 itemInfoPanel을 구성한다.
        switch (slotType)
        {
            case SlotType.INVENTORY:
            case SlotType.STORE_INVENTORY:
                itemInfoPanel.GetComponent<ShowItemInfo>().SetItemInfo(StaticManager.Instance.inventory[index].item, gameObject.GetComponent<Slot>());
                break;
            case SlotType.STORE:
                itemInfoPanel.GetComponent<ShowItemInfo>().SetItemInfo(ObjectManager.Instance.ShopItemSlotArray[index].item, gameObject.GetComponent<Slot>());
                break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        itemInfoPanel.SetActive(false);
    }



}
