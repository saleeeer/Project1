using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Empires")]
    public List<EmpireData> empires = new List<EmpireData>();

    [Header("Player")]
    public int playerEmpireIndex;

    [Header("Fleet")]
    public int maxFleetSize = 10;

    [Header("Ships")]
    public ShipType selectedShipType = ShipType.Fighter;

    void Awake()
    {
        Instance = this;

        // 🔥 cargar selección del jugador
        playerEmpireIndex = PlayerPrefs.GetInt("SelectedEmpire", 0);
    }

    // ================= COLOR =================

    public Color GetEmpireColor(int index)
    {
        if (index < 0 || index >= empires.Count)
            return Color.white;

        return empires[index].color;
    }

    // ================= STATS =================

    public EmpireStats GetEmpireTotalStats(int index)
    {
        if (index < 0 || index >= empires.Count)
            return new EmpireStats();

        return empires[index].stats;
    }

    // ================= ECONOMÍA (placeholder) =================

    public int GetPlanetIncome(PlanetData planet)
    {
        return planet.baseIncome;
    }

    public int GetCredits(int empireIndex)
    {
        return 9999; // temporal
    }

    public bool SpendCredits(int empireIndex, int amount)
    {
        return true; // temporal
    }

    // ================= SHIPS =================

    public GameObject fighterPrefab;
    public GameObject bomberPrefab;
    public GameObject commanderPrefab;

    public GameObject GetShipPrefab(ShipType type, bool isPlayer)
    {
        switch (type)
        {
            case ShipType.Fighter: return fighterPrefab;
            case ShipType.Bomber: return bomberPrefab;
            case ShipType.Commander: return commanderPrefab;
        }

        return null;
    }

    public int GetShipCost(ShipType type)
    {
        return 0;
    }

    public bool CanSpawnShip(int empireIndex)
    {
        return true;
    }

    public ShipType GetAIShipType(int empireIndex)
    {
        return ShipType.Fighter;
    }

    // ================= REGISTRO =================

    public void RegisterShip(int empireIndex) { }
    public void UnregisterShip(int empireIndex) { }
}