using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using static Constants;
using static Excel;

public class StageManager : MonoBehaviour
{
    [SerializeField] private Transform cube;
    private bool isCubeMove;

    [SerializeField] private CubeManager cubeManager;
    [SerializeField] private ColorCheckManager colorCheckManager;
    [SerializeField] private ObjectManager objectManager;

    [SerializeField] private Text[] stageTexts;
    private int[] stageDatas;
    private int moveCount, rotateCount, changeCount;
    private int additionalMoveCount;

    private GameObject[] enemy, friend, treasure;
    [SerializeField] private GameObject player;

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
                    for (int i = 0; i < 6; i++)
                        colorCheckManager.BingoCheck(i, false);
                    colorCheckManager.ToNextBingo();
                    stageTexts[0].text = $"{StaticManager.Instance.Stage}층 PLAYER";
                    StageTextChange(true, StageText.ALL, 0);
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

    public void StageInit(string data)
    {
        StatusOfStage = StageStatus.INIT;

        stageDatas = new int[data.Split(',').Length - 1];
        for (int i = 0; i < data.Split(',').Length - 1; i++)
            stageDatas[i] = int.Parse(data.Split(',')[i]);

        player.GetComponent<Object>().init(ObjectType.PLAYER, new string[] { stageDatas[MAX_HP].ToString() });

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
        cubeManager.StartRandomTurn(stageDatas[MIX]); // 큐브를 섞는다

        yield return new WaitForSeconds(10f);

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
                enemy[index] = objectManager.Summons(cube, ObjectType.ENEMY, int.Parse(stageEnemy[i].Split(',')[STAGE_ENEMY_ID]));
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
            treasure[i] = objectManager.Summons(cube, ObjectType.TREASURE, 0);
            treasure[i].GetComponent<Object>().SetWeapon(stageDatas[TREASURE_MIN], stageDatas[TREASURE_MAX], WeaponType.NULL);
            yield return new WaitForFixedUpdate();
        }

