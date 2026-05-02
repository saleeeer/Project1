using UnityEngine;

public class ShipSelectionUI : MonoBehaviour
{
    public void SelectShip(int typeIndex)
    {
        if (GameManager.Instance == null) return;

        ShipType type = (ShipType)typeIndex;
        GameManager.Instance.selectedShipType = type;

        Debug.Log("Jugador selecciona " + type);
    }
}