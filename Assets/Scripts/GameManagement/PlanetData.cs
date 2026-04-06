using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlanetData : MonoBehaviour
{
    [Header("Connections")]
    public List<PlanetData> neighbors = new List<PlanetData>();

    [Header("Ownership")]
    public int ownerEmpireIndex = -1;

    [Header("Units")]
    public int units = 0;
    public int maxUnits = 50;

    [Header("Production")]
    public float spawnInterval = 2f;

    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        StartCoroutine(ProductionRoutine());
    }

    public void SetOwner(int index)
    {
        ownerEmpireIndex = index;
        UpdateColor();
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

    // ================= PRODUCCIÓN =================

    IEnumerator ProductionRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (ownerEmpireIndex == -1)
            {
                Debug.Log(name + " es neutral, no produce");
                continue;
            }

            if (units < maxUnits)
            {
                units++;
                Debug.Log(name + " produce unidad. Total: " + units);
            }

            // 🔥 TEST AUTOMÁTICO DE ENVÍO
            PlanetData target = GetRandomPlanet();

            if (target != null && target != this)
            {
                Debug.Log(name + " intenta enviar flota a " + target.name);
                SendFleet(target);
            }
        }
    }

    // ================= FLOTAS =================

    public void SendFleet(PlanetData target)
    {
        if (target == null) return;
        if (units <= 0)
        {
            Debug.Log(name + " no tiene unidades para enviar");
            return;
        }

        GameManager gm = FindObjectOfType<GameManager>();
        if (gm == null) return;

        int amount = Mathf.Min(units, gm.maxFleetSize);

        Debug.Log(name + " enviando flota de tamaño: " + amount);

        units -= amount;

        for (int i = 0; i < amount; i++)
        {
            if (!gm.CanSpawnShip(ownerEmpireIndex))
            {
                Debug.Log("Límite de naves alcanzado para imperio " + ownerEmpireIndex);
                break;
            }

            SpawnShip(target);
        }
    }

    void SpawnShip(PlanetData target)
    {
        GameManager gm = FindObjectOfType<GameManager>();

        int playerEmpire = PlayerPrefs.GetInt("SelectedEmpire");

        GameObject prefab = (ownerEmpireIndex == playerEmpire)
            ? gm.playerShipPrefab
            : gm.enemyShipPrefab;

        Vector2 offset = Random.insideUnitCircle.normalized * 2f;

        GameObject ship = Instantiate(
            prefab,
            transform.position + (Vector3)offset,
            Quaternion.identity
        );

        Debug.Log("SPAWNING SHIP desde " + name);

        ShipMovement m = ship.GetComponent<ShipMovement>();

        m.currentPlanet = this;
        m.empireIndex = ownerEmpireIndex;
        m.isPlayerControlled = (ownerEmpireIndex == playerEmpire);

        m.SetTarget(target);

        if (!m.isPlayerControlled && ship.GetComponent<AIShipController>() == null)
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

    // ================= UTIL =================

    PlanetData GetRandomPlanet()
    {
        PlanetData[] all = FindObjectsOfType<PlanetData>();

        if (all.Length <= 1) return null;

        return all[Random.Range(0, all.Length)];
    }
}