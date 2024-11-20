using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShowStatus : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject targetObject;
    public int index;

    public void OnPointerClick(PointerEventData eventData)
    {
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
