using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlanetData : MonoBehaviour
{
    [Header("Type")]
    public PlanetType planetType;

    [Header("Economy")]
    public int baseIncome = 1;

    [Header("Connections")]
    public List<PlanetData> neighbors = new List<PlanetData>();

    [Header("Ownership")]
    public int ownerEmpireIndex = -1;

    [Header("Units")]
    public int units = 0;
    public int maxUnits = 50;

    [Header("Production")]
    public float spawnInterval = 2f;

    [Header("Fleet Sending")]
    public float sendInterval = 3f;
    public int minUnitsToSend = 5;

    [Header("Stat Buffs")]
    public EmpireStats statBuff = new EmpireStats();

    SpriteRenderer sr;

    float sendTimer;
    PlanetData lastTarget;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        AssignPlanetTypeData();
        StartCoroutine(ProductionRoutine());
    }

    // ================= TYPE =================

    void AssignPlanetTypeData()
    {
        switch (planetType)
        {
            case PlanetType.AstraPrime: baseIncome = 5; break;
            case PlanetType.Valkurion: baseIncome = 4; break;
            case PlanetType.Novaeon: baseIncome = 3; break;
            case PlanetType.HeliosIX: baseIncome = 2; break;
            case PlanetType.Calystrum: baseIncome = 2; break;
            case PlanetType.Orionis: baseIncome = 1; break;
            case PlanetType.Dominia: baseIncome = 1; break;
        }
    }

    public int GetIncome()
    {
        if (GameManager.Instance == null) return baseIncome;
        return GameManager.Instance.GetPlanetIncome(this);
    }

    // ================= OWNER =================

    public void SetOwner(int index)
    {
        ownerEmpireIndex = index;
        UpdateColor();

        Debug.Log(name + " ahora pertenece al imperio " + index);
    }

    void UpdateColor()
    {
        if (sr == null) return;

        if (ownerEmpireIndex == -1)
        {
            sr.color = Color.gray;
            return;
        }

        if (GameManager.Instance == null)
        {
            sr.color = Color.magenta;
            return;
        }

        Color c = GameManager.Instance.GetEmpireColor(ownerEmpireIndex);
        c.a = 1f;

        sr.color = c;
    }

    // ================= PRODUCTION + SPAWN =================

    IEnumerator ProductionRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (ownerEmpireIndex == -1) continue;

            // PRODUCCIÓN
            if (units < maxUnits)
                units++;

            // CONTROL DE ENVÍO
            sendTimer += spawnInterval;

            if (sendTimer >= sendInterval)
            {
                sendTimer = 0f;
                TrySendFleet();
            }
        }
    }

    void TrySendFleet()
    {
        if (units < minUnitsToSend) return;

        PlanetData target = GetTargetFromNeighbors();

        if (target == null) return;

        if (target == lastTarget) return;

        lastTarget = target;

        SendFleet(target);
    }

    // ================= TARGET INTELIGENTE =================

    PlanetData GetTargetFromNeighbors()
    {
        if (neighbors == null || neighbors.Count == 0)
            return null;

        List<PlanetData> enemies = new List<PlanetData>();
        List<PlanetData> neutrals = new List<PlanetData>();

        foreach (PlanetData n in neighbors)
        {
            if (n == null) continue;

            if (n.ownerEmpireIndex == ownerEmpireIndex)
                continue;

            if (n.ownerEmpireIndex == -1)
                neutrals.Add(n);
            else
                enemies.Add(n);
        }

        // prioridad enemigos
        if (enemies.Count > 0)
            return enemies[Random.Range(0, enemies.Count)];

        // si no hay enemigos → neutrales
        if (neutrals.Count > 0)
            return neutrals[Random.Range(0, neutrals.Count)];

        return null;
    }

    // ================= FLEET =================

    public void SendFleet(PlanetData target)
    {
        if (target == null) return;
        if (units <= 0) return;

        GameManager gm = GameManager.Instance;
        if (gm == null) return;

        int amount = Mathf.Min(units, gm.maxFleetSize);

        int spawned = 0;

        for (int i = 0; i < amount; i++)
        {
            if (!gm.CanSpawnShip(ownerEmpireIndex))
                break;

            SpawnShip(target);
            spawned++;
        }

        units -= spawned;
    }

    void SpawnShip(PlanetData target)
    {
        GameManager gm = GameManager.Instance;
        if (gm == null) return;

        int playerEmpire = gm.playerEmpireIndex;
        bool isPlayer = ownerEmpireIndex == playerEmpire;

        ShipType type = isPlayer
            ? gm.selectedShipType
            : gm.GetAIShipType(ownerEmpireIndex);

        GameObject prefab = gm.GetShipPrefab(type, isPlayer);

        if (prefab == null)
        {
            Debug.LogError("No prefab encontrado para " + type);
            return;
        }

        int cost = gm.GetShipCost(type);

        if (!gm.SpendCredits(ownerEmpireIndex, cost))
            return;

        Vector2 offset = Random.insideUnitCircle.normalized * 2f;

        GameObject ship = Instantiate(
            prefab,
            transform.position + (Vector3)offset,
            Quaternion.identity
        );

        ShipMovement m = ship.GetComponent<ShipMovement>();

        m.currentPlanet = this;
        m.empireIndex = ownerEmpireIndex;
        m.isPlayerControlled = isPlayer;

        m.SetTarget(target);

        if (!isPlayer && ship.GetComponent<AIShipController>() == null)
        {
            ship.AddComponent<AIShipController>();
        }

        ApplyColor(ship);

        gm.RegisterShip(ownerEmpireIndex);
    }

    void ApplyColor(GameObject ship)
    {
        if (GameManager.Instance == null) return;

        Color color = GameManager.Instance.GetEmpireColor(ownerEmpireIndex);

        foreach (SpriteRenderer sr in ship.GetComponentsInChildren<SpriteRenderer>())
        {
            sr.color = color;
        }
    }
}