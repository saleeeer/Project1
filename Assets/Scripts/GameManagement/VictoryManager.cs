
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class VictoryManager : MonoBehaviour
{
    public GameObject victoryPanel;
    public GameObject defeatPanel;

    [Header("Pop Out Animation")]
    public float popDuration = 0.45f;

    bool gameEnded = false;

    void Start()
    {
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            Win();
            return;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            Lose();
            return;
        }

        if (gameEnded)
            return;

        CheckGameState();
    }


    void CheckGameState()
    {
        GameManager gm = GameManager.Instance;

        if (gm == null)
            return;

        int playerEmpire = gm.playerEmpireIndex;

        // Derrota
        if (!gm.IsEmpireAlive(playerEmpire))
        {
            Lose();
            return;
        }

        // Victoria
        bool enemyAlive = false;

        for (int i = 0; i < gm.empires.Count; i++)
        {
            if (i == playerEmpire)
                continue;

            if (gm.IsEmpireAlive(i))
            {
                enemyAlive = true;
                break;
            }
        }

        if (!enemyAlive)
        {
            Win();
        }
    }

    void Win()
    {
        if (gameEnded)
            return;

        gameEnded = true;

        victoryPanel.SetActive(true);
        Time.timeScale = 0f;

        StartCoroutine(PopOut(victoryPanel));
    }

    void Lose()
    {
        if (gameEnded)
            return;

        gameEnded = true;

        defeatPanel.SetActive(true);
        Time.timeScale = 0f;

        StartCoroutine(PopOut(defeatPanel));
    }

    IEnumerator PopOut(GameObject panel)
    {
        RectTransform rect = panel.GetComponent<RectTransform>();

        if (rect == null)
            yield break;

        float elapsed = 0f;
        rect.localScale = Vector3.zero;

        while (elapsed < popDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / popDuration);

            float eased = 1f
                + 2.70158f * Mathf.Pow(t - 1f, 3f)
                + 1.70158f * Mathf.Pow(t - 1f, 2f);

            rect.localScale = Vector3.one * eased;

            yield return null;
        }

        rect.localScale = Vector3.one;
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
