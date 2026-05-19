using UnityEngine;

public class StoryEventManager : MonoBehaviour
{
    public static StoryEventManager Instance;

    [Header("Events")]
    public StoryEventData[] possibleEvents;

    void Awake()
    {
        Instance = this;
    }

    public void TriggerRandomEvent()
    {
        if (possibleEvents.Length == 0)
            return;

        int random =
            Random.Range(0, possibleEvents.Length);

        StoryEventData selected =
            possibleEvents[random];

        ShowEvent(selected);
    }

    void ShowEvent(StoryEventData data)
    {
        Debug.Log("EVENTO: " + data.eventTitle);
        Debug.Log(data.description);

        // después conectamos UI real
    }

    public void ResolveChoice(EventChoiceData choice)
    {
        if (GameManager.Instance == null)
            return;

        int player =
            GameManager.Instance.playerEmpireIndex;

        EmpireStats stats =
            GameManager.Instance.GetEmpireTotalStats(player);

        bool success =
            stats.power >= choice.requiredPower &&
            stats.defense >= choice.requiredDefense &&
            stats.intelligence >= choice.requiredIntelligence &&
            stats.morale >= choice.requiredMorale;

        if (success)
        {
            GameManager.Instance.AddCredits(
                player,
                choice.rewardCredits
            );

            Debug.Log(choice.successMessage);
        }
        else
        {
            GameManager.Instance.RemoveCredits(
                player,
                choice.penaltyCredits
            );

            Debug.Log(choice.failureMessage);
        }
    }
}