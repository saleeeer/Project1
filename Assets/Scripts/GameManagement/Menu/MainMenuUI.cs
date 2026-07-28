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

    void Start()
    {
        videoPanel.SetActive(false);

        videoPlayer.loopPointReached += OnVideoFinished;
    }

    public void PlayMenu()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(
                AudioManager.Instance.buttonClick
            );
        }
        PlayVideo(MenuCanvas, selectionCanvas);
    }

    public void PlayOptions()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(
                AudioManager.Instance.buttonClick
            );
        }

        PlayVideo(MenuCanvas, OptionCanvas);
    }

    void PlayVideo(VideoClip clip, GameObject canvasToOpen)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(
                AudioManager.Instance.buttonClick
            );
        }

        nextCanvas = canvasToOpen;

        videoPanel.SetActive(true);

        videoPlayer.Stop();
        videoPlayer.clip = clip;
        videoPlayer.Play();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        CloseVideo();

        if (currentCanvas != null)
            currentCanvas.SetActive(false);

        if (nextCanvas != null)
            nextCanvas.SetActive(true);
    }

    public void CloseVideo()
    {
        videoPlayer.Stop();
        videoPanel.SetActive(false);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(
                AudioManager.Instance.buttonClick
            );
        }
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
}