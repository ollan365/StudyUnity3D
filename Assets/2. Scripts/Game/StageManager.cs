using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using static Constants;
using static Excel;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }
    [SerializeField] private GameObject player;
    public Object Player { get => player.GetComponent<Object>(); }

    [Header("Cube")]
    [SerializeField] private Transform cube;
    public bool isCubeMove;

    [Header("Manager")]
    [SerializeField] private CubeManager cubeManager;

    [Header("Logic")]
    [SerializeField] private FightLogic fightLogic;
    [SerializeField] private PlayLogic playLogic;
    [SerializeField] private EnvLogic envLogic;
    public PlayLogic StagePlayLogic { get => playLogic; }

    [Header("Texts")]
    [SerializeField] private Text[] stageTexts;
    private int[] stageTextValues;

    private int[] stageDatas;
    public int StageData(int column)
    {
        return stageDatas[column];
    }

    [Header("Objects")]
    [SerializeField] private GameObject[] enemy;
    [SerializeField] private GameObject[] friend;
    [SerializeField] private GameObject[] treasure;
    public GameObject[] EnemyList { get => enemy; }
    public GameObject[] FriendList { get => friend; }
    public GameObject[] TreasureList { get => treasure; }

    [Header("Panel")]
    [SerializeField] private GameObject stageStartPanel;
    private Text startPanelText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject clickIgnorePanel;

    [Header("Status")]
    private int turn = 1;
    private StageStatus status;
    public StageStatus StatusOfStage
    {
        get => status;
        private set
        {
            status = value;
            stageTexts[0].text = $"{StaticManager.Instance.Stage}층 {status}";
        }
    }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        stageTextValues = new int[8];
        StageInit(StaticManager.Instance.stageDatas[StaticManager.Instance.Stage]);
    }

    public void StageInit(string data)
    {
        StatusOfStage = StageStatus.INIT;
        StaticManager.Instance.player = player.GetComponent<Object>();
        StaticManager.Instance.PlayerWeapon = StaticManager.Instance.PlayerWeapon;

        stageDatas = new int[data.Split(',').Length - 1];
        for (int i = 0; i < data.Split(',').Length - 1; i++)
            stageDatas[i] = int.Parse(data.Split(',')[i]);

        Player.Init(ObjectType.PLAYER, new string[] { stageDatas[MAX_HP].ToString() }, StageCube.Instance.touchArray[WHITE][4]);

        // 나중에 계산 공식으로 바꾸기!
        stageTextValues[StageText.MONSTER.ToInt()] = stageTextValues[StageText.MONSTER_INIT.ToInt()]
            = stageDatas[ENEMY_COUNT];
        stageTextValues[StageText.ROTATE.ToInt()] = stageTextValues[StageText.ROTATE_INIT.ToInt()]
            = stageDatas[ROTATE_COUNT] = 100;
        stageTextValues[StageText.MOVE.ToInt()] = stageTextValues[StageText.MOVE_INIT.ToInt()]
            = stageDatas[MOVE_COUNT] = 10;
        stageTextValues[StageText.WEAPON_CHANGE.ToInt()] = stageTextValues[StageText.WEAPON_CHANGE_INIT.ToInt()]
            = stageDatas[WEAPON_CHANGE];

        
        enemy = new GameObject[stageDatas[ENEMY_COUNT]];
        friend = new GameObject[3];
        treasure = new GameObject[stageDatas[TREASURE_COUNT]];

        clickIgnorePanel.SetActive(true);
        cubeManager.StartRandomTurn(stageDatas[MIX]);
    }
    public int GetStageTextValue(StageText text)
    {
        return stageTextValues[text.ToInt()];
    }
    public void SetStageTextValue(StageText text, int addValue)
    {
        if (StatusOfStage == StageStatus.END)
        {
            stageTexts[1].text = "CLEAR !";
            stageTexts[2].text = "INFINITY";
            stageTexts[3].text = "INFINITY";
            stageTexts[4].text = "INFINITY";
            stageTexts[5].text = "ALL DIE !";

            return;
        }
        else if (text == StageText.ALL_INIT)
        {
            stageTextValues[StageText.MOVE.ToInt()] = stageTextValues[StageText.MOVE_INIT.ToInt()];
            stageTextValues[StageText.ROTATE.ToInt()] = stageTextValues[StageText.ROTATE_INIT.ToInt()];
        }
        else stageTextValues[text.ToInt()] += addValue;

        stageTextValues[StageText.MONSTER.ToInt()] = 0;
        foreach (GameObject e in enemy)
        {
            if (e.GetComponent<Object>().HP > 0) stageTextValues[StageText.MONSTER.ToInt()]++;
        }
            
        stageTexts[2].text = $"{stageTextValues[StageText.ROTATE.ToInt()]} / {stageTextValues[StageText.ROTATE_INIT.ToInt()]}";
        stageTexts[3].text = $"{stageTextValues[StageText.MOVE.ToInt()]} / {stageTextValues[StageText.MOVE_INIT.ToInt()]}";
        stageTexts[4].text = $"{stageTextValues[StageText.WEAPON_CHANGE.ToInt()]}";
        stageTexts[5].text = $"{stageTextValues[StageText.MONSTER.ToInt()]} / {stageTextValues[StageText.MONSTER_INIT.ToInt()]}";
    }
    public IEnumerator StartStage()
    {
        //섞은 후 플레이어 활성화
        player.SetActive(true);
        StartCoroutine(CubeRotate(player.GetComponent<Object>().Color));

        // 값 초기화
        ObjectManager.Instance.isShopChanged = false;

        int index = 0;
        
        List<string> stageEnemy = StaticManager.Instance.stageEnemyDatas[StaticManager.Instance.Stage];
        for (int i = 0; i < stageEnemy.Count; i++) // enemy 배치
        {
            for (int j = 0; j < int.Parse(stageEnemy[i].Split(',')[STAGE_ENEMY_COUNT]); j++)
            {
                enemy[index] = ObjectManager.Instance.Summons(null, ObjectType.ENEMY, int.Parse(stageEnemy[i].Split(',')[STAGE_ENEMY_ID]));
                Debug.Log(enemy[index]);
                
                index++;
                yield return new WaitForFixedUpdate();
            }
        }
        
        for (int i = 0; i < stageDatas[TREASURE_COUNT]; i++) // treasure 배치
        {
            treasure[i] = ObjectManager.Instance.Summons(null, ObjectType.TRIGGER, 0);
            treasure[i].GetComponent<Object>().SetWeapon(stageDatas[TREASURE_MIN], stageDatas[TREASURE_MAX], WeaponType.NULL);
            yield return new WaitForFixedUpdate();
        }

        startPanelText = stageStartPanel.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        startPanelText.text = $"Stage {StaticManager.Instance.Stage}";
        stageStartPanel.SetActive(true);

        yield return new WaitForSeconds(1.2f);
        
        stageStartPanel.SetActive(false);
        clickIgnorePanel.SetActive(false);

        ChangeStatus();

    }
    public void ChangeStatus()
    {
        cubeManager.ChangeToNormal();

        if (StatusOfStage == StageStatus.PLAYER)
        {
            //Debug.Log("fight");
            StatusOfStage = StageStatus.FIGHT;
            StartCoroutine(fightLogic.Attack());
        }
        else if (StatusOfStage == StageStatus.FIGHT)
        {
            StartCoroutine(envLogic.MoveEnemy());
            StatusOfStage = StageStatus.ENV;
        }
        else if (StatusOfStage == StageStatus.ENV || StatusOfStage == StageStatus.INIT)
        {
            if (StatusOfStage == StageStatus.ENV)
            {
                turn++;
                stageTexts[1].text = $"{turn} Turn";
                EventManager.Instance.Effect.Effect(); // 축복이나 저주 발동
            }

            StartCoroutine(CubeRotate(player.GetComponent<Object>().Color)); // 플레이어 쪽으로 회전

            SetStageTextValue(StageText.ALL_INIT, 0);
            StatusOfStage = StageStatus.PLAYER;
            clickIgnorePanel.SetActive(false);
        }
    }
    public void CheckStageClear()
    {
        if (stageTextValues[StageText.MONSTER.ToInt()] > 0) return;

        Debug.Log("clear");

        // 스테이지를 클리어한 경우
        StopCoroutine(fightLogic.Attack());

        StatusOfStage = StageStatus.END;
        clickIgnorePanel.SetActive(false);

        // 스테이지 종료 시 동료, 트리거 소멸
        foreach (GameObject f in friend)
            if (f != null && f.activeSelf) f.GetComponent<Object>().OnHit(StatusEffect.HP_PERCENT, 100);
        foreach (GameObject t in treasure)
            t.GetComponent<Object>().OnHit(StatusEffect.HP_PERCENT, 100);
        EventManager.Instance.StageEnd();

        // 상인과 포탈 소환
        ObjectType[] summonObjectArray = new ObjectType[2] { ObjectType.MERCHANT, ObjectType.PORTAL };
        foreach (ObjectType type in summonObjectArray)
        {
            ObjectManager.Instance.Summons(null, type, 0);
        }

        SetStageTextValue(StageText.END, 0);
    }
    public void NextStage()
    {
        Debug.Log("Next Stage!");
        StaticManager.Instance.GameStart(false);
    }
    public void GameOver()
    {
        gameOverPanel.SetActive(true);
    }

    public IEnumerator CubeRotate(Colors color)
    {
        isCubeMove = true;

        Quaternion startRotation = cube.transform.localRotation;
        Vector3 endRotationVector = new();

        switch (color)
        {
            case Colors.WHITE:
                endRotationVector = new(0, 0, 0);
                break;
            case Colors.RED:
                endRotationVector = new(0, 0, 90);
                break;
            case Colors.BLUE:
                endRotationVector = new(-90, 0, 90);
                break;
            case Colors.GREEN:
                endRotationVector = new(90, 0, -90);
                break;
            case Colors.ORANGE:
                endRotationVector = new(0, 0, -90);
                break;
            case Colors.YELLOW:
                endRotationVector = new(0, 0, -180);
                break;
        }

        float elapsedTime = 0f;

        while (elapsedTime < 1)
        {
            cube.transform.localRotation = Quaternion.Slerp(startRotation, Quaternion.Euler(endRotationVector), elapsedTime / 1);
            elapsedTime += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        // 보정을 위해 최종 회전 각도로 설정
        cube.transform.localRotation = Quaternion.Euler(endRotationVector);
        isCubeMove = false;
    }

    public void SummonsFriend(Colors color, int index, int scrollID)
    {
        Touch cube = StageCube.Instance.touchArray[color.ToInt()][index];
        if (cube.Obj != null) return;
        for (int i = 0; i < 3; i++)
        {
            if (friend[i] == null) // 이건 동료 소환이 한 스테이지에서 3번만 가능할 때긴 함
            {
                friend[i] = ObjectManager.Instance.Summons(cube, ObjectType.FRIEND, StaticManager.Instance.scrollDatas[scrollID].FriendIndex);
                ObjectManager.Instance.UseItem(ItemType.SCROLL, scrollID);
                Debug.Log("summons success!");
                break;
            }
        }
    }

}
