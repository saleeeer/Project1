using UnityEngine;

public class PlayerSpawnUI : MonoBehaviour
{
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

        selected.SpawnPlayerShip();
    }
}