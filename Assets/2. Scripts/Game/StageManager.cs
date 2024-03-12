using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using static Constants;
using static Excel;

public class StageManager : MonoBehaviour
{
    [SerializeField] private GameObject[] whiteCheckCubeArray, redCheckCubeArray, blueCheckCubeArray, greenCheckCubeArray, orangeCheckCubeArray, yellowCheckCubeArray;
    private GameObject[][] colorCheckCubeArray;

    [SerializeField] private Transform cube;
    private bool isCubeMove;

    [SerializeField] private CubeManager cubeManager;
    [SerializeField] private ColorCheckManager colorCheckManager;
    [SerializeField] private ObjectManager objectManager;
    [SerializeField] private StaticManager staticManager;

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
                    stageTexts[0].text = $"{staticManager.Stage}층 INIT";
                    break;
                case StageStatus.PLAYER:
                    for (int i = 0; i < 6; i++)
                        colorCheckManager.BingoCheck(i, false);
                    stageTexts[0].text = $"{staticManager.Stage}층 PLAYER";
                    StageTextChange(true, StageText.ALL, 0);
                    break;
                case StageStatus.FIGHT:
                    stageTexts[0].text = $"{staticManager.Stage}층 FIGHT";
                    break;
                case StageStatus.END:
                    stageTexts[0].text = $"{staticManager.Stage}층 END";
                    break;
            }
        }
    }

    private void Start()
    {
        colorCheckCubeArray = new GameObject[][] { whiteCheckCubeArray, redCheckCubeArray, blueCheckCubeArray, greenCheckCubeArray, orangeCheckCubeArray, yellowCheckCubeArray };
        StageInit();
    }
    private void StageInit()
    {
        StatusOfStage = StageStatus.INIT;

        TextAsset textFile = Resources.Load("StageInfo") as TextAsset;
        StringReader stringReader = new StringReader(textFile.text);

        string line = stringReader.ReadLine();
        for (int i = 0; i <= staticManager.Stage; i++)
            line = stringReader.ReadLine();

        string[] stageDateString = line.Split(',');
        stageDatas = new int[stageDateString.Length];
        for (int i = 0; i < stageDateString.Length; i++)
            stageDatas[i] = int.Parse(stageDateString[i]);

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

        TextAsset textFile = Resources.Load("StageEnemy") as TextAsset;
        StringReader stringReader = new StringReader(textFile.text);

        string line = stringReader.ReadLine();

        do { line = stringReader.ReadLine(); }
        while (line != null && int.Parse(line.Split(',')[0]) != staticManager.Stage);

        int index = 0;
        while (index < stageDatas[ENEMY_COUNT] && line != null && int.Parse(line.Split(',')[0]) == staticManager.Stage) // enemy 배치
        {
            ColorCheckCube cube;

            for (int i = 0; i < int.Parse(line.Split(',')[2]); i++)
            {
                while (true)
                {
                    cube = colorCheckCubeArray[Random.Range(0, 6)][Random.Range(0, 9)].GetComponent<ColorCheckCube>();
                    if (cube.GetObjectType() == ObjectType.NULL)
                        break;
                }

                enemy[index + i] = objectManager.Summons(cube, ObjectType.ENEMY, int.Parse(line.Split(',')[1]));
                yield return new WaitForFixedUpdate();
            }

            index += int.Parse(line.Split(',')[2]);
            line = stringReader.ReadLine();
        }

        for (int i = 0; i < stageDatas[TREASURE_COUNT]; i++) // enemy 배치
        {
            ColorCheckCube cube;
            while (true)
            {
                cube = colorCheckCubeArray[Random.Range(0, 6)][Random.Range(0, 9)].GetComponent<ColorCheckCube>();
                if (cube.GetObjectType() == ObjectType.NULL)
                    break;
            }
            treasure[i] = objectManager.Summons(cube, ObjectType.TREASURE, 0);
            treasure[i].GetComponent<Object>().weapon.SetDamage(stageDatas[TREASURE_MIN], stageDatas[TREASURE_MAX]);
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
            ColorCheckCube cube = colorCheckCubeArray[Random.Range(0, 6)][Random.Range(0, 9)].GetComponent<ColorCheckCube>();
            if (cube.GetObjectType() == ObjectType.NULL)
            {
                objectManager.Summons(cube, ObjectType.MERCHANT, 0);
                break;
            }
        }
        while (true)
        {
            ColorCheckCube cube = colorCheckCubeArray[Random.Range(0, 6)][Random.Range(0, 9)].GetComponent<ColorCheckCube>();
            if (cube.GetObjectType() == ObjectType.NULL)
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

            int bingo = colorCheckManager.BingoCheck(i, true);
            int random = Random.Range(0, 2);

            if (bingo == BINGO_DEFAULT) continue;

            StartCoroutine(CubeRotate(i.ToColor())); // 빙고 완성 시 그 면으로 회전
            while (isCubeMove) yield return new WaitForFixedUpdate();

            if (random == 0)
            {
                if (bingo == BINGO_ALL || player.GetComponent<Object>().GetPosition().Color.ToInt() == i)
                    player.GetComponent<Object>().HP_Percent(10);
                foreach(GameObject f in friend)
                {
                    if (f == null || !f.activeSelf) continue;

                    if (bingo == BINGO_ALL || f.GetComponent<Object>().GetPosition().Color.ToInt() == i)
                        f.GetComponent<Object>().HP_Percent(10);
                }
            }
            else
            {
                if (bingo == BINGO_ONE) additionalMoveCount++;
                else changeCount++;
            }

            yield return new WaitForSeconds(0.5f);
        }
        
        // 플레이어부터 공격
        Object playerObj = player.GetComponent<Object>();

        StartCoroutine(CubeRotate(playerObj.GetPosition().Color));
        while (isCubeMove) yield return new WaitForFixedUpdate();

        List<GameObject> attackableEnemy = AttackableObject(playerObj.GetWeaponType(), playerObj.GetPosition().Color, playerObj.GetPosition().Index, ObjectType.ENEMY);
        foreach (GameObject enemy in attackableEnemy)
        {
            enemy.GetComponent<Object>().OnHit(playerObj.GetDamage());
            yield return new WaitForFixedUpdate();

            yield return new WaitForSeconds(0.1f);
        }

        // 적과 동료의 공격 순서 결정을 위해 List 생성
        List<KeyValuePair<int, int>> enemyAttackOrder = new List<KeyValuePair<int, int>>();
        for (int i = 0; i < stageDatas[ENEMY_COUNT]; i++)
        {
            Object enemyObject = enemy[i].GetComponent<Object>();
            if (enemyObject.gameObject.activeSelf)
                enemyAttackOrder.Add(new KeyValuePair<int, int>(enemyObject.GetDamage(), i));
            else // enemyAttackOrder의 크기가 friendAttackOrder 보다 커야하므로 죽은 애들도 일단 list에 넣기
                enemyAttackOrder.Add(new KeyValuePair<int, int>(0, i));
        }
        List<KeyValuePair<int, int>> friendAttackOrder = new List<KeyValuePair<int, int>>();
        for (int i = 0; i < 3; i++)
        {
            if (friend[i] == null) continue;
            Object friendObject = friend[i].GetComponent<Object>();
            if (friendObject.gameObject.activeSelf)
                friendAttackOrder.Add(new KeyValuePair<int, int>(friendObject.GetDamage(), i));
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
                attackableEnemy = AttackableObject(friendObj.GetWeaponType(), friendObj.GetPosition().Color, friendObj.GetPosition().Index, ObjectType.ENEMY);
                
                StartCoroutine(CubeRotate(friendObj.GetPosition().Color));
                while (isCubeMove) yield return new WaitForFixedUpdate();

                foreach (GameObject enemy in attackableEnemy)
                {
                    enemy.GetComponent<Object>().OnHit(friendAttackOrder[i].Key);
                    yield return new WaitForFixedUpdate();

                    yield return new WaitForSeconds(0.1f);
                }
            }

            // 적 공격
            Object enemyObj = enemy[enemyAttackOrder[i].Value].GetComponent<Object>();
            List<GameObject> attackablePlayerTeam;

            attackablePlayerTeam = AttackableObject(enemyObj.GetWeaponType(), enemyObj.GetPosition().Color, enemyObj.GetPosition().Index, ObjectType.PLAYER);

            StartCoroutine(CubeRotate(enemyObj.GetPosition().Color));
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

            attackablePlayerTeam = AttackableObject(enemyObj.GetWeaponType(), enemyObj.GetPosition().Color, enemyObj.GetPosition().Index, ObjectType.FRIEND);
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
            ColorCheckCube playerPosition = player.GetComponent<Object>().GetPosition();
            if (weaponType == WeaponType.MELEE || weaponType == WeaponType.AD)
            {
                if (playerPosition.Color == color && AttackableRange(weaponType, index)[playerPosition.Index])
                    attackable.Add(player);
            }
            else if(weaponType == WeaponType.AP)
            {
                if (playerPosition.Color != color && playerPosition.Index == index)
                    attackable.Add(player);
            }
        }
        else if (objType == ObjectType.FRIEND)
        {
            for(int i = 0;i<3;i++)
            {
                if (friend[i] == null || !friend[i].activeSelf) continue;

                ColorCheckCube friendPosition = friend[i].GetComponent<Object>().GetPosition();
                if (weaponType == WeaponType.MELEE || weaponType == WeaponType.AD)
                {
                    if (friendPosition.Color == color && AttackableRange(weaponType, index)[friendPosition.Index])
                        attackable.Add(friend[i]);
                }
                else if (weaponType == WeaponType.AP)
                {
                    if (friendPosition.Color != color && friendPosition.Index == index)
                        attackable.Add(friend[i]);
                }
            }
        }
        else if(objType == ObjectType.ENEMY)
        {
            foreach(GameObject enemyObj in enemy)
            {
                if (!enemyObj.activeSelf) continue;

                ColorCheckCube enemyPosition = enemyObj.GetComponent<Object>().GetPosition();
                if (weaponType == WeaponType.MELEE || weaponType == WeaponType.AD)
                {
                    if (enemyPosition.Color == color && AttackableRange(weaponType, index)[enemyPosition.Index])
                        attackable.Add(enemyObj);
                }
                else if (weaponType == WeaponType.AP)
                {
                    if (enemyPosition.Color != color && enemyPosition.Index == index)
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

        if (weaponType == WeaponType.MELEE)
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
        else if (weaponType == WeaponType.AD)
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
        ColorCheckCube cube = colorCheckCubeArray[color.ToInt()][index].GetComponent<ColorCheckCube>();
        if (cube.GetObjectType() != ObjectType.NULL)
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
