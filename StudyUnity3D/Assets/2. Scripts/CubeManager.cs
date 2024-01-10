using UnityEngine;
using System.Collections;

public class CubeManager : MonoBehaviour
{
    public enum TouchPlaneColor { WHITE, RED, BLUE, GREEN, ORANGE, YELLOW }
    private readonly int WHITE = 0, RED = 1, BLUE = 2, GREEN = 3, ORANGE = 4, YELLOW = 5;

    [SerializeField] private GameObject[] whiteArray; // 2차원 배열이 inspector에서 할당이 안 돼서 만든 array
    [SerializeField] private GameObject[] redArray;
    [SerializeField] private GameObject[] blueArray;
    [SerializeField] private GameObject[] greenArray;
    [SerializeField] private GameObject[] orangeArray;
    [SerializeField] private GameObject[] yellowArray;

    private GameObject[][] colorArray;

    [SerializeField] private GameObject[] whiteArrayForAssignment; // 다차원 배열이 inspector에서 할당이 안 돼서 만든 array
    [SerializeField] private GameObject[] redArrayForAssignment;
    [SerializeField] private GameObject[] blueArrayForAssignment;
    [SerializeField] private GameObject[] greenArrayForAssignment;
    [SerializeField] private GameObject[] orangeArrayForAssignment;
    [SerializeField] private GameObject[] yellowArrayForAssignment;

    private GameObject[][][] colorChangeArray;

    [SerializeField] private GameObject[] turnPoints;
    [SerializeField] private GameObject touchPanels;
    [SerializeField] private GameObject cubeParent;

    [SerializeField] private float duration; // 회전에 걸리는 시간
    private void Start()
    {
        colorArray = new GameObject[][] { whiteArray, redArray, blueArray, greenArray, orangeArray, yellowArray };
        colorChangeArray = new GameObject[6][][];

        int index = 0;
        colorChangeArray[WHITE] = new GameObject[6][];
        for (int i = 0; i < colorChangeArray[WHITE].Length; i++)
        {
            colorChangeArray[WHITE][i] = new GameObject[3];

            switch (i)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                    for (int j = 0; j < 3; j++)
                    {
                        colorChangeArray[WHITE][i][j] = whiteArrayForAssignment[index];
                        index++;
                    }
                    break;
                default:
                    break;
            }
        }
        index = 0;

        colorChangeArray[RED] = new GameObject[6][];
        for (int i = 0; i < colorChangeArray[RED].Length; i++)
        {
            colorChangeArray[RED][i] = new GameObject[3];

            switch (i)
            {
                case 0:
                case 2:
                case 3:
                case 5:
                    for (int j = 0; j < 3; j++)
                    {
                        colorChangeArray[RED][i][j] = redArrayForAssignment[index];
                        index++;
                    }
                    break;
                default:
                    break;
            }
        }
        index = 0;

        colorChangeArray[BLUE] = new GameObject[6][];
        for (int i = 0; i < colorChangeArray[BLUE].Length; i++)
        {
            colorChangeArray[BLUE][i] = new GameObject[3];

            switch (i)
            {
                case 0:
                case 1:
                case 4:
                case 5:
                    for (int j = 0; j < 3; j++)
                    {
                        colorChangeArray[BLUE][i][j] = blueArrayForAssignment[index];
                        index++;
                    }
                    break;
                default:
                    break;
            }
        }
        index = 0;

        colorChangeArray[GREEN] = new GameObject[6][];
        for (int i = 0; i < colorChangeArray[GREEN].Length; i++)
        {
            colorChangeArray[GREEN][i] = new GameObject[3];

            switch (i)
            {
                case 0:
                case 1:
                case 4:
                case 5:
                    for (int j = 0; j < 3; j++)
                    {
                        colorChangeArray[GREEN][i][j] = greenArrayForAssignment[index];
                        index++;
                    }
                    break;
                default:
                    break;
            }
        }
        index = 0;

        colorChangeArray[ORANGE] = new GameObject[6][];
        for (int i = 0; i < colorChangeArray[ORANGE].Length; i++)
        {
            colorChangeArray[ORANGE][i] = new GameObject[3];

            switch (i)
            {
                case 0:
                case 2:
                case 3:
                case 5:
                    for (int j = 0; j < 3; j++)
                    {
                        colorChangeArray[ORANGE][i][j] = orangeArrayForAssignment[index];
                        index++;
                    }
                    break;
                default:
                    break;
            }
        }
        index = 0;

