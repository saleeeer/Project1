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

    void Start()
    {
        //if (videoPanel != null)
            //videoPanel.SetActive(false);

        videoPlayer.loopPointReached += OnVideoFinished;
    }

    public void SelectEmpire(int empireIndex)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(
                AudioManager.Instance.buttonClick
            );
        }

        selectedEmpire = empireIndex;

        PlayerPrefs.SetInt("SelectedEmpire", empireIndex);
        PlayerPrefs.Save();

        videoPanel.SetActive(true);

        //videoPlayer.Stop();
        videoPlayer.clip = transitionVideo;
        videoPlayer.Play();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene("Level");
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoFinished;
    }
}