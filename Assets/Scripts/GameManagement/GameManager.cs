using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player")]
    public int playerEmpireIndex;

    [Header("Economy")]
    public int startingCredits = 100;

    Dictionary<int, int> empireCredits = new Dictionary<int, int>();

    [Header("Ships")]
    public ShipType selectedShipType;
    
    [Header("Fleet")]
    public int maxFleetSize = 10;

    // ================= INIT =================

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        playerEmpireIndex = PlayerPrefs.GetInt("SelectedEmpire", 0);
    }

    void Start()
    {
        InitializeEconomy();
    }

    void InitializeEconomy()
    {
        empireCredits.Clear();

        foreach (EmpireType empire in System.Enum.GetValues(typeof(EmpireType)))
        {
            empireCredits[(int)empire] = startingCredits;
        }
    }

    // ================= ECONOMY =================

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

    public void AddCredits(int empireIndex, int amount)
    {
        if (!empireCredits.ContainsKey(empireIndex))
            empireCredits[empireIndex] = 0;

        empireCredits[empireIndex] += amount;
    }

    // ================= PLACEHOLDERS (para no romper nada) =================

    public Color GetEmpireColor(int index)
    {
        switch (index)
        {
            case 0: return Color.blue;
            case 1: return Color.red;
            case 2: return Color.green;
            case 3: return Color.yellow;
        }

        return Color.white;
    }

    public int GetShipCost(ShipType type)
    {
        return 1; // no rompe nada por ahora
    }

    public bool CanSpawnShip(int empireIndex)
    {
        return true;
    }

    public GameObject GetShipPrefab(ShipType type, bool isPlayer)
    {
        return null;
    }

    public ShipType GetAIShipType(int empireIndex)
    {
        return ShipType.Fighter;
    }

    public void RegisterShip(int empireIndex)
    {
        if (!empireShipCount.ContainsKey(empireIndex))
            empireShipCount[empireIndex] = 0;

        empireShipCount[empireIndex]++;
    }

    public EmpireStats GetEmpireTotalStats(int empireIndex)
    {
        return new EmpireStats();
    }

    public int GetPlanetIncome(PlanetData planet)
    {
        return planet.baseIncome;
    }


    Dictionary<int, int> empireShipCount = new Dictionary<int, int>();

    public void UnregisterShip(int empireIndex)
    {
        if (!empireShipCount.ContainsKey(empireIndex))
            return;

        empireShipCount[empireIndex]--;

        if (empireShipCount[empireIndex] < 0)
            empireShipCount[empireIndex] = 0;
    }

    public int GetShipCount(int empireIndex)
    {
        if (!empireShipCount.ContainsKey(empireIndex))
            return 0;

        return empireShipCount[empireIndex];
    }
}