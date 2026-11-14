
using UnityEngine;
using UnityEngine.Video;

public class MainMenuUI : MonoBehaviour
{
    [Header("Video")]
    public GameObject videoPanel;
    public VideoPlayer videoPlayer;

    [Header("Video Clips")]
    public VideoClip MenuCanvas;
    public VideoClip SelectionCanvas_1;
    public VideoClip SelectionCanvas_2;

    [Header("Canvas")]
    public GameObject currentCanvas;
    public GameObject selectionCanvas;
    public GameObject OptionCanvas;

    GameObject nextCanvas;

    bool isTransitioning = false;

    void Start()
    {
        videoPanel.SetActive(false);
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    public void PlayMenu()
    {
        PlayVideo(SelectionCanvas_1, selectionCanvas);
    }

    public void PlayOptions()
    {
        PlayVideo(MenuCanvas, OptionCanvas);
    }

    void PlayVideo(VideoClip clip, GameObject canvasToOpen)
    {
        if (isTransitioning)
            return;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(
                AudioManager.Instance.buttonClick
            );
        }

        isTransitioning = true;
        nextCanvas = canvasToOpen;

        videoPanel.SetActive(true);

        videoPlayer.Stop();
        videoPlayer.clip = clip;
        videoPlayer.Play();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        if (currentCanvas != null)
            currentCanvas.SetActive(false);

        if (nextCanvas != null)
        {
            nextCanvas.SetActive(true);
            currentCanvas = nextCanvas;
        }

        videoPanel.SetActive(false);
        nextCanvas = null;
        isTransitioning = false;
    }

    public void ExitGame()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(
                AudioManager.Instance.buttonClick
            );
        }

        Debug.Log("Cerrando juego...");
        Application.Quit();
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoFinished;
    }
}
