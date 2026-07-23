using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

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
            rt.sizeDelta = new Vector2(500, 80);

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

        int player = GameManager.Instance.playerEmpireIndex;

        // Estadísticas BASE (para guardar cambios permanentes)
        EmpireStats baseStats =
            GameManager.Instance.GetEmpireBaseStats(player);

        // Estadísticas TOTALES (base + buffs de planetas)
        EmpireStats totalStats =
            GameManager.Instance.GetEmpireTotalStats(player);

        bool success =
            totalStats.power >= choice.requiredPower &&
            totalStats.defense >= choice.requiredDefense &&
            totalStats.intelligence >= choice.requiredIntelligence &&
            totalStats.morale >= choice.requiredMorale;

        if (success)
        {
            GameManager.Instance.AddCredits(
                player,
                choice.rewardCredits
            );

            baseStats.power += choice.rewardPower;
            baseStats.defense += choice.rewardDefense;
            baseStats.accuracy += choice.rewardAccuracy;
            baseStats.morale += choice.rewardMorale;
            baseStats.intelligence += choice.rewardIntelligence;

            resultText.text = choice.successMessage;
        }
        else
        {
            GameManager.Instance.RemoveCredits(
                player,
                choice.penaltyCredits
            );

            baseStats.power -= choice.penaltyPower;
            baseStats.defense -= choice.penaltyDefense;
            baseStats.accuracy -= choice.penaltyAccuracy;
            baseStats.morale -= choice.penaltyMorale;
            baseStats.intelligence -= choice.penaltyIntelligence;

            resultText.text = choice.failureMessage;
        }

        StartCoroutine(CloseAfterDelay());
    }

    IEnumerator CloseAfterDelay()
    {
        yield return new WaitForSecondsRealtime(2f);

        CloseEvent();
    }

    void CloseEvent()
    {
        eventPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void HideEvent()
    {
        gameObject.SetActive(false);
    }
}