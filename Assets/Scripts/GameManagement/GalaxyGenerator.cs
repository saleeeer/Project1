using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalaxyGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject planetPrefab;

    [Header("Map Settings")]
    public int gridSize = 5;
    public float cellSize = 8f;
    public float spawnChance = 0.7f;
    public float randomOffset = 2f;

    [Header("Planet Distance")]
    public float minPlanetDistance = 3f;

    [Header("Connections")]
    public float connectionDistance = 12f;

    [Header("Planet List (For AI)")]
    public List<PlanetData> allPlanets = new List<PlanetData>();

    List<Vector3> planetPositions = new List<Vector3>();

    void Start()
    {
        GenerateGalaxy();
    }

    void GenerateGalaxy()
    {
        planetPositions.Clear();
        allPlanets.Clear();

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector3 basePosition = new Vector3(x * cellSize, y * cellSize, 0);

                if (Random.value > spawnChance)
                    continue;

                Vector3 offset = new Vector3(
                    Random.Range(-randomOffset, randomOffset),
                    Random.Range(-randomOffset, randomOffset),
                    0
                );

                Vector3 finalPosition = basePosition + offset;

                if (IsTooClose(finalPosition))
                    continue;

                SpawnPlanet(finalPosition);
            }
        }

        ConnectPlanets();
    }

    void SpawnPlanet(Vector3 position)
    {
        GameObject planet = Instantiate(planetPrefab, position, Quaternion.identity);

        PlanetData data = planet.GetComponent<PlanetData>();

        if (data == null)
            data = planet.AddComponent<PlanetData>();

        data.SetOwner(-1);

        allPlanets.Add(data);
        planetPositions.Add(position);
    }

    bool IsTooClose(Vector3 position)
    {
        foreach (Vector3 other in planetPositions)
        {
            if (Vector3.Distance(position, other) < minPlanetDistance)
                return true;
        }

        return false;
    }

    void ConnectPlanets()
    {
        int maxConnections = 3;

        foreach (PlanetData a in allPlanets)
        {
            // Lista de planetas ordenados por distancia
            List<PlanetData> sorted = new List<PlanetData>(allPlanets);

            sorted.Sort((b, c) =>
            {
                float distB = Vector2.Distance(a.transform.position, b.transform.position);
                float distC = Vector2.Distance(a.transform.position, c.transform.position);
                return distB.CompareTo(distC);
            });

            int connections = 0;

            foreach (PlanetData b in sorted)
            {
                if (a == b) continue;

                if (!a.neighbors.Contains(b))
                {
                    a.neighbors.Add(b);

                    if (!b.neighbors.Contains(a))
                        b.neighbors.Add(a);

                    connections++;

                    if (connections >= maxConnections)
                        break;
                }
            }
        }
    }

    // ================= GIZMOS =================

    void OnDrawGizmos()
    {
        DrawGrid();
        DrawConnections();
        DrawPlanets();
    }

    void DrawGrid()
    {
        Gizmos.color = Color.gray;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector3 pos = new Vector3(x * cellSize, y * cellSize, 0);
                Gizmos.DrawWireCube(pos, Vector3.one * cellSize);
            }
        }
    }

    void DrawConnections()
    {
        if (allPlanets == null) return;

        Gizmos.color = Color.yellow;

        foreach (PlanetData planet in allPlanets)
        {
            if (planet == null) continue;

            foreach (PlanetData neighbor in planet.neighbors)
            {
                if (neighbor == null) continue;

                Gizmos.DrawLine(
                    planet.transform.position,
                    neighbor.transform.position
                );
            }
        }
    }

    void DrawPlanets()
    {
        if (allPlanets == null) return;

        Gizmos.color = Color.cyan;

        foreach (PlanetData planet in allPlanets)
        {
            if (planet != null)
            {
                Gizmos.DrawSphere(planet.transform.position, 0.3f);
            }
        }
    }
}

