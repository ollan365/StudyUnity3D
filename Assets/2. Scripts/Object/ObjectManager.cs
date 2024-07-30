using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static Constants;
using TMPro;

public class ObjectManager : MonoBehaviour
{
    [Space(10f)]
    [SerializeField] private GameObject[] friendObjectStatus;
    [SerializeField] private Transform dieObject;
    [SerializeField] private Object player;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text shopGoldText;

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

    [Header("Enemy Info UI")]
    [SerializeField] private GameObject enemyInfoPanel;
    [SerializeField] private Image enemyInfoImage;
    [SerializeField] private Sprite[] enemyInfoImages;
    [SerializeField] private TMP_Text enemyInfoName;
    [SerializeField] private Slider enemyInfoHPslider;
    [SerializeField] private TMP_Text enemyInfoHPText;
    [SerializeField] private TMP_Text enemyInfoAttackType;
    [SerializeField] private TMP_Text enemyInfoBasicAttack;
    public GameObject EnemyInfoPanel { get => enemyInfoPanel; }

    [Header("Object Info UI")]
    [SerializeField] private GameObject objectInfoPanel;
    [SerializeField] private Image objectInfoImage;
    [SerializeField] private Sprite[] objectInfoImages;
    [SerializeField] private TMP_Text objectInfoName;
    [SerializeField] private Slider objectInfoHPslider;
    [SerializeField] private TMP_Text objectInfoHPText;
    [SerializeField] private TMP_Text objectInfoAttackType;
    [SerializeField] private TMP_Text objectInfoBasicAttack;
    public GameObject ObjectInfoPanel { get => objectInfoPanel; }


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        shopItemSlotArray = new ItemSlot[9];
        ChangeShop();
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
            case ObjectType.PLAYER:
                newObject = StageManager.Instance.Player.gameObject;
                break;

            case ObjectType.ENEMY:
                newObject = Instantiate(enemyPrefab);
                string value = StaticManager.Instance.enemyDatas[objectID];
                newObject.GetComponent<Object>().Init(objectType, value.Split(','), cube);
                //newObject.transform.GetChild(objectID - 100000).gameObject.SetActive(true);
                int meshCnt = newObject.transform.GetChild(1).childCount;

                //disable default mesh and activate the mesh that matches the object ID.
                int idx = (objectID - 100000) % meshCnt;
                //newObject.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
                newObject.transform.GetChild(1).GetChild(idx).gameObject.SetActive(true);
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

