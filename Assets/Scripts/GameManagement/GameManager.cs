using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Prefabs")]
    public GameObject playerShipPrefab;
    public GameObject enemyShipPrefab;

    [Header("Empires")]
    public List<EmpireData> empires = new List<EmpireData>();

    public GalaxyGenerator galaxy;

    int selectedEmpireIndex;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        selectedEmpireIndex = PlayerPrefs.GetInt("SelectedEmpire", 0);
        StartCoroutine(SpawnShips());
    }

    IEnumerator SpawnShips()
    {
        yield return null;

        if (galaxy == null || galaxy.allPlanets.Count == 0)
        {
            Debug.LogError("No hay planetas!");
            yield break;
        }

        PlanetData playerPlanet = galaxy.allPlanets[0];
        SpawnPlayerShip(playerPlanet);

        for (int i = 0; i < empires.Count; i++)
        {
            if (i == selectedEmpireIndex) continue;

            PlanetData aiPlanet = GetRandomPlanetDifferentFrom(playerPlanet);

            if (aiPlanet != null)
                SpawnEnemyShip(aiPlanet, i);
        }
    }

    public Color GetEmpireColor(int index)
    {
        if (index >= 0 && index < empires.Count)
            return empires[index].color;

        return Color.white;
    }

    void SpawnPlayerShip(PlanetData planet)
    {
        planet.SetOwner(selectedEmpireIndex);

        GameObject ship = Instantiate(playerShipPrefab, planet.transform.position, Quaternion.identity);

        ShipMovement m = ship.GetComponent<ShipMovement>();
        m.isPlayerControlled = true;
        m.currentPlanet = planet;
        m.empireIndex = selectedEmpireIndex;
        ApplyEmpireVisual(ship, selectedEmpireIndex);
    }

    void SpawnEnemyShip(PlanetData planet, int empireIndex)
    {
        planet.SetOwner(empireIndex);

        GameObject ship = Instantiate(enemyShipPrefab, planet.transform.position, Quaternion.identity);

        ShipMovement m = ship.GetComponent<ShipMovement>();
        m.isPlayerControlled = false;
        m.currentPlanet = planet;
        m.empireIndex = empireIndex;

        if (!ship.GetComponent<AIShipController>())
            ship.AddComponent<AIShipController>();
        ApplyEmpireVisual(ship, empireIndex);
    }

    PlanetData GetRandomPlanetDifferentFrom(PlanetData exclude)
    {
        List<PlanetData> planets = galaxy.allPlanets;

        for (int i = 0; i < 20; i++)
        {
            PlanetData p = planets[Random.Range(0, planets.Count)];
            if (p != exclude) return p;
        }

        return null;
    }

    void ApplyEmpireVisual(GameObject ship, int empireIndex)
    {
        Color color = empires[empireIndex].color;

        SpriteRenderer[] renderers = ship.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sr in renderers)
        {
            sr.color = color;
        }
    }
}