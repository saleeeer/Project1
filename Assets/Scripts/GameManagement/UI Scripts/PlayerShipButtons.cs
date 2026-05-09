using UnityEngine;

public class PlayerShipButtons : MonoBehaviour
{
    public void SpawnFighter()
    {
        SpawnShip(ShipType.Fighter);
    }

    public void SpawnBomber()
    {
        SpawnShip(ShipType.Bomber);
    }

    public void SpawnCommander()
    {
        SpawnShip(ShipType.Commander);
    }

    void SpawnShip(ShipType type)
    {
        if (GameManager.Instance == null)
            return;

        PlanetData selectedPlanet =
            GameManager.Instance.selectedPlanet;

        if (selectedPlanet == null)
        {
            Debug.Log("No hay planeta seleccionado");
            return;
        }

        // seleccionar tipo
        GameManager.Instance.selectedShipType = type;

        // crear nave
        selectedPlanet.SpawnPlayerShip();

        Debug.Log("Spawn manual de " + type);
    }
}