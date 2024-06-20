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

    // �̺�Ʈ �߻� ��, �� �Լ��� ȣ���ϸ� �˾Ƽ� �� ó���ǵ���
    public void EventAdd(int eventIndex)
    {
        switch (eventIndex)
        {
            // ���� �̻�
            case 1:
                // �÷��̾� ������ n �ο�
                break;
            case 2:
                // ���� ���� n������ ħ�� n �ο�
                break;
            case 3:
                // �÷��̾��� ���� 1���� ȸ�� n �ο�
                break;
            case 4:
                // ���� ���� n������ ���� ���� �ο�
                break;
            case 5:
                // �÷��̾� �Ƿ� ���� �ο�
                break;
            case 6:
                // �÷��̾� �踮�� n �ο�
                break;
            case 7:
                // ���� 1�� ���� 1���� ħ�� n �ο�
                break;

            case 18:
                // �÷��̾��� ���� ü���� �ִ� ü���� N%��ŭ ȸ�� -> ���� �Ͽ� �޴� ������ N% ����
                break;
            case 19:
                // �޴� ������ �ִ� ������ N%  N�� ���� �޶���
                break;
            case 20:
                // ���������� �ִ� ü���� N%��ŭ ���� �ִ� ������ N% ����
                break;
            case 21:
                // 500 ��� ȹ�� ���� ���� 1�� �ִ� ������ N% ����
                break;
            case 22:
                // ������ ���� 1 ����, n�� ���� �ִ� ������ N% ����
                break;

                //�뺴�� ���� �̻����� ó���� ����...?


            // ü�� ���� (��Ȱ&���� ����)
            case 11:
                // ��� ���� �� �ϳ� ���� ��Ͽ� ��Ȱ
                break;
            case 12:
                // ���� 1�� �� 1�� ����
                break;
            case 13:
                // ��� ������ ���� ü���� �ִ� ü���� N% ��ŭ ȸ��
                break;
            case 14:
                // ��� ������ ���� ü���� �ִ� ü���� N% ��ŭ ����
                break;
            case 15:
                // ���� ����ŭ ��� ������ ���� ü���� �ִ� ü���� N%��ŭ ����
                break;
            case 17:
                // �� ���� �� �� ���� ���� �ִ� ��� ������ ���� ü���� �ִ� ü���� N%��ŭ ȸ��
                break;
            case 23:
                // ���� ���� 1 ���� �÷��̾��� ���� ü���� ���ŵ� ������ ���� ü���� N%��ŭ ȸ��
                break;


            // ����
            case 0:
                // ť�� n�� ȸ��
                break;
            case 8:
                // ���ǰ� ��ȯ
                break;
            case 9:
                // ���� ��ȯ
                break;
            case 10:
                // ���ݴ뿡 �÷��̾� ����
                break;
            case 16:
                // �÷��̾�� ���ݴ��� ������Ʈ ����(�ſ�)
                break;
            case 24:
                // �÷��̾�� ���� �鿡 �ִ� ���� 1���� N% Ȯ���� ȸ�� -> ���� �� ���Ͱ� �뺴 / ���� �� ���� ü���� �ִ� ü���� N% ��ŭ ����
                break;
        }
    }



    // ========== ���Ǹ� ���� ���� �Լ� ========== //
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
        // ���� ������ ���� üũ���� ����غ��� ���� �� �����
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
}