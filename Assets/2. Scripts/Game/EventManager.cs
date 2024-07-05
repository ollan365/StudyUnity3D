using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Constants;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    [SerializeField] private Button[] eventButtons;
    [SerializeField] private EventCard[] eventCards;
    private BingoStatus[] bingoStatus;
    private ColorEffect colorEffect = new ColorEffect(Colors.NULL);
    public ColorEffect Effect { get => colorEffect; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        bingoStatus = new BingoStatus[6];
        for (int i = 0; i < 6; i++) bingoStatus[i] = BingoStatus.NONE;
    }
    public int BingoCheck()
    {
        int[][] bingoNums = new int[6][];
        for (int i = 0; i < 6; i++)
        {
            int[] colorOfSide = new int[9];
            int[] bingoNum = new int[6];

            for (int j = 0; j < 9; j++)
            {
                colorOfSide[j] = StageCube.Instance.touchArray[i][j].RelativeColor.ToInt();
                bingoNum[j % 6] = 0; // %는 그냥 배열 크기가 6이라 에러 안 나도록 해둔거
            }

            if (colorOfSide[0] == colorOfSide[1] && colorOfSide[1] == colorOfSide[2])
                bingoNum[colorOfSide[0]]++;
            if (colorOfSide[3] == colorOfSide[4] && colorOfSide[4] == colorOfSide[5])
                bingoNum[colorOfSide[3]]++;
            if (colorOfSide[6] == colorOfSide[7] && colorOfSide[7] == colorOfSide[8])
                bingoNum[colorOfSide[6]]++;
            if (colorOfSide[0] == colorOfSide[3] && colorOfSide[3] == colorOfSide[6])
                bingoNum[colorOfSide[0]]++;
            if (colorOfSide[1] == colorOfSide[4] && colorOfSide[4] == colorOfSide[7])
                bingoNum[colorOfSide[1]]++;
            if (colorOfSide[2] == colorOfSide[5] && colorOfSide[5] == colorOfSide[8])
                bingoNum[colorOfSide[2]]++;

            bingoNums[i] = bingoNum;

        }

        bool[] playerTeam = new bool[6] { false, false, false, false, false, false };
        playerTeam[StageManager.Instance.Player.Color.ToInt()] = true;
        foreach (GameObject f in StageManager.Instance.FriendList)
            if (f != null && f.activeSelf)
                playerTeam[f.GetComponent<Object>().Color.ToInt()] = true;

        for (int j = 0; j < 6; j++) // 각각의 색
        {
            for (int i = 0; i < 6; i++) // 각각의 면
            {
                if (!playerTeam[i]) continue;

                if (bingoNums[i][j] == 6 && bingoStatus[j] != BingoStatus.ALL)
                {
                    bingoStatus[j] = BingoStatus.ALL;
                    Bingo(i, BingoStatus.ALL);
                }
                else if (bingoNums[i][j] > 0 & bingoStatus[j] == BingoStatus.NONE)
                {
                    bingoStatus[j] = BingoStatus.ONE;
                    Bingo(i, BingoStatus.ONE);
                }
            }
        }

        return 0;
    }

    public void Bingo(int color, BingoStatus bingo)
    {
        // 이벤트 창 띄우기 필요

        colorEffect = new ColorEffect(color.ToColor());
        EventAdd(color);
    }
    public void StageEnd()
    {
        // 선악과 괴뢰 소멸
        // 상태 이상 초기화
    }
    private void EventAdd(int color)
    {
        foreach (Button b in eventButtons) b.onClick.RemoveAllListeners();

        List<EventCard> eventList = new();

        foreach(EventCard card in eventCards)
        {
            foreach(Colors c in card.colors)
            {
                if (c == color.ToColor() && CheckEvent(card.name)) eventList.Add(card);
            }
        }

        int random_1 = Random.Range(0, eventList.Count);
        int random_2 = random_1;
        while (random_1 == random_2) random_2 = Random.Range(0, eventList.Count);

        eventButtons[0].onClick.AddListener(() => Event(eventList[random_1].name));
        eventButtons[1].onClick.AddListener(() => Event(eventList[random_2].name));
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

            case "이름미정_1":
            case "이름미정_3":
            case "복권 구매":
                if (StaticManager.Instance.Gold > 100) return true;
                else return false;

            case "이름미정_2":
                if (StaticManager.Instance.Gold > 100 && RandomDeadFriend() != null) return true;
                else return false;

            default: return true;
        }
    }
    private void Event(string name)
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

            case "괴뢰":
                ObjectManager.Instance.Summons(null, ObjectType.TRIGGER, 2);
                break;

            case "너 죽고 나 죽자":
                foreach(Object obj in ObjectList(Colors.RED))
                    obj.OnHit(StatusEffect.HP_PERCENT, 50);
                break;

            case "축복의 땅":
                foreach (Object obj in ObjectList(Colors.RED))
                    obj.OnHit(StatusEffect.HP_PERCENT, -50);
                break;

            case "피의 각성":
                colorEffect.Add(StatusEffect.POWERFUL);
                colorEffect.Add(StatusEffect.CURSE);
                break;

            case "비폭력주의":
                colorEffect.Add(StatusEffect.SLIENCE);
                break;

            case "공격태세":
                colorEffect.Add(StatusEffect.POWERFUL);
                break;

            case "낙인":
                colorEffect.Add(StatusEffect.WEAKEN);
                break;

            case "회피의 달인":
                colorEffect.Add(StatusEffect.INVINCIBILITY);
                break;

            case "땅 파면 오백원 나오나?":
                StaticManager.Instance.Gold += 500;
                break;

            case "이름미정_1":
                StaticManager.Instance.Gold -= 100;
                RandomObejectOfPEnemy().OnHit(StatusEffect.HP_PERCENT, 100);
                break;

            case "이름미정_2":
                StaticManager.Instance.Gold -= 100;
                StartCoroutine(Revive(RandomDeadFriend()));
                break;

            case "복권 구매":
                StaticManager.Instance.Gold -= 100;
                StaticManager.Instance.Gold += Random.Range(50, 200);
                break;

            case "이름미정_4":
                StaticManager.Instance.Gold += 100;
                StartCoroutine(Revive(RandomDeadEnemy()));
                break;

            case "뒤통수":
                StaticManager.Instance.Gold += 100;
                RandomObejectOfPlayerTeam(false).OnHit(StatusEffect.HP_PERCENT, 100);
                break;

            case "이름미정_5":
                StageManager.Instance.SetStageTextValue(StageText.ROTATE, 5);
                break;

            case "이름미정_6":
                StageManager.Instance.SetStageTextValue(StageText.ROTATE_INIT, 5);
                break;

            case "이름미정_7":
                StageManager.Instance.SetStageTextValue(StageText.WEAPON_CHANGE, 5);
                break;

            case "이름미정_8":
                StageManager.Instance.SetStageTextValue(StageText.MOVE, 5);
                break;

            case "이름미정_9":
                StageManager.Instance.SetStageTextValue(StageText.MOVE_INIT, 5);
                break;
        }
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
        }

        ColorCheckManager.Instance.CharacterSelect(StageManager.Instance.Player.gameObject);
        StartCoroutine(ColorCheckManager.Instance.MoveCoroutine(inverseTouch.Color, inverseTouch.Index));
        ColorCheckManager.Instance.CharacterSelectCancel(null, true);

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
        // 게임 오버를 언제 체크할지 고민해보고 변경 후 지우기
        bool gameOver = true;
        foreach (GameObject e in StageManager.Instance.EnemyList)
            if (e.activeSelf) gameOver = false;
        if (gameOver)
        {
            StageManager.Instance.GameOver();
            return null;
        }

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
    private Object[] AllObject
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
        int color = -1;
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

