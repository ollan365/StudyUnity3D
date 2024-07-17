using UnityEngine;
using System.Collections.Generic;
using static Constants;

[CreateAssetMenu(fileName = "New EventCard", menuName = "Event/EventCard")]
public class EventCard : ScriptableObject
{
    public Colors[] eventColors;
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
            descriptions[0] = descriptions[0].Replace("{SLIENCE}", "<b>ħ��</b>");
            descriptions.Add("�ִ� �������� 0���� �����");
        }
        if (description.Contains("{POWERFUL}"))
        {
            descriptions[0] = descriptions[0].Replace("{POWERFUL}", "<b>��ȭ</b>");
            descriptions.Add("�ִ� �������� 2�谡 �ȴ�");
        }
        if (description.Contains("{INVINCIBILITY}"))
        {
            descriptions[0] = descriptions[0].Replace("{INVINCIBILITY}", "<b>����</b>");
            descriptions.Add("�޴� �������� 0���� �����");
        }
        if (description.Contains("{WEAKEN}"))
        {
            descriptions[0] = descriptions[0].Replace("{WEAKEN}", "<b>���</b>");
            descriptions.Add("�޴� �������� 2�谡 �ȴ�");
        }
        if (description.Contains("{BLESS}"))
        {
            descriptions[0] = descriptions[0].Replace("{BLESS}", "<b>�ູ</b>");
            descriptions.Add("ü���� �Ϻ� ȸ���Ѵ�");
        }
        if (description.Contains("{CURSE}"))
        {
            descriptions[0] = descriptions[0].Replace("{CURSE}", "<b>����</b>");
            descriptions.Add("ü���� �Ϻ� �Ҵ´�");
        }

        return descriptions;
    }
}
