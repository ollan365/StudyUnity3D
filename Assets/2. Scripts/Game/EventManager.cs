using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static Constants;
using TMPro;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    [SerializeField] private CubeManager cubeManager;

    [Header("Bingo")]
    [SerializeField] private BingoStatus[] bingoStatus;
    [SerializeField] private BingoUI[] bingoUI;

    [Header("UI")]
    [SerializeField] private GameObject[] setActiveFalse;

    [Header("Event")]
    [SerializeField] private GameObject eventPanel;
    [SerializeField] private Button[] eventButtons;
    [SerializeField] private TMP_Text[] eventNameTexts;
    [SerializeField] private TMP_Text[] eventDescriptionTexts;
    [SerializeField] private EventCard[] eventCards;
    [SerializeField] private ColorEffect colorEffect = new ColorEffect(Colors.NULL);
    public ColorEffect Effect { get => colorEffect; }
    private Colors[][] bingoCheck;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        bingoStatus = new BingoStatus[6];
        for (int i = 0; i < 6; i++) bingoStatus[i] = BingoStatus.NONE;

        bingoCheck = new Colors[6][];
        for (int i = 0; i < 6; i++)
        {
            bingoCheck[i] = new Colors[9];
            for (int j = 0; j < 9; j++) bingoCheck[i][j] = Colors.NULL;
        }
    }

    public void BingoCheck()
    {
        if (StageManager.Instance.StatusOfStage != StageStatus.PLAYER) return;

        BingoMark();

        Touch cube = StageManager.Instance.Player.touchCube;
        Colors color = bingoCheck[cube.Color.ToInt()][cube.Index];

        if (color == Colors.NULL) return;

        if (color == Colors.WY) bingoStatus[cube.RelativeColor.ToInt()] = BingoStatus.ALL;
        else if (color != Colors.WY) bingoStatus[color.ToInt()] = BingoStatus.ONE;

        eventPanel.SetActive(true);
        colorEffect = new ColorEffect(color);
        bingoUI[color.ToInt()].SetLineIcon();
        cubeManager.IsEvent = true;
        foreach (GameObject obj in setActiveFalse) obj.SetActive(false);
        EventAdd();
    }
    private void BingoMark()
    {
        // 빙고 조건
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 9; j++) bingoCheck[i][j] = Colors.NULL;
        }

        List<int>[] list = new List<int>[6]
        {
                new () {0,1,2 },
                new () {3,4,5 },
                new () {6,7,8 },
                new () {0,3,6 },
                new () {1,4,7 },
                new () {2,5,8 }
        };

        for (int i = 0; i < 6; i++)
        {
            int[] bingoNum = new int[6] { 0, 0, 0, 0, 0, 0 };
            int[] colorOfSide = new int[9];

            for (int j = 0; j < 9; j++)
            {
                colorOfSide[j] = StageCube.Instance.touchArray[i][j].RelativeColor.ToInt();
            }

            for(int j = 0; j < 6; j++)
            {
                int color = colorOfSide[list[j][0]];
                bool bingo = true;
                for(int k= 0; k < list[j].Count; k++)
                {
                    if (color != colorOfSide[list[j][k]]) bingo = false;
                }
                if (bingo)
                {
                    bingoNum[color]++;

                    if (bingoStatus[color] != BingoStatus.NONE) continue;

                    Debug.Log($"면: {i.ToColor()} / 빙고색: {color.ToColor()}");

                    for (int k = 0; k < list[j].Count; k++)
                    {
                        bingoCheck[i][list[j][k]] = color.ToColor();
                    }
                }
            }

            // 한면 빙고가 있는지 확인
            for (int j = 0; j < 6; j++)
            {
                if (bingoNum[j] == 6 && bingoStatus[j] != BingoStatus.ALL)
                {
                    for (int k = 0; k < 9; k++) bingoCheck[i][k] = Colors.WY;
                }
            }
        }
    }

    public void StageEnd()
    {
        // 선악과 괴뢰 소멸
        // 상태 이상 초기화
    }
    private void EventAdd()
    {
        foreach (Button b in eventButtons) b.onClick.RemoveAllListeners();

        List<EventCard> eventList = new();

        foreach(EventCard card in eventCards)
        {
            foreach(Colors c in card.eventColors)
            {
                if (c == colorEffect.color && CheckEvent(card.eventName)) eventList.Add(card);
            }
        }

        int random_1 = Random.Range(0, eventList.Count);
        int random_2 = random_1;
        while (eventList.Count > 1 && random_1 == random_2) random_2 = Random.Range(0, eventList.Count);

        eventButtons[0].onClick.AddListener(() => Event(eventList[random_1].eventName));
        eventButtons[1].onClick.AddListener(() => Event(eventList[random_2].eventName));

        eventNameTexts[0].text = eventList[random_1].eventName;
        eventNameTexts[1].text = eventList[random_2].eventName;

        eventDescriptionTexts[0].text = eventList[random_1].EventDescription[0];
        eventDescriptionTexts[1].text = eventList[random_2].EventDescription[0];
    }
    

    private bool CheckEvent(string name)
    {
        switch (name)
        {
            case "악마의 교활함":
            case "잔혹한 계약":
            case "등가교환":
            case "뒤통수":
                if (FriendCount > 0) return true;
                else return false;

            case "최후의 저항":
                if (RandomDeadObject() != null) return true;
                else return false;

            case "청부살인":
            case "오아시스":
            case "복권 구매":
                if (StaticManager.Instance.Gold > 100) return true;
                else return false;

            case "매수":
                if (StaticManager.Instance.Gold > 100 && RandomDeadFriend() != null) return true;
                else return false;

            case "병 주고 약 주고":
                if (RandomDeadEnemy() != null) return true;
                else return false;

            default: return true;
        }
    }
    public void Event(string name)
    {
        switch (name)
        {
            case "거울":
                StartCoroutine(Event_16());
                break;

            case "최후의 저항":
                StartCoroutine(Revive(RandomDeadObject()));
                break;

            case "등가교환":
                RandomObejectOfPlayerTeam(false).OnHit(StatusEffect.HP_PERCENT, 100);
                RandomObejectOfPEnemy().OnHit(StatusEffect.HP_PERCENT, 100);
                break;

            case "잔혹한 계약":
                Object friendObj = RandomObejectOfPlayerTeam(false);
                StageManager.Instance.Player.OnHit(StatusEffect.HP, friendObj.HP * 50 / 100);
                friendObj.OnHit(StatusEffect.HP_PERCENT, 100);
                break;

            case "선악과":
                ObjectManager.Instance.Summons(null, ObjectType.TRIGGER, 1);
                break;

            case "지뢰":
                ObjectManager.Instance.Summons(null, ObjectType.TRIGGER, 2);
                break;

            case "너 죽고 나 죽자":
                foreach (Object obj in ObjectList(Colors.RED))
                    obj.OnHit(StatusEffect.HP_PERCENT, 50);
                break;

            case "축복의 땅":
                foreach (Object obj in ObjectList(Colors.RED))
                    obj.OnHit(StatusEffect.HP_PERCENT, -50);
                break;

            case "피의 대가":
                colorEffect.Add(POWERFUL);
                colorEffect.Add(CURSE);
                break;

            case "비폭력주의":
                colorEffect.Add(SLIENCE);
                break;

            case "공격태세":
                colorEffect.Add(POWERFUL);
                break;

            case "낙인":
                colorEffect.Add(WEAKEN);
                break;

            case "회피의 달인":
                colorEffect.Add(INVINCIBILITY);
                break;

            case "땅 파면 오백원 나오나?":
                StaticManager.Instance.Gold += 500;
                break;

            case "청부살인":
                StaticManager.Instance.Gold -= 100;
                RandomObejectOfPEnemy().OnHit(StatusEffect.HP_PERCENT, 100);
                break;

            case "매수":
                StaticManager.Instance.Gold -= 100;
                StartCoroutine(Revive(RandomDeadFriend()));
                break;

            case "복권 구매":
                StaticManager.Instance.Gold -= 100;
                StaticManager.Instance.Gold += Random.Range(50, 200);
                break;

            case "병 주고 약 주고":
                StaticManager.Instance.Gold += 100;
                StartCoroutine(Revive(RandomDeadEnemy()));
                break;

            case "뒤통수":
                StaticManager.Instance.Gold += 100;
                RandomObejectOfPlayerTeam(false).OnHit(StatusEffect.HP_PERCENT, 100);
                break;

            case "회전 증폭":
                StageManager.Instance.SetStageTextValue(StageText.ROTATE, 5);
                break;

            case "회전 추가":
                StageManager.Instance.SetStageTextValue(StageText.ROTATE_INIT, 5);
                break;

            case "빠른 손놀림":
                StageManager.Instance.SetStageTextValue(StageText.WEAPON_CHANGE, 5);
                break;

            case "기민한 발놀림":
                StageManager.Instance.SetStageTextValue(StageText.MOVE, 5);
                break;

            case "민첩한 걸음":
                StageManager.Instance.SetStageTextValue(StageText.MOVE_INIT, 5);
                break;

            case "운수대통":
                foreach (GameObject obj in StageManager.Instance.TreasureList)
                    if (obj.GetComponent<Object>().touchCube.RelativeColor == Effect.color)
                        StageManager.Instance.StagePlayLogic.Trigger(obj);
                break;

            case "오아시스":
                StaticManager.Instance.Gold -= 100;
                ObjectManager.Instance.AddItem(110000, null);
                break;

            case "미궁":
                cubeManager.StartRandomTurn(5, true);
                break;

            default: Debug.Log(name); break;
        }

        StartCoroutine(EffectAfterEvent());
    }
    private IEnumerator EffectAfterEvent()
    {
        eventPanel.SetActive(false);
        yield return new WaitForSeconds(1);

        foreach (GameObject obj in setActiveFalse) obj.SetActive(true);
        cubeManager.IsEvent = false;
    }

    private IEnumerator Revive(Object obj)
    {
        Touch cube;
        while (true)
        {
            cube = StageCube.Instance.touchArray[Random.Range(0, 6)][Random.Range(0, 9)];
            if (cube.Obj == null)
                break;
        }

        StageManager.Instance.CubeRotate(cube.Color);
        yield return new WaitForSeconds(1f);

        obj.gameObject.SetActive(true);

        ColorCheckManager.Instance.CharacterSelect(obj.gameObject);
        StartCoroutine(ColorCheckManager.Instance.MoveCoroutine(cube.Color, cube.Index));
        ColorCheckManager.Instance.CharacterSelectCancel(null, true);

        obj.OnHit(StatusEffect.HP_PERCENT, -100);

        Debug.Log($"{obj} : {obj.HP} / Touch: {cube.Color} {cube.Index}");
    }
    private IEnumerator Event_16()
    {
        Touch inverseTouch = InverseCube(StageManager.Instance.Player.touchCube);

        if (inverseTouch.Obj != null)
        {
            Touch objInverseTouch = StageManager.Instance.Player.touchCube;
            ColorCheckManager.Instance.CharacterSelect(inverseTouch.Obj.gameObject);
            StartCoroutine(ColorCheckManager.Instance.MoveCoroutine(objInverseTouch.Color, objInverseTouch.Index));
            ColorCheckManager.Instance.CharacterSelectCancel(null, true);
            inverseTouch.Obj.transform.eulerAngles = Vector3.zero;
        }

        ColorCheckManager.Instance.CharacterSelect(StageManager.Instance.Player.gameObject);
        StartCoroutine(ColorCheckManager.Instance.MoveCoroutine(inverseTouch.Color, inverseTouch.Index));
        ColorCheckManager.Instance.CharacterSelectCancel(null, true);
        StageManager.Instance.Player.transform.eulerAngles = Vector3.zero;

        yield return null;
    }

    // ========== 편의를 위해 만든 함수 ========== //
    public List<Object> ObjectList(Colors color)
    {
        List<Object> output = new();

        if (StageManager.Instance.Player.touchCube.RelativeColor == color)
            output.Add(StageManager.Instance.Player);

        foreach(GameObject f in StageManager.Instance.FriendList)
        {
            if (f == null || !f.activeSelf) continue;

            Object friendObject = f.GetComponent<Object>();
            if (friendObject.touchCube.RelativeColor == color) output.Add(friendObject);
        }

        foreach (GameObject e in StageManager.Instance.EnemyList)
        {
            if (e == null || !e.activeSelf) continue;

            Object enemyObject = e.GetComponent<Object>();
            if (enemyObject.touchCube.RelativeColor == color) output.Add(enemyObject);
        }

        return output;
    }
    private Object RandomDeadObject()
    {
        if (RandomDeadFriend() == null && RandomDeadEnemy() == null) return null;
        else if (RandomDeadFriend() == null) return RandomDeadEnemy();
        else if (RandomDeadEnemy() == null) return RandomDeadFriend();

        int random = Random.Range(0, 100);

        if (random < 50) return RandomDeadFriend();
        else return RandomDeadEnemy();
    }
    private Object RandomDeadFriend()
    {
        int deadFriendCount = 0;
        foreach (GameObject obj in StageManager.Instance.FriendList)
            if (obj != null && !obj.activeSelf) deadFriendCount++;

        if (deadFriendCount == 0) return null;

        int random = Random.Range(0, deadFriendCount);
        foreach (GameObject obj in StageManager.Instance.FriendList)
        {
            if (obj != null && !obj.activeSelf)
            {
                if (random > 0) { random--; continue; }

                return obj.GetComponent<Object>();
            }
        }
        return null;
    }
    private Object RandomDeadEnemy()
    {
        int deadEnemyCount = 0;
        foreach (GameObject obj in StageManager.Instance.EnemyList)
            if (!obj.activeSelf) deadEnemyCount++;

        if (deadEnemyCount == 0) return null;

        int random = Random.Range(0, deadEnemyCount);
        foreach (GameObject obj in StageManager.Instance.EnemyList)
        {
            if (obj != null && !obj.activeSelf)
            {
                if (random > 0) { random--; continue; }

                return obj.GetComponent<Object>();
            }
        }
        return null;
    }
    private int FriendCount
    {
        get
        {
            int count = 0;
            for (int i = 0; i < 3; i++)
            {
                if (StageManager.Instance.FriendList[i] != null &&
                    StageManager.Instance.FriendList[i].activeSelf)
                    count++;
            }
            return count;
        }
    }
    private int EnemyCount
    {
        get
        {
            int count = 0;
            for (int i = 0; i < StageManager.Instance.EnemyList.Length; i++)
            {
                if (StageManager.Instance.EnemyList[i].activeSelf)
                    count++;
            }
            return count;
        }
    }
    private Object RandomObejectOfPlayerTeam(bool includePlayer)
    {
        if (FriendCount == 0 && !includePlayer) return null;

        int count = FriendCount;
        if (includePlayer) count++;

        int random = Random.Range(0, count);

        if (random == count - 1 && includePlayer) return StageManager.Instance.Player;
        else
        {
            for (int i = 0; i < 3; i++)
            {
                if (StageManager.Instance.FriendList[i] != null && StageManager.Instance.FriendList[i].activeSelf)
                {
                    if (random == 0) return StageManager.Instance.FriendList[i].GetComponent<Object>();
                    else random--;
                }
            }
        }

        return null;
    }
    private Object RandomObejectOfPEnemy()
    {
        int random = Random.Range(0, EnemyCount);
        for (int i = 0; i < StageManager.Instance.EnemyList.Length; i++)
        {
            if (StageManager.Instance.EnemyList[i].activeSelf)
            {
                if (random == 0) return StageManager.Instance.EnemyList[i].GetComponent<Object>();
                else random--;
            }
        }

        return null;
    }
    public Object[] AllObject
    {
        get
        {
            Object[] allObject = new Object[1 + FriendCount + EnemyCount];

            int index = 0;

            // 플레이어 추가
            allObject[index] = StageManager.Instance.Player;
            index++;

            // 살아있는 동료 추가
            for(int i = 0; i < 3; i++)
            {
                if(StageManager.Instance.FriendList[i] != null &&
                    StageManager.Instance.FriendList[i].activeSelf)
                {
                    allObject[index] = StageManager.Instance.FriendList[i].GetComponent<Object>();
                    index++;
                }
            }

            // 살아있는 적 추가
            for (int i = 0; i < 3; i++)
            {
                if (StageManager.Instance.EnemyList[i].activeSelf)
                {
                    allObject[index] = StageManager.Instance.EnemyList[i].GetComponent<Object>();
                    index++;
                }
            }

            return allObject;
        }
    }
    private Object[] RandomObejectOfALL(int count)
    {
        // 일단 현재까지로는, count보다 전체 유닛이 더 적으면 그냥 모든 유닛에 효과 적용
        if (AllObject.Length < count) return AllObject;

        // array를 랜덤하게 섞음
        int[] objectIndex = new int[AllObject.Length];
        for (int i = 0; i < objectIndex.Length; i++)
            objectIndex[i] = i;

        for (int i = 0; i < objectIndex.Length; i++)
        {
            int temp = objectIndex[i];
            int randomIndex = Random.Range(i, objectIndex.Length);
            objectIndex[i] = objectIndex[randomIndex];
            objectIndex[randomIndex] = temp;
        }

        Object[] output = new Object[count];
        for(int i = 0; i < count; i++)
        {
            output[i] = AllObject[objectIndex[i]];
        }

        return output;
    }
    private Touch InverseCube(Touch cube)
    {
        int color;
        switch (cube.Color.ToInt())
        {
            case WHITE: color = YELLOW; break;
            case RED: color = ORANGE; break;
            case BLUE: color = GREEN; break;
            case GREEN: color = BLUE; break;
            case ORANGE: color = RED; break;
            case YELLOW: color = WHITE; break;

            default: color = -1; break;
        }

        return StageCube.Instance.touchArray[color][cube.Index];
    }
}

[System.Serializable]
public class ColorEffect
{
    public Colors color;
    public bool[] statusEffects = new bool[6] { false, false, false, false, false, false};

    public ColorEffect(Colors color)
    {
        this.color = color;
    }
    public void Add(int effect)
    {
        statusEffects[effect] = true;
    }
    public int Dealt(Object obj)
    {
        if (obj.touchCube.RelativeColor != color) return 1;

        if (statusEffects[SLIENCE]) return 0;
        else if (statusEffects[POWERFUL]) return 2;
        else return 1;
    }

    public float Received(Object obj)
    {
        if (obj.touchCube.RelativeColor != color) return 1;

        if (statusEffects[INVINCIBILITY]) return 0;
        else if (statusEffects[WEAKEN]) return 2;
        else return 1;
    }

    public void Effect()
    {
        if (statusEffects[BLESS])
        {
            foreach (Object obj in EventManager.Instance.ObjectList(color))
                obj.OnHit(StatusEffect.HP_PERCENT, -10);
        }
        else if (statusEffects[CURSE])
        {
            foreach (Object obj in EventManager.Instance.ObjectList(color))
                obj.OnHit(StatusEffect.HP_PERCENT, 10);
        }
    }
}