public class ColorEffect
{
    private Colors color;
    private bool[] statusEffects = new bool[6] { false, false, false, false, false, false};

    public ColorEffect(Colors color)
    {
        this.color = color;
    }
    public void Add(StatusEffect effect)
    {
        statusEffects[Int(effect)] = true;
    }
    public int Dealt(Object obj)
    {
        if (obj.touchCube.RelativeColor != color) return 1;

        if (statusEffects[Int(StatusEffect.SLIENCE)]) return 0;
        else if (statusEffects[Int(StatusEffect.POWERFUL)]) return 2;
        else return 1;
    }

    public float Received(Object obj)
    {
        if (obj.touchCube.RelativeColor != color) return 1;

        if (statusEffects[Int(StatusEffect.INVINCIBILITY)]) return 0;
        else if (statusEffects[Int(StatusEffect.WEAKEN)]) return 2;
        else return 1;
    }

    public void Effect()
    {
        if (statusEffects[Int(StatusEffect.BLESS)])
        {
            foreach (Object obj in EventManager.Instance.ObjectList(color))
                obj.OnHit(StatusEffect.HP_PERCENT, -10);
        }
        else if (statusEffects[Int(StatusEffect.CURSE)])
        {
            foreach (Object obj in EventManager.Instance.ObjectList(color))
                obj.OnHit(StatusEffect.HP_PERCENT, 10);
        }
    }

    public int Int(StatusEffect effect)
    {
        switch (effect)
        {
            case StatusEffect.SLIENCE: return 0; // 침묵
            case StatusEffect.POWERFUL: return 1; // 강화
            case StatusEffect.INVINCIBILITY: return 2; // 무적
            case StatusEffect.WEAKEN: return 3; // 취약
            case StatusEffect.BLESS: return 4; // 축복
            case StatusEffect.CURSE: return 5; // 저주

            default: return -1; // ALL
        }
    }
}