using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialSceneLoader : MonoBehaviour
{
    public string tutorialSceneName = "Tutorial";

    public void LoadTutorial()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(
                AudioManager.Instance.buttonClick
            );
        }

        SceneManager.LoadScene(tutorialSceneName);
    }
}