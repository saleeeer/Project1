using UnityEngine;
using TMPro;

public class CreditsUI : MonoBehaviour
{
    public TextMeshProUGUI creditsText;

    int playerEmpire;
    float timer;

    void Start()
    {
        if (GameManager.Instance == null) return;

        playerEmpire = GameManager.Instance.playerEmpireIndex;
        Refresh();
    }

    void Update()
    {
        if (GameManager.Instance == null) return;

        timer += Time.deltaTime;

        if (timer < 0.25f) return;

        timer = 0f;
        Refresh();
    }

    void Refresh()
    {
        int credits = GameManager.Instance.GetCredits(playerEmpire);
        creditsText.text = "💰 Credits: " + credits;
    }
}