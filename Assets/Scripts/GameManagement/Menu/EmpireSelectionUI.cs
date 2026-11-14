
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class EmpireSelectionUI : MonoBehaviour
{
    [Header("Transition")]
    public GameObject videoPanel;
    public VideoPlayer videoPlayer;
    public VideoClip transitionVideo;

    int selectedEmpire;
    bool loadingLevel = false;

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    public void SelectEmpire(int empireIndex)
    {
        if (loadingLevel)
            return;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(
                AudioManager.Instance.buttonClick
            );
        }

        selectedEmpire = empireIndex;

        PlayerPrefs.SetInt("SelectedEmpire", selectedEmpire);
        PlayerPrefs.Save();

        loadingLevel = true;

        videoPanel.SetActive(true);
        videoPlayer.Stop();
        videoPlayer.clip = transitionVideo;
        videoPlayer.Play();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        if (!loadingLevel)
            return;

        loadingLevel = false;
        SceneManager.LoadScene("Level");
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoFinished;
    }
}
