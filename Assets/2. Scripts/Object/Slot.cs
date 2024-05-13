using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] int index;
    public enum SlotType { INVENTORY, STORE_INVENTORY, STORE }
    [SerializeField] private SlotType slotType;

    [SerializeField] private GameObject item;
    [SerializeField] private GameObject itemImage;
    [SerializeField] private GameObject equip;
    [SerializeField] private GameObject count;

    private void Start()
    {
        if (slotType == SlotType.INVENTORY)
        {
            itemImage.GetComponent<Button>().onClick.AddListener(() => ObjectManager.Instance.ClickInventoryBTN(index));
        }
        else if (slotType == SlotType.STORE)
        {
            itemImage.GetComponent<Button>().onClick.AddListener(() => ObjectManager.Instance.Buy(index));
        }
    }
    public void SetActive(bool on)
    {
        item.SetActive(on);
    }
    public void ChangeImage(Color color)
    {
        itemImage.GetComponent<Image>().color = color;
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
            count.GetComponentInChildren<Text>().text = text;
        }
    }
}
