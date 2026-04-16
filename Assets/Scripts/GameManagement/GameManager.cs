using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Ship Prefabs")]
    public List<GameObject> playerShipPrefabs;
    public List<GameObject> enemyShipPrefabs;

    [Header("Empires")]
    public List<EmpireData> empires = new List<EmpireData>();

    [Header("Limits")]
    public int maxShipsPerEmpire = 20;
    public int maxFleetSize = 5;

    Dictionary<int, int> empireShipCount = new Dictionary<int, int>();

    int selectedEmpireIndex;

    void Awake()
    {
        selectedEmpireIndex = PlayerPrefs.GetInt("SelectedEmpire", 0);

        for (int i = 0; i < empires.Count; i++)
        {
            empireShipCount[i] = 0;
        }
    }

    void Start()
    {
        StartCoroutine(AssignStartingPlanets());
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

        Debug.Log("Jugador recibe planeta: " + playerPlanet.name);

        for (int i = 0; i < empires.Count; i++)
        {
            if (i == selectedEmpireIndex) continue;

            PlanetData aiPlanet = GetRandomPlanetDifferentFrom(playerPlanet, galaxy.allPlanets);

            if (aiPlanet != null)
            {
                aiPlanet.SetOwner(i);
                Debug.Log("IA " + i + " recibe planeta: " + aiPlanet.name);
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