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

    Color originalColor;
    bool isSelected = false;

    [Header("Visuals")]
    public Sprite astraPrimeSprite;
    public Sprite valkurionSprite;
    public Sprite novaeonSprite;
    public Sprite heliosIXSprite;
    public Sprite calystrumSprite;
    public Sprite orionisSprite;
    public Sprite dominiaSprite;
    public Sprite SpriteneutralPlanetSprite;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    void Start()
    {
        AssignPlanetTypeData();
        ApplyPlanetSprite();
        StartCoroutine(ProductionRoutine());
    }

    private void Update()
    {
        HandleSelectionVisual();
    }

    // ================= TYPE =================


    void HandleSelectionVisual()
    {
        if (GameManager.Instance == null)
            return;

        bool selected =
            GameManager.Instance.selectedOriginPlanet == this;

        if (selected && !isSelected)
        {
            sr.color = Color.white;
            isSelected = true;
        }
        else if (!selected && isSelected)
        {
            UpdateColor();
            isSelected = false;
        }
    }

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
            case PlanetType.NeutralPlanet:baseIncome = 1; break;
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

    // ================= PRODUCTION =================

    IEnumerator ProductionRoutine()
    {
        while (true)
        {
            float interval = spawnInterval;

            yield return new WaitForSeconds(interval);

            // neutro
            if (ownerEmpireIndex == -1)
                continue;

            // producir unidades
            if (units < maxUnits)
                units++;

            // verificar GameManager
            if (GameManager.Instance == null)
                continue;

            // 🔥 SI ES JUGADOR → NO HACER NADA MÁS
            if (ownerEmpireIndex == GameManager.Instance.playerEmpireIndex)
                continue;

            // ================= IA =================

            sendTimer += spawnInterval;

            if (sendTimer >= sendInterval)
            {
                sendTimer = 0f;
                TrySendFleet();
            }
        }
    }

    // ================= IA =================

    void TrySendFleet()
    {
        Debug.Log(
    "TrySendFleet -> " +
    name +
    " Owner=" +
    ownerEmpireIndex +
    " Player=" +
    GameManager.Instance.playerEmpireIndex);
        if (units < minUnitsToSend) return;

        PlanetData target = GetTargetFromNeighbors();

        if (target == null) return;

        // Evitar ataques suicidas
        if (target.units > units * 1.2f)
            return;

        if (target == lastTarget) return;

        lastTarget = target;

        SendFleet(target);
    }

    PlanetData GetTargetFromNeighbors()
    {
        if (neighbors == null || neighbors.Count == 0)
            return null;

        PlanetData bestTarget = null;
        float bestScore = float.MinValue;

        foreach (PlanetData n in neighbors)
        {
            if (n == null) continue;
            if (n.ownerEmpireIndex == ownerEmpireIndex) continue;

            float score = 0f;

            // Prioridad enemigo > neutral
            if (n.ownerEmpireIndex == -1)
                score += 5f;
            else
                score += 10f;

            // Preferir débiles
            score += (maxUnits - n.units);

            // Evitar suicidio
            if (units < n.units)
                score -= 20f;

            // Bonus si es muy débil
            if (n.units < units * 0.5f)
                score += 10f;

            if (score > bestScore)
            {
                bestScore = score;
                bestTarget = n;
            }
        }

        return bestTarget;
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
        Debug.Log(
    "SPAWN IA -> "
    + name
    + " Empire=" + ownerEmpireIndex);
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

        AudioManager.Instance.PlaySFX(AudioManager.Instance.shipSpawn);

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

    void OnMouseDown()
    {
        if (GameManager.Instance == null)
            return;

        int playerEmpire =
            GameManager.Instance.playerEmpireIndex;

        // ================= PRIMER CLICK =================

        if (ownerEmpireIndex == playerEmpire)
        {
            GameManager.Instance.selectedOriginPlanet = this;

            Debug.Log("Origen seleccionado: " + name);

            return;
        }

        // ================= SEGUNDO CLICK =================

        PlanetData origin =
            GameManager.Instance.selectedOriginPlanet;

        if (origin == null)
            return;

        if (origin.ownerEmpireIndex != playerEmpire)
            return;

        // enviar flota
        origin.SendFleet(this);

        Debug.Log(
            origin.name +
            " ataca " +
            name
        );
    }


    public void SpawnPlayerShip()
    {
        Debug.Log(
     "SPAWN PLAYER SHIP -> "
     + name
     + " ID="
     + GetInstanceID());
        if (GameManager.Instance == null)
            return;

        if (ownerEmpireIndex != GameManager.Instance.playerEmpireIndex)
            return;

        if (units <= 0)
        {
            Debug.Log("No hay unidades disponibles");
            return;
        }

        GameManager gm = GameManager.Instance;

        if (!gm.CanSpawnShip(ownerEmpireIndex))
        {
            Debug.Log("Límite de naves alcanzado");
            return;
        }

        ShipType type = gm.selectedShipType;

        int cost = gm.GetShipCost(type);

        if (!gm.SpendCredits(ownerEmpireIndex, cost))
        {
            Debug.Log("No hay créditos");
            return;
        }

        GameObject prefab = gm.GetShipPrefab(type, true);

        if (prefab == null)
        {
            Debug.LogError("Prefab nulo");
            return;
        }

        units--;

        Vector2 offset =
            Random.insideUnitCircle.normalized * 2f;

        AudioManager.Instance.PlaySFX(AudioManager.Instance.shipSpawn);

        GameObject ship = Instantiate(
            prefab,
            transform.position + (Vector3)offset,
            Quaternion.identity
        );

        ShipMovement movement =
            ship.GetComponent<ShipMovement>();

        movement.currentPlanet = this;
        movement.empireIndex = ownerEmpireIndex;
        movement.isPlayerControlled = true;

        ApplyColor(ship);

        gm.RegisterShip(ownerEmpireIndex);

        Debug.Log("Spawn manual de " + type);
    }

    void ApplyPlanetSprite()
    {
        if (sr == null)
            sr = GetComponent<SpriteRenderer>();

        switch (planetType)
        {
            case PlanetType.AstraPrime:
                sr.sprite = astraPrimeSprite;
                break;

            case PlanetType.Valkurion:
                sr.sprite = valkurionSprite;
                break;

            case PlanetType.Novaeon:
                sr.sprite = novaeonSprite;
                break;

            case PlanetType.HeliosIX:
                sr.sprite = heliosIXSprite;
                break;

            case PlanetType.Calystrum:
                sr.sprite = calystrumSprite;
                break;

            case PlanetType.Orionis:
                sr.sprite = orionisSprite;
                break;

            case PlanetType.Dominia:
                sr.sprite = dominiaSprite;
                break;

            case PlanetType.NeutralPlanet:
                sr.sprite = SpriteneutralPlanetSprite;
                break;
        }
    }
}