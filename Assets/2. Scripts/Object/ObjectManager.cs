using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static Constants;

public class ObjectManager : MonoBehaviour
{
    [Space(10f)]
    [SerializeField] private GameObject[] friendObjectStatus;
    [SerializeField] private Transform dieObject;
    [SerializeField] private Object player;
    [SerializeField] private Text goldText;

    public static ObjectManager Instance { get; private set; }
    [Header("Manager")]
    [SerializeField] private CubeManager cubeManager;


    [Header("Prefabs")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject friendPrefab;
    [SerializeField] private GameObject treasurePrefab;
    [SerializeField] private GameObject merchantPrefab;
    [SerializeField] private GameObject portalPrefab;



    private ItemObject[] allShopWeapon;
    private ItemObject[] allShopPortion;
    private ItemObject[] allShopScroll;

    [Header("UI")]
    [SerializeField] private Slider[] friendHpSlider;
    [SerializeField] private GameObject shopPopup;
    [SerializeField] private GameObject[] inventorySlotButton;
    [SerializeField] private GameObject[] shopSlotButton;
    private ItemSlot[] shopItemArray;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        shopItemArray = new ItemSlot[16];
    }
    
    public GameObject Summons(Touch cube, ObjectType objectType, int objectID)
    {
        GameObject newObject;

        switch (objectType) {
            case ObjectType.ENEMY:
                newObject = Instantiate(enemyPrefab);
                string value = StaticManager.Instance.enemyDatas[objectID];
                newObject.GetComponent<Object>().Init(objectType, value.Split(','));
                break;
            case ObjectType.FRIEND:
                newObject = Instantiate(friendPrefab);
                for (int i = 0; i < 3; i++)
                    if (!friendObjectStatus[i].activeSelf)
                    {
                        newObject.GetComponent<Object>().bottomHP = friendHpSlider[i];
                        friendObjectStatus[i].SetActive(true);
                        break;
                    }
                Dictionary<int, string> values = StaticManager.Instance.friendDatas[objectID];
                value = values[StaticManager.Instance.Stage];
                Debug.Log($"{value}");
                newObject.GetComponent<Object>().Init(objectType, value.Split(','));
                ChangePlayerInventory();
                break;
            case ObjectType.TREASURE:
                newObject = Instantiate(treasurePrefab);
                newObject.GetComponent<Object>().Init(objectType, null);
                break;
            case ObjectType.MERCHANT:
                newObject = Instantiate(merchantPrefab);
                newObject.GetComponent<Object>().Init(objectType, null);
                break;
            case ObjectType.PORTAL:
                newObject = Instantiate(portalPrefab);
                newObject.GetComponent<Object>().Init(objectType, null);
                break;
            default:
                newObject = null;
                break;
        }

        newObject.transform.parent = cube.ObjectPostion;
        newObject.transform.position = cube.ObjectPostion.position;
        newObject.transform.rotation = cube.ObjectPostion.rotation;

        newObject.GetComponent<Object>().objectManager = this;
        newObject.GetComponent<Object>().touchCube = cube;

        return newObject;
    }

    public void ObjectDie(GameObject obj)
    {
        if (obj.GetComponent<Object>().Type == ObjectType.FRIEND)
            for (int i = 0; i < 3; i++)
                if (obj.GetComponent<Object>().bottomHP == friendHpSlider[i])
                {
                    friendObjectStatus[i].SetActive(false);
                    break;
                }

        // 죽은 애들은 현재 그 개수 맞추기를 위해서 일단은 다른 곳에 옮겨둔다
        // 나중에 공격 시스템만 어째 바꾸면 그냥 destroy 해도 될듯
        obj.transform.position = dieObject.position;
        obj.transform.parent = dieObject;
    }

