using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIShipController : MonoBehaviour
{
    ShipMovement ship;

    [Header("AI Timing")]
    public float waitBeforeAttack = 3f; // 🔥 tiempo defendiendo

    bool isThinking = false;

    void Start()
    {
        ship = GetComponent<ShipMovement>();
    }

    void Update()
    {
        if (ship == null) return;
        if (ship.isPlayerControlled) return;

        if (ship.isOrbiting && !isThinking)
        {
            StartCoroutine(AIBehaviour());
        }
    }

    IEnumerator AIBehaviour()
    {
        isThinking = true;

        // 🔥 1. DEFENDER (espera)
        yield return new WaitForSeconds(waitBeforeAttack);

        // 🔥 2. ELEGIR OBJETIVO
        PlanetData target = ChooseTarget();

        if (target != null)
        {
            ship.SetTarget(target);
        }

        // 🔥 pequeño delay para evitar spam
        yield return new WaitForSeconds(0.5f);

        isThinking = false;
    }

    PlanetData ChooseTarget()
    {
        PlanetData[] allPlanets = FindObjectsOfType<PlanetData>();

        List<PlanetData> validTargets = new List<PlanetData>();

        foreach (PlanetData p in allPlanets)
        {
            if (p == ship.currentPlanet) continue;

            // 🔥 solo atacar planetas que NO son tuyos
            if (p.ownerEmpireIndex != ship.empireIndex)
            {
                validTargets.Add(p);
            }
        }

        if (validTargets.Count == 0) return null;

        return validTargets[Random.Range(0, validTargets.Count)];
    }
}