using UnityEngine;
using System.Collections.Generic;
using static Constants;

[CreateAssetMenu(fileName = "New EventCard", menuName = "Event/EventCard")]
public class EventCard : ScriptableObject
{
    public string eventName;
    [SerializeField] private string eventDescription;
    public List<string> EventDescription => GetEventDescription(eventDescription);
    public EventCard(string name, string description)
    {
        eventName = name;
        eventDescription = description;
    }
    private List<string> GetEventDescription(string description)
    {
        List<string> descriptions = new List<string>();
        descriptions.Add(description.Replace("{TargetBlockColor}", EventManager.Instance.Effect.color.ToString()));

        if (description.Contains("{SLIENCE}"))
        {
            descriptions[0] = descriptions[0].Replace("{SLIENCE}", "<b>침묵</b>");
            descriptions.Add("주는 데미지를 0으로 만든다");
        }
        if (description.Contains("{POWERFUL}"))
        {
            descriptions[0] = descriptions[0].Replace("{POWERFUL}", "<b>강화</b>");
            descriptions.Add("주는 데미지가 2배가 된다");
        }
        if (description.Contains("{INVINCIBILITY}"))
        {
            descriptions[0] = descriptions[0].Replace("{INVINCIBILITY}", "<b>무적</b>");
            descriptions.Add("받는 데미지를 0으로 만든다");
        }
        if (description.Contains("{WEAKEN}"))
        {
            descriptions[0] = descriptions[0].Replace("{WEAKEN}", "<b>취약</b>");
            descriptions.Add("받는 데미지가 2배가 된다");
        }
        if (description.Contains("{BLESS}"))
        {
            descriptions[0] = descriptions[0].Replace("{BLESS}", "<b>축복</b>");
            descriptions.Add("체력을 일부 회복한다");
        }
        if (description.Contains("{CURSE}"))
        {
            descriptions[0] = descriptions[0].Replace("{CURSE}", "<b>저주</b>");
            descriptions.Add("체력을 일부 잃는다");
        }

        return descriptions;
    }
}
