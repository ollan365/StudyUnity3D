using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubeManager : MonoBehaviour
{
    public enum TouchPlaneColor { WHITE = 0, RED, BLUE, GREEN, ORANGE, YELLOW }
    [SerializeField] private GameObject[] whiteArray;
    [SerializeField] private GameObject[] redArray;
    [SerializeField] private GameObject[] blueArray;
    [SerializeField] private GameObject[] greenArray;
    [SerializeField] private GameObject[] orangeArray;
    [SerializeField] private GameObject[] yellowArray;

    [SerializeField] private GameObject[] turnPoints;
    [SerializeField] private GameObject touchPanels;
    public void Turn(TouchPlaneColor color, int direction)
    {
        touchPanels.SetActive(false); // ���� Ŭ���� �� �ǵ��� 
        GameObject turnPoint = turnPoints[(int)color];
        GameObject[] array;
        Vector3 rotation = turnPoint.transform.rotation.eulerAngles;
        switch (color)
        {
            case TouchPlaneColor.WHITE:
                rotation.y += direction * 90;
                array = whiteArray;
                break;
            case TouchPlaneColor.RED:
                rotation.x += direction * 90;
                array = redArray;
                break;
            case TouchPlaneColor.BLUE:
                rotation.z += direction * 90;
                array = blueArray;
                break;
            case TouchPlaneColor.GREEN:
                rotation.z -= direction * 90;
                array = greenArray;
                break;
            case TouchPlaneColor.ORANGE:
                rotation.x -= direction * 90;
                array = orangeArray;
                break;
            case TouchPlaneColor.YELLOW:
                rotation.y -= direction * 90;
                array = yellowArray;
                break;
            default:
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
        Quaternion startRotation = turnPoint.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(rotation);

        Debug.Log($"rotation: {rotation} / endRotation: {endRotation.eulerAngles}");

        float elapsedTime = 0f;
        float duration = 1.0f; // ȸ���� �ɸ� �ð�

        while (elapsedTime < duration)
        {
            turnPoint.transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ������ ���� ���� ȸ�� ������ ����
        turnPoint.transform.rotation = endRotation;

        foreach (GameObject cube in array) // ť����� �ٽ� ������ �θ� ������ ������
        {
            cube.transform.parent = transform;
        }
        touchPanels.SetActive(true);
        Debug.Log("rotate end!");
    }
}
