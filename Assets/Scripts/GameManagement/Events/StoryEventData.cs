using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "RTS/Story Event")]
public class StoryEventData : ScriptableObject
{
    public string eventTitle;

    [TextArea]
    public string description;

    public List<EventChoiceData> choices =
        new List<EventChoiceData>();
}