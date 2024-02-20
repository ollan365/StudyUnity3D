using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using static Constants;

public class StageManager : MonoBehaviour
{
    [SerializeField] private GameObject[] whiteCheckCubeArray, redCheckCubeArray, blueCheckCubeArray, greenCheckCubeArray, orangeCheckCubeArray, yellowCheckCubeArray;
    private GameObject[][] colorCheckCubeArray;

    [SerializeField] private CubeManager cubeManager;
    [SerializeField] private int startRandomTurnCount;
    [SerializeField] private GameObject enemyPrefab;

    [SerializeField] private int enemyCount;
    public GameObject[] enemy; // 일단은 public

    [SerializeField] private GameObject player;

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
                    break;
                case StageStatus.FIGHT:
                    stageStatusText.text = "FIGHT";
                    break;
            }
        }
    }
    private void Start()
    {
        colorCheckCubeArray = new GameObject[][] { whiteCheckCubeArray, redCheckCubeArray, blueCheckCubeArray, greenCheckCubeArray, orangeCheckCubeArray, yellowCheckCubeArray };
        enemy = new GameObject[enemyCount];
        StatusOfStage = StageStatus.INIT;
        StartCoroutine(StartStage());
    }
    private IEnumerator StartStage()
    {
        cubeManager.StartRandomTurn(startRandomTurnCount); // 큐브를 섞는다

        yield return new WaitForSeconds(5f);

        for (int i = 0; i < enemyCount; i++) // enemy 배치
        {
            ColorCheckCube cube;
            while (true)
            {
                cube = colorCheckCubeArray[Random.Range(0, 6)][Random.Range(0, 9)].GetComponent<ColorCheckCube>();
                if (cube.GetObjectType() == ObjectType.NULL)
                    break;
            }
            GameObject newEnemy = Instantiate(enemyPrefab);
            newEnemy.transform.parent = cube.colorPointCube.transform.GetChild(0);
            newEnemy.transform.position = cube.colorPointCube.transform.GetChild(0).position;
            newEnemy.transform.rotation = cube.colorPointCube.transform.GetChild(0).rotation;

            enemy[i] = newEnemy;
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
        // 적 공격
        foreach (GameObject enemyGameObject in enemy)
        {
            Object enemyObj = enemyGameObject.GetComponent<Object>();
            List<GameObject> attackablePlayerTeam;

            attackablePlayerTeam = AttackableObject(enemyObj.GetWeaponType(), enemyObj.GetPosition().Color, enemyObj.GetPosition().Index, ObjectType.PLAYER);

            foreach (GameObject p in attackablePlayerTeam)
            {
                p.GetComponent<Object>().OnHit(enemyObj.GetDamage());
                yield return new WaitForFixedUpdate();

                yield return new WaitForSeconds(0.1f);
            }

            attackablePlayerTeam = AttackableObject(enemyObj.GetWeaponType(), enemyObj.GetPosition().Color, enemyObj.GetPosition().Index, ObjectType.FRIEND);
            foreach (GameObject pTeam in attackablePlayerTeam)
            {
                pTeam.GetComponent<Object>().OnHit(enemyObj.GetDamage());
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

        if(objType == ObjectType.PLAYER)
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
        else if(objType == ObjectType.ENEMY)
        {
            foreach(GameObject enemyObj in enemy)
            {
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
}
