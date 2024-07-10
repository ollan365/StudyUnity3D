using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Constants;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    [SerializeField] private GameObject eventPanel;
    [SerializeField] private Button[] eventButtons;
    [SerializeField] private EventCard[] eventCards;
    private BingoStatus[] bingoStatus;
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
        BingoMark();

        Touch cube = StageManager.Instance.Player.touchCube;
        Colors color = bingoCheck[cube.Color.ToInt()][cube.Index];

        if (color == Colors.NULL) return;

        if (color == Colors.WY) bingoStatus[cube.RelativeColor.ToInt()] = BingoStatus.ALL;
        else if (color != Colors.WY) bingoStatus[color.ToInt()] = BingoStatus.ONE;

        eventPanel.SetActive(true);
        colorEffect = new ColorEffect(color);
        EventAdd();
    }
    private void BingoMark()
    {
        // ���� ����
        List<int>[] list = new List<int>[7]
        {
                new () {0,1,2 },
                new () {3,4,5 },
                new () {6,7,8 },
                new () {0,3,6 },
                new () {1,4,7 },
                new () {2,5,8 },
                new () {0,1,2,3,4,5,6,7,8 }
        };

        for (int i = 0; i < 6; i++)
        {
            int[] bingoNum = new int[6] { 0, 0, 0, 0, 0, 0 };
            int[] colorOfSide = new int[9];

            for (int j = 0; j < 9; j++)
            {
                colorOfSide[j] = StageCube.Instance.touchArray[i][j].RelativeColor.ToInt();
            }


            for(int j = 0; j < 7; j++)
            {
                int color = colorOfSide[list[j][0]];
                bool bingo = true;
                for(int k= 0; k < list[j].Count; k++)
                {
                    if (color != colorOfSide[list[j][k]]) bingo = false;
                }
                if (bingo)
                {
                    Debug.Log($"��: {i.ToColor()} / �����: {color.ToColor()}");

                    bingoNum[color]++;

                    if (bingoStatus[color] != BingoStatus.NONE) continue;

                    for (int k = 0; k < list[j].Count; k++)
                    {
                        bingoCheck[i][list[j][k]] = color.ToColor();
                    }
                }
            }

            // �Ѹ� ���� �ִ��� Ȯ��
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
        // ���ǰ� ���� �Ҹ�
        // ���� �̻� �ʱ�ȭ
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
        foreach (Button b in eventButtons) b.onClick.AddListener(() => eventPanel.SetActive(false));

        eventButtons[0].GetComponentInChildren<Text>().text = eventList[random_1].eventName;
        eventButtons[1].GetComponentInChildren<Text>().text = eventList[random_2].eventName;
    }
    private bool CheckEvent(string name)
    {
        switch (name)
        {
            case "�Ǹ��� ��Ȱ��":
            case "��Ȥ�� ���":
            case "���ȯ":
            case "�����":
                if (FriendCount > 0) return true;
                else return false;

            case "������ ����":
                if (RandomDeadObject() != null) return true;
                else return false;

            case "�̸�����_1":
            case "�̸�����_3":
            case "���� ����":
                if (StaticManager.Instance.Gold > 100) return true;
                else return false;

            case "�̸�����_2":
                if (StaticManager.Instance.Gold > 100 && RandomDeadFriend() != null) return true;
                else return false;

            default: return true;
        }
    }
    private void Event(string name)
    {
        switch (name)
        {
            case "�ſ�":
                StartCoroutine(Event_16());
                break;

            case "������ ����":
                StartCoroutine(Revive(RandomDeadObject()));
                break;

            case "���ȯ":
                RandomObejectOfPlayerTeam(false).OnHit(StatusEffect.HP_PERCENT, 100);
                RandomObejectOfPEnemy().OnHit(StatusEffect.HP_PERCENT, 100);
                break;

            case "��Ȥ�� ���":
                Object friendObj = RandomObejectOfPlayerTeam(false);
                StageManager.Instance.Player.OnHit(StatusEffect.HP, friendObj.HP * 50 / 100);
                friendObj.OnHit(StatusEffect.HP_PERCENT, 100);
                break;

            case "���ǰ�":
                ObjectManager.Instance.Summons(null, ObjectType.TRIGGER, 1);
                break;

            case "����":
                ObjectManager.Instance.Summons(null, ObjectType.TRIGGER, 2);
                break;

            case "�� �װ� �� ����":
                foreach(Object obj in ObjectList(Colors.RED))
                    obj.OnHit(StatusEffect.HP_PERCENT, 50);
                break;

            case "�ູ�� ��":
                foreach (Object obj in ObjectList(Colors.RED))
                    obj.OnHit(StatusEffect.HP_PERCENT, -50);
                break;

            case "���� ����":
                colorEffect.Add(StatusEffect.POWERFUL);
                colorEffect.Add(StatusEffect.CURSE);
                break;

            case "����������":
                colorEffect.Add(StatusEffect.SLIENCE);
                break;

            case "�����¼�":
                colorEffect.Add(StatusEffect.POWERFUL);
                break;

            case "����":
                colorEffect.Add(StatusEffect.WEAKEN);
                break;

            case "ȸ���� ����":
                colorEffect.Add(StatusEffect.INVINCIBILITY);
                break;

            case "�� �ĸ� ����� ������?":
                StaticManager.Instance.Gold += 500;
                break;

            case "�̸�����_1":
                StaticManager.Instance.Gold -= 100;
                RandomObejectOfPEnemy().OnHit(StatusEffect.HP_PERCENT, 100);
                break;

            case "�̸�����_2":
                StaticManager.Instance.Gold -= 100;
                StartCoroutine(Revive(RandomDeadFriend()));
                break;

            case "���� ����":
                StaticManager.Instance.Gold -= 100;
                StaticManager.Instance.Gold += Random.Range(50, 200);
                break;

            case "�̸�����_4":
                StaticManager.Instance.Gold += 100;
                StartCoroutine(Revive(RandomDeadEnemy()));
                break;

            case "�����":
                StaticManager.Instance.Gold += 100;
                RandomObejectOfPlayerTeam(false).OnHit(StatusEffect.HP_PERCENT, 100);
                break;

            case "�̸�����_5":
                StageManager.Instance.SetStageTextValue(StageText.ROTATE, 5);
                break;

            case "�̸�����_6":
                StageManager.Instance.SetStageTextValue(StageText.ROTATE_INIT, 5);
                break;

            case "�̸�����_7":
                StageManager.Instance.SetStageTextValue(StageText.WEAPON_CHANGE, 5);
                break;

            case "�̸�����_8":
                StageManager.Instance.SetStageTextValue(StageText.MOVE, 5);
                break;

            case "�̸�����_9":
                StageManager.Instance.SetStageTextValue(StageText.MOVE_INIT, 5);
                break;

            case "�̸�����_10":
                foreach (GameObject obj in StageManager.Instance.TreasureList)
                    if (obj.GetComponent<Object>().touchCube.RelativeColor == Effect.color)
                        StageManager.Instance.StagePlayLogic.Trigger(obj);
                break;

            default: Debug.Log(name); break;
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

    // ========== ���Ǹ� ���� ���� �Լ� ========== //
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
    private Object[] AllObject
    {
        get
        {
            Object[] allObject = new Object[1 + FriendCount + EnemyCount];

            int index = 0;

            // �÷��̾� �߰�
            allObject[index] = StageManager.Instance.Player;
            index++;

            // ����ִ� ���� �߰�
            for(int i = 0; i < 3; i++)
            {
                if(StageManager.Instance.FriendList[i] != null &&
                    StageManager.Instance.FriendList[i].activeSelf)
                {
                    allObject[index] = StageManager.Instance.FriendList[i].GetComponent<Object>();
                    index++;
                }
            }

            // ����ִ� �� �߰�
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
        // �ϴ� ��������δ�, count���� ��ü ������ �� ������ �׳� ��� ���ֿ� ȿ�� ����
        if (AllObject.Length < count) return AllObject;

        // array�� �����ϰ� ����
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
            case StatusEffect.SLIENCE: return 0; // ħ��
            case StatusEffect.POWERFUL: return 1; // ��ȭ
            case StatusEffect.INVINCIBILITY: return 2; // ����
            case StatusEffect.WEAKEN: return 3; // ���
            case StatusEffect.BLESS: return 4; // �ູ
            case StatusEffect.CURSE: return 5; // ����

            default: return -1; // ALL
        }
    }
}