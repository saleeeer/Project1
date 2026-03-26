using UnityEngine;
using UnityEngine.SceneManagement;

public class EmpireSelectionUI : MonoBehaviour
{
    public void SelectEmpire(int empireIndex)
    {
        PlayerPrefs.SetInt("SelectedEmpire", empireIndex);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Level");
    }
}