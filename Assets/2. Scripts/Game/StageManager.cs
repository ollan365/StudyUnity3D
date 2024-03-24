using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using static Constants;
using static Excel;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [SerializeField] private Transform cube;
    public bool isCubeMove;

    [SerializeField] private CubeManager cubeManager;
    [SerializeField] private ColorCheckManager colorCheckManager;

    [SerializeField] private FightLogic fightLogic;
    [SerializeField] private PlayLogic playLogic;
    [SerializeField] private EnvLogic envLogic;
    public PlayLogic StagePlayLogic { get => playLogic; }

    [SerializeField] private Text[] stageTexts;
    private int[] stageDatas;
    public int StageData(int column)
    {
        return stageDatas[column];
    }
    private int moveCount, rotateCount, changeCount;
    private int additionalMoveCount;

    private GameObject[] enemy, friend, treasure;
    public GameObject[] EnemyList { get => enemy; }
    public GameObject[] FriendList { get => friend; }
    public GameObject[] TreasureList { get => treasure; }

    [SerializeField] private GameObject player;
    public Object Player { get => player.GetComponent<Object>(); }

    [SerializeField] private GameObject stageStartPanel;
    [SerializeField] private GameObject gameOverPanel;

    private StageStatus status;
    public StageStatus StatusOfStage
    { 
        get => status;
        private set
        { 
            status = value;
            switch (value)
            {
                case StageStatus.INIT:
                    stageTexts[0].text = $"{StaticManager.Instance.Stage}층 INIT";
                    break;
                case StageStatus.PLAYER:
                    colorCheckManager.BingoTextChange(-1);
                    stageTexts[0].text = $"{StaticManager.Instance.Stage}층 PLAYER";
                    StageTextChange(true, StageText.ALL, 0);
                    break;
                case StageStatus.ENV:
                    StartCoroutine(envLogic.MoveEnemy());
                    stageTexts[0].text = $"{StaticManager.Instance.Stage}층 ENV";
                    break;
                case StageStatus.FIGHT:
                    stageTexts[0].text = $"{StaticManager.Instance.Stage}층 FIGHT";
                    break;
                case StageStatus.END:
                    stageTexts[0].text = $"{StaticManager.Instance.Stage}층 END";
                    break;
            }
        }
    }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StageInit(string data)
    {
        StatusOfStage = StageStatus.INIT;

        stageDatas = new int[data.Split(',').Length - 1];
        for (int i = 0; i < data.Split(',').Length - 1; i++)
            stageDatas[i] = int.Parse(data.Split(',')[i]);

        Player.init(ObjectType.PLAYER, new string[] { stageDatas[MAX_HP].ToString() });

        stageDatas[ROTATE_COUNT] = 10; // 나중에 계산 공식으로 바꾸기!
        stageDatas[MOVE_COUNT] = 10;
        changeCount = stageDatas[WEAPON_CHANGE];

        enemy = new GameObject[stageDatas[ENEMY_COUNT]];
        friend = new GameObject[3];
        treasure = new GameObject[stageDatas[TREASURE_COUNT]];

        StartCoroutine(StartStage());
    }
    public bool StageTextChange(bool wantChange, StageText text, int value)
    {
        switch (text)
        {
            case StageText.MONSTER:
                int nowEnemyCount = 0;
                for (int i = 0; i < enemy.Length; i++)
                    if (enemy[i].activeSelf) nowEnemyCount++;
                stageTexts[1].text = $"{nowEnemyCount} / {stageDatas[ENEMY_COUNT]}";
                return true;
            case StageText.MOVE:
                if (moveCount + value < 0) return false;
                if (!wantChange) return true;
                moveCount += value;
                break;
            case StageText.ROTATE:
                if (rotateCount + value < 0) return false;
                if (!wantChange) return true;
                rotateCount += value;
                break;
            case StageText.WEAPON_CHANGE:
                if (changeCount + value < 0) return false;
                if (!wantChange) return true;
                changeCount += value;
                break;
            case StageText.ALL:
                if (value != 0) return false;

                nowEnemyCount = 0;
                for (int i = 0; i < enemy.Length; i++)
                    if (enemy[i].activeSelf) nowEnemyCount++;
                
                stageTexts[1].text = $"{nowEnemyCount} / {stageDatas[ENEMY_COUNT]}";

                moveCount = stageDatas[MOVE_COUNT] + additionalMoveCount;
                rotateCount = stageDatas[ROTATE_COUNT];
                break;
            default:
                return false;
        }
        stageTexts[2].text = $"{moveCount} / {stageDatas[MOVE_COUNT] + additionalMoveCount}";
        stageTexts[3].text = $"{rotateCount} / {stageDatas[ROTATE_COUNT]}";
        stageTexts[4].text = $"{changeCount} / 3";
        return true;
    }
    private IEnumerator StartStage()
    {
        //섞기 전 플레이어 비활성화
        player.SetActive(false);

        cubeManager.StartRandomTurn(stageDatas[MIX]); // 큐브를 섞는다

        yield return new WaitForSeconds(10f);

        //섞은 후 플레이어 활성화
        player.SetActive(true);

        List<string> stageEnemy = StaticManager.Instance.stageEnemyDatas[StaticManager.Instance.Stage];
        int index = 0;
        for(int i = 0; i < stageEnemy.Count; i++)
        {
            Touch cube;

            for (int j = 0; j < int.Parse(stageEnemy[i].Split(',')[STAGE_ENEMY_COUNT]); j++)
            {
                while (true)
                {
                    cube = StageCube.Instance.touchArray[Random.Range(0, 6)][Random.Range(0, 9)];
                    if (cube.Obj == null)
                        break;
                }
                enemy[index] = ObjectManager.Instance.Summons(cube, ObjectType.ENEMY, int.Parse(stageEnemy[i].Split(',')[STAGE_ENEMY_ID]));
                index++;
                yield return new WaitForFixedUpdate();
            }
        }

        for (int i = 0; i < stageDatas[TREASURE_COUNT]; i++) // enemy 배치
        {
            Touch cube;
            while (true)
            {
                cube = StageCube.Instance.touchArray[Random.Range(0, 6)][Random.Range(0, 9)];
                if (cube.Obj == null)
                    break;
            }
            treasure[i] = ObjectManager.Instance.Summons(cube, ObjectType.TREASURE, 0);
            treasure[i].GetComponent<Object>().SetWeapon(stageDatas[TREASURE_MIN], stageDatas[TREASURE_MAX], WeaponType.NULL);
            yield return new WaitForFixedUpdate();
        }

        stageStartPanel.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        stageStartPanel.SetActive(false);

        StatusOfStage = StageStatus.PLAYER;

    }
    public void ChangeStatus()
    {
        if(StatusOfStage == StageStatus.PLAYER)
        {
            StatusOfStage = StageStatus.FIGHT;
            StartCoroutine(fightLogic.Attack());
        }
        else if(StatusOfStage == StageStatus.FIGHT)
        {
            StatusOfStage = StageStatus.ENV;
        }
        else if (StatusOfStage == StageStatus.ENV)
        {
            colorCheckManager.ToNextBingo();
            StatusOfStage = StageStatus.PLAYER;
        }
    }
    public void ClearStage()
    {
        StatusOfStage = StageStatus.END;

        foreach(GameObject t in treasure) // 스테이지 종료 시 보물상자 소멸
            t.GetComponent<Object>().OnHit(9999);
        
        while (true)
        {
            Touch cube = StageCube.Instance.touchArray[Random.Range(0, 6)][Random.Range(0, 9)];
            if (cube.Obj == null)
            {
                ObjectManager.Instance.Summons(cube, ObjectType.MERCHANT, 0);
                break;
            }
        }
        while (true)
        {
            Touch cube = StageCube.Instance.touchArray[Random.Range(0, 6)][Random.Range(0, 9)];
            if (cube.Obj == null)
            {
                ObjectManager.Instance.Summons(cube, ObjectType.PORTAL, 0);
                break;
            }
        }

        changeCount = 9999;
        moveCount = 9999;
        rotateCount = 9999;
        stageTexts[2].text = $"{moveCount} / {stageDatas[MOVE_COUNT] + additionalMoveCount}";
        stageTexts[3].text = $"{rotateCount} / {stageDatas[ROTATE_COUNT]}";
        stageTexts[4].text = $"{changeCount} / 3";
    }
    public void NextStage()
    {
        Debug.Log("Next Stage!");
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

    public bool SummonsFriend(Colors color, int index, int scrollID)
    {
        Touch cube = StageCube.Instance.touchArray[color.ToInt()][index];
        if (cube.Obj != null)
            return false;
        for (int i = 0; i < 3; i++)
        {
            if (friend[i] == null) // 이건 동료 소환이 한 스테이지에서 3번만 가능할 때긴 함
            {
                friend[i] = ObjectManager.Instance.Summons(cube, ObjectType.FRIEND, StaticManager.Instance.scrollDatas[scrollID].FriendIndex);
                Debug.Log("summons success!");
                return true;
            }
        }
        return false;
    }
    
}
