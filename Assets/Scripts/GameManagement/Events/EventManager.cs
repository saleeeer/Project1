using UnityEngine;
using TMPro;
using System.Collections;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    [Header("UI")]
    public TextMeshProUGUI eventText;

    [Header("Timing")]
    public float minEventTime = 20f;
    public float maxEventTime = 40f;

    [Header("Event Duration")]
    public float eventDuration = 15f;

    [Header("Modifiers")]
    public float globalIncomeMultiplier = 1f;
    public float globalDamageMultiplier = 1f;
    public float globalProductionMultiplier = 1f;

    Coroutine currentRoutine;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(EventLoop());
    }

    IEnumerator EventLoop()
    {
        while (true)
        {
            float wait =
                Random.Range(minEventTime, maxEventTime);

            yield return new WaitForSeconds(wait);

            TriggerRandomEvent();
        }
    }

    void TriggerRandomEvent()
    {
        int random =
            Random.Range(0, 3);

        switch (random)
        {
            case 0:
                StartEvent(
                    "💰 GOLDEN AGE: ingresos x2",
                    2f,
                    1f,
                    1f
                );
                break;

            case 1:
                StartEvent(
                    "⚔️ WAR FRENZY: daño x2",
                    1f,
                    2f,
                    1f
                );
                break;

            case 2:
                StartEvent(
                    "🚀 RAPID PRODUCTION: producción x2",
                    1f,
                    1f,
                    2f
                );
                break;
        }
    }

    void StartEvent(
        string message,
        float income,
        float damage,
        float production
    )
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(
            EventRoutine(
                message,
                income,
                damage,
                production
            )
        );
    }

    IEnumerator EventRoutine(
        string message,
        float income,
        float damage,
        float production
    )
    {
        globalIncomeMultiplier = income;
        globalDamageMultiplier = damage;
        globalProductionMultiplier = production;

        if (eventText != null)
            eventText.text = message;

        Debug.Log("EVENTO: " + message);

        yield return new WaitForSeconds(eventDuration);

        globalIncomeMultiplier = 1f;
        globalDamageMultiplier = 1f;
        globalProductionMultiplier = 1f;

        if (eventText != null)
            eventText.text = "";
    }
}