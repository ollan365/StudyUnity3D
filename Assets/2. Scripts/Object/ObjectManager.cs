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
    [SerializeField] private Text shopGoldText;

    public static ObjectManager Instance { get; private set; }
    [Header("Manager")]
    [SerializeField] private CubeManager cubeManager;


    [Header("Prefabs")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject friendPrefab;
    [SerializeField] private GameObject[] triggerPrefabs;
    [SerializeField] private GameObject merchantPrefab;
    [SerializeField] private GameObject portalPrefab;

    [Header("UI")]
    [SerializeField] private Slider[] friendHpSlider;
    [SerializeField] private GameObject clickIgnorePanel;
    [SerializeField] private GameObject shopPopup;
    [SerializeField] private GameObject inventoryPopup;
    [SerializeField] private Slot[] inventorySlot;
    [SerializeField] private Slot[] shopInventorySlot;
    [SerializeField] private Slot[] shopSlot;
    [SerializeField] private ItemObject[] shopItemArray;
    private ItemSlot[] shopItemSlotArray;
    // 변수
    public bool isShopChanged = false;

    [Header("Enemy Info UI")]
    [SerializeField] private GameObject enemyInfoPanel;
    [SerializeField] private Image enemyInfoImage;
    [SerializeField] private Sprite[] enemyInfoImages;
    [SerializeField] private Text enemyInfoName;
    [SerializeField] private Slider enemyInfoHPslider;
    [SerializeField] private Text enemyInfoHPText;
    [SerializeField] private Text enemyInfoAttackType;
    [SerializeField] private Text enemyInfoBasicAttack;
    public GameObject EnemyInfoPanel { get => enemyInfoPanel; }

    [Header("Object Info UI")]
    [SerializeField] private GameObject objectInfoPanel;
    [SerializeField] private Image objectInfoImage;
    [SerializeField] private Sprite[] objectInfoImages;
    [SerializeField] private Text objectInfoName;
    [SerializeField] private Slider objectInfoHPslider;
    [SerializeField] private Text objectInfoHPText;
    [SerializeField] private Text objectInfoAttackType;
    [SerializeField] private Text objectInfoBasicAttack;
    public GameObject ObjectInfoPanel { get => objectInfoPanel; }


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        shopItemSlotArray = new ItemSlot[9];
    }

    public GameObject Summons(Touch cube, ObjectType objectType, int objectID)
    {
        if (cube == null)
        {
            while (true)
            {
                cube = StageCube.Instance.touchArray[Random.Range(0, 6)][Random.Range(0, 9)];
                if (cube.Obj == null)
                    break;
            }
        }

        GameObject newObject;

        switch (objectType) {
            case ObjectType.ENEMY:
                newObject = Instantiate(enemyPrefab);
                string value = StaticManager.Instance.enemyDatas[objectID];
                newObject.GetComponent<Object>().Init(objectType, value.Split(','), cube);
                //newObject.transform.GetChild(objectID - 100000).gameObject.SetActive(true);

                //disable default mesh and activate the mesh that matches the object ID.
                //
                newObject.transform.GetChild(1).gameObject.SetActive(false);
                if (objectID == 100001)
                    newObject.transform.GetChild(1).gameObject.SetActive(true);
                else if (objectID == 100002)
                    newObject.transform.GetChild(2).gameObject.SetActive(true);
                else
                    newObject.transform.GetChild(3).gameObject.SetActive(true);
                break;
            case ObjectType.FRIEND:
                Dictionary<int, string> values = StaticManager.Instance.friendDatas[objectID];
                value = values[StaticManager.Instance.Stage];
                if (value == null) return null;
                newObject = Instantiate(friendPrefab);
                

                for (int i = 0; i < 3; i++)
                    if (!friendObjectStatus[i].activeSelf)
                    {
                        newObject.GetComponent<Object>().bottomHP = friendHpSlider[i];
                        friendHpSlider[i].transform.parent.GetComponent<ShowStatus>().targetObject = newObject;
                        friendHpSlider[i].transform.parent.GetComponent<ShowStatus>().index = i + 1;
                        friendObjectStatus[i].SetActive(true);
                        break;
                    }
                newObject.GetComponent<Object>().Init(objectType, value.Split(','), cube);
                ChangePlayerInventory();
                break;
            case ObjectType.TRIGGER:
                string[] name = new string[1];
                if (objectID == 0) name[0] = "Treasure";
                else if (objectID == 1) name[0] = "ForbiddenFruit";
                else if (objectID == 2) name[0] = "Thunder";

                newObject = Instantiate(triggerPrefabs[objectID]);
                newObject.GetComponent<Object>().Init(objectType, name, cube);
                break;
            case ObjectType.MERCHANT:
                newObject = Instantiate(merchantPrefab);
                newObject.GetComponent<Object>().Init(objectType, null, cube);
                break;
            case ObjectType.PORTAL:
                newObject = Instantiate(portalPrefab);
                newObject.GetComponent<Object>().Init(objectType, null, cube);
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
                    inventorySlot[i].ChangeImage(StaticManager.Instance.inventory[i].item.Icon);
                    if (StaticManager.Instance.PlayerWeapon == (Weapon)StaticManager.Instance.inventory[i].item)
                        inventorySlot[i].ChangeText("use");
                    else
                        inventorySlot[i].ChangeText("unuse");
                    inventorySlot[i].SetActive(true);
                    break;
                case ItemType.PORTION:
                case ItemType.SCROLL:
                    inventorySlot[i].ChangeImage(StaticManager.Instance.inventory[i].item.Icon);
                    inventorySlot[i].ChangeText(StaticManager.Instance.inventory[i].count.ToString());
                    inventorySlot[i].SetActive(true);
                    break;
                case ItemType.NULL:
                    inventorySlot[i].SetActive(false);
                    break;
            }
        }


        shopGoldText.text = StaticManager.Instance.Gold.ToString();

        for (int i = 0; i < StaticManager.Instance.inventory.Length; i++)
        {
            if (StaticManager.Instance.inventory[i].count == 0) StaticManager.Instance.inventory[i].init();

            switch (StaticManager.Instance.inventory[i].item.ItemType)
            {
                case ItemType.WEAPON:
                    shopInventorySlot[i].ChangeImage(StaticManager.Instance.inventory[i].item.Icon);
                    if (StaticManager.Instance.PlayerWeapon == (Weapon)StaticManager.Instance.inventory[i].item)
                        shopInventorySlot[i].ChangeText("use");
                    else
                        shopInventorySlot[i].ChangeText("unuse");
                    shopInventorySlot[i].SetActive(true);
                    break;
                case ItemType.PORTION:
                case ItemType.SCROLL:
                    shopInventorySlot[i].ChangeImage(StaticManager.Instance.inventory[i].item.Icon);
                    shopInventorySlot[i].ChangeText(StaticManager.Instance.inventory[i].count.ToString());
                    shopInventorySlot[i].SetActive(true);
                    break;
                case ItemType.NULL:
                    shopInventorySlot[i].SetActive(false);
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
                for (int i = 0; i < 3; i++)
                {
                    if (StageManager.Instance.FriendList[i] == null) // 이건 동료 소환이 한 스테이지에서 3번만 가능할 때긴 함
                    {
                        cubeManager.SwitchPlayerTurnStatus(StaticManager.Instance.inventory[index].item.ID, ItemType.SCROLL);
                        return;
                    }
                }
                Debug.Log("Already 3 Friends!");
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
    public void ChangeShop() // 나중에 갈아엎자... 
    {
        clickIgnorePanel.SetActive(true);

        if (!isShopChanged)
        {
            isShopChanged = true;

            int[] scroll = new int[3] { -1, -1, -1 };
            int stage = StaticManager.Instance.Stage;

            for (int i = 0; i < shopItemSlotArray.Length; i++)
            {
                if (i < 2) // 포션 2개 (무조건)
                {
                    if (shopItemArray[i].SpawnMinStage > stage || shopItemArray[i].SpawnMaxStage < stage)
                    {
                        shopSlot[i].SetActive(false);
                        continue;
                    }

                    shopSlot[i].SetActive(true);

                    shopItemSlotArray[i] = new(shopItemArray[i], 5);
                    shopSlot[i].ChangeImage(shopItemArray[i].Icon);
                    shopSlot[i].ChangeText(shopItemSlotArray[i].count + " / $" + shopItemArray[i].SellCost.ToString());
                }
                else if (i < 5) // 스크롤 3개 랜덤
                {
                    if (StaticManager.Instance.Stage < 5) // 스크롤은 5 스테이지 이후부터 구매 가능
                    {
                        shopSlot[i].SetActive(false);
                        continue;
                    }

                    shopSlot[i].SetActive(true);

                    int random = Random.Range(2, 11);
                    while (random == scroll[0] || random == scroll[1] || random == scroll[2])
                        random = Random.Range(2, 11);
                    scroll[i - 2] = random;

                    shopItemSlotArray[i] = new(shopItemArray[random], 3);
                    shopSlot[i].ChangeImage(shopItemArray[random].Icon);
                    shopSlot[i].ChangeText(shopItemSlotArray[i].count + " / $" + shopItemArray[random].SellCost.ToString());
                }
                else // 무기 4개
                {
                    int index = 0;
                    for (int j = 11; j < 25; j++)
                    {
                        if (shopItemArray[j].SpawnMinStage > stage || shopItemArray[j].SpawnMaxStage < stage) continue;

                        shopSlot[i + index].SetActive(true);

                        shopItemSlotArray[i + index] = new(shopItemArray[j], 1);
                        shopSlot[i + index].ChangeImage(shopItemArray[j].Icon);
                        shopSlot[i + index].ChangeText(shopItemSlotArray[i + index].count + " / $" + shopItemArray[i + index].SellCost.ToString());
                    }
                    for (i = i + index; i < shopItemSlotArray.Length; i++)
                        shopSlot[i].SetActive(false);

                    return;
                }
            }
        }

        shopPopup.SetActive(true);
        ChangePlayerInventory();
        inventoryPopup.SetActive(true);
    }
    public void Buy(int index)
    {
        if (StaticManager.Instance.Gold < shopItemSlotArray[index].item.SellCost) return;

        int inventoryIndex = -1;

        for (int i = 0; i < StaticManager.Instance.inventory.Length; i++) // 먼저, 이미 있는 아이템인지 확인
        {
            if (StaticManager.Instance.inventory[i].item == shopItemSlotArray[index].item) // 이미 있는 아이템인 경우
            {
                if (shopItemSlotArray[index].item.ItemType == ItemType.WEAPON) return; // 무기는 하나만 소유 가능
                inventoryIndex = i;
                break;
            }
        }

        if (inventoryIndex == -1) // 없는 아이템인 경우
        {
            for (int i = 0; i < StaticManager.Instance.inventory.Length; i++)
            {
                if (StaticManager.Instance.inventory[i].item.ItemType == ItemType.NULL)
                {
                    inventoryIndex = i;
                    StaticManager.Instance.inventory[i].item = shopItemSlotArray[index].item;
                    StaticManager.Instance.inventory[i].count = 0;
                    break;
                }
            }
        }

        if(inventoryIndex == -1) // 인벤토리에 더 이상 남은 슬롯이 없을 때
        {
            Debug.Log("There is no more slot!");
            return;
        }

        // 인벤토리의 아이템 개수 변경
        StaticManager.Instance.inventory[inventoryIndex].count++;
        inventorySlot[inventoryIndex].ChangeText(StaticManager.Instance.inventory[inventoryIndex].count.ToString());
        shopInventorySlot[inventoryIndex].ChangeText(StaticManager.Instance.inventory[inventoryIndex].count.ToString());

        // 가진 돈 변경
        StaticManager.Instance.Gold -= shopItemSlotArray[index].item.SellCost;

        // 상점 슬롯 변경
        shopItemSlotArray[index].count--;
        if (shopItemSlotArray[index].count <= 0) // 물건이 다 팔렸을 때
            shopSlot[index].SetActive(false);
        else
            shopSlot[index].ChangeText(shopItemSlotArray[index].count + " / $" + shopItemArray[index].SellCost.ToString());

        // 인벤토리 변경
        ChangePlayerInventory();
        return;
    }

    public void SetObjectInfo(Object obj, int index = 0)
    {
        Object targetObj = obj;
        
        //적과 플레이어, 동료의 공통 변수 값 설정
        string objName = targetObj.name;
        float hpValue = targetObj.HP / targetObj.MaxHp;
        string hpText = $"{Mathf.CeilToInt(targetObj.HP)} / {Mathf.CeilToInt(targetObj.MaxHp)}";
        string attackType = "타입: 타입";
        switch (targetObj.GetComponent<Object>().AttackType)
        {
            case WeaponType.CAD:
                attackType = "공격 타입: 근거리";
                break;
            case WeaponType.LAD:
                attackType = "공격 타입: 원거리";
                break;
            case WeaponType.NULL:
                attackType = "공격 타입: NULL";
                break;
            default:
                Debug.Log("공격타입 없음");
                break;
        }
        string basicAttackText = $"기본 공격력: {targetObj.MinDamage} ~ {targetObj.MaxDamage}";

        if (targetObj.Type == ObjectType.PLAYER || targetObj.Type == ObjectType.FRIEND) //플레이어 버튼 가져다대서 호출되는 부분
        {
            //set player or mercenary image
            //int idx = clickedObj.ID - 100000; //ID 값을 받아 배열의 인덱스로 변환
            //if (idx + 1 > enemyInfoImages.Length)
            //    enemyInfoImage.sprite = enemyInfoImages[0];
            //else
            //    enemyInfoImage.sprite = enemyInfoImages[idx];

            //name
            objectInfoName.text = objName;

            //Hpbar
            objectInfoHPslider.value = hpValue;
            objectInfoHPText.text = hpText;

            //Attack Type
            objectInfoAttackType.text = attackType;

            //Basic Attack
            objectInfoBasicAttack.text = basicAttackText;

            //panel position
            RectTransform panelTransform = ObjectInfoPanel.GetComponent<RectTransform>();
            panelTransform.position = new Vector3(index * 155 , 200, 0);
            Debug.Log("ㅇ");

        }
        else if (targetObj.Type == ObjectType.ENEMY) //적 클릭해서 호출되는 경우
        {
            //image
            int idx = targetObj.ID - 100000; //ID 값을 받아 배열의 인덱스로 변환
            if (idx + 1 > enemyInfoImages.Length)
                enemyInfoImage.sprite = enemyInfoImages[0];
            else
                enemyInfoImage.sprite = enemyInfoImages[idx];

            //name
            enemyInfoName.text = objName;

            //Hpbar
            enemyInfoHPslider.value = hpValue;
            enemyInfoHPText.text = hpText;

            //Attack Type
            enemyInfoAttackType.text = attackType;

            //Basic Attack
            enemyInfoBasicAttack.text = basicAttackText;

            //Set Active = true
            EnemyInfoPanel.SetActive(true);
            
        }
    }

}
