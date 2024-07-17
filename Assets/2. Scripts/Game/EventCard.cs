using UnityEngine;
using static Constants;

[CreateAssetMenu(fileName = "New EventCard", menuName = "Event/EventCard")]
public class EventCard : ScriptableObject
{
    public Colors[] eventColors;
    public string eventName;
    [SerializeField] private string eventDescription;
    public string EventDescription => GetEventDescription(eventDescription);
    public EventCard(string name, string description)
    {
        eventName = name;
        eventDescription = description;
    }
    private string GetEventDescription(string description)
    {
        description = description.Replace("{TargetBlockColor}", EventManager.Instance.Effect.color.ToString());
        return description;
    }
}
