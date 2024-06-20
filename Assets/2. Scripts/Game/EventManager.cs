using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                // 플레이어 비폭력 n 부여
                break;
            case 2:
                // 랜덤 유닛 n개에게 침묵 n 부여
                break;
            case 3:
                // 플레이어팀 랜덤 1명에게 회피 n 부여
                break;
            case 4:
                // 랜덤 유닛 n개에게 낙인 영구 부여
                break;
            case 5:
                // 플레이어 피로 영구 부여
                break;
            case 6:
                // 플레이어 배리어 n 부여
                break;
            case 7:
                // 플팀 1명 적팀 1명에게 침묵 n 부여
                break;

            case 18:
                // 플레이어의 현재 체력을 최대 체력의 N%만큼 회복 -> 전투 턴에 받는 데미지 N% 증가
                break;
            case 19:
                // 받는 데미지 주는 데미지 N%  N턴 동안 달라짐
                break;
            case 20:
                // 영구적으로 최대 체력이 N%만큼 감소 최대 데미지 N% 증가
                break;
            case 21:
                // 500 골드 획득 랜덤 몬스터 1의 주는 데미지 N% 증가
                break;
            case 22:
                // 랜덤한 동료 1 제거, n턴 동안 주는 데미지 N% 증가
                break;

                //용병도 상태 이상으로 처리할 수도...?


            // 체력 조정 (부활&제거 포함)
            case 11:
                // 사망 유닛 중 하나 랜덤 블록에 부활
                break;
            case 12:
                // 동료 1명 적 1명 제거
                break;
            case 13:
                // 모든 유닛의 현재 체력을 최대 체력의 N% 만큼 회복
                break;
            case 14:
                // 모든 유닛의 현재 체력을 최대 체력의 N% 만큼 감소
                break;
            case 15:
                // 몬스터 수만큼 모든 유닛의 현재 체력을 최대 체력의 N%만큼 감소
                break;
            case 17:
                // 색 선택 후 그 색의 위에 있는 모든 유닛의 현재 체력을 최대 체력의 N%만큼 회복
                break;
            case 23:
                // 랜덤 동료 1 제거 플레이어의 현재 체력을 제거된 동료의 현재 체력의 N%만큼 회복
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
                // 정반대에 플레이어 생성
                break;
            case 16:
                // 플레이어와 정반대의 오브젝트 변경(거울)
                break;
            case 24:
                // 플레이어와 같은 면에 있는 몬스터 1명을 N% 확률로 회유 -> 성공 시 몬스터가 용병 / 실패 시 현재 체력을 최대 체력의 N% 만큼 감소
                break;
        }
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
}