        // ���� �ֵ��� ���� �� ���� ���߱⸦ ���ؼ� �ϴ��� �ٸ� ���� �Űܵд�
        // ���߿� ���� �ý��۸� ��° �ٲٸ� �׳� destroy �ص� �ɵ�
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
                    if (StageManager.Instance.FriendList[i] == null) // �̰� ���� ��ȯ�� �� ������������ 3���� ������ ���� ��
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
    public void OpenShop()
    {
        clickIgnorePanel.SetActive(true);
        shopPopup.SetActive(true);
        ChangePlayerInventory();
        inventoryPopup.SetActive(true);
    }
    public void ChangeShop()
    {
        int stage = StaticManager.Instance.Stage;
        int[] scroll = new int[3] { 0, 0, 0 };
        int scrollIndex = 3;

        for (int i = 0; i < scroll.Length; i++)
        {
            int random = Random.Range(2, 11);
            while (random == scroll[0] || random == scroll[1] || random == scroll[2])
                random = Random.Range(2, 11);
            scroll[i] = random;
        }

        int itemIndex = -1;
        for (int shopIndex = 0; shopIndex < shopSlot.Length; shopIndex++)
        {
            bool moreItem = false;
            for (int i = itemIndex + 1; i < shopItemArray.Length; i++)
            {
                if (shopItemArray[i].SpawnMinStage > stage || shopItemArray[i].SpawnMaxStage < stage) continue;

                moreItem = true;

                itemIndex = i;

                if (shopItemArray[itemIndex].ItemType == ItemType.SCROLL && scrollIndex > 0)
                    itemIndex = scroll[--scrollIndex];
                else if (shopItemArray[itemIndex].ItemType == ItemType.SCROLL && scrollIndex == 0)
                    continue;

                break;
            }
            if (!moreItem) break;

            shopSlot[shopIndex].SetActive(true);

            if (shopItemArray[itemIndex].ItemType == ItemType.PORTION)
                shopItemSlotArray[shopIndex] = new(shopItemArray[itemIndex], 10 + 5 * itemIndex);
            else
                shopItemSlotArray[shopIndex] = new(shopItemArray[itemIndex], 1);

            shopSlot[shopIndex].ChangeImage(shopItemArray[itemIndex].Icon);
            shopSlot[shopIndex].ChangeText(shopItemSlotArray[shopIndex].count + " / $" + shopItemArray[itemIndex].SellCost.ToString());
        }

    }
    public void AddItem(int index, bool buy)
    {
        if (buy && StaticManager.Instance.Gold < shopItemSlotArray[index].item.SellCost) return;

        int inventoryIndex = -1;

        for (int i = 0; i < StaticManager.Instance.inventory.Length; i++) // ����, �̹� �ִ� ���������� Ȯ��
        {
            if (StaticManager.Instance.inventory[i].item == shopItemSlotArray[index].item) // �̹� �ִ� �������� ���
            {
                if (shopItemSlotArray[index].item.ItemType == ItemType.WEAPON) return; // ����� �ϳ��� ���� ����
                inventoryIndex = i;
                break;
            }
        }

        if (inventoryIndex == -1) // ���� �������� ���
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

        if(inventoryIndex == -1) // �κ��丮�� �� �̻� ���� ������ ���� ��
        {
            Debug.Log("There is no more slot!");
            return;
        }

        // �κ��丮�� ������ ���� ����
        StaticManager.Instance.inventory[inventoryIndex].count++;
        inventorySlot[inventoryIndex].ChangeText(StaticManager.Instance.inventory[inventoryIndex].count.ToString());
        shopInventorySlot[inventoryIndex].ChangeText(StaticManager.Instance.inventory[inventoryIndex].count.ToString());

        // �κ��丮 ����
        ChangePlayerInventory();

        if (!buy) return; // ������ �� �ƴϸ� return

        // ���� �� ����
        StaticManager.Instance.Gold -= shopItemSlotArray[index].item.SellCost;

        // ���� ���� ����
        shopItemSlotArray[index].count--;
        if (shopItemSlotArray[index].count <= 0) // ������ �� �ȷ��� ��
            shopSlot[index].SetActive(false);
        else
            shopSlot[index].ChangeText(shopItemSlotArray[index].count + " / $" + shopItemArray[index].SellCost.ToString());
    }

    public void SetObjectInfo(Object obj, int index = 0)
    {
        Object targetObj = obj;
        
        //���� �÷��̾�, ������ ���� ���� �� ����
        string objName = targetObj.name;
        float hpValue = targetObj.HP / targetObj.MaxHp;
        string hpText = $"{Mathf.CeilToInt(targetObj.HP)} / {Mathf.CeilToInt(targetObj.MaxHp)}";
        string attackType = "Ÿ��: Ÿ��";
        switch (targetObj.GetComponent<Object>().AttackType)
        {
            case WeaponType.CAD:
                attackType = "���� Ÿ��: �ٰŸ�";
                break;
            case WeaponType.LAD:
                attackType = "���� Ÿ��: ���Ÿ�";
                break;
            case WeaponType.NULL:
                attackType = "���� Ÿ��: NULL";
                break;
            default:
                Debug.Log("����Ÿ�� ����");
                break;
        }
        string basicAttackText = $"�⺻ ���ݷ�: {targetObj.MinDamage} ~ {targetObj.MaxDamage}";



        if (targetObj.Type == ObjectType.PLAYER || targetObj.Type == ObjectType.FRIEND) //�÷��̾� ��ư �����ٴ뼭 ȣ��Ǵ� �κ�
        {
            //�÷��̾�, ������� ���� �����ؼ� �ֱ� �ϴ��� ���� ��������
            if(targetObj.Type == ObjectType.PLAYER)
                objectInfoImage.sprite = objectInfoImages[0];
            else
                objectInfoImage.sprite = objectInfoImages[1];

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

        }
        else if (targetObj.Type == ObjectType.ENEMY) //�� Ŭ���ؼ� ȣ��Ǵ� ���
        {
            //image
            int idx = targetObj.ID - 100000; //ID ���� �޾� �迭�� �ε����� ��ȯ
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
