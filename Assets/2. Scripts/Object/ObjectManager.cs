using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static Constants;
public class ObjectManager : MonoBehaviour
{
    [SerializeField] private CubeManager cubeManager;
    [SerializeField] private Object player;
    [SerializeField] private Text goldText;
    private int gold;
    public int Gold
    {
        get => gold;
        set
        {
            gold = value;
            goldText.text = gold.ToString();
        }
    }

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject friendPrefab;
    [SerializeField] private GameObject treasurePrefab;
    [SerializeField] private GameObject merchantPrefab;
    [SerializeField] private GameObject portalPrefab;

    [SerializeField] private GameObject[] friendObjectStatus;
    [SerializeField] private Slider[] friendHpSlider;

    [SerializeField] private Transform dieObject;

    [SerializeField] private Weapon[] enemyWeapons;
    [SerializeField] private Weapon[] friendWeapons;

    [SerializeField] private ItemObject[] allShopWeapon;
    [SerializeField] private ItemObject[] allShopPortion;
    [SerializeField] private ItemObject[] allShopETC;
    [SerializeField] private ItemObject nullObject;

    [SerializeField] private GameObject[] inventorySlotButton;
    private KeyValuePair<ItemObject, int>[] inventoryItemArray;
    [SerializeField] private GameObject[] shopSlotButton;
    private KeyValuePair<ItemObject, int>[] shopItemArray;

    private void Awake()
    {
        Gold = 999;

        inventoryItemArray = new KeyValuePair<ItemObject, int>[16]; // 일단은 저장이 없음
        inventoryItemArray[0] = new(player.weapon, 1);
        for (int i = 1; i < inventoryItemArray.Length; i++)
            inventoryItemArray[i] = new(nullObject, 0);
        
        shopItemArray = new KeyValuePair<ItemObject, int>[16];
    }
    public GameObject Summons(ColorCheckCube cube, ObjectType objectType, int objectIndex)
    {
        GameObject newObject;

        switch (objectType) {
            case ObjectType.ENEMY:
                newObject = Instantiate(enemyPrefab);
                newObject.GetComponent<Object>().weapon = enemyWeapons[objectIndex];
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
                newObject.GetComponent<Object>().weapon = friendWeapons[objectIndex];
                break;
            case ObjectType.TREASURE:
                newObject = Instantiate(treasurePrefab);
                break;
            case ObjectType.MERCHANT:
                newObject = Instantiate(merchantPrefab);
                break;
            case ObjectType.PORTAL:
                newObject = Instantiate(portalPrefab);
                break;
            default:
                newObject = null;
                break;
        }

        newObject.transform.parent = cube.colorPointCube.transform.GetChild(0);
        newObject.transform.position = cube.colorPointCube.transform.GetChild(0).position;
        newObject.transform.rotation = cube.colorPointCube.transform.GetChild(0).rotation;

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

    public void OpenTreasureBox(GameObject obj)
    {
        Debug.Log("Open treasure box!");
        obj.transform.position = dieObject.position;
        obj.transform.parent = dieObject;
    }

    public void ChangePlayerInventory()
    {
        for (int i = 0; i < inventoryItemArray.Length; i++)
        {
            if (inventoryItemArray[i].Value == 0) inventoryItemArray[i] = new(nullObject, 1);

            switch (inventoryItemArray[i].Key.ItemType)
            {
                case ItemType.WEAPON:
                    inventorySlotButton[i].GetComponent<Image>().color = inventoryItemArray[i].Key.Icon;
                    if (player.weapon == (Weapon)inventoryItemArray[i].Key)
                        inventorySlotButton[i].transform.Find("Item Count").GetComponent<Text>().text = "use";
                    else
                        inventorySlotButton[i].transform.Find("Item Count").GetComponent<Text>().text = "";
                    inventorySlotButton[i].SetActive(true);
                    break;
                case ItemType.PORTION:
                    inventorySlotButton[i].GetComponent<Image>().color = inventoryItemArray[i].Key.Icon;
                    inventorySlotButton[i].transform.Find("Item Count").GetComponent<Text>().text
                        = inventoryItemArray[i].Value.ToString();
                    inventorySlotButton[i].SetActive(true);
                    break;
                case ItemType.SCROLL:
                    inventorySlotButton[i].GetComponent<Image>().color = inventoryItemArray[i].Key.Icon;
                    inventorySlotButton[i].transform.Find("Item Count").GetComponent<Text>().text
                        = inventoryItemArray[i].Value.ToString();
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
        switch (inventoryItemArray[index].Key.ItemType)
        {
            case ItemType.WEAPON:
                if (player.weapon != (Weapon)inventoryItemArray[index].Key && cubeManager.CanChangeWeapon())
                    player.ChangeWeapon((Weapon)inventoryItemArray[index].Key);
                break;
            case ItemType.PORTION:
                Portion p = (Portion)inventoryItemArray[index].Key;
                switch (p.StatusEffectType)
                {
                    case StatusEffect.HP:
                        player.HP += p.Value;
                        inventoryItemArray[index] = new(inventoryItemArray[index].Key, inventoryItemArray[index].Value - 1);
                        break;
                }
                break;
            case ItemType.SCROLL:
                Scroll s = (Scroll)inventoryItemArray[index].Key;
                cubeManager.SelectSummonsButton(s.FriendIndex);
                break;
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
                int random = Random.Range(0, allShopETC.Length);

                shopItemArray[i] = new(allShopETC[random], allShopETC[random].Cost);
                shopSlotButton[i].GetComponent<Image>().color = allShopETC[random].Icon;
                shopSlotButton[i].transform.Find("Item Count").GetComponent<Text>().text
                    = allShopETC[random].Cost.ToString();
            }
        }
    }
    public void Buy(int index)
    {
        if (Gold < shopItemArray[index].Value) return;

        for (int i = 0; i < inventoryItemArray.Length; i++)
        {
            if (inventoryItemArray[i].Key == shopItemArray[index].Key)
            {
                if (shopItemArray[index].Key.ItemType == ItemType.WEAPON) return; // 무기는 하나만 소유 가능

                inventoryItemArray[i] = new(inventoryItemArray[i].Key, inventoryItemArray[i].Value + 1);
                inventorySlotButton[i].transform.Find("Item Count").GetComponent<Text>().text
                    = inventoryItemArray[i].Value.ToString();
                Gold -= shopItemArray[index].Value;
                shopSlotButton[index].SetActive(false);
                ChangePlayerInventory();
                break;
            }
            else if (inventoryItemArray[i].Key.ItemType == ItemType.NULL)
            {
                inventoryItemArray[i] = new(shopItemArray[index].Key, 1);
                inventorySlotButton[i].transform.Find("Item Count").GetComponent<Text>().text
                    = inventoryItemArray[i].Value.ToString();
                Gold -= shopItemArray[index].Value;
                shopSlotButton[index].SetActive(false);
                ChangePlayerInventory();
                break;
            }
        }
    }
}
