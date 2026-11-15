
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

        // Evitar suscripciones duplicadas.
        videoPlayer.loopPointReached -= OnVideoFinished;

        // Escuchar únicamente la transición de selección.
        videoPlayer.loopPointReached += OnVideoFinished;

        videoPanel.SetActive(true);

        videoPlayer.Stop();
        videoPlayer.clip = transitionVideo;
        videoPlayer.Play();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        // Dejar de escuchar antes de cambiar de escena.
        videoPlayer.loopPointReached -= OnVideoFinished;

        SceneManager.LoadScene("Level");
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }
}
