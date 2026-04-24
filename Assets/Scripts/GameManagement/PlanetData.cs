using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlanetData : MonoBehaviour
{
    [Header("Type")]
    public PlanetType planetType;

    [Header("Economy")]
    public int baseIncome = 1; // 🔥 NUEVO

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

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        AssignPlanetTypeData(); // 🔥 IMPORTANTE
        StartCoroutine(ProductionRoutine());
    }

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
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm == null) return 1;

        return gm.GetPlanetIncome(this);
    }

    public void SetOwner(int index)
    {
        ownerEmpireIndex = index;
        UpdateColor();

        Debug.Log(name + " ahora pertenece al imperio " + index);
    }

    void UpdateColor()
    {
        if (sr == null) return;

        GameManager gm = FindObjectOfType<GameManager>();

        if (ownerEmpireIndex == -1)
        {
            sr.color = Color.white;
            return;
        }

        if (gm == null) return;

        sr.color = gm.GetEmpireColor(ownerEmpireIndex);
    }

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

    // ================= FLOTAS =================

    public void SendFleet(PlanetData target)
    {
        if (target == null) return;
        if (units <= 0) return;

        GameManager gm = FindObjectOfType<GameManager>();
        if (gm == null) return;

        int amount = Mathf.Min(units, gm.maxFleetSize);

        units -= amount;

        for (int i = 0; i < amount; i++)
        {
            if (!gm.CanSpawnShip(ownerEmpireIndex))
                break;

            SpawnShip(target);
        }
    }

    // SOLO TE PONGO EL MÉTODO MODIFICADO PARA NO ROMPER LO DEMÁS

    void SpawnShip(PlanetData target)
    {
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm == null) return;

        int playerEmpire = PlayerPrefs.GetInt("SelectedEmpire");
        bool isPlayer = ownerEmpireIndex == playerEmpire;

        // 🔥 NUEVO: elegir tipo
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
            Debug.Log("❌ No hay créditos");
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
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm == null) return;

        Color color = gm.GetEmpireColor(ownerEmpireIndex);

        foreach (SpriteRenderer sr in ship.GetComponentsInChildren<SpriteRenderer>())
        {
            sr.color = color;
        }
    }

    PlanetData GetRandomPlanet()
    {
        PlanetData[] all = FindObjectsOfType<PlanetData>();

        if (all.Length <= 1) return null;

        return all[Random.Range(0, all.Length)];
    }
}