using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class StoryEventManager : MonoBehaviour
{
    public static StoryEventManager Instance;

    [Header("Events")]
    public StoryEventData[] possibleEvents;

    [Header("Random Timing")]
    public float minEventTime = 20f;
    public float maxEventTime = 40f;

    [Header("Event Intro Video")]
    public GameObject eventVideoPanel;
    public VideoPlayer eventVideoPlayer;
    public VideoClip eventIntroVideo;

    bool eventRunning = false;

    StoryEventData pendingEvent;

    void Awake()
    {
        Instance = this;

        if (eventVideoPanel != null)
            eventVideoPanel.SetActive(false);

        if (eventVideoPlayer != null)
        {
            eventVideoPlayer.playOnAwake = false;
            eventVideoPlayer.isLooping = false;

            // El video sigue avanzando aunque el juego esté pausado.
            eventVideoPlayer.timeUpdateMode =
                VideoTimeUpdateMode.UnscaledGameTime;
        }
    }

    void Start()
    {
        if (eventVideoPlayer != null)
        {
            eventVideoPlayer.loopPointReached += OnVideoFinished;
            eventVideoPlayer.prepareCompleted += OnVideoPrepared;
            eventVideoPlayer.errorReceived += OnVideoError;
            eventVideoPlayer.started += OnVideoStarted;
        }

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
                Random.Range(
                    minEventTime,
                    maxEventTime
                );

            yield return new WaitForSeconds(wait);

            if (!eventRunning)
            {
                TriggerRandomEvent();
            }
        }
    }

    // ================= EVENTO =================

    public void TriggerRandomEvent()
    {
        if (possibleEvents == null ||
            possibleEvents.Length == 0)
        {
            return;
        }

        int random =
            Random.Range(
                0,
                possibleEvents.Length
            );

        pendingEvent =
            possibleEvents[random];

        StartEventIntro();
    }

    void StartEventIntro()
    {
        eventRunning = true;

        // Pausar el gameplay.
        Time.timeScale = 0f;

        if (eventVideoPanel == null ||
            eventVideoPlayer == null ||
            eventIntroVideo == null)
        {
            ShowPendingEvent();
            return;
        }

        eventVideoPanel.SetActive(true);

        eventVideoPlayer.Stop();

        eventVideoPlayer.clip =
            eventIntroVideo;

        Debug.Log("Preparando video del evento...");

        eventVideoPlayer.Prepare();
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        if (vp != eventVideoPlayer)
            return;

        Debug.Log("Video preparado. Reproduciendo...");

        eventVideoPlayer.Play();
    }

    void OnVideoStarted(VideoPlayer vp)
    {
        Debug.Log("Video del evento iniciado correctamente.");
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Video del evento terminado.");

        if (eventVideoPanel != null)
            eventVideoPanel.SetActive(false);

        ShowPendingEvent();
    }

    void OnVideoError(VideoPlayer vp, string message)
    {
        Debug.LogError(
            "Error en VideoPlayer del evento: " + message
        );

        if (eventVideoPanel != null)
            eventVideoPanel.SetActive(false);

        ShowPendingEvent();
    }

    // ================= MOSTRAR EVENTO =================

    void ShowPendingEvent()
    {
        if (pendingEvent == null)
        {
            Time.timeScale = 1f;
            eventRunning = false;
            return;
        }

        if (EventUIManager.Instance == null)
        {
            Debug.LogError("No EventUIManager");

            Time.timeScale = 1f;
            eventRunning = false;
            return;
        }

        EventUIManager.Instance.ShowEvent(
            pendingEvent
        );

        StartCoroutine(WaitUntilClosed());
    }

    IEnumerator WaitUntilClosed()
    {
        while (Time.timeScale == 0f)
        {
            yield return null;
        }

        eventRunning = false;
        pendingEvent = null;
    }

    // ================= CLEANUP =================

    void OnDestroy()
    {
        if (eventVideoPlayer != null)
        {
            eventVideoPlayer.loopPointReached -=
                OnVideoFinished;

            eventVideoPlayer.prepareCompleted -=
                OnVideoPrepared;

            eventVideoPlayer.errorReceived -=
                OnVideoError;

            eventVideoPlayer.started -=
                OnVideoStarted;
        }
    }
}