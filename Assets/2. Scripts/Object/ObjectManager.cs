using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static Constants;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Instance { get; private set; }
    [SerializeField] private CubeManager cubeManager;
    [SerializeField] private Object player;
    [SerializeField] private Text goldText;

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject friendPrefab;
    [SerializeField] private GameObject treasurePrefab;
    [SerializeField] private GameObject merchantPrefab;
    [SerializeField] private GameObject portalPrefab;

    [SerializeField] private GameObject[] friendObjectStatus;
    [SerializeField] private Slider[] friendHpSlider;

    [SerializeField] private Transform dieObject;

    [SerializeField] private GameObject shopPopup;
    private ItemObject[] allShopWeapon;
    private ItemObject[] allShopPortion;
    private ItemObject[] allShopScroll;
    [SerializeField] private ItemObject nullObject;

    [SerializeField] private GameObject[] inventorySlotButton;
    [SerializeField] private GameObject[] shopSlotButton;
    private KeyValuePair<ItemObject, int>[] shopItemArray;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        shopItemArray = new KeyValuePair<ItemObject, int>[16];
    }
    
    public GameObject Summons(Touch cube, ObjectType objectType, int objectID)
    {
        GameObject newObject;

        switch (objectType) {
            case ObjectType.ENEMY:
                newObject = Instantiate(enemyPrefab);
                string value = StaticManager.Instance.enemyDatas[objectID];
                newObject.GetComponent<Object>().init(objectType, value.Split(','));
                break;
            case ObjectType.FRIEND:
                newObject = Instantiate(friendPrefab);
                for (int i = 0; i < 3; i++)
                    if (!friendObjectStatus[i].activeSelf)
                    {
                        newObject.GetComponent<Object>().hpSlider = friendHpSlider[i];
                        friendObjectStatus[i].SetActive(true);
                        break;
                    }
                Dictionary<int, string> values = StaticManager.Instance.friendDatas[objectID];
                values.TryGetValue(StaticManager.Instance.Stage, out value);
                newObject.GetComponent<Object>().init(objectType, value.Split(','));
                UseItem(ItemType.SCROLL, objectID);
                ChangePlayerInventory();
                break;
            case ObjectType.TREASURE:
                newObject = Instantiate(treasurePrefab);
                newObject.GetComponent<Object>().init(objectType, null);
                break;
            case ObjectType.MERCHANT:
                newObject = Instantiate(merchantPrefab);
                newObject.GetComponent<Object>().init(objectType, null);
                break;
            case ObjectType.PORTAL:
                newObject = Instantiate(portalPrefab);
                newObject.GetComponent<Object>().init(objectType, null);
                break;
            default:
                newObject = null;
                break;
        }

        newObject.transform.parent = cube.ObjectPostion;
        newObject.transform.position = cube.ObjectPostion.position;
        newObject.transform.rotation = cube.ObjectPostion.rotation;

        newObject.GetComponent<Object>().objectManager = this;

        return newObject;
    }

    public void ObjectDie(GameObject obj)
    {
        if (obj.GetComponent<Object>().Type == ObjectType.FRIEND)
            for (int i = 0; i < 3; i++)
                if (obj.GetComponent<Object>().hpSlider == friendHpSlider[i])
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
            if (StaticManager.Instance.inventory[i].Value == 0) StaticManager.Instance.inventory[i] = new(nullObject, 1);

            switch (StaticManager.Instance.inventory[i].Key.ItemType)
            {
                case ItemType.WEAPON:
                    inventorySlotButton[i].GetComponent<Image>().color = StaticManager.Instance.inventory[i].Key.Icon;
                    if (StaticManager.Instance.PlayerWeapon == (Weapon)StaticManager.Instance.inventory[i].Key)
                        inventorySlotButton[i].transform.Find("Item Count").GetComponent<Text>().text = "use";
                    else
                        inventorySlotButton[i].transform.Find("Item Count").GetComponent<Text>().text = "";
                    inventorySlotButton[i].SetActive(true);
                    break;
                case ItemType.PORTION:
                    inventorySlotButton[i].GetComponent<Image>().color = StaticManager.Instance.inventory[i].Key.Icon;
                    inventorySlotButton[i].transform.Find("Item Count").GetComponent<Text>().text
                        = StaticManager.Instance.inventory[i].Value.ToString();
                    inventorySlotButton[i].SetActive(true);
                    break;
                case ItemType.SCROLL:
                    inventorySlotButton[i].GetComponent<Image>().color = StaticManager.Instance.inventory[i].Key.Icon;
                    inventorySlotButton[i].transform.Find("Item Count").GetComponent<Text>().text
                        = StaticManager.Instance.inventory[i].Value.ToString();
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
        switch (StaticManager.Instance.inventory[index].Key.ItemType)
        {
            case ItemType.WEAPON:
                if (StaticManager.Instance.PlayerWeapon != (Weapon)StaticManager.Instance.inventory[index].Key && StageManager.Instance.StageTextChange(true, StageText.WEAPON_CHANGE, -1))
                    StaticManager.Instance.PlayerWeapon = (Weapon)StaticManager.Instance.inventory[index].Key;
                break;
            case ItemType.PORTION:
                cubeManager.SwitchPlayerTurnStatus(StaticManager.Instance.inventory[index].Key.ID, ItemType.PORTION);
                break;
            case ItemType.SCROLL:
                cubeManager.SwitchPlayerTurnStatus(StaticManager.Instance.inventory[index].Key.ID, ItemType.SCROLL);
                break;
        }
        ChangePlayerInventory();
    }
    public void UseItem(ItemType itemType, int itemID)
    {
        for(int i = 0; i < StaticManager.Instance.inventory.Length; i++)
        {
            if(itemType == StaticManager.Instance.inventory[i].Key.ItemType && itemID == StaticManager.Instance.inventory[i].Key.ID)
            {
                StaticManager.Instance.inventory[i] = new(StaticManager.Instance.inventory[i].Key, StaticManager.Instance.inventory[i].Value - 1);
                break;
            }
        }
    }
    public void ChangeShop()
    {
        for(int i = 0; i<shopItemArray.Length; i++)
        {
            if (i < 4)
            {
                int random = Random.Range(0, allShopWeapon.Length);

                shopItemArray[i] = new(allShopWeapon[random], allShopWeapon[random].Cost);
                shopSlotButton[i].GetComponent<Image>().color = allShopWeapon[random].Icon;
                shopSlotButton[i].transform.Find("Item Count").GetComponent<Text>().text
                    = allShopWeapon[random].Cost.ToString(); 
            }
            else if (i < 8)
            {
                int random = Random.Range(0, allShopPortion.Length);

                shopItemArray[i] = new(allShopPortion[random], allShopPortion[random].Cost);
                shopSlotButton[i].GetComponent<Image>().color = allShopPortion[random].Icon;
                shopSlotButton[i].transform.Find("Item Count").GetComponent<Text>().text
                    = allShopPortion[random].Cost.ToString();
            }
            else
            {
                int random = Random.Range(0, allShopScroll.Length);

                shopItemArray[i] = new(allShopScroll[random], allShopScroll[random].Cost);
                shopSlotButton[i].GetComponent<Image>().color = allShopScroll[random].Icon;
                shopSlotButton[i].transform.Find("Item Count").GetComponent<Text>().text
                    = allShopScroll[random].Cost.ToString();
            }
        }

        shopPopup.SetActive(true);
    }
    public void Buy(int index)
    {
        if (StaticManager.Instance.Gold < shopItemArray[index].Value) return;

        for (int i = 0; i < StaticManager.Instance.inventory.Length; i++)
        {
            if (StaticManager.Instance.inventory[i].Key == shopItemArray[index].Key)
            {
                if (shopItemArray[index].Key.ItemType == ItemType.WEAPON) return; // 무기는 하나만 소유 가능

                StaticManager.Instance.inventory[i] = new(StaticManager.Instance.inventory[i].Key, StaticManager.Instance.inventory[i].Value + 1);
                inventorySlotButton[i].transform.Find("Item Count").GetComponent<Text>().text
                    = StaticManager.Instance.inventory[i].Value.ToString();
                StaticManager.Instance.Gold -= shopItemArray[index].Value;
                shopSlotButton[index].SetActive(false);
                ChangePlayerInventory();
                break;
            }
            else if (StaticManager.Instance.inventory[i].Key.ItemType == ItemType.NULL)
            {
                StaticManager.Instance.inventory[i] = new(shopItemArray[index].Key, 1);
                inventorySlotButton[i].transform.Find("Item Count").GetComponent<Text>().text
                    = StaticManager.Instance.inventory[i].Value.ToString();
                StaticManager.Instance.Gold -= shopItemArray[index].Value;
                shopSlotButton[index].SetActive(false);
                ChangePlayerInventory();
                break;
            }
        }
    }
}
