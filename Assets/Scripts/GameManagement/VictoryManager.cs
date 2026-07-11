using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour
{
    public GameObject victoryPanel;
    public GameObject defeatPanel;

    bool gameEnded = false;

    void Start()
    {
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
    }

    void Update()
    {
        if (gameEnded)
            return;

        // ================= TEST =================

#if UNITY_EDITOR
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
#endif

        // ================= NORMAL =================

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
        gameEnded = true;

        victoryPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    void Lose()
    {
        gameEnded = true;

        defeatPanel.SetActive(true);

        Time.timeScale = 0f;
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