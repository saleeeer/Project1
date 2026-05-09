using UnityEngine;
using UnityEngine.UI;

public class ShipButtonUI : MonoBehaviour
{
    public ShipType shipType;

    Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    void Update()
    {
        if (GameManager.Instance == null)
            return;

        int playerEmpire =
            GameManager.Instance.playerEmpireIndex;

        int credits =
            GameManager.Instance.GetCredits(playerEmpire);

        int cost =
            GameManager.Instance.GetShipCost(shipType);

        bool canAfford = credits >= cost;

        // límite global
        bool canSpawn =
            GameManager.Instance.CanSpawnShip(playerEmpire);

        button.interactable = canAfford && canSpawn;
    }
}