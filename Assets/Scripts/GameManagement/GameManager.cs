using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Ship Prefabs")]
    public List<ShipPrefabData> playerShips;
    public List<ShipPrefabData> enemyShips;

    [Header("Empires")]
    public List<EmpireData> empires = new List<EmpireData>();

    [Header("Limits")]
    public int maxShipsPerEmpire = 20;
    public int maxFleetSize = 5;

    [Header("Ship Costs")]
    public List<ShipCostData> shipCosts = new List<ShipCostData>();

    [Header("Planet Income")]
    public List<PlanetIncomeData> planetIncomes = new List<PlanetIncomeData>();

    [Range(0f, 1f)]
    public float neutralIncomeMultiplier = 0.5f;

    Dictionary<int, int> empireShipCount = new Dictionary<int, int>();
    Dictionary<int, int> empireCredits = new Dictionary<int, int>();

    int selectedEmpireIndex;

    [Header("Economy")]
    public float incomeInterval = 2f;

    [Header("Player Control")]
    public ShipType selectedShipType = ShipType.Fighter;

    void Awake()
    {
        selectedEmpireIndex = PlayerPrefs.GetInt("SelectedEmpire", 0);

        for (int i = 0; i < empires.Count; i++)
        {
            empireShipCount[i] = 0;
            empireCredits[i] = 0;
        }

        foreach (var sc in shipCosts)
        {
            Debug.Log($"Coste {sc.shipType}: {sc.cost}");
        }
    }

    void Start()
    {
        StartCoroutine(AssignStartingPlanets());
        StartCoroutine(EconomyRoutine());
    }

    IEnumerator AssignStartingPlanets()
    {
        yield return null;

        GalaxyGenerator galaxy = FindObjectOfType<GalaxyGenerator>();

        if (galaxy == null || galaxy.allPlanets.Count == 0)
        {
            Debug.LogError("No hay planetas!");
            yield break;
        }

        PlanetData playerPlanet = galaxy.allPlanets[0];
        playerPlanet.SetOwner(selectedEmpireIndex);

        for (int i = 0; i < empires.Count; i++)
        {
            if (i == selectedEmpireIndex) continue;

            PlanetData aiPlanet = GetRandomPlanetDifferentFrom(playerPlanet, galaxy.allPlanets);

            if (aiPlanet != null)
            {
                aiPlanet.SetOwner(i);
            }
        }
    }

    PlanetData GetRandomPlanetDifferentFrom(PlanetData exclude, List<PlanetData> all)
    {
        int attempts = 20;

        while (attempts-- > 0)
        {
            PlanetData p = all[Random.Range(0, all.Count)];

            if (p != exclude && p.ownerEmpireIndex == -1)
                return p;
        }

        return null;
    }

    // ================= ECONOMÍA =================

    IEnumerator EconomyRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(incomeInterval);
            GenerateIncome();
        }
    }

    void GenerateIncome()
    {
        PlanetData[] planets = FindObjectsOfType<PlanetData>();

        for (int i = 0; i < empires.Count; i++)
        {
            int totalIncome = 0;

            foreach (PlanetData p in planets)
            {
                if (p.ownerEmpireIndex == i)
                {
                    int income = GetPlanetIncome(p);
                    totalIncome += income;
                }
            }

            empireCredits[i] += totalIncome;

            Debug.Log($"💰 Imperio {i} gana {totalIncome}. Total: {empireCredits[i]}");
        }
    }

    public int GetPlanetIncome(PlanetData planet)
    {
        int baseIncome = 1;

        foreach (var pi in planetIncomes)
        {
            if (pi.planetType == planet.planetType)
            {
                baseIncome = pi.income;
                break;
            }
        }

        if (planet.ownerEmpireIndex == -1)
        {
            baseIncome = Mathf.RoundToInt(baseIncome * neutralIncomeMultiplier);
        }

        return baseIncome;
    }

    public bool SpendCredits(int empireIndex, int amount)
    {
        if (!empireCredits.ContainsKey(empireIndex)) return false;

        if (empireCredits[empireIndex] < amount)
            return false;

        empireCredits[empireIndex] -= amount;
        return true;
    }

    public int GetCredits(int empireIndex)
    {
        if (!empireCredits.ContainsKey(empireIndex))
            return 0;

        return empireCredits[empireIndex];
    }

    // ================= SHIPS =================

    public int GetShipCost(ShipType type)
    {
        foreach (var sc in shipCosts)
        {
            if (sc.shipType == type)
                return sc.cost;
        }

        Debug.LogWarning("No cost definido para " + type);
        return 1;
    }

    public GameObject GetShipPrefab(ShipType type, bool isPlayer)
    {
        List<ShipPrefabData> list = isPlayer ? playerShips : enemyShips;

        foreach (var s in list)
        {
            if (s.shipType == type)
                return s.prefab;
        }

        Debug.LogError("No prefab para " + type);
        return null;
    }

    public ShipType GetAIShipType(int empireIndex)
    {
        int credits = GetCredits(empireIndex);

        if (credits > 15)
            return ShipType.Commander;

        if (credits > 8)
            return ShipType.Bomber;

        return ShipType.Fighter;
    }

    // ================= LIMITES =================

    public bool CanSpawnShip(int empireIndex)
    {
        if (!empireShipCount.ContainsKey(empireIndex))
            empireShipCount[empireIndex] = 0;

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
    }

    // ================= STATS =================

    public EmpireStats GetEmpireTotalStats(int empireIndex)
    {
        EmpireStats baseStats = empires[empireIndex].stats;

        EmpireStats total = new EmpireStats();

        total.power = baseStats.power;
        total.defense = baseStats.defense;
        total.accuracy = baseStats.accuracy;
        total.morale = baseStats.morale;
        total.intelligence = baseStats.intelligence;

        PlanetData[] planets = FindObjectsOfType<PlanetData>();

        foreach (PlanetData p in planets)
        {
            if (p.ownerEmpireIndex != empireIndex) continue;

            total.power += p.statBuff.power;
            total.defense += p.statBuff.defense;
            total.accuracy += p.statBuff.accuracy;
            total.morale += p.statBuff.morale;
            total.intelligence += p.statBuff.intelligence;
        }

        return total;
    }

    public Color GetEmpireColor(int empireIndex)
    {
        if (empireIndex >= 0 && empireIndex < empires.Count)
            return empires[empireIndex].color;

        return Color.white;
    }
}