using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject playerShipPrefab;
    public GameObject enemyShipPrefab;

    [Header("Spawn Settings")]
    public int aiShipCount = 1;

    GalaxyGenerator galaxy;

    int selectedEmpire;

    void Start()
    {
        selectedEmpire = PlayerPrefs.GetInt("SelectedEmpire", 0);

        StartCoroutine(SpawnShips());
    }

    IEnumerator SpawnShips()
    {
        yield return null;

        galaxy = FindObjectOfType<GalaxyGenerator>();

        if (galaxy == null || galaxy.allPlanets.Count == 0)
        {
            Debug.LogError("No hay planetas para spawnear naves!");
            yield break;
        }

        // 🔵 PLAYER
        PlanetData playerPlanet = galaxy.allPlanets[0];
        SpawnPlayerShip(playerPlanet);

        // 🔴 ENEMIGOS
        for (int i = 0; i < aiShipCount; i++)
        {
            PlanetData aiPlanet = GetRandomPlanetDifferentFrom(playerPlanet);

            if (aiPlanet != null)
            {
                SpawnEnemyShip(aiPlanet);
            }
        }
    }

    void SpawnPlayerShip(PlanetData planet)
    {
        planet.SetOwner(Faction.Player);

        GameObject ship = Instantiate(playerShipPrefab);

        ShipMovement movement = ship.GetComponent<ShipMovement>();

        if (movement == null)
        {
            Debug.LogError("El prefab del jugador no tiene ShipMovement!");
            return;
        }

        movement.isPlayerControlled = true;
        movement.currentPlanet = planet;

        ship.transform.position = planet.transform.position;

        ApplyEmpireVisual(ship, true);
    }

    void SpawnEnemyShip(PlanetData planet)
    {
        planet.SetOwner(Faction.Enemy);

        GameObject ship = Instantiate(enemyShipPrefab);

        ShipMovement movement = ship.GetComponent<ShipMovement>();

        if (movement == null)
        {
            Debug.LogError("El prefab enemigo no tiene ShipMovement!");
            return;
        }

        movement.isPlayerControlled = false;
        movement.currentPlanet = planet;

        if (ship.GetComponent<AIShipController>() == null)
        {
            ship.AddComponent<AIShipController>();
        }

        ship.transform.position = planet.transform.position;

        ApplyEmpireVisual(ship, false);
    }

    PlanetData GetRandomPlanetDifferentFrom(PlanetData exclude)
    {
        List<PlanetData> planets = galaxy.allPlanets;

        int attempts = 20;

        while (attempts > 0)
        {
            PlanetData p = planets[Random.Range(0, planets.Count)];

            if (p != exclude)
                return p;

            attempts--;
        }

        return null;
    }

    // 🔵 PLAYER COLOR
    public Color GetPlayerColor()
    {
        switch (selectedEmpire)
        {
            case 0: return Color.blue;
            case 1: return Color.red;
            case 2: return Color.green;
            case 3: return Color.yellow;
        }

        return Color.blue;
    }

    // 🔴 ENEMY COLOR DINÁMICO
    public Color GetEnemyColor()
    {
        // si jugador es rojo → enemigo azul
        if (GetPlayerColor() == Color.red)
            return Color.blue;

        // si jugador es azul → enemigo rojo
        if (GetPlayerColor() == Color.blue)
            return Color.red;

        // fallback (para otros colores)
        return Color.red;
    }

    void ApplyEmpireVisual(GameObject ship, bool isPlayer)
    {
        Color color = isPlayer ? GetPlayerColor() : GetEnemyColor();

        SpriteRenderer[] renderers = ship.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sr in renderers)
        {
            sr.color = color;
        }
    }
}