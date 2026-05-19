using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Empires")]
    public List<EmpireData> empires = new List<EmpireData>();

    [Header("Player")]
    public int playerEmpireIndex;

    [Header("Selection")]
    public PlanetData selectedPlanet;

    [Header("Fleet")]
    public int maxFleetSize = 10;

    [Header("Fleet Limit")]
    public int maxShipsPerEmpire = 50;

    Dictionary<int, int> empireShipCount = new Dictionary<int, int>();
    Dictionary<int, int> empireCredits = new Dictionary<int, int>();

    [Header("Economy")]
    public float incomeInterval = 2f;

    [Header("Ships")]
    public ShipType selectedShipType = ShipType.Fighter;

    [Header("Prefabs")]
    public GameObject fighterPrefab;
    public GameObject bomberPrefab;
    public GameObject commanderPrefab;

    [Header("Costs")]
    public List<ShipCostData> shipCosts = new List<ShipCostData>();

    [Header("RTS Selection")]
    public PlanetData selectedOriginPlanet;

    void Awake()
    {
        Instance = this;

        playerEmpireIndex = PlayerPrefs.GetInt("SelectedEmpire", 0);

        InitEmpires();
        InvokeRepeating(nameof(GenerateIncome), incomeInterval, incomeInterval);
    }

    void InitEmpires()
    {
        for (int i = 0; i < empires.Count; i++)
        {
            empireShipCount[i] = 0;
            empireCredits[i] = 50; // старт inicial
        }
    }

    // ================= ECONOMÍA =================

    void GenerateIncome()
    {
        PlanetData[] planets = FindObjectsOfType<PlanetData>();

        foreach (PlanetData p in planets)
        {
            if (p.ownerEmpireIndex == -1) continue;

            int income = p.GetIncome();

            if (EventManager.Instance != null)
            {
                income = Mathf.RoundToInt(
                    income *
                    EventManager.Instance.globalIncomeMultiplier
                );
            }

            empireCredits[p.ownerEmpireIndex] += income;
        }
    }

    public int GetCredits(int empireIndex)
    {
        if (!empireCredits.ContainsKey(empireIndex))
            return 0;

        return empireCredits[empireIndex];
    }

    public bool SpendCredits(int empireIndex, int amount)
    {
        if (!empireCredits.ContainsKey(empireIndex))
            return false;

        if (empireCredits[empireIndex] < amount)
            return false;

        empireCredits[empireIndex] -= amount;
        return true;
    }

    public int GetPlanetIncome(PlanetData planet)
    {
        return planet.baseIncome;
    }

    // ================= COSTOS =================

    public int GetShipCost(ShipType type)
    {
        foreach (var data in shipCosts)
        {
            if (data.shipType == type)
                return data.cost;
        }

        return 0;
    }

    public void AddCredits(int empire, int amount)
    {
        if (!empireCredits.ContainsKey(empire))
            return;

        empireCredits[empire] += amount;
    }

    public void RemoveCredits(int empire, int amount)
    {
        if (!empireCredits.ContainsKey(empire))
            return;

        empireCredits[empire] -= amount;

        if (empireCredits[empire] < 0)
            empireCredits[empire] = 0;
    }

    // ================= SPAWN CONTROL =================

    public bool CanSpawnShip(int empireIndex)
    {
        if (!empireShipCount.ContainsKey(empireIndex))
            return false;

        return empireShipCount[empireIndex] < maxShipsPerEmpire;
    }

    public void RegisterShip(int empireIndex)
    {
        if (!empireShipCount.ContainsKey(empireIndex))
            empireShipCount[empireIndex] = 0;

        empireShipCount[empireIndex]++;
    }

    public void UnregisterShip(int empireIndex)
    {
        if (!empireShipCount.ContainsKey(empireIndex))
            return;

        empireShipCount[empireIndex]--;
        empireShipCount[empireIndex] = Mathf.Max(0, empireShipCount[empireIndex]);
    }

    // ================= PREFABS =================

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

    public ShipType GetAIShipType(int empireIndex)
    {
        // IA simple: mezcla
        int r = Random.Range(0, 3);

        if (r == 0) return ShipType.Fighter;
        if (r == 1) return ShipType.Bomber;

        return ShipType.Commander;
    }

    // ================= COLOR =================

    public Color GetEmpireColor(int index)
    {
        if (index < 0 || index >= empires.Count)
            return Color.white;

        return empires[index].color;
    }

    public EmpireStats GetEmpireTotalStats(int index)
    {
        if (index < 0 || index >= empires.Count)
            return new EmpireStats();

        return empires[index].stats;
    }
}