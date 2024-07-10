using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShowStatus : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject targetObject;
    public int index;
    public void OnPointerEnter(PointerEventData eventData)
    {
        //��ġ �����ϰ�, ���� �ְ�, Ȱ��ȭ ��ġ 150��
        ObjectManager.Instance.SetObjectInfo(targetObject.GetComponent<Object>(), index);
        ObjectManager.Instance.ObjectInfoPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //��Ȱ��ȭ
        ObjectManager.Instance.ObjectInfoPanel.SetActive(false);
    }

}
