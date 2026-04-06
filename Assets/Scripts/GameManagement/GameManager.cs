using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject playerShipPrefab;
    public GameObject enemyShipPrefab;

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

    // 🔥 ASIGNAR PLANETAS A IMPERIOS
    IEnumerator AssignStartingPlanets()
    {
        yield return null;

        GalaxyGenerator galaxy = FindObjectOfType<GalaxyGenerator>();

        if (galaxy == null || galaxy.allPlanets.Count == 0)
        {
            Debug.LogError("No hay planetas!");
            yield break;
        }

        // 🔵 PLANETA DEL JUGADOR
        PlanetData playerPlanet = galaxy.allPlanets[0];
        playerPlanet.SetOwner(selectedEmpireIndex);

        // 🔴 PLANETAS DE IA
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

    // ================= COLOR =================

    public Color GetEmpireColor(int empireIndex)
    {
        if (empireIndex >= 0 && empireIndex < empires.Count)
            return empires[empireIndex].color;

        return Color.white;
    }
}