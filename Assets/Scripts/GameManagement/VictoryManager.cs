using UnityEngine;

public class VictoryManager : MonoBehaviour
{
    bool gameEnded = false;

    void Update()
    {
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
        gameEnded = true;

        Debug.Log("¡Victoria!");

        Time.timeScale = 0f;
    }

    void Lose()
    {
        gameEnded = true;

        Debug.Log("¡Derrota!");

        Time.timeScale = 0f;
    }
}