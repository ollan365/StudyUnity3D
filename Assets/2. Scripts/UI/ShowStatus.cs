using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShowStatus : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Object Info UI")]
    [SerializeField] private GameObject objectInfoPanel;
    [SerializeField] private Image info_Image;
    [SerializeField] private Sprite[] info_images;
    [SerializeField] private Text info_Name;
    [SerializeField] private Slider info_HPslider;
    [SerializeField] private Text info_HPText;
    [SerializeField] private Text info_AttackType;
    [SerializeField] private Text info_BasicAttack;
    public GameObject ObjectInfoPanel { get => objectInfoPanel; }
    public void OnPointerEnter(PointerEventData eventData)
    {
        //��ġ �����ϰ�, ���� �ְ�, Ȱ��ȭ ��ġ 150��
        ObjectManager.Instance.ObjectInfo(StageManager.Instance.Player);
        ObjectManager.Instance.ObjectInfoPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //��Ȱ��ȭ
        ObjectManager.Instance.ObjectInfoPanel.SetActive(false);
    }

    private void setInfo()
    {

    }
}
