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

    GalaxyGenerator galaxy;

    int selectedEmpireIndex;

    void Start()
    {
        selectedEmpireIndex = PlayerPrefs.GetInt("SelectedEmpire", 0);

        StartCoroutine(SpawnShips());
    }

    IEnumerator SpawnShips()
    {
        yield return null;

        galaxy = FindObjectOfType<GalaxyGenerator>();

        if (galaxy == null || galaxy.allPlanets.Count == 0)
        {
            Debug.LogError("No hay planetas!");
            yield break;
        }

        // 🔵 PLAYER
        PlanetData playerPlanet = galaxy.allPlanets[0];
        SpawnPlayerShip(playerPlanet);

        // 🔴 ENEMIGOS (UNO POR IMPERIO)
        for (int i = 0; i < empires.Count; i++)
        {
            if (i == selectedEmpireIndex)
                continue;

            PlanetData aiPlanet = GetRandomPlanetDifferentFrom(playerPlanet);

            if (aiPlanet != null)
            {
                SpawnEnemyShip(aiPlanet, i);
            }
        }
    }

    void SpawnPlayerShip(PlanetData planet)
    {
        // asignar dueño del planeta
        planet.SetOwner(selectedEmpireIndex);

        GameObject ship = Instantiate(playerShipPrefab);

        ShipMovement movement = ship.GetComponent<ShipMovement>();

        if (movement == null)
        {
            Debug.LogError("El prefab del jugador no tiene ShipMovement!");
            return;
        }

        movement.isPlayerControlled = true;
        movement.currentPlanet = planet;
        movement.empireIndex = selectedEmpireIndex;

        ship.transform.position = planet.transform.position;

        ApplyEmpireVisual(ship, selectedEmpireIndex);
    }

    void SpawnEnemyShip(PlanetData planet, int empireIndex)
    {
        // asignar dueño del planeta
        planet.SetOwner(empireIndex);

        GameObject ship = Instantiate(enemyShipPrefab);

        ShipMovement movement = ship.GetComponent<ShipMovement>();

        if (movement == null)
        {
            Debug.LogError("El prefab enemigo no tiene ShipMovement!");
            return;
        }

        movement.isPlayerControlled = false;
        movement.currentPlanet = planet;
        movement.empireIndex = empireIndex;

        if (ship.GetComponent<AIShipController>() == null)
        {
            ship.AddComponent<AIShipController>();
        }

        ship.transform.position = planet.transform.position;

        ApplyEmpireVisual(ship, empireIndex);
    }

    PlanetData GetRandomPlanetDifferentFrom(PlanetData exclude)
    {
        List<PlanetData> planets = galaxy.allPlanets;

        int attempts = 20;

        while (attempts-- > 0)
        {
            PlanetData p = planets[Random.Range(0, planets.Count)];

            if (p != exclude)
                return p;
        }

        return null;
    }

    // 🔥 COLOR POR IMPERIO (usado por planetas y naves)
    public Color GetEmpireColor(int empireIndex)
    {
        if (empireIndex >= 0 && empireIndex < empires.Count)
            return empires[empireIndex].color;

        return Color.white;
    }

    void ApplyEmpireVisual(GameObject ship, int empireIndex)
    {
        if (empireIndex < 0 || empireIndex >= empires.Count)
            return;

        Color color = empires[empireIndex].color;

        SpriteRenderer[] renderers = ship.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sr in renderers)
        {
            sr.color = color;
        }
    }
}