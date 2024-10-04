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
    [SerializeField] private Transform eventButtonParent;
    [SerializeField] private GameObject eventButtonPrefab;
    [SerializeField] private EventCard[] eventCards;
    [SerializeField] private ColorEffect colorEffect = new ColorEffect(Colors.NULL);

    [Header("Weapon")]
    [SerializeField] private Weapon bestSword;
    [SerializeField] private Weapon bestStaff;
    private Weapon originWeapon;

    private bool IsEnhancedWeapon { get; set; }
    private bool IsDual { get; set; }


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

        IsDual = false;
        IsEnhancedWeapon = false;
    }

    public void BingoCheck()
    {
        if (StageManager.Instance.StatusOfStage != StageStatus.PLAYER) return;

        BingoMark();

        Touch cube = StageManager.Instance.Player.touchCube;
        Colors color = bingoCheck[cube.Color.ToInt()][cube.Index];

        if (color == Colors.NULL) return;

        Debug.Log(color);

        if (color == Colors.WY)
        {
            bingoStatus[cube.RelativeColor.ToInt()] = BingoStatus.ALL;
            bingoUI[cube.RelativeColor.ToInt()].SetSideIcon();
        }
        else if (color == Colors.RO)
        {
            for (int i = 0; i < bingoStatus.Length; i++)
            {
                bingoStatus[i] = BingoStatus.ALL;
                bingoUI[i].SetSideIcon();
            }
        }
        else
        {
            bingoStatus[cube.RelativeColor.ToInt()] = BingoStatus.ONE;
            bingoUI[cube.RelativeColor.ToInt()].SetLineIcon();
        }
        cubeManager.ChangeToNormal();
        eventPanel.SetActive(true);
        colorEffect = new ColorEffect(color);
        cubeManager.IsEvent = true;
        foreach (GameObject obj in setActiveFalse) obj.SetActive(false);
        EventAdd(color);
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

        int allCount = 0;
        for (int i = 0; i < 6; i++)
        {
            int[] bingoNum = new int[6] { 0, 0, 0, 0, 0, 0 };
            int[] colorOfSide = new int[9];

            for (int j = 0; j < 9; j++)
            {
                colorOfSide[j] = StageCube.Instance.touchArray[i][j].RelativeColor.ToInt();
            }

            for (int j = 0; j < 6; j++)
            {
                int color = colorOfSide[list[j][0]];
                bool bingo = true;
                for (int k = 0; k < list[j].Count; k++)
                {
                    if (color != colorOfSide[list[j][k]]) bingo = false;
                }
                if (bingo)
                {
                    bingoNum[color]++;

                    if (bingoStatus[color] != BingoStatus.NONE) continue;

                    for (int k = 0; k < list[j].Count; k++)
                    {
                        bingoCheck[i][list[j][k]] = color.ToColor();
                    }
                }
            }

            // 한면 빙고가 있는지 확인
            for (int j = 0; j < 6; j++)
            {
                if (bingoNum[j] == 6)
                {
                    allCount++;
                    if (bingoStatus[j] != BingoStatus.ALL)
                    {
                        for (int k = 0; k < 9; k++) bingoCheck[i][k] = Colors.WY;
                    }
                }
            }
        }

        if(allCount == 6)
        {
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 9; j++) bingoCheck[i][j] = Colors.RO;
            }
        }
    }

    public void StageEnd()
    {
        // 선악과 괴뢰 소멸
        // 상태 이상 초기화
        // 무기 초기화 (Dual, 강화 등)

        // Dual 이었으면 다시 일반 타입으로 변경
        StaticManager.Instance.PlayerWeapon.ChangeWeaponType(false);
    }
    public void ChangeAtPlayerTurn()
    {
        if(IsEnhancedWeapon)
        {
            StaticManager.Instance.PlayerWeapon = originWeapon;
        }
    }
    private void EventAdd(Colors color)
    {
        foreach (Transform child in eventButtonParent) Destroy(child.gameObject);

        // 발생 가능한 이벤트들을 저장한 리스트
        List<EventCard> eventList = new();

        foreach(EventCard card in eventCards)
        {
            if(CheckEvent(card.eventName, color)) eventList.Add(card);
        }

        // 리스트를 랜덤으로 섞는다
        for (int i = 0; i < eventList.Count; i++)
        {
            int randomIndex = Random.Range(i, eventList.Count);
            EventCard temp = eventList[i];
            eventList[i] = eventList[randomIndex];
            eventList[randomIndex] = temp;
        }

        // 발생 가능한 이벤트들을 UI에 버튼으로 띄운다
        for (int i = 0; i < eventList.Count; i++)
        {
            int index = i;
            GameObject newEventCard = Instantiate(eventButtonPrefab, eventButtonParent);
            newEventCard.GetComponent<Button>().onClick.AddListener(() => Event(eventList[index].eventName));

            TextMeshProUGUI[] childTexts = newEventCard.GetComponentsInChildren<TextMeshProUGUI>();
            childTexts[0].text = eventList[i].eventName;
            childTexts[1].text = eventList[i].EventDescription[0];
        }
    }
    

    private bool CheckEvent(string name, Colors color)
    {
        if (color == Colors.WY)
        {
            switch (name)
            {
                case "운수특대통":
                case "긴급구조":
                case "이름1":
                case "이름2":
                case "이름4":
                    return true;

                case "이름3":
                    if (StaticManager.Instance.Stage >= 6) return true;
                    else return false;

                default: return false;
            }
        }

        if (color == Colors.RO)
        {
            switch (name)
            {
                case "이름5":
                case "이름6":
                    return true;

                default: return false;
            }
        }

        switch (name)
        {
            case "악마의 교활함": return false;

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

            case "용병술":
                if (StaticManager.Instance.Stage >= 6 && FriendCount < 3) return true;
                else return false;

            case "운수특대통":
            case "긴급구조":
            case "이름1":
            case "이름2":
            case "이름3":
            case "이름4":
            case "이름5":
            case "이름6":
                return false;

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
                StageManager.Instance.Player.OnHit(StatusEffect.HP, -friendObj.HP * 50 / 100);
                friendObj.OnHit(StatusEffect.HP_PERCENT, 100);
                break;

            case "선악과":
                foreach(Touch touch in RelativeColorCubeList(StageManager.Instance.Player.touchCube.RelativeColor))
                    if(touch.Obj == null)
                    {
                        ObjectManager.Instance.Summons(touch, ObjectType.TRIGGER, 1);
                        break;
                    }
                break;

            case "지뢰":
                foreach (Touch touch in RelativeColorCubeList(StageManager.Instance.Player.touchCube.RelativeColor))
                    if (touch.Obj == null)
                    {
                        ObjectManager.Instance.Summons(touch, ObjectType.TRIGGER, 2);
                        break;
                    }
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
                StageManager.Instance.SetStageTextValue(StageText.ROTATE, 10);
                break;

            case "회전 추가":
                StageManager.Instance.SetStageTextValue(StageText.ROTATE_INIT, 5);
                break;

            case "빠른 손놀림":
                StageManager.Instance.SetStageTextValue(StageText.WEAPON_CHANGE, 5);
                break;

            case "기민한 발놀림":
                StageManager.Instance.SetStageTextValue(StageText.MOVE, 10);
                break;

            case "민첩한 걸음":
                StageManager.Instance.SetStageTextValue(StageText.MOVE_INIT, 5);
                break;

            case "운수대통":
                foreach (GameObject obj in StageManager.Instance.TreasureList)
                    if (obj.GetComponent<Object>().touchCube.RelativeColor == Effect.color)
                        StageManager.Instance.StagePlayLogic.Trigger(obj, true);
                break;

            case "오아시스":
                StaticManager.Instance.Gold -= 100;
                ObjectManager.Instance.AddItem(110000, null);
                break;

            case "미궁":
                cubeManager.StartRandomTurn(5, true);
                break;

            case "보물찾기":
                StageManager.Instance.SummonStageTreasure(3);
                break;

            case "떠돌이 상인":
                ObjectManager.Instance.OpenShop();
                break;

            case "용병술":
                StageManager.Instance.SummonsFriend(null, Random.Range(110002, 110005));
                break;

            case "회복 구슬":
                foreach (Touch touch in RelativeColorCubeList(StageManager.Instance.Player.touchCube.RelativeColor))
                    if (touch.Obj == null)
                    {
                        ObjectManager.Instance.Summons(touch, ObjectType.TRIGGER, 3);
                        break;
                    }
                break;

            case "운수특대통":
                //"보물상자를 모두 획득한다.
                // 이 때, 각 보물상자에서 얻는 금액은 10배로 계산한다."
                break;

            case "긴급구조":
                foreach (Object obj in AllObject)
                    if (obj.Type == ObjectType.PLAYER || obj.Type == ObjectType.FRIEND)
                        obj.OnHit(StatusEffect.HP_PERCENT, -100);
                break;

            case "이름1":
                originWeapon = StaticManager.Instance.PlayerWeapon;
                if (StaticManager.Instance.PlayerWeapon.WeaponType == WeaponType.SWORD || StaticManager.Instance.PlayerWeapon.WeaponType == WeaponType.DUAL_SWORD)
                    StaticManager.Instance.PlayerWeapon = bestSword;

                if (StaticManager.Instance.PlayerWeapon.WeaponType == WeaponType.STAFF || StaticManager.Instance.PlayerWeapon.WeaponType == WeaponType.DUAL_STAFF)
                    StaticManager.Instance.PlayerWeapon = bestStaff;
                IsEnhancedWeapon = true;
                break;

            case "이름2":
                StaticManager.Instance.PlayerWeapon.ChangeWeaponType(true);
                IsDual = true;
                break;

            case "이름3":
                for (int i = 0; i < 3; i++)
                    StageManager.Instance.SummonsFriend(null, Random.Range(110002, 110005));
                break;

            case "이름4":
                foreach (Object obj in AllObject)
                    if (obj.Type == ObjectType.ENEMY)
                        obj.OnHit(StatusEffect.HP_PERCENT, 30);
                break;

            case "이름5":
                break;

            case "이름6":
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

    public IEnumerator Revive(Object obj, float hpPercent = 100)
    {
        Touch cube;
        while (true)
        {
            cube = StageCube.Instance.touchArray[Random.Range(0, 6)][Random.Range(0, 9)];
            if (cube.Obj == null)
                break;
        }

        StartCoroutine(StageManager.Instance.CubeRotate(cube.Color));
        yield return new WaitForSeconds(1f);
        obj.OnHit(StatusEffect.HP_PERCENT, -hpPercent);

        if (obj.Type == ObjectType.ENEMY) ParticleManager.Instance.PlayParticle(cube.gameObject, Particle.Enemy_Summon);
        else if (obj.Type == ObjectType.FRIEND) ParticleManager.Instance.PlayParticle(cube.gameObject, Particle.Friend_Summon);

        ColorCheckManager.Instance.CharacterSelect(obj.gameObject);
        StartCoroutine(ColorCheckManager.Instance.MoveCoroutine(cube.Color, cube.Index, true));

        yield return new WaitForSeconds(0.5f);
        obj.gameObject.SetActive(true);

        obj.transform.localRotation = Quaternion.Euler(Vector3.zero);

        Debug.Log($"{obj} : {obj.HP} / Touch: {cube.Color} {cube.Index}");
    }
    private IEnumerator Event_16()
    {
        Touch inverseTouch = InverseCube(StageManager.Instance.Player.touchCube);


        Debug.Log("inverseTouch: " + inverseTouch + "  Obj" + inverseTouch.Obj + "  transform: " + inverseTouch.Obj.transform);
        if (inverseTouch.Obj != null)
        {

            Touch objInverseTouch = StageManager.Instance.Player.touchCube;
            Object obj = inverseTouch.Obj;
            ColorCheckManager.Instance.CharacterSelect(obj.gameObject);
            StartCoroutine(ColorCheckManager.Instance.MoveCoroutine(objInverseTouch.Color, objInverseTouch.Index, true));
            yield return new WaitForFixedUpdate();
            obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }

        ColorCheckManager.Instance.CharacterSelect(StageManager.Instance.Player.gameObject);
        StartCoroutine(ColorCheckManager.Instance.MoveCoroutine(inverseTouch.Color, inverseTouch.Index, true));
        yield return new WaitForFixedUpdate();
        StageManager.Instance.Player.transform.localRotation = Quaternion.Euler(Vector3.zero);

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
    public List<Touch> RelativeColorCubeList(Colors color)
    {
        List<Touch> output = new();

        foreach(Touch[] touchArray in StageCube.Instance.touchArray)
        {
            foreach(Touch touch in touchArray)
            {
                if (touch.RelativeColor == color) output.Add(touch);
            }
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
    public Object RandomDeadEnemy()
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
    public int FriendCount
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
    public int EnemyCount
    {
        get
        {
            int count = 0;
            for (int i = 0; i < StageManager.Instance.EnemyList.Length; i++)
            {
                if (StageManager.Instance.EnemyList[i].GetComponent<Object>().HP > 0)
                    count++;
            }
            return count;
        }
    }
    public Object RandomObejectOfPlayerTeam(bool includePlayer)
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