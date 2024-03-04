using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using static Constants;

public class StageManager : MonoBehaviour
{
    [SerializeField] private GameObject[] whiteCheckCubeArray, redCheckCubeArray, blueCheckCubeArray, greenCheckCubeArray, orangeCheckCubeArray, yellowCheckCubeArray;
    private GameObject[][] colorCheckCubeArray;

    [SerializeField] private CubeManager cubeManager;
    [SerializeField] private ObjectManager objectManager;
    [SerializeField] private StaticManager staticManager;

    private int startRandomTurnCount;
    private int rotateCountPerTurn;
    private int moveCountPerTurn;
    private int changeWeaponCountPerTurn;

    private int enemyCount;
    private int treasureCount;
    private GameObject[] enemy;
    private GameObject[] friend;
    private GameObject[] treasure;
    [SerializeField] private GameObject player;

    [SerializeField] private GameObject gameOverPanel;

    [SerializeField] private Text stageStatusText;
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
                    stageStatusText.text = "INIT";
                    break;
                case StageStatus.PLAYER:
                    stageStatusText.text = "PLAYER";
                    cubeManager.StageCountTextChange(rotateCountPerTurn, moveCountPerTurn, changeWeaponCountPerTurn);
                    break;
                case StageStatus.FIGHT:
                    stageStatusText.text = "FIGHT";
                    break;
                case StageStatus.END:
                    stageStatusText.text = "END";
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

        startRandomTurnCount = int.Parse(line.Split(',')[1]);
        rotateCountPerTurn = int.Parse(line.Split(',')[2]);
        moveCountPerTurn = int.Parse(line.Split(',')[3]);
        changeWeaponCountPerTurn = int.Parse(line.Split(',')[4]);
        enemyCount = int.Parse(line.Split(',')[5]);
        treasureCount = int.Parse(line.Split(',')[6]);

        enemy = new GameObject[enemyCount];
        friend = new GameObject[3];
        treasure = new GameObject[treasureCount];

        StartCoroutine(StartStage());
    }
    private IEnumerator StartStage()
    {
        cubeManager.StartRandomTurn(startRandomTurnCount); // 큐브를 섞는다

        yield return new WaitForSeconds(5f);

        TextAsset textFile = Resources.Load("StageEnemy") as TextAsset;
        StringReader stringReader = new StringReader(textFile.text);

        string line = stringReader.ReadLine();

        do { line = stringReader.ReadLine(); }
        while (line != null && int.Parse(line.Split(',')[0]) != staticManager.Stage);

        int index = 0;
        while (index < enemyCount && line != null && int.Parse(line.Split(',')[0]) == staticManager.Stage) // enemy 배치
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

        for (int i = 0; i < treasureCount; i++) // enemy 배치
        {
            ColorCheckCube cube;
            while (true)
            {
                cube = colorCheckCubeArray[Random.Range(0, 6)][Random.Range(0, 9)].GetComponent<ColorCheckCube>();
                if (cube.GetObjectType() == ObjectType.NULL)
                    break;
            }
            treasure[i] = objectManager.Summons(cube, ObjectType.TREASURE, 0);
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

    private IEnumerator Attack()
    {
        // 플레이어부터 공격
        Object playerObj = player.GetComponent<Object>();
        List<GameObject> attackableEnemy = AttackableObject(playerObj.GetWeaponType(), playerObj.GetPosition().Color, playerObj.GetPosition().Index, ObjectType.ENEMY);
        foreach (GameObject enemy in attackableEnemy)
        {
            enemy.GetComponent<Object>().OnHit(playerObj.GetDamage());
            yield return new WaitForFixedUpdate();

            yield return new WaitForSeconds(0.1f);
        }

        // 적과 동료의 공격 순서 결정을 위해 List 생성
        List<KeyValuePair<int, int>> enemyAttackOrder = new List<KeyValuePair<int, int>>();
        for(int i = 0;i<enemyCount;i++)
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

    public bool SummonsFriend(Colors color, int index)
    {
        ColorCheckCube cube = colorCheckCubeArray[color.ToInt()][index].GetComponent<ColorCheckCube>();
        if (cube.GetObjectType() != ObjectType.NULL)
            return false;
        for (int i = 0; i < 3; i++)
        {
            if (friend[i] == null) // 이건 동료 소환이 한 스테이지에서 3번만 가능할 때긴 함
            {
                friend[i] = objectManager.Summons(cube, ObjectType.FRIEND, 0);
                Debug.Log("summons success!");
                return true;
            }
        }
        return false;
    }
}
