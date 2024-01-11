using UnityEngine;
using System.Collections;

public class CubeManager : MonoBehaviour
{
    public enum TouchPlaneColor { WHITE, RED, BLUE, GREEN, ORANGE, YELLOW }
    private readonly int WHITE = 0, RED = 1, BLUE = 2, GREEN = 3, ORANGE = 4, YELLOW = 5;

    [SerializeField] private GameObject[] whiteArray; // 2���� �迭�� inspector���� �Ҵ��� �� �ż� ���� array
    [SerializeField] private GameObject[] redArray;
    [SerializeField] private GameObject[] blueArray;
    [SerializeField] private GameObject[] greenArray;
    [SerializeField] private GameObject[] orangeArray;
    [SerializeField] private GameObject[] yellowArray;

    private GameObject[][] colorArray;

    [SerializeField] private GameObject[] turnPoints;
    [SerializeField] private GameObject touchPanels;

    [SerializeField] private float duration; // ȸ���� �ɸ��� �ð�
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
        touchPanels.SetActive(false); // ���� Ŭ���� �� �ǵ��� 
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
        foreach (GameObject position in array) // ť���� �θ� turnPoint�� �����Ͽ� �Բ� ȸ����Ų��
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

        // ������ ���� ���� ȸ�� ������ ����
        turnPoint.transform.localRotation = endRotation;

        foreach (GameObject position in array)
            position.GetComponent<CubePosition>().FindChild();

        touchPanels.SetActive(true);
        turning = false;
    }
}
