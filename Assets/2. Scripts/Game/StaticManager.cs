using UnityEngine;
using System.Collections.Generic;
using static Excel;

public class StaticManager : MonoBehaviour
{
    public static StaticManager Instance { get; private set; }

    [SerializeField] private int stage;
    [SerializeField] private Object player;
    public int Stage { get => stage; }

    [SerializeField] private Weapon playerWeapon;
    public Weapon PlayerWeapon
    {
        get => playerWeapon;
        set
        {
            playerWeapon = value;
            player.SetWeapon(playerWeapon.MinDamage, playerWeapon.MaxDamage, playerWeapon.WeaponType);
        }
    }
    private int gold;
    public int Gold { get => gold; set => gold = value; }

    
    [SerializeField] private List<Portion> portionList;
    [SerializeField] private List<Scroll> scrollList;
    [SerializeField] private List<Weapon> weaponList;

    public Dictionary<int, string> stageDatas;
    public Dictionary<int, List<string>> stageEnemyDatas;
    public Dictionary<int, string> enemyDatas;
    public Dictionary<int, Dictionary<int, string>> friendDatas;

    public Dictionary<int, Portion> portionDatas;
    public Dictionary<int, Scroll> scrollDatas;
    public Dictionary<int, Weapon> weaponDatas;

    public KeyValuePair<ItemObject, int>[] inventory;
    [SerializeField] private Material[] material1, material2, material3, material4, material5, material6;
    public Material[][] cubeMaterialSet;
    
    public ItemObject nullObject;

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

        inventory = new KeyValuePair<ItemObject, int>[16]; // �ϴ��� ������ ����
        for(int i = 0; i < 16; i++)
            inventory[i] = new(nullObject, 0);
        inventory[0] = new(playerWeapon, 1);

        StageManager.Instance.StageInit(stageDatas[stage]);
        PlayerWeapon = playerWeapon;

        cubeMaterialSet = new Material[][] { material1, material2, material3, material4, material5, material6 };

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

            while (data[i].Split(',')[STAGE_ENEMY_STAGE] == data[i + add].Split(',')[STAGE_ENEMY_STAGE])
            {
                valueList.Add(data[i + add]);
                add++;
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

            while (data[i].Split(',')[FRIEND_ID] == data[i + add].Split(',')[FRIEND_ID])
            {
                valueList.Add(int.Parse(data[i + add].Split(',')[FRIEND_STAGE]), data[i]);
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
