using UnityEngine;
using System.Collections;

public class StoryEventManager : MonoBehaviour
{
    public static StoryEventManager Instance;

    [Header("Events")]
    public StoryEventData[] possibleEvents;

    [Header("Random Timing")]
    public float minEventTime = 20f;
    public float maxEventTime = 40f;

    bool eventRunning = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(EventLoop());
    }

    void Update()
    {
        // TEST MANUAL
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!eventRunning)
            {
                TriggerRandomEvent();
            }
        }
    }

    IEnumerator EventLoop()
    {
        while (true)
        {
            float wait =
                Random.Range(minEventTime, maxEventTime);

            yield return new WaitForSeconds(wait);

            if (!eventRunning)
            {
                TriggerRandomEvent();
            }
        }
    }

    public void TriggerRandomEvent()
    {
        if (possibleEvents == null || possibleEvents.Length == 0)
            return;

        int random =
            Random.Range(0, possibleEvents.Length);

        StoryEventData selected =
            possibleEvents[random];

        ShowEvent(selected);
    }

    void ShowEvent(StoryEventData data)
    {
        if (EventUIManager.Instance == null)
        {
            Debug.LogError("No EventUIManager");
            return;
        }

        eventRunning = true;

        EventUIManager.Instance.ShowEvent(data);

        StartCoroutine(WaitUntilClosed());
    }

    IEnumerator WaitUntilClosed()
    {
        while (Time.timeScale == 0f)
        {
            yield return null;
        }

        eventRunning = false;
    }
}