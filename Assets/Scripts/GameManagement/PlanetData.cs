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

    [Header("Stat Buffs")]
    public EmpireStats statBuff = new EmpireStats();

    SpriteRenderer sr;

    Coroutine productionCoroutine;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        AssignPlanetTypeData();

        productionCoroutine = StartCoroutine(ProductionRoutine());
    }

    // ================= TYPE =================

    void AssignPlanetTypeData()
    {
        switch (planetType)
        {
            case PlanetType.AstraPrime:
                baseIncome = 5;
                break;

            case PlanetType.Valkurion:
                baseIncome = 4;
                break;

            case PlanetType.Novaeon:
                baseIncome = 3;
                break;

            case PlanetType.HeliosIX:
                baseIncome = 2;
                break;

            case PlanetType.Calystrum:
                baseIncome = 2;
                break;

            case PlanetType.Orionis:
                baseIncome = 1;
                break;

            case PlanetType.Dominia:
                baseIncome = 1;
                break;
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
        if (GameManager.Instance == null) return;

        if (ownerEmpireIndex == -1)
        {
            sr.color = Color.white;
            return;
        }

        sr.color = GameManager.Instance.GetEmpireColor(ownerEmpireIndex);
    }

    // ================= PRODUCTION =================

    IEnumerator ProductionRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (ownerEmpireIndex == -1) continue;

            if (units < maxUnits)
                units++;

            if (units >= 1)
            {
                PlanetData target = GetRandomPlanet();

                if (target != null && target != this)
                {
                    SendFleet(target);
                }
            }
        }
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
        {
            return;
        }

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

    // ================= TARGET =================

    PlanetData GetRandomPlanet()
    {
        if (GalaxyGenerator.Instance == null) return null;

        var all = GalaxyGenerator.Instance.allPlanets;

        if (all == null || all.Count <= 1) return null;

        return all[Random.Range(0, all.Count)];
    }
}