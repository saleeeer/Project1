using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
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