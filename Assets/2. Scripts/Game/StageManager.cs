using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using static Constants;
using static Excel;
using TMPro;
using DG.Tweening;

public class StageManager : MonoBehaviour
{
    Sequence sequence;

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

    [Header("UI")]
    [SerializeField] private TMP_Text[] stageTexts;
    [SerializeField] private List<CustomElement> stageTextValues;

    private int[] stageDatas;
    public int StageData(int column)
    {
        return stageDatas[column];
    }

    [Header("Objects")]
    [SerializeField] private GameObject[] enemy;
    [SerializeField] private GameObject[] friend;
    [SerializeField] private List<GameObject> treasure;
    public GameObject[] EnemyList { get => enemy; }
    public GameObject[] FriendList { get => friend; }
    public List<GameObject> TreasureList { get => treasure; }

    [Header("Panel")]
    [SerializeField] private GameObject stageStartPanel;
    private TMP_Text startPanelText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject clickIgnorePanel;

    [Header("Status")]
    public bool isBossStage = false;
    private int turn = 0;
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

        ChangeValueForTest();
        StageInit(StaticManager.Instance.stageDatas[StaticManager.Instance.Stage]);
    }

    public void ChangeValueForTest()
    {
        stageTextValues = new();

        stageTextValues.Add(new CustomElement("Enemy Count", 0));
        stageTextValues.Add(new CustomElement("Move Count", 0));
        stageTextValues.Add(new CustomElement("Rotate Count", 0));
        stageTextValues.Add(new CustomElement("Weapon Change Count", 0));
        stageTextValues.Add(new CustomElement("Monster Init Count", 0));
        stageTextValues.Add(new CustomElement("Move Init Count", 0));
        stageTextValues.Add(new CustomElement("Rotate Init Count", 0));
        stageTextValues.Add(new CustomElement("Weapon Change Count", 0));
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
        stageTextValues[StageText.MONSTER.ToInt()].value = stageDatas[ENEMY_COUNT];
        stageTextValues[StageText.MONSTER_INIT.ToInt()].value = stageDatas[ENEMY_COUNT];
        stageTextValues[StageText.ROTATE.ToInt()].value = stageDatas[ROTATE_COUNT];
        stageTextValues[StageText.ROTATE_INIT.ToInt()].value = stageDatas[ROTATE_COUNT];
        stageTextValues[StageText.MOVE.ToInt()].value = 2;
        stageTextValues[StageText.MOVE_INIT.ToInt()].value = 2;
        stageTextValues[StageText.WEAPON_CHANGE.ToInt()].value = stageDatas[WEAPON_CHANGE];
        stageTextValues[StageText.WEAPON_CHANGE_INIT.ToInt()].value = stageDatas[WEAPON_CHANGE];

        
        enemy = new GameObject[stageDatas[ENEMY_COUNT]];
        friend = new GameObject[3];
        treasure = new();

        clickIgnorePanel.SetActive(true);

        // if(StaticManager.Instance.Stage != 1) 
            StartCoroutine(StartStage());
    }
    public int GetStageTextValue(StageText text)
    {
        return stageTextValues[text.ToInt()].value;
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
            stageTextValues[StageText.MOVE.ToInt()].value = stageTextValues[StageText.MOVE_INIT.ToInt()].value;
            stageTextValues[StageText.ROTATE.ToInt()].value = stageTextValues[StageText.ROTATE_INIT.ToInt()].value;
        }
        else stageTextValues[text.ToInt()].value += addValue;

        stageTextValues[StageText.MONSTER.ToInt()].value = 0;
        foreach (GameObject e in enemy)
        {
            if (e.GetComponent<Object>().HP > 0) stageTextValues[StageText.MONSTER.ToInt()].value++;
        }
            
        stageTexts[2].text = $"{stageTextValues[StageText.ROTATE.ToInt()].value} / {stageTextValues[StageText.ROTATE_INIT.ToInt()].value}";
        stageTexts[3].text = $"{stageTextValues[StageText.MOVE.ToInt()].value} / {stageTextValues[StageText.MOVE_INIT.ToInt()].value}";
        stageTexts[4].text = $"{stageTextValues[StageText.WEAPON_CHANGE.ToInt()].value}";
        stageTexts[5].text = $"{stageTextValues[StageText.MONSTER.ToInt()].value} / {stageTextValues[StageText.MONSTER_INIT.ToInt()].value}";
    }
    public IEnumerator StartStage()
    {
        yield return new WaitForFixedUpdate();

        // 보물 소환
        SummonStageTreasure(stageDatas[TREASURE_COUNT]);
        yield return new WaitForFixedUpdate();

        // 화면 밝아짐
        ScreenEffect.Instance.Fade(1, 0, 1);
        yield return new WaitForSeconds(2f);

        //포탈에서 플레이어 생성
        GameObject portal = ObjectManager.Instance.Summons(StageCube.Instance.touchArray[WHITE][3], ObjectType.PORTAL, 0);
        ObjectEffect.Instance.MakeBig(portal);
        yield return new WaitForSeconds(1f);
        ObjectManager.Instance.Summons(StageCube.Instance.touchArray[WHITE][3], ObjectType.PLAYER, 0);
        yield return new WaitForSeconds(1f);
        player.gameObject.SetActive(true);
        ColorCheckManager.Instance.CharacterSelect(player);
        StartCoroutine(ColorCheckManager.Instance.MoveCoroutine(WHITE, 4, true));
        yield return new WaitForSeconds(1f);
        ObjectEffect.Instance.MakeSmall(portal);
        yield return new WaitForSeconds(2f);

        // 큐브 섞기
        cubeManager.StartRandomTurn(stageDatas[MIX], false);
        yield return new WaitForSeconds(5);

        // 플레이어 쪽으로 회전
        StartCoroutine(CubeRotate(player.GetComponent<Object>().Color));
        yield return new WaitForSeconds(1.2f);

        // 적 소환 (1. 소환할 큐브 블록을 미리 정함 -> 2. 파티클 재생 -> 3. 적 소환)
        SummonStageEnemy();
        yield return new WaitForSeconds(1.2f);

        // UI 및 체력바 활성화
        startPanelText = stageStartPanel.transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>();
        startPanelText.text = $"Stage {StaticManager.Instance.Stage}";

        // UI 활성화
        ScreenEffect.Instance.SetUIActive(true);
        stageStartPanel.SetActive(true);
        yield return new WaitForSeconds(1f);
        stageStartPanel.SetActive(false);
        clickIgnorePanel.SetActive(false);

        // 체력바 활성화
        player.GetComponent<Object>().OverheadCanvas.SetActive(true);
        foreach (GameObject obj in enemy) obj.GetComponent<Object>().OverheadCanvas.SetActive(true);

        ChangeStatus(StageStatus.PLAYER);

        EventManager.Instance.BingoCheck();
    }
    public void SummonStageTreasure(int count)
    {
        for (int i = 0; i < count; i++) // treasure 배치
        {
            Touch cube;
            while (true)
            {
                int cubeColor = Random.Range(0, 6), cubeIndex = Random.Range(0, 9);
                cube = StageCube.Instance.touchArray[cubeColor][cubeIndex];

                if (cube.Obj != null) continue;

                if (StatusOfStage == StageStatus.INIT)
                {
                    if ((cubeColor == WHITE && cubeIndex == 3) || (cubeColor == WHITE && cubeIndex == 4))
                        continue;
                }

                break;
            }

            treasure.Add(ObjectManager.Instance.Summons(cube, ObjectType.TRIGGER, 0));
            treasure[i].GetComponent<Object>().SetWeapon(stageDatas[TREASURE_MIN], stageDatas[TREASURE_MAX], WeaponType.NULL);
        }
    }
    public void SummonStageEnemy()
    {
        int index = 0;
        List<string> stageEnemy = StaticManager.Instance.stageEnemyDatas[StaticManager.Instance.Stage];

        for (int i = 0; i < stageEnemy.Count; i++) // enemy 배치
        {
            for (int j = 0; j < int.Parse(stageEnemy[i].Split(',')[STAGE_ENEMY_COUNT]); j++)
            {
                enemy[index] = ObjectManager.Instance.Summons(null, ObjectType.ENEMY, int.Parse(stageEnemy[i].Split(',')[STAGE_ENEMY_ID]));
                index++;
            }
        }
    }
    public void ClickFightButton()
    {
        if (StatusOfStage == StageStatus.PLAYER) ChangeStatus(StageStatus.FIGHT);
    }
    public void ChangeStatus(StageStatus stageStatus)
    {
        cubeManager.ChangeToNormal();
        sequence = DOTween.Sequence();

        StopCoroutine(fightLogic.FightLogicStart());
        StopCoroutine(envLogic.EnvLogicStart());

        switch (stageStatus)
        {
            case StageStatus.PLAYER:
                ScreenEffect.Instance.SetUIActive(true);
                StatusOfStage = StageStatus.PLAYER;
                SetStageTextValue(StageText.ALL_INIT, 0);
                clickIgnorePanel.SetActive(false);
                StartCoroutine(CubeRotate(player.GetComponent<Object>().Color)); // 플레이어 쪽으로 회전
                TurnLimit();
                EventManager.Instance.ChangeAtPlayerTurn();
                break;

            case StageStatus.FIGHT:
                StatusOfStage = StageStatus.FIGHT;
                if (Boss.Instance) Boss.Instance.FightTurn();
                ScreenEffect.Instance.SetUIActive(false);
                ScreenEffect.Instance.StatusChangeEffect();
                EventManager.Instance.Effect.Effect(); // 축복이나 저주 발동
                StartCoroutine(fightLogic.FightLogicStart());
                break;

            case StageStatus.ENV:
                StatusOfStage = StageStatus.ENV;
                StartCoroutine(envLogic.EnvLogicStart());
                break;

            case StageStatus.END:
                StatusOfStage = StageStatus.END;
                break;
        }
    }
    public void CheckStageClear()
    {
        if (stageTextValues[StageText.MONSTER.ToInt()].value > 0) return;
        StartCoroutine(StageClear());
    }
    private IEnumerator StageClear()
    {
        clickIgnorePanel.SetActive(true);
        ChangeStatus(StageStatus.END);
        SetStageTextValue(StageText.END, 0);

        // 스테이지 종료 시 동료, 트리거 소멸
        foreach (GameObject f in friend)
            if (f != null && f.activeSelf) f.GetComponent<Object>().OnHit(StatusEffect.HP_PERCENT, 100);
        foreach (GameObject t in treasure)
            ObjectManager.Instance.ObjectDie(t);
            
        EventManager.Instance.StageEnd();

        yield return new WaitForSeconds(3);
        //스테이지 종료 시 플레이어 HP Bar 비활성화
        player.GetComponent<Object>().OverheadCanvas.SetActive(false);
        cubeManager.InverseTurn();
        yield return new WaitForSeconds(3);
        StartCoroutine(CubeRotate(Player.Color));
        yield return new WaitForSeconds(2f);

        // 스테이지 클리어 시 골드 획득
        StaticManager.Instance.Gold += stageDatas[REWARD];

        // 상인과 포탈 소환
        ObjectType[] summonObjectArray = new ObjectType[2] { ObjectType.MERCHANT, ObjectType.PORTAL };
        for (int i = 0; i < summonObjectArray.Length; i++)
        {
            Touch cube;
            while (true)
            {
                cube = StageCube.Instance.touchArray[Player.Color.ToInt()][Random.Range(0, 9)];
                if (cube.Obj == null)
                {
                    ObjectManager.Instance.Summons(cube, summonObjectArray[i], 0);
                    break;
                }
            }
        }

        clickIgnorePanel.SetActive(false);
    }
    public void NextStage(GameObject portal)
    {
        StartCoroutine(NextStageCoroutine(portal));
    }
    private IEnumerator NextStageCoroutine(GameObject portal)
    {
        cubeManager.ChangeToNormal();
        yield return new WaitForSeconds(1f);
        player.SetActive(false);
        ObjectEffect.Instance.MakeSmall(portal);
        yield return new WaitForSeconds(1);
        StaticManager.Instance.GameStart(false);
    }
    public void GameOver()
    {
        // 게임 오버
        gameOverPanel.SetActive(true);
    }
    public void LoadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public IEnumerator CubeRotate(Colors color)
    {
        if (isCubeMove) yield break;
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

    private void TurnLimit()
    {
        stageTexts[1].text = $"{++turn} Turn";

        if (StaticManager.Instance.Stage < 10 && turn  >= 3)
        {
            player.GetComponent<Object>().OnHit(StatusEffect.HP_PERCENT, 10);
        }
    }

    public void SummonsFriend(Touch cube, int scrollID)
    {
        if (cube != null && cube.Obj != null) return;

        for (int i = 0; i < 3; i++)
        {
            if (friend[i].GetComponent<Object>().HP <= 0) // 이건 동료 소환이 한 스테이지에서 3번만 가능할 때긴 함
            {
                friend[i] = ObjectManager.Instance.Summons(cube, ObjectType.FRIEND, StaticManager.Instance.scrollDatas[scrollID].FriendIndex);
                ObjectManager.Instance.UseItem(ItemType.SCROLL, scrollID);
                break;
            }
        }
    }
}
