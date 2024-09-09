using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static Constants;
using TMPro;

public class BingoUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Colors color;
    [SerializeField] private GameObject lineIcon;
    [SerializeField] private GameObject sideIcon;
    [SerializeField] private GameObject panel;
    [SerializeField] private Vector3 position;
    
    public void SetLineIcon() { lineIcon.SetActive(true); }
    public void SetSideIcon() { sideIcon.SetActive(true); }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EventManager.Instance.Effect.color == color)
        {
            panel.transform.position = position;
            panel.SetActive(true);

            string text = "";
            for (int i = 0; i< EventManager.Instance.Effect.statusEffects.Length; i++)
            {
                if (!EventManager.Instance.Effect.statusEffects[i]) continue;

                switch (i)
                {
                    case SLIENCE:
                        text += "SLIENCE ";
                        break;
                    case POWERFUL:
                        text += "POWERFUL ";
                        break;
                    case INVINCIBILITY:
                        text += "INVINCIBILITY ";
                        break;
                    case WEAKEN:
                        text += "WEAKEN ";
                        break;
                    case BLESS:
                        text += "BLESS ";
                        break;
                    case CURSE:
                        text += "CURSE ";
                        break;
                }
            }
            //텍스트 길이에 맞춰서 사이즈 늘리기
            TMP_Text efftect_Text = panel.transform.GetChild(0).GetComponent<TMP_Text>();
            efftect_Text.text = text;

            RectTransform textRect = panel.GetComponent<RectTransform>();
            Vector2 rectSize = textRect.sizeDelta;
            rectSize.x = efftect_Text.preferredWidth + 20f;
            textRect.sizeDelta = rectSize;
            
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        panel.SetActive(false);
    }
}
