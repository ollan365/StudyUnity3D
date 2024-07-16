using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static Constants;

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
            panel.GetComponentInChildren<Text>().text = text;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        panel.SetActive(false);
    }
}
