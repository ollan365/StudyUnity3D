using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 이벤트 발생 시, 이 함수만 호출하면 알아서 다 처리되도록
    public void EventAdd(int eventIndex)
    {
        switch (eventIndex)
        {
            // 상태 이상
            case 1:
                // 플레이어 침묵, 무적 n 부여
                PlayerEventEffect.EffectAdd(StatusEffect.SLIENCE, 10);
                PlayerEventEffect.EffectAdd(StatusEffect.INVINCIBILITY, 10);
                break;
            case 2:
                // 랜덤 유닛 n개에게 침묵 n 부여
                Object[] random1 = RandomObejectOfALL(10);
                foreach (Object o in random1) o.eventEffect.EffectAdd(StatusEffect.SLIENCE, 10);
                break;
            case 3:
                // 플레이어팀 랜덤 1명에게 회피 n 부여
                RandomObejectOfPlayerTeam(true).eventEffect.EffectAdd(StatusEffect.EVASION, 10);
                break;
            case 4:
                // 랜덤 유닛 n개에게 취약 n 부여 (취약 = 낙인)
                Object[] random2 = RandomObejectOfALL(10);
                foreach (Object o in random2) o.eventEffect.EffectAdd(StatusEffect.STIGMA, 10);
                break;
            case 5:
                // 플레이어 피로 부여
                PlayerEventEffect.EffectAdd(StatusEffect.FATIGUE, 10);
                break;
            case 6:
                // 플레이어 약화, 무적 n 부여
                PlayerEventEffect.EffectAdd(StatusEffect.WEAKEN, 10);
                PlayerEventEffect.EffectAdd(StatusEffect.INVINCIBILITY, 10);
                break;
            case 7:
                // 플팀 1명 적팀 1명에게 침묵 n 부여
                RandomObejectOfPlayerTeam(true).eventEffect.EffectAdd(StatusEffect.SLIENCE, 10);
                RandomObejectOfPEnemy().eventEffect.EffectAdd(StatusEffect.SLIENCE, 10);
                break;
            case 18:
                // 플레이어의 현재 체력을 최대 체력의 N%만큼 회복, 취약 부여
                StageManager.Instance.Player.OnHit(StatusEffect.HP_PERCENT, -10);
                PlayerEventEffect.EffectAdd(StatusEffect.STIGMA, 10);
                break;
            case 19:
                // 플레이어에게 n의 취약, 강화 부여
                PlayerEventEffect.EffectAdd(StatusEffect.STIGMA, 10);
                PlayerEventEffect.EffectAdd(StatusEffect.POWERFUL, 10);
                break;
            case 20:
                // 영구적으로 최대 체력이 N%만큼 감소 최대 데미지 N% 증가 // (구현 미완료)
                break;
            case 21:
                // 500 골드 획득 랜덤 몬스터 하나에게 n의 강화
                StaticManager.Instance.Gold += 500; 
                RandomObejectOfPEnemy().eventEffect.EffectAdd(StatusEffect.SUNSHINE, 10);
                break;
            case 22:
                // 랜덤한 동료 1 제거, 플레이어가 n의 강화 획득
                RandomObejectOfPlayerTeam(false).OnHit(StatusEffect.HP_PERCENT, 100);
                PlayerEventEffect.EffectAdd(StatusEffect.POWERFUL, 10);
                break;


            // 체력 조정 (부활&제거 포함)
            case 11:
                // 사망 유닛 중 하나 랜덤 블록에 부활
                StartCoroutine(Event_11());
                break;
            case 12:
                // 동료 1명 적 1명 제거
                RandomObejectOfPlayerTeam(false).OnHit(StatusEffect.HP_PERCENT, 100);
                RandomObejectOfPEnemy().OnHit(StatusEffect.HP_PERCENT, 100);
                break;
            case 13:
                // 모든 유닛의 현재 체력을 최대 체력의 N% 만큼 회복
                foreach (Object o in AllObject) o.OnHit(StatusEffect.HP_PERCENT, -10);
                break;
            case 14:
                // 모든 유닛의 현재 체력을 최대 체력의 N% 만큼 감소
                foreach (Object o in AllObject) o.OnHit(StatusEffect.HP_PERCENT, 10);
                break;
            case 15:
                // 몬스터 수만큼 모든 유닛의 현재 체력을 최대 체력의 N%만큼 감소
                foreach (Object o in AllObject) o.OnHit(StatusEffect.HP_PERCENT, EnemyCount);
                break;
            case 17:
                // 색 선택 후 그 색의 위에 있는 모든 유닛의 현재 체력을 최대 체력의 N%만큼 회복
                int randomColor = Random.Range(0, 6);
                foreach(Object o in AllObject)
                {
                    if (o.touchCube.RelativeColor.ToInt() == randomColor)
                        o.OnHit(StatusEffect.HP_PERCENT, -10);
                }
                break;
            case 23:
                // 랜덤 동료 1 제거 플레이어의 현재 체력을 제거된 동료의 현재 체력의 N%만큼 회복
                float friendHP = RandomObejectOfPlayerTeam(false).HP;
                StageManager.Instance.Player.OnHit(StatusEffect.HP, -(int)friendHP * 10 / 100);
                break;


            // 예외
            case 0:
                // 큐브 n번 회전
                break;
            case 8:
                // 선악과 소환
                break;
            case 9:
                // 괴뢰 소환
                break;
            case 10:
                // 정반대에 플레이어(용병) 생성
                break;
            case 16:
                // 플레이어와 정반대의 오브젝트 변경(거울)
                StartCoroutine(Event_16());
                break;
            case 24:
                // 플레이어와 같은 면에 있는 몬스터 1명을 N% 확률로 회유 -> 성공 시 몬스터가 용병 / 실패 시 현재 체력을 최대 체력의 N% 만큼 감소
                break;
        }
    }

    private IEnumerator Event_11()
    {
        int deadFriendCount = 0;
        foreach(GameObject obj in StageManager.Instance.FriendList)
            if (obj != null && !obj.activeSelf) deadFriendCount++;

        int deadEnemyCount = 0;
        foreach (GameObject obj in StageManager.Instance.EnemyList)
            if (!obj.activeSelf) deadEnemyCount++;

        if (deadFriendCount == 0 && deadEnemyCount == 0) yield break;

        bool reviveFriend = false;
        if (deadFriendCount > 0 && deadEnemyCount > 0)
        {
            int random = Random.Range(0, 100);
            if (random < 50) reviveFriend = true;
        }
        else if (deadFriendCount > 0) reviveFriend = true;

        Object revive = null;

        if (reviveFriend)
        {
            int random = Random.Range(0, deadFriendCount);

            foreach (GameObject obj in StageManager.Instance.FriendList)
            {
                if (obj != null && !obj.activeSelf)
                {
                    if (random > 0) { random--; continue; }

                    revive = obj.GetComponent<Object>(); break;
                }
            }
        }
        else
        {
            int random = Random.Range(0, deadEnemyCount);

            foreach (GameObject obj in StageManager.Instance.EnemyList)
            {
                if (!obj.activeSelf)
                {
                    if (random > 0) { random--; continue; }

                    revive = obj.GetComponent<Object>(); break;
                }
            }
        }

        Touch cube;
        while (true)
        {
            cube = StageCube.Instance.touchArray[Random.Range(0, 6)][Random.Range(0, 9)];
            if (cube.Obj == null)
                break;
        }

        StageManager.Instance.CubeRotate(cube.Color);
        yield return new WaitForSeconds(1f);

        revive.gameObject.SetActive(true);

        ColorCheckManager.Instance.CharacterSelect(revive.gameObject);
        StartCoroutine(ColorCheckManager.Instance.MoveCoroutine(cube.Color, cube.Index));
        ColorCheckManager.Instance.CharacterSelectCancel(null, true);

        revive.OnHit(StatusEffect.HP_PERCENT, -100);
        revive.eventEffect.Init();

        Debug.Log($"{revive} : {revive.HP} / Touch: {cube.Color} {cube.Index}");
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
    private EventEffect PlayerEventEffect
    {
        get => StageManager.Instance.Player.eventEffect;
    }
    private int FreindCount
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
    private EventEffect GetFriendEventEffect(int index)
    {
        return StageManager.Instance.FriendList[index].GetComponent<Object>().eventEffect;
    }
    private EventEffect GetEnemyEventEffect(int index)
    {
        return StageManager.Instance.EnemyList[index].GetComponent<Object>().eventEffect;
    }
    private Object RandomObejectOfPlayerTeam(bool includePlayer)
    {
        if (FreindCount == 0 && !includePlayer) return null;

        int count = FreindCount;
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
            Object[] allObject = new Object[1 + FreindCount + EnemyCount];

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