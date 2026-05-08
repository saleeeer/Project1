using UnityEngine;

public class PlayerSpawnUI : MonoBehaviour
{
    public PlanetData manualTarget;

    public void SpawnFleet()
    {
        if (GameManager.Instance == null)
            return;

        PlanetData selected =
            GameManager.Instance.selectedPlanet;

        if (selected == null)
        {
            Debug.Log("No hay planeta seleccionado");
            return;
        }

        if (manualTarget == null)
        {
            Debug.Log("No hay objetivo");
            return;
        }

        selected.SendFleet(manualTarget);
    }
}