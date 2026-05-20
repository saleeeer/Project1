using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class EventUIManager : MonoBehaviour
{
    public static EventUIManager Instance;

    [Header("Panel")]
    public GameObject eventPanel;

    [Header("Texts")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI resultText;

    [Header("Choices")]
    public Transform choicesParent;
    public GameObject choiceButtonPrefab;

    StoryEventData currentEvent;

    void Awake()
    {
        Instance = this;

        eventPanel.SetActive(false);
    }

    public void ShowEvent(StoryEventData data)
    {
        currentEvent = data;

        eventPanel.SetActive(true);

        titleText.text = data.eventTitle;
        descriptionText.text = data.description;

        resultText.text = "";

        ClearChoices();

        foreach (EventChoiceData choice in data.choices)
        {
            GameObject obj =
     Instantiate(
         choiceButtonPrefab,
         choicesParent
     );

            RectTransform rt =
                obj.GetComponent<RectTransform>();

            rt.localScale = Vector3.one;

            rt.sizeDelta =
                new Vector2(500, 80);

            EventChoiceButton button =
                obj.GetComponent<EventChoiceButton>();

            button.Setup(choice);
        }

        Time.timeScale = 0f;
    }

    void ClearChoices()
    {
        foreach (Transform child in choicesParent)
        {
            Destroy(child.gameObject);
        }
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

            resultText.text =
                choice.successMessage;
        }
        else
        {
            GameManager.Instance.RemoveCredits(
                player,
                choice.penaltyCredits
            );

            resultText.text =
                choice.failureMessage;
        }

        Invoke(nameof(CloseEvent), 2f);
    }

    void CloseEvent()
    {
        eventPanel.SetActive(false);

        Time.timeScale = 1f;
    }
}