        StatusOfStage = StageStatus.PLAYER;
    }
    public void ChangeStatus()
    {
        if(StatusOfStage == StageStatus.PLAYER)
        {
            StatusOfStage = StageStatus.FIGHT;
            StartCoroutine(Attack()); 
        }
    }
    private void ClearStage()
    {
        StatusOfStage = StageStatus.END;

        foreach(GameObject t in treasure) // 스테이지 종료 시 보물상자 소멸
            t.GetComponent<Object>().OnHit(9999);
        
        while (true)
        {
            Touch cube = StageCube.Instance.touchArray[Random.Range(0, 6)][Random.Range(0, 9)];
            if (cube.Obj == null)
            {
                objectManager.Summons(cube, ObjectType.MERCHANT, 0);
                break;
            }
        }
        while (true)
        {
            Touch cube = StageCube.Instance.touchArray[Random.Range(0, 6)][Random.Range(0, 9)];
            if (cube.Obj == null)
            {
                objectManager.Summons(cube, ObjectType.PORTAL, 0);
                break;
            }
        }
    }
    public void NextStage()
    {
        Debug.Log("Next Stage!");
    }
    private void GameOver()
    {
        gameOverPanel.SetActive(true);
    }

    private IEnumerator CubeRotate(Colors color)
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

    private IEnumerator Attack()
    {
        additionalMoveCount = 0;
        for(int i = 0; i < 6; i++) // 빙고 확인
        {
            BingoStatus bingo = colorCheckManager.BingoCheck(i, true);
            int random = Random.Range(0, 2);

            if (bingo == BingoStatus.DEFAULT) continue;

            StartCoroutine(CubeRotate(i.ToColor())); // 빙고 완성 시 그 면으로 회전
            while (isCubeMove) yield return new WaitForFixedUpdate();

            if (random == 0)
            {
                if (bingo == BingoStatus.ALL || player.GetComponent<Object>().Color.ToInt() == i)
                    player.GetComponent<Object>().HP_Percent(10);
                foreach(GameObject f in friend)
                {
                    if (f == null || !f.activeSelf) continue;

                    if (bingo == BingoStatus.ALL || f.GetComponent<Object>().Color.ToInt() == i)
                        f.GetComponent<Object>().HP_Percent(10);
                }
            }
            else
            {
                if (bingo == BingoStatus.ONE) additionalMoveCount++;
                else changeCount++;
            }

            yield return new WaitForSeconds(0.5f);
        }
        
        // 플레이어부터 공격
        Object playerObj = player.GetComponent<Object>();

        StartCoroutine(CubeRotate(playerObj.Color));
        while (isCubeMove) yield return new WaitForFixedUpdate();

        List<GameObject> attackableEnemy = AttackableObject(playerObj.AttackType, playerObj.Color, playerObj.Index, ObjectType.ENEMY);
        foreach (GameObject enemy in attackableEnemy)
        {
            enemy.GetComponent<Object>().OnHit(playerObj.Damage);
            yield return new WaitForFixedUpdate();

            yield return new WaitForSeconds(0.1f);
        }

        // 적과 동료의 공격 순서 결정을 위해 List 생성
        List<KeyValuePair<int, int>> enemyAttackOrder = new List<KeyValuePair<int, int>>();
        for (int i = 0; i < stageDatas[ENEMY_COUNT]; i++)
        {
            Object enemyObject = enemy[i].GetComponent<Object>();
            if (enemyObject.gameObject.activeSelf)
                enemyAttackOrder.Add(new KeyValuePair<int, int>(enemyObject.Damage, i));
            else // enemyAttackOrder의 크기가 friendAttackOrder 보다 커야하므로 죽은 애들도 일단 list에 넣기
                enemyAttackOrder.Add(new KeyValuePair<int, int>(0, i));
        }
        List<KeyValuePair<int, int>> friendAttackOrder = new List<KeyValuePair<int, int>>();
        for (int i = 0; i < 3; i++)
        {
            if (friend[i] == null) continue;
            Object friendObject = friend[i].GetComponent<Object>();
            if (friendObject.gameObject.activeSelf)
                friendAttackOrder.Add(new KeyValuePair<int, int>(friendObject.Damage, i));
        }
        enemyAttackOrder = enemyAttackOrder.OrderByDescending(enemyAttackOrder => enemyAttackOrder.Key).ToList(); // 공격력 순으로 내림차순 정렬
        friendAttackOrder = friendAttackOrder.OrderByDescending(friendAttackOrder => friendAttackOrder.Key).ToList();

        if(!enemy[enemyAttackOrder[0].Value].activeSelf) // 살아있는 enemy가 없으면
        {
            ClearStage();
            yield break;
        }

        for (int i = 0; i < enemyAttackOrder.Count; i++)
        {
            if (i < friendAttackOrder.Count) // 동료도 있다면
            {
                Object friendObj = friend[friendAttackOrder[i].Value].GetComponent<Object>();
                attackableEnemy = AttackableObject(friendObj.AttackType, friendObj.Color, friendObj.Index, ObjectType.ENEMY);
                
                StartCoroutine(CubeRotate(friendObj.Color));
                while (isCubeMove) yield return new WaitForFixedUpdate();

                foreach (GameObject enemy in attackableEnemy)
                {
                    enemy.GetComponent<Object>().OnHit(friendAttackOrder[i].Key);
                    yield return new WaitForFixedUpdate();

                    yield return new WaitForSeconds(0.1f);
                }
            }

            // 적 공격
            if (!enemy[enemyAttackOrder[i].Value].activeSelf) continue;

            Object enemyObj = enemy[enemyAttackOrder[i].Value].GetComponent<Object>();
            List<GameObject> attackablePlayerTeam;

            attackablePlayerTeam = AttackableObject(enemyObj.AttackType, enemyObj.Color, enemyObj.Index, ObjectType.PLAYER);

            StartCoroutine(CubeRotate(enemyObj.Color));
            while (isCubeMove) yield return new WaitForFixedUpdate();

            foreach (GameObject p in attackablePlayerTeam)
            {
                p.GetComponent<Object>().OnHit(enemyAttackOrder[i].Key);
                yield return new WaitForFixedUpdate();

                yield return new WaitForSeconds(0.1f);
            }

            if(!player.activeSelf) // 플레이어가 죽으면 게임 종료
            {
                GameOver();
                yield break;
            }

            attackablePlayerTeam = AttackableObject(enemyObj.AttackType, enemyObj.Color, enemyObj.Index, ObjectType.FRIEND);
            foreach (GameObject pTeam in attackablePlayerTeam)
            {
                pTeam.GetComponent<Object>().OnHit(enemyAttackOrder[i].Key);
                yield return new WaitForFixedUpdate();

                yield return new WaitForSeconds(0.1f);
            }
        }

        // statge statue를 바꾼다
        StatusOfStage = StageStatus.PLAYER;
    }
    private List<GameObject> AttackableObject(WeaponType weaponType, Colors color, int index, ObjectType objType)
    {
        List<GameObject> attackable = new List<GameObject>();

        if(objType == ObjectType.PLAYER && player.activeSelf)
        {
            Object p = player.GetComponent<Object>();
            if (weaponType == WeaponType.CAD || weaponType == WeaponType.LAD)
            {
                if (p.Color == color && AttackableRange(weaponType, index)[p.Index])
                    attackable.Add(player);
            }
            else if(weaponType == WeaponType.AP)
            {
                if (p.Color != color && p.Index == index)
                    attackable.Add(player);
            }
        }
        else if (objType == ObjectType.FRIEND)
        {
            for(int i = 0;i<3;i++)
            {
                if (friend[i] == null || !friend[i].activeSelf) continue;

                Object f = friend[i].GetComponent<Object>();
                if (weaponType == WeaponType.CAD || weaponType == WeaponType.LAD)
                {
                    if (f.Color == color && AttackableRange(weaponType, index)[f.Index])
                        attackable.Add(friend[i]);
                }
                else if (weaponType == WeaponType.AP)
                {
                    if (f.Color != color && f.Index == index)
                        attackable.Add(friend[i]);
                }
            }
        }
        else if(objType == ObjectType.ENEMY)
        {
            foreach(GameObject enemyObj in enemy)
            {
                if (!enemyObj.activeSelf) continue;

                Object e = enemyObj.GetComponent<Object>();
                if (weaponType == WeaponType.CAD || weaponType == WeaponType.LAD)
                {
                    if (e.Color == color && AttackableRange(weaponType, index)[e.Index])
                        attackable.Add(enemyObj);
                }
                else if (weaponType == WeaponType.AP)
                {
                    if (e.Color != color && e.Index == index)
                        attackable.Add(enemyObj);
                }
            }
        }
        return attackable;
    }
    private bool[] AttackableRange(WeaponType weaponType, int index)
    {
        bool[] attackable = new bool[9];
        for (int i = 0; i < 9; i++)
            attackable[i] = false;

        if (weaponType == WeaponType.CAD)
        {
            switch (index)
            {
                case 0:
                    attackable[1] = true;
                    attackable[3] = true;
                    break;
                case 1:
                    attackable[0] = true;
                    attackable[2] = true;
                    attackable[4] = true;
                    break;
                case 2:
                    attackable[1] = true;
                    attackable[5] = true;
                    break;
                case 3:
                    attackable[0] = true;
                    attackable[4] = true;
                    attackable[6] = true;
                    break;
                case 4:
                    attackable[1] = true;
                    attackable[3] = true;
                    attackable[5] = true;
                    attackable[7] = true;
                    break;
                case 5:
                    attackable[2] = true;
                    attackable[4] = true;
                    attackable[8] = true;
                    break;
                case 6:
                    attackable[3] = true;
                    attackable[7] = true;
                    break;
                case 7:
                    attackable[4] = true;
                    attackable[6] = true;
                    attackable[8] = true;
                    break;
                case 8:
                    attackable[5] = true;
                    attackable[7] = true;
                    break;
                default:
                    break;
            }
        }
        else if (weaponType == WeaponType.LAD)
        {
            switch (index)
            {
                case 0:
                    attackable[5] = true;
                    break;
                case 1:
                    attackable[3] = true;
                    attackable[5] = true;
                    break;
                case 2:
                    attackable[4] = true;
                    break;
                case 3:
                    attackable[1] = true;
                    attackable[7] = true;
                    break;
                case 4:
                    attackable[0] = true;
                    attackable[2] = true;
                    attackable[6] = true;
                    attackable[8] = true;
                    break;
                case 5:
                    attackable[1] = true;
                    attackable[7] = true;
                    break;
                case 6:
                    attackable[4] = true;
                    break;
                case 7:
                    attackable[3] = true;
                    attackable[5] = true;
                    break;
                case 8:
                    attackable[4] = true;
                    break;
                default:
                    break;
            }
        }

        return attackable;
    }

    public bool SummonsFriend(Colors color, int index, int friendIndex)
    {
        Touch cube = StageCube.Instance.touchArray[color.ToInt()][index];
        if (cube.Obj != null)
            return false;
        for (int i = 0; i < 3; i++)
        {
            if (friend[i] == null) // 이건 동료 소환이 한 스테이지에서 3번만 가능할 때긴 함
            {
                friend[i] = objectManager.Summons(cube, ObjectType.FRIEND, friendIndex);
                Debug.Log("summons success!");
                return true;
            }
        }
        return false;
    }
}
