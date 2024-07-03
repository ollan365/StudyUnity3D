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

    // �̺�Ʈ �߻� ��, �� �Լ��� ȣ���ϸ� �˾Ƽ� �� ó���ǵ���
    public void EventAdd(int eventIndex)
    {
        switch (eventIndex)
        {
            // ���� �̻�
            case 1:
                // �÷��̾� ħ��, ���� n �ο�
                PlayerEventEffect.EffectAdd(StatusEffect.SLIENCE, 10);
                PlayerEventEffect.EffectAdd(StatusEffect.INVINCIBILITY, 10);
                break;
            case 2:
                // ���� ���� n������ ħ�� n �ο�
                Object[] random1 = RandomObejectOfALL(10);
                foreach (Object o in random1) o.eventEffect.EffectAdd(StatusEffect.SLIENCE, 10);
                break;
            case 3:
                // �÷��̾��� ���� 1���� ȸ�� n �ο�
                RandomObejectOfPlayerTeam(true).eventEffect.EffectAdd(StatusEffect.EVASION, 10);
                break;
            case 4:
                // ���� ���� n������ ��� n �ο� (��� = ����)
                Object[] random2 = RandomObejectOfALL(10);
                foreach (Object o in random2) o.eventEffect.EffectAdd(StatusEffect.STIGMA, 10);
                break;
            case 5:
                // �÷��̾� �Ƿ� �ο�
                PlayerEventEffect.EffectAdd(StatusEffect.FATIGUE, 10);
                break;
            case 6:
                // �÷��̾� ��ȭ, ���� n �ο�
                PlayerEventEffect.EffectAdd(StatusEffect.WEAKEN, 10);
                PlayerEventEffect.EffectAdd(StatusEffect.INVINCIBILITY, 10);
                break;
            case 7:
                // ���� 1�� ���� 1���� ħ�� n �ο�
                RandomObejectOfPlayerTeam(true).eventEffect.EffectAdd(StatusEffect.SLIENCE, 10);
                RandomObejectOfPEnemy().eventEffect.EffectAdd(StatusEffect.SLIENCE, 10);
                break;
            case 18:
                // �÷��̾��� ���� ü���� �ִ� ü���� N%��ŭ ȸ��, ��� �ο�
                StageManager.Instance.Player.OnHit(StatusEffect.HP_PERCENT, -10);
                PlayerEventEffect.EffectAdd(StatusEffect.STIGMA, 10);
                break;
            case 19:
                // �÷��̾�� n�� ���, ��ȭ �ο�
                PlayerEventEffect.EffectAdd(StatusEffect.STIGMA, 10);
                PlayerEventEffect.EffectAdd(StatusEffect.POWERFUL, 10);
                break;
            case 20:
                // ���������� �ִ� ü���� N%��ŭ ���� �ִ� ������ N% ���� // (���� �̿Ϸ�)
                break;
            case 21:
                // 500 ��� ȹ�� ���� ���� �ϳ����� n�� ��ȭ
                StaticManager.Instance.Gold += 500; 
                RandomObejectOfPEnemy().eventEffect.EffectAdd(StatusEffect.SUNSHINE, 10);
                break;
            case 22:
                // ������ ���� 1 ����, �÷��̾ n�� ��ȭ ȹ��
                RandomObejectOfPlayerTeam(false).OnHit(StatusEffect.HP_PERCENT, 100);
                PlayerEventEffect.EffectAdd(StatusEffect.POWERFUL, 10);
                break;


            // ü�� ���� (��Ȱ&���� ����)
            case 11:
                // ��� ���� �� �ϳ� ���� ��Ͽ� ��Ȱ
                StartCoroutine(Event_11());
                break;
            case 12:
                // ���� 1�� �� 1�� ����
                RandomObejectOfPlayerTeam(false).OnHit(StatusEffect.HP_PERCENT, 100);
                RandomObejectOfPEnemy().OnHit(StatusEffect.HP_PERCENT, 100);
                break;
            case 13:
                // ��� ������ ���� ü���� �ִ� ü���� N% ��ŭ ȸ��
                foreach (Object o in AllObject) o.OnHit(StatusEffect.HP_PERCENT, -10);
                break;
            case 14:
                // ��� ������ ���� ü���� �ִ� ü���� N% ��ŭ ����
                foreach (Object o in AllObject) o.OnHit(StatusEffect.HP_PERCENT, 10);
                break;
            case 15:
                // ���� ����ŭ ��� ������ ���� ü���� �ִ� ü���� N%��ŭ ����
                foreach (Object o in AllObject) o.OnHit(StatusEffect.HP_PERCENT, EnemyCount);
                break;
            case 17:
                // �� ���� �� �� ���� ���� �ִ� ��� ������ ���� ü���� �ִ� ü���� N%��ŭ ȸ��
                int randomColor = Random.Range(0, 6);
                foreach(Object o in AllObject)
                {
                    if (o.touchCube.RelativeColor.ToInt() == randomColor)
                        o.OnHit(StatusEffect.HP_PERCENT, -10);
                }
                break;
            case 23:
                // ���� ���� 1 ���� �÷��̾��� ���� ü���� ���ŵ� ������ ���� ü���� N%��ŭ ȸ��
                float friendHP = RandomObejectOfPlayerTeam(false).HP;
                StageManager.Instance.Player.OnHit(StatusEffect.HP, -(int)friendHP * 10 / 100);
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
                // ���ݴ뿡 �÷��̾�(�뺴) ����
                break;
            case 16:
                // �÷��̾�� ���ݴ��� ������Ʈ ����(�ſ�)
                StartCoroutine(Event_16());
                break;
            case 24:
                // �÷��̾�� ���� �鿡 �ִ� ���� 1���� N% Ȯ���� ȸ�� -> ���� �� ���Ͱ� �뺴 / ���� �� ���� ü���� �ִ� ü���� N% ��ŭ ����
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