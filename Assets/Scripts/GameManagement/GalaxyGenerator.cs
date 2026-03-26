using UnityEngine;
using System.Collections.Generic;

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

        int center = gridSize / 2;

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

                PlanetType randomType = GetRandomPlanetType();

                SpawnPlanet(finalPosition, randomType);
            }
        }

        ConnectPlanets();
    }

    void SpawnPlanet(Vector3 position, PlanetType type)
    {
        GameObject planet = Instantiate(planetPrefab, position, Quaternion.identity);

        planetPositions.Add(position);

        PlanetData data = planet.GetComponent<PlanetData>();

        if (data == null)
            data = planet.AddComponent<PlanetData>();

        data.planetType = type;

        // 🔥 TODOS empiezan neutrales
        data.SetOwner(Faction.Neutral);

        allPlanets.Add(data);
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
        foreach (PlanetData a in allPlanets)
        {
            foreach (PlanetData b in allPlanets)
            {
                if (a == b) continue;

                float dist = Vector2.Distance(a.transform.position, b.transform.position);

                if (dist < connectionDistance)
                {
                    if (!IsPathBlocked(a.transform.position, b.transform.position))
                    {
                        if (!a.neighbors.Contains(b))
                            a.neighbors.Add(b);
                    }
                }
            }
        }
    }

    bool IsPathBlocked(Vector2 start, Vector2 end)
    {
        foreach (PlanetData p in allPlanets)
        {
            Vector2 center = p.transform.position;

            if (center == start || center == end)
                continue;

            float radius = 0.5f;
            CircleCollider2D col = p.GetComponent<CircleCollider2D>();

            if (col != null)
                radius = col.radius * p.transform.localScale.x;

            float distance = DistancePointToLine(center, start, end);

            if (distance < radius)
                return true;
        }

        return false;
    }

    float DistancePointToLine(Vector2 point, Vector2 a, Vector2 b)
    {
        float l2 = (b - a).sqrMagnitude;

        if (l2 == 0) return Vector2.Distance(point, a);

        float t = Vector2.Dot(point - a, b - a) / l2;
        t = Mathf.Clamp01(t);

        Vector2 projection = a + t * (b - a);

        return Vector2.Distance(point, projection);
    }

    PlanetType GetRandomPlanetType()
    {
        int value = Random.Range(0, 7);

        switch (value)
        {
            case 0: return PlanetType.AstraPrime;
            case 1: return PlanetType.Valkurion;
            case 2: return PlanetType.Novaeon;
            case 3: return PlanetType.HeliosIX;
            case 4: return PlanetType.Calystrum;
            case 5: return PlanetType.Orionis;
            case 6: return PlanetType.Dominia;
        }

        return PlanetType.AstraPrime;
    }

    void OnDrawGizmos()
    {
        DrawGridGizmos();
        DrawConnectionsGizmos();
    }

    void DrawGridGizmos()
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

    void DrawConnectionsGizmos()
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
}

public enum PlanetType
{
    AstraPrime,
    Valkurion,
    Novaeon,
    HeliosIX,
    Calystrum,
    Orionis,
    Dominia
}

public enum Faction
{
    Neutral,
    Player,
    Enemy
}

public class PlanetData : MonoBehaviour
{
    public PlanetType planetType;

    public List<PlanetData> neighbors = new List<PlanetData>();

    public Faction ownerFaction = Faction.Neutral;

    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetOwner(Faction newOwner)
    {
        ownerFaction = newOwner;

        UpdateColor();
    }

    void UpdateColor()
    {
        if (sr == null) return;

        // 🔥 obtener colores desde GameManager
        GameManager gm = FindObjectOfType<GameManager>();

        if (ownerFaction == Faction.Neutral)
        {
            sr.color = Color.white;
            return;
        }

        if (gm == null)
            return;

        if (ownerFaction == Faction.Player)
        {
            sr.color = gm.GetPlayerColor();
        }
        else if (ownerFaction == Faction.Enemy)
        {
            sr.color = gm.GetEnemyColor();
        }
    }


}