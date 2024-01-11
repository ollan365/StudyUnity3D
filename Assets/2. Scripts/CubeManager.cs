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

    [SerializeField] private GameObject[] turnPoints;
    [SerializeField] private GameObject touchPanels;

    [SerializeField] private float duration; // 회전에 걸리는 시간
    private bool turning;
    private void Start()
    {
        turning = false;
        colorArray = new GameObject[][] { whiteArray, redArray, blueArray, greenArray, orangeArray, yellowArray };
    }
    public void StartRandomTurn(int randomCount)
    {
        StartCoroutine(RandomTurn(randomCount));
    }
    private IEnumerator RandomTurn(int randomCount)
    {
        for(int i = 0; i < randomCount; i++)
        {
            while (turning)
                yield return null;

            turning = true;

            int direction = Random.Range(0, 2);
            direction = direction == 1 ? 1 : -1;
            int value = Random.Range(0, 6);

            switch (value)
            {
                case 0:
                    Turn(TouchPlaneColor.WHITE, direction); break;
                case 1:
                    Turn(TouchPlaneColor.RED, direction); break;
                case 2:
                    Turn(TouchPlaneColor.BLUE, direction); break;
                case 3:
                    Turn(TouchPlaneColor.GREEN, direction); break;
                case 4:
                    Turn(TouchPlaneColor.ORANGE, direction); break;
                case 5:
                    Turn(TouchPlaneColor.YELLOW, direction); break;
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
        foreach (GameObject position in array) // 큐브의 부모를 turnPoint로 설정하여 함께 회전시킨다
        {
            position.transform.GetChild(0).parent = turnPoint.transform;
        }
        StartCoroutine(TurnEffect(turnPoint, rotation, array));
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

        foreach (GameObject position in array)
            position.GetComponent<CubePosition>().FindChild();

        touchPanels.SetActive(true);
        turning = false;
    }
}
