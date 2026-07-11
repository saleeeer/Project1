using UnityEngine;
using TMPro;

public class EmpireStatsUI : MonoBehaviour
{
    public TextMeshProUGUI powerText;
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI accuracyText;
    public TextMeshProUGUI moraleText;
    public TextMeshProUGUI intelligenceText;

    float timer;

    void Update()
    {
        if (GameManager.Instance == null)
            return;

        timer += Time.deltaTime;

        if (timer < 0.25f)
            return;

        timer = 0f;

        Refresh();
    }

    void Refresh()
    {
        EmpireStats stats =
            GameManager.Instance.GetEmpireTotalStats(
                GameManager.Instance.playerEmpireIndex
            );

        powerText.text = stats.power.ToString("0.0");
        defenseText.text = stats.defense.ToString("0.0");
        accuracyText.text = stats.accuracy.ToString("0.0");
        moraleText.text = stats.morale.ToString("0.0");
        intelligenceText.text = stats.intelligence.ToString("0.0");
    }
}