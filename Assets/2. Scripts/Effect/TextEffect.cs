using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextEffect : MonoBehaviour
{
    [SerializeField] private GameObject detailBox;
    private TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        CheckWord();
    }
    private void CheckWord()
    {
        Vector3 mousePosition = Input.mousePosition;

        if (!RectTransformUtility.RectangleContainsScreenPoint(text.rectTransform, mousePosition, null))
        { if (detailBox.activeSelf) detailBox.SetActive(false); return; }

        int charIndex = TMP_TextUtilities.FindIntersectingCharacter(text, mousePosition, null, true);
        if (charIndex == -1) { detailBox.SetActive(false); return; }

        int wordIndex = TMP_TextUtilities.FindIntersectingWord(text, mousePosition, null);
        if (wordIndex == -1) { detailBox.SetActive(false); return; }

        TMP_WordInfo wInfo = text.textInfo.wordInfo[wordIndex];
        switch (wInfo.GetWord())
        {
            case "ħ��":
                detailBox.SetActive(true);
                detailBox.GetComponentInChildren<TMP_Text>().text = "[�ִ� ������]�� 0���� �����.";
                break;
            case "��ȭ":
                detailBox.SetActive(true);
                detailBox.GetComponentInChildren<TMP_Text>().text = "[�ִ� ������]�� 2�谡 �ȴ�.";
                break;
            case "����":
                detailBox.SetActive(true);
                detailBox.GetComponentInChildren<TMP_Text>().text = "[�޴� ������]�� 0���� �����.";
                break;
            case "���":
                detailBox.SetActive(true);
                detailBox.GetComponentInChildren<TMP_Text>().text = "[�޴� ������]�� 2�谡 �ȴ�.";
                break;
            case "����":
                detailBox.SetActive(true);
                detailBox.GetComponentInChildren<TMP_Text>().text = "[���� ü��]�� [10%] ���ҽ�Ų��.";
                break;
            case "�ູ":
                detailBox.SetActive(true);
                detailBox.GetComponentInChildren<TMP_Text>().text = "[���� ü��]�� [10%] ������Ų��.";
                break;
        }
    }
}
