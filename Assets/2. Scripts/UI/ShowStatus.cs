using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShowStatus : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject targetObject;
    public int index;
    public int cameraPositionIndex = 0;
    [SerializeField] private int[] cameraFieldOfView;
    [SerializeField] private Vector3[] cameraPositions;
    [SerializeField] private Vector3[] cameraRotations;

    public void OnPointerClick(PointerEventData eventData)
    {
        cameraPositionIndex = (cameraPositionIndex + 1) % 3;

        Camera.main.transform.position = cameraPositions[cameraPositionIndex];
        Camera.main.transform.rotation = Quaternion.Euler(cameraRotations[cameraPositionIndex]);
        Camera.main.GetComponent<Camera>().fieldOfView = cameraFieldOfView[cameraPositionIndex];

        if (cameraPositionIndex == 1)
            StartCoroutine(StageManager.Instance.CubeRotate(targetObject.GetComponent<Object>().Color));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //위치 설정하고, 정보 넣고, 활성화 위치 150씩
        ObjectManager.Instance.SetObjectInfo(targetObject.GetComponent<Object>(), index);
        ObjectManager.Instance.ObjectInfoPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //비활성화
        ObjectManager.Instance.ObjectInfoPanel.SetActive(false);
    }

}
