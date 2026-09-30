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

    [Header("Empire 0 Ship Prefabs")]
    public GameObject empire0FighterPrefab;
    public GameObject empire0BomberPrefab;
    public GameObject empire0CommanderPrefab;

    [Header("Empire 1 Ship Prefabs")]
    public GameObject empire1FighterPrefab;
    public GameObject empire1BomberPrefab;
    public GameObject empire1CommanderPrefab;

    [Header("Costs")]
    public List<ShipCostData> shipCosts = new List<ShipCostData>();

    [Header("RTS Selection")]
    public PlanetData selectedOriginPlanet;

    public List<ShipMovement> allShips = new List<ShipMovement>();

    void Awake()
    {
        Instance = this;

        playerEmpireIndex =
            PlayerPrefs.GetInt("SelectedEmpire", 0);

        InitEmpires();

        InvokeRepeating(
            nameof(GenerateIncome),
            incomeInterval,
            incomeInterval
        );
    }

    void InitEmpires()
    {
        for (int i = 0; i < empires.Count; i++)
        {
            empireShipCount[i] = 0;
            empireCredits[i] = 50;
        }
    }

    // ================= ECONOMÍA =================

    void GenerateIncome()
    {
        PlanetData[] planets =
            FindObjectsOfType<PlanetData>();

        foreach (PlanetData p in planets)
        {
            if (p.ownerEmpireIndex == -1)
                continue;

            int income =
                p.GetIncome();

            empireCredits[p.ownerEmpireIndex] += income;
        }
    }

    public int GetCredits(int empireIndex)
    {
        if (!empireCredits.ContainsKey(empireIndex))
            return 0;

        return empireCredits[empireIndex];
    }

    public bool SpendCredits(
        int empireIndex,
        int amount
    )
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

    public void AddCredits(
        int empire,
        int amount
    )
    {
        if (!empireCredits.ContainsKey(empire))
            return;

        empireCredits[empire] += amount;
    }

    public void RemoveCredits(
        int empire,
        int amount
    )
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

        return empireShipCount[empireIndex] <
               maxShipsPerEmpire;
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

        empireShipCount[empireIndex] =
            Mathf.Max(
                0,
                empireShipCount[empireIndex]
            );
    }

    // ================= PREFABS =================

    public GameObject GetShipPrefab(
        ShipType type,
        int empireIndex
    )
    {
        switch (empireIndex)
        {
            // ================= IMPERIO 0 =================

            case 0:

                switch (type)
                {
                    case ShipType.Fighter:
                        return empire0FighterPrefab;

                    case ShipType.Bomber:
                        return empire0BomberPrefab;

                    case ShipType.Commander:
                        return empire0CommanderPrefab;
                }

                break;

            // ================= IMPERIO 1 =================

            case 1:

                switch (type)
                {
                    case ShipType.Fighter:
                        return empire1FighterPrefab;

                    case ShipType.Bomber:
                        return empire1BomberPrefab;

                    case ShipType.Commander:
                        return empire1CommanderPrefab;
                }

                break;
        }

        Debug.LogError(
            "No existe un prefab para Empire " +
            empireIndex +
            " y ShipType " +
            type
        );

        return null;
    }

    public ShipType GetAIShipType(int empireIndex)
    {
        int r =
            Random.Range(0, 3);

        if (r == 0)
            return ShipType.Fighter;

        if (r == 1)
            return ShipType.Bomber;

        return ShipType.Commander;
    }

    // ================= COLOR =================

    public Color GetEmpireColor(int index)
    {
        if (index < 0 ||
            index >= empires.Count)
        {
            return Color.white;
        }

        return empires[index].color;
    }

    // ================= STATS =================

    public EmpireStats GetEmpireTotalStats(int index)
    {
        if (index < 0 ||
            index >= empires.Count)
        {
            return new EmpireStats();
        }

        EmpireStats total =
            new EmpireStats();

        EmpireStats baseStats =
            empires[index].stats;

        total.power =
            baseStats.power;

        total.defense =
            baseStats.defense;

        total.accuracy =
            baseStats.accuracy;

        total.morale =
            baseStats.morale;

        total.intelligence =
            baseStats.intelligence;

        PlanetData[] planets =
            FindObjectsOfType<PlanetData>();

        foreach (PlanetData planet in planets)
        {
            if (planet.ownerEmpireIndex != index)
                continue;

            total.power +=
                planet.statBuff.power;

            total.defense +=
                planet.statBuff.defense;

            total.accuracy +=
                planet.statBuff.accuracy;

            total.morale +=
                planet.statBuff.morale;

            total.intelligence +=
                planet.statBuff.intelligence;
        }

        return total;
    }

    public bool IsEmpireAlive(int empireIndex)
    {
        PlanetData[] planets =
            FindObjectsOfType<PlanetData>();

        foreach (PlanetData planet in planets)
        {
            if (planet.ownerEmpireIndex ==
                empireIndex)
            {
                return true;
            }
        }

        return false;
    }

    public EmpireStats GetEmpireBaseStats(int index)
    {
        if (index < 0 ||
            index >= empires.Count)
        {
            return null;
        }

        return empires[index].stats;
    }

    // ================= SHIPS =================

    public void RegisterShip(ShipMovement ship)
    {
        if (!allShips.Contains(ship))
            allShips.Add(ship);

        if (!empireShipCount.ContainsKey(
            ship.empireIndex))
        {
            empireShipCount[ship.empireIndex] = 0;
        }

        empireShipCount[ship.empireIndex]++;
    }

    public void UnregisterShip(ShipMovement ship)
    {
        allShips.Remove(ship);

        if (!empireShipCount.ContainsKey(
            ship.empireIndex))
        {
            return;
        }

        empireShipCount[ship.empireIndex]--;

        empireShipCount[ship.empireIndex] =
            Mathf.Max(
                0,
                empireShipCount[ship.empireIndex]
            );
    }
}