    public void ChangePlayerInventory()
    {
        goldText.text = StaticManager.Instance.Gold.ToString();

        for (int i = 0; i < StaticManager.Instance.inventory.Length; i++)
        {
            if (StaticManager.Instance.inventory[i].count == 0) StaticManager.Instance.inventory[i].init();

            switch (StaticManager.Instance.inventory[i].item.ItemType)
            {
                case ItemType.WEAPON:
                    inventorySlotButton[i].GetComponent<Image>().color = StaticManager.Instance.inventory[i].item.Icon;
                    if (StaticManager.Instance.PlayerWeapon == (Weapon)StaticManager.Instance.inventory[i].item)
                        inventorySlotButton[i].transform.Find("Item Count").GetComponent<Text>().text = "use";
                    else
                        inventorySlotButton[i].transform.Find("Item Count").GetComponent<Text>().text = "";
                    inventorySlotButton[i].SetActive(true);
                    break;
                case ItemType.PORTION:
                    inventorySlotButton[i].GetComponent<Image>().color = StaticManager.Instance.inventory[i].item.Icon;
                    inventorySlotButton[i].transform.Find("Item Count").GetComponent<Text>().text
                        = StaticManager.Instance.inventory[i].count.ToString();
                    inventorySlotButton[i].SetActive(true);
                    break;
                case ItemType.SCROLL:
                    inventorySlotButton[i].GetComponent<Image>().color = StaticManager.Instance.inventory[i].item.Icon;
                    inventorySlotButton[i].transform.Find("Item Count").GetComponent<Text>().text
                        = StaticManager.Instance.inventory[i].count.ToString();
                    inventorySlotButton[i].SetActive(true);
                    break;
                case ItemType.NULL:
                    inventorySlotButton[i].SetActive(false);
                    break;
            }
        }
    }
    public void ClickInventoryBTN(int index)
    {
        switch (StaticManager.Instance.inventory[index].item.ItemType)
        {
            case ItemType.WEAPON:
                if (StaticManager.Instance.PlayerWeapon != (Weapon)StaticManager.Instance.inventory[index].item && StageManager.Instance.GetStageTextValue(StageText.WEAPON_CHANGE) > 0)
                {
                    StaticManager.Instance.PlayerWeapon = (Weapon)StaticManager.Instance.inventory[index].item;
                    StageManager.Instance.SetStageTextValue(StageText.WEAPON_CHANGE, -1);
                }
                break;
            case ItemType.PORTION:
                cubeManager.SwitchPlayerTurnStatus(StaticManager.Instance.inventory[index].item.ID, ItemType.PORTION);
                break;
            case ItemType.SCROLL:
                cubeManager.SwitchPlayerTurnStatus(StaticManager.Instance.inventory[index].item.ID, ItemType.SCROLL);
                break;
        }
        ChangePlayerInventory();
    }
    public void UseItem(ItemType itemType, int itemID)
    {
        for(int i = 0; i < StaticManager.Instance.inventory.Length; i++)
        {
            if(itemType == StaticManager.Instance.inventory[i].item.ItemType && itemID == StaticManager.Instance.inventory[i].item.ID)
            {
                StaticManager.Instance.inventory[i].count--;
                break;
            }
        }
        ChangePlayerInventory();
    }
    public void ChangeShop()
    {
        for(int i = 0; i<shopItemArray.Length; i++)
        {
            if (i < 4)
            {
                int random = Random.Range(0, allShopWeapon.Length);

                shopItemArray[i] = new(allShopWeapon[random], allShopWeapon[random].SellCost);
                shopSlotButton[i].GetComponent<Image>().color = allShopWeapon[random].Icon;
                shopSlotButton[i].transform.Find("Item Count").GetComponent<Text>().text
                    = allShopWeapon[random].SellCost.ToString(); 
            }
            else if (i < 8)
            {
                int random = Random.Range(0, allShopPortion.Length);

                shopItemArray[i] = new(allShopPortion[random], allShopPortion[random].SellCost);
                shopSlotButton[i].GetComponent<Image>().color = allShopPortion[random].Icon;
                shopSlotButton[i].transform.Find("Item Count").GetComponent<Text>().text
                    = allShopPortion[random].SellCost.ToString();
            }
            else
            {
                int random = Random.Range(0, allShopScroll.Length);

                shopItemArray[i] = new(allShopScroll[random], allShopScroll[random].SellCost);
                shopSlotButton[i].GetComponent<Image>().color = allShopScroll[random].Icon;
                shopSlotButton[i].transform.Find("Item Count").GetComponent<Text>().text
                    = allShopScroll[random].SellCost.ToString();
            }
        }

        shopPopup.SetActive(true);
    }
    public void Buy(int index)
    {
        if (StaticManager.Instance.Gold < shopItemArray[index].count) return;

        for (int i = 0; i < StaticManager.Instance.inventory.Length; i++)
        {
            if (StaticManager.Instance.inventory[i].item == shopItemArray[index].item)
            {
                if (shopItemArray[index].item.ItemType == ItemType.WEAPON) return; // 무기는 하나만 소유 가능

                StaticManager.Instance.inventory[i].count++;
                inventorySlotButton[i].transform.Find("Item Count").GetComponent<Text>().text
                    = StaticManager.Instance.inventory[i].count.ToString();
                StaticManager.Instance.Gold -= shopItemArray[index].count;
                shopSlotButton[index].SetActive(false);
                ChangePlayerInventory();
                break;
            }
            else if (StaticManager.Instance.inventory[i].item.ItemType == ItemType.NULL)
            {
                StaticManager.Instance.inventory[i].item = shopItemArray[index].item;
                StaticManager.Instance.inventory[i].count = 1;
                inventorySlotButton[i].transform.Find("Item Count").GetComponent<Text>().text
                    = StaticManager.Instance.inventory[i].count.ToString();
                StaticManager.Instance.Gold -= shopItemArray[index].count;
                shopSlotButton[index].SetActive(false);
                ChangePlayerInventory();
                break;
            }
        }
    }
}
