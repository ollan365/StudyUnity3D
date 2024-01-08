using UnityEngine;
using System.Collections;

public class CubeManager : MonoBehaviour
{
    public enum TouchPlaneColor { WHITE, RED, BLUE, GREEN, ORANGE, YELLOW }
    [SerializeField] private GameObject[] whiteArray;
    [SerializeField] private GameObject[] redArray;
    [SerializeField] private GameObject[] blueArray;
    [SerializeField] private GameObject[] greenArray;
    [SerializeField] private GameObject[] orangeArray;
    [SerializeField] private GameObject[] yellowArray;

    [SerializeField] private GameObject[] turnPoints;
    [SerializeField] private GameObject touchPanels;
    [SerializeField] private GameObject cubeParent;

    [SerializeField] private float duration; // ȸ���� �ɸ��� �ð�
    public void Turn(TouchPlaneColor color, int direction)
    {
        touchPanels.SetActive(false); // ���� Ŭ���� �� �ǵ��� 
        GameObject turnPoint;
        GameObject[] array;
        Vector3 rotation = Vector3.zero;
        switch (color)
        {
            case TouchPlaneColor.WHITE:
                turnPoint = turnPoints[0];
                rotation.y += direction * 90;
                array = whiteArray;
                break;
            case TouchPlaneColor.RED:
                turnPoint = turnPoints[1];
                rotation.x += direction * 90;
                array = redArray;
                break;
            case TouchPlaneColor.BLUE:
                turnPoint = turnPoints[2];
                rotation.z += direction * 90;
                array = blueArray;
                break;
            case TouchPlaneColor.GREEN:
                turnPoint = turnPoints[3];
                rotation.z -= direction * 90;
                array = greenArray;
                break;
            case TouchPlaneColor.ORANGE:
                turnPoint = turnPoints[4];
                rotation.x -= direction * 90;
                array = orangeArray;
                break;
            case TouchPlaneColor.YELLOW:
                turnPoint = turnPoints[5];
                rotation.y -= direction * 90;
                array = yellowArray;
                break;
            default:
                turnPoint = null;
                rotation = Vector3.zero;
                array = null;
                break;
        }
        foreach (GameObject cube in array) // ť���� �θ� turnPoint�� �����Ͽ� �Բ� ȸ����Ų��
        {
            cube.transform.parent = turnPoints[(int)color].transform;
        }
        StartCoroutine(TurnEffect(turnPoint, rotation, array));
        
    }

    // white: y -90 / y +90
    // red: x -90 / x +90
    // blue: z -90 / z +90
    // green: z +90 / z -90
    // orange: x +90 / x -90
    // yellow: y +90 / y -90
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

        foreach (GameObject cube in array) // ť����� �ٽ� ������ �θ� ������ ������
        {
            cube.transform.parent = cubeParent.transform;
        }
        touchPanels.SetActive(true);
    }
}