        colorChangeArray[YELLOW] = new GameObject[6][];
        for (int i = 0; i < colorChangeArray[YELLOW].Length; i++)
        {
            colorChangeArray[YELLOW][i] = new GameObject[3];

            switch (i)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                    for (int j = 0; j < 3; j++)
                    {
                        colorChangeArray[YELLOW][i][j] = yellowArrayForAssignment[index];
                        index++;
                    }
                    break;
                default:
                    break;
            }
        }
    }
    public void Turn(TouchPlaneColor color, int direction)
    {
        touchPanels.SetActive(false); // 연속 클릭이 안 되도록 
        GameObject turnPoint;
        GameObject[] array;
        Vector3 rotation = Vector3.zero;
        switch (color)
        {
            case TouchPlaneColor.WHITE:
                turnPoint = turnPoints[WHITE];
                rotation.y += direction * 90;
                array = colorArray[WHITE];
                break;
            case TouchPlaneColor.RED:
                turnPoint = turnPoints[RED];
                rotation.x += direction * 90;
                array = colorArray[RED];
                break;
            case TouchPlaneColor.BLUE:
                turnPoint = turnPoints[BLUE];
                rotation.z += direction * 90;
                array = colorArray[BLUE];
                break;
            case TouchPlaneColor.GREEN:
                turnPoint = turnPoints[GREEN];
                rotation.z -= direction * 90;
                array = colorArray[GREEN];
                break;
            case TouchPlaneColor.ORANGE:
                turnPoint = turnPoints[ORANGE];
                rotation.x -= direction * 90;
                array = colorArray[ORANGE];
                break;
            case TouchPlaneColor.YELLOW:
                turnPoint = turnPoints[YELLOW];
                rotation.y -= direction * 90;
                array = colorArray[YELLOW];
                break;
            default:
                turnPoint = null;
                rotation = Vector3.zero;
                array = null;
                break;
        }
        foreach (GameObject cube in array) // 큐브의 부모를 turnPoint로 설정하여 함께 회전시킨다
        {
            cube.transform.parent = turnPoint.transform;
        }
        StartCoroutine(TurnEffect(turnPoint, rotation, array));
        ChangeArray(color, direction);
    }

    private IEnumerator TurnEffect(GameObject turnPoint, Vector3 rotation, GameObject[] array)
    {
        Quaternion startRotation = turnPoint.transform.localRotation;
        Quaternion endRotation = Quaternion.Euler(rotation) * startRotation;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            turnPoint.transform.localRotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 보정을 위해 최종 회전 각도로 설정
        turnPoint.transform.localRotation = endRotation;

        foreach (GameObject cube in array) // 큐브들을 다시 본래의 부모 밑으로 보낸다
        {
            cube.transform.parent = cubeParent.transform;
        }
        touchPanels.SetActive(true);
    }

    private void ChangeArray(TouchPlaneColor color, int direction)
    {
        int index = 0;
        int[] originColor = new int[4];
        switch (color)
        {
            case TouchPlaneColor.WHITE:
                index = WHITE;
                originColor = new int[] { RED, BLUE, ORANGE, GREEN };
                break;
            case TouchPlaneColor.RED:
                index = RED;
                originColor = new int[] { WHITE, GREEN, YELLOW, BLUE };
                break;
            case TouchPlaneColor.BLUE:
                index = BLUE;
                originColor = new int[] { WHITE, RED, YELLOW, ORANGE };
                break;
            case TouchPlaneColor.GREEN:
                index = GREEN;
                originColor = new int[] { WHITE, ORANGE, YELLOW, RED };
                break;
            case TouchPlaneColor.ORANGE:
                index = ORANGE;
                originColor = new int[] { WHITE, BLUE, YELLOW, GREEN };
                break;
            case TouchPlaneColor.YELLOW:
                index = YELLOW;
                originColor = new int[] { RED, GREEN, ORANGE, BLUE };
                break;
        }

        if(direction > 0)
        {
            int swap = originColor[1];
            originColor[1] = originColor[3];
            originColor[3] = swap;
        }

        GameObject[][] nextColorChangeArray = new GameObject[4][];
        for(int i = 0; i< 4; i++)
        {
            nextColorChangeArray[i] = new GameObject[3];
            for(int j = 0; j < 3; j++)
                nextColorChangeArray[i][j] = colorChangeArray[originColor[(i + 3) % 4]][index][j];
            
        }

        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 3; j++)
                for (int k = 0; k < colorArray[0].Length; k++)
                    if(colorChangeArray[originColor[i]][index][j] == colorArray[originColor[i]][k])
                    {
                        colorArray[originColor[i]][k] = nextColorChangeArray[i][j];
                        colorChangeArray[originColor[i]][index][j] = nextColorChangeArray[i][j];
                        colorChangeArray[index][originColor[i]][j] = nextColorChangeArray[i][j];
                        break;
                    }
    }
}
