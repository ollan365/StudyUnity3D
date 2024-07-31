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
    private TMP_Text startPanelText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject clickIgnorePanel;

    [Header("Status")]
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
        StartCoroutine(StartStage());
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
        yield return new WaitForFixedUpdate();

        // 보물 소환
        for (int i = 0; i < stageDatas[TREASURE_COUNT]; i++) // treasure 배치
        {
            Touch cube;
            while (true)
            {
                int cubeColor = Random.Range(0, 6), cubeIndex = Random.Range(0, 9);
                cube = StageCube.Instance.touchArray[cubeColor][cubeIndex];
                if (cube.Obj != null
                    || cubeColor == WHITE && cubeIndex == 3
                    || cubeColor == WHITE && cubeIndex == 4)
                    continue;

                break;
            }

            treasure[i] = ObjectManager.Instance.Summons(null, ObjectType.TRIGGER, 0);
            treasure[i].GetComponent<Object>().SetWeapon(stageDatas[TREASURE_MIN], stageDatas[TREASURE_MAX], WeaponType.NULL);
            yield return new WaitForFixedUpdate();
        }
        
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
        StartCoroutine(ColorCheckManager.Instance.MoveCoroutine(WHITE, 4));
        yield return new WaitForSeconds(1f);
        ColorCheckManager.Instance.CharacterSelectCancel(null, true);
        ObjectEffect.Instance.MakeSmall(portal);
        yield return new WaitForSeconds(2f);

        // 큐브 섞기
        cubeManager.StartRandomTurn(stageDatas[MIX], false);
        yield return new WaitForSeconds(5);

        // 플레이어 쪽으로 회전
        StartCoroutine(CubeRotate(player.GetComponent<Object>().Color));
        yield return new WaitForSeconds(1.2f);

        // 적 소환 (1. 소환할 큐브 블록을 미리 정함 -> 2. 파티클 재생 -> 3. 적 소환)
        int index = 0;
        List<string> stageEnemy = StaticManager.Instance.stageEnemyDatas[StaticManager.Instance.Stage];

        Touch[] enemyPositions = new Touch[stageDatas[ENEMY_COUNT]];
        for(int i = 0; i < stageDatas[ENEMY_COUNT]; i++)
        {
            Touch cube;
            while (true)
            {
                cube = StageCube.Instance.touchArray[Random.Range(0, 6)][Random.Range(0, 9)];
                if (cube.Obj == null)
                {
                    enemyPositions[i] = cube;
                    ParticleManager.Instance.PlayParticle(cube.gameObject, Particle.Enemy_Summon);
                    break;
                }
            }
        }

        yield return new WaitForSeconds(0.4f);

        for (int i = 0; i < stageEnemy.Count; i++) // enemy 배치
        {
            for (int j = 0; j < int.Parse(stageEnemy[i].Split(',')[STAGE_ENEMY_COUNT]); j++)
            {
                enemy[index] = ObjectManager.Instance.Summons(enemyPositions[index], ObjectType.ENEMY, int.Parse(stageEnemy[i].Split(',')[STAGE_ENEMY_ID]));
                Debug.Log(enemy[index]);
                
                index++;
            }
        }
        yield return new WaitForSeconds(1.2f);

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
                break;

            case StageStatus.FIGHT:
                StatusOfStage = StageStatus.FIGHT;
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
        if (stageTextValues[StageText.MONSTER.ToInt()] > 0) return;
        StartCoroutine(StageClear());
    }
    private IEnumerator StageClear()
    {
        ChangeStatus(StageStatus.END);
        SetStageTextValue(StageText.END, 0);

        // 스테이지 종료 시 동료, 트리거 소멸
        foreach (GameObject f in friend)
            if (f != null && f.activeSelf) f.GetComponent<Object>().OnHit(StatusEffect.HP_PERCENT, 100);
        foreach (GameObject t in treasure)
            t.GetComponent<Object>().OnHit(StatusEffect.HP_PERCENT, 100);
        EventManager.Instance.StageEnd();

        yield return new WaitForSeconds(3);
        cubeManager.InverseTurn();
        yield return new WaitForSeconds(3);
        StartCoroutine(CubeRotate(Player.Color));
        yield return new WaitForSeconds(2f);

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
        ColorCheckManager.Instance.CharacterSelectCancel(player, true);
        yield return new WaitForSeconds(1);
        player.SetActive(false);
        ObjectEffect.Instance.MakeSmall(portal);
        yield return new WaitForSeconds(1);
        StaticManager.Instance.GameStart(false);
    }
    public void GameOver()
    {
        // 게임 오버
        StopCoroutine(fightLogic.FightLogicStart());

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

    private void TurnLimit()
    {
        stageTexts[1].text = $"{++turn} Turn";

        if (StaticManager.Instance.Stage < 10 && turn  >= 3)
        {
            player.GetComponent<Object>().OnHit(StatusEffect.HP_PERCENT, 10);
        }
    }

    public void SummonsFriend(Colors color, int index, int scrollID)
    {
        Touch cube = StageCube.Instance.touchArray[color.ToInt()][index];
        if (cube.Obj != null) return;

        for (int i = 0; i < 3; i++)
        {
            if (friend[i] == null) // 이건 동료 소환이 한 스테이지에서 3번만 가능할 때긴 함
            {
                StartCoroutine(SummonFriendCoroutine(cube, i, scrollID));
                ObjectManager.Instance.UseItem(ItemType.SCROLL, scrollID);
                break;
            }
        }
    }
    private IEnumerator SummonFriendCoroutine(Touch cube, int index, int scrollID)
    {
        ParticleManager.Instance.PlayParticle(cube.gameObject, Particle.Friend_Summon);
        yield return new WaitForSeconds(0.3f);
        friend[index] = ObjectManager.Instance.Summons(cube, ObjectType.FRIEND, StaticManager.Instance.scrollDatas[scrollID].FriendIndex);
    }
}
