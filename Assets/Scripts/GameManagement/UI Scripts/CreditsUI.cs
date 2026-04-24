using UnityEngine;
using TMPro;

public class CreditsUI : MonoBehaviour
{
    public TextMeshProUGUI creditsText;

    GameManager gm;
    int playerEmpire;

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        playerEmpire = PlayerPrefs.GetInt("SelectedEmpire", 0);
    }

    void Update()
    {
        if (gm == null) return;

        int credits = gm.GetCredits(playerEmpire);

        creditsText.text = "💰 Credits: " + credits;
    }
}