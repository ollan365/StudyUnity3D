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
            case "침묵":
                detailBox.SetActive(true);
                detailBox.GetComponentInChildren<TMP_Text>().text = "[주는 데미지]를 0으로 만든다.";
                break;
            case "강화":
                detailBox.SetActive(true);
                detailBox.GetComponentInChildren<TMP_Text>().text = "[주는 데미지]가 2배가 된다.";
                break;
            case "무적":
                detailBox.SetActive(true);
                detailBox.GetComponentInChildren<TMP_Text>().text = "[받는 데미지]를 0으로 만든다.";
                break;
            case "취약":
                detailBox.SetActive(true);
                detailBox.GetComponentInChildren<TMP_Text>().text = "[받는 데미지]가 2배가 된다.";
                break;
            case "저주":
                detailBox.SetActive(true);
                detailBox.GetComponentInChildren<TMP_Text>().text = "[현재 체력]을 [10%] 감소시킨다.";
                break;
            case "축복":
                detailBox.SetActive(true);
                detailBox.GetComponentInChildren<TMP_Text>().text = "[현재 체력]을 [10%] 증가시킨다.";
                break;
        }
    }
}
