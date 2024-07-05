using UnityEngine;
using static Constants;

[CreateAssetMenu(fileName = "New EventCard", menuName = "Event/EventCard")]
public class EventCard : ScriptableObject
{
    public Colors[] colors;
    public string eventName;
    public string eventDescription;
    public EventCard(string name, string description)
    {
        eventName = name;
        eventDescription = description;
    }
}
