using UnityEngine;

public class ShipSelectionUI : MonoBehaviour
{
    GameManager gm;

    void Start()
    {
        gm = FindObjectOfType<GameManager>();

        if (gm == null)
        {
            Debug.LogError("❌ No se encontró GameManager");
        }
    }

    public void SelectFighter()
    {
        gm.selectedShipType = ShipType.Fighter;
        Debug.Log("Jugador selecciona FIGHTER");
    }

    public void SelectBomber()
    {
        gm.selectedShipType = ShipType.Bomber;
        Debug.Log("Jugador selecciona BOMBER");
    }

    public void SelectCommander()
    {
        gm.selectedShipType = ShipType.Commander;
        Debug.Log("Jugador selecciona COMMANDER");
    }
}