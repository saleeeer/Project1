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
        AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClick);
        if (GameManager.Instance == null)
            return;

        PlanetData selectedPlanet =
            GameManager.Instance.selectedOriginPlanet;

        if (selectedPlanet == null)
        {
            Debug.Log("No hay planeta seleccionado");
            return;
        }

        GameManager.Instance.selectedShipType = type;

        Debug.Log("BOTON PRESIONADO");

        selectedPlanet.SpawnPlayerShip();
    }
}