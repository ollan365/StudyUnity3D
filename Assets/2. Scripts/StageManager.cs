using UnityEngine;
using System.Collections;
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

    private StageStatus status;
    private void Start()
    {
        colorCheckCubeArray = new GameObject[][] { whiteCheckCubeArray, redCheckCubeArray, blueCheckCubeArray, greenCheckCubeArray, orangeCheckCubeArray, yellowCheckCubeArray };
        enemy = new GameObject[enemyCount];
        status = StageStatus.INIT;
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
                if (cube.GetObjectType() == ObjectType.Null)
                    break;
            }
            GameObject newEnemy = Instantiate(enemyPrefab);
            newEnemy.transform.parent = cube.colorPointCube.transform.GetChild(0);
            newEnemy.transform.position = cube.colorPointCube.transform.GetChild(0).position;
            newEnemy.transform.rotation = cube.colorPointCube.transform.GetChild(0).rotation;

            enemy[i] = newEnemy;
        }

        status = StageStatus.PLAYER;
    }

    public void ChangeStatus()
    {
        if(status == StageStatus.PLAYER)
        {
            status = StageStatus.FIGHT;
        }
        else if (status == StageStatus.FIGHT)
        {
            status = StageStatus.PLAYER;
        }
    }
}
