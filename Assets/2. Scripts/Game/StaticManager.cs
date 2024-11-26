using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using static Excel;

public class StaticManager : MonoBehaviour
{
    public static StaticManager Instance { get; private set; }

    [Range(1, 24)]
    [SerializeField] private int stage;
    public ItemSlot[] inventory;
    public ItemObject nullObject;
    public ItemObject rustySword;

    [Header("Player")]
    public Object player;
    public int Stage { get => stage; }

    [SerializeField] private Weapon playerWeapon;
    public Weapon PlayerWeapon
    {
        get => playerWeapon;
        set
        {
            playerWeapon = value;
            player.SetWeapon(playerWeapon.MinDamage, playerWeapon.MaxDamage, playerWeapon.WeaponType);

            if (ObjectManager.Instance.PlayerWeapon.transform.childCount != 0)
                Destroy(ObjectManager.Instance.PlayerWeapon.transform.GetChild(0).gameObject);

            int idx = (playerWeapon.ID - 110014);
            idx = (ObjectManager.Instance.Weapons.Length == 0) ? 0 : idx % (ObjectManager.Instance.Weapons.Length);
            Instantiate(ObjectManager.Instance.Weapons[idx], ObjectManager.Instance.PlayerWeapon.transform);
        }
    }
    [SerializeField] private int gold = 0; // 일단 테스트를 위해 1000골드
    public int Gold
    {
        get => gold; 
        
        set
        {
            if (gold < value) player.PoppingText("+" + (value - gold).ToString(), Color.yellow);
            else if (gold > value) player.PoppingText("-" + (gold - value).ToString(), Color.red);
            
            gold = value;
        }

    }

    [Header("Item List")]
    [SerializeField] private List<Portion> portionList;
    [SerializeField] private List<Scroll> scrollList;
    [SerializeField] private List<Weapon> weaponList;


    //Object Datas
    public Dictionary<int, string> stageDatas;
    public Dictionary<int, List<string>> stageEnemyDatas;
    public Dictionary<int, string> enemyDatas;
    public Dictionary<int, Dictionary<int, string>> friendDatas;

    public Dictionary<int, Portion> portionDatas;
    public Dictionary<int, Scroll> scrollDatas;
    public Dictionary<int, Weapon> weaponDatas;

    [Header("Cube Materials")]
    [SerializeField] private Material[] material1;
    [SerializeField] private Material[] material2;
    [SerializeField] private Material[] material3;
    [SerializeField] private Material[] material4;
    [SerializeField] private Material[] material5;
    [SerializeField] private Material[] material6;
    public Material[][] cubeMaterialSet;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SaveExcelDatas();
            SaveItemDatas();
        }
        else
        {
            Destroy(gameObject);
        }
        cubeMaterialSet = new Material[][] { material1, material2, material3, material4, material5, material6 };

    }
    public void GameInfoInit()
    {
        // 게임 오버 후 초기화: 스테이지, 골드, 인벤토리, 플레이어 무기
        stage = 1;
        Gold = 0;

        for (int i = 0; i < inventory.Length; i++) inventory[i].Init();

        inventory[0] = new ItemSlot(rustySword, 1);
        PlayerWeapon = (Weapon)rustySword;
    }
    public void GameStart(bool isLobby)
    {
        if (isLobby) // 여기서 저장된 게임 데이터 로드 해야될듯?
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        if (!isLobby)
        {
            stage++;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    private void SaveExcelDatas()
    {
        TextAsset csvData = Resources.Load<TextAsset>(StageInfo);
        string[] data = csvData.text.Split(new char[] { '\n' });
        stageDatas = new();
        for (int i = 1; i < data.Length; i++)
        {
            string[] row = data[i].Split(',');
            if (row[STAGE] == "") break;
            stageDatas.Add(int.Parse(row[STAGE]), data[i]);
        }

        csvData = Resources.Load<TextAsset>(StageEnemy);
        data = csvData.text.Split(new char[] { '\n' });
        stageEnemyDatas = new();
        for (int i = 1; i < data.Length;)
        {
            int add = 0;
            List<string> valueList = new();

            if (data[i].Split(',')[STAGE_ENEMY_STAGE] == "") break;

            while ( (i + add < data.Length) && data[i].Split(',')[STAGE_ENEMY_STAGE] == data[i + add].Split(',')[STAGE_ENEMY_STAGE])
            {
                valueList.Add(data[i + add]);
                add++;

                if (data.Length <= i + add) break;
            }

            stageEnemyDatas.Add(int.Parse(data[i].Split(',')[STAGE_ENEMY_STAGE]), valueList);
            i += add;
            
        }

        csvData = Resources.Load<TextAsset>(EnemyInfo);
        data = csvData.text.Split(new char[] { '\n' });
        enemyDatas = new();
        for (int i = 1; i < data.Length; i++)
        {
            string[] row = data[i].Split(new char[] { ',' });
            if (row[STAGE] == "") break;
            enemyDatas.Add(int.Parse(row[ENEMY_ID]), data[i]);
        }

        csvData = Resources.Load<TextAsset>(FriendInfo);
        data = csvData.text.Split(new char[] { '\n' });
        friendDatas = new();
        for (int i = 1; i < data.Length;)
        {
            int add = 0;
            Dictionary<int, string> valueList = new();

            if (data[i].Split(',')[FRIEND_ID] == "") break;

            while ((i + add < data.Length) && data[i].Split(',')[FRIEND_ID] == data[i + add].Split(',')[FRIEND_ID])
            {
                valueList.Add(int.Parse(data[i + add].Split(',')[FRIEND_STAGE]), data[i + add]);
                add++;
            }

            friendDatas.Add(int.Parse(data[i].Split(',')[FRIEND_ID]), valueList);
            i += add;
        }

    }
    private void SaveItemDatas()
    {
        portionDatas = new();
        scrollDatas = new();
        weaponDatas = new();

        foreach(Portion p in portionList)
            portionDatas.Add(p.ID, p);

        foreach (Scroll s in scrollList)
            scrollDatas.Add(s.ID, s);

        foreach (Weapon w in weaponList)
            weaponDatas.Add(w.ID, w);
    }
}
