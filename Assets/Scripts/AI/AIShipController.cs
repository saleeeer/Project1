using UnityEngine;
using System.Collections.Generic;

public class AIShipController : MonoBehaviour
{
    ShipMovement ship;

    void Start()
    {
        ship = GetComponent<ShipMovement>();
    }

    void Update()
    {
        if (ship == null) return;
        if (ship.isPlayerControlled) return;

        if (ship.isOrbiting && ship.targetPlanet == null)
        {
            PlanetData target = FindTarget();

            if (target != null)
            {
                ship.SetTarget(target);
            }
        }
    }

    PlanetData FindTarget()
    {
        PlanetData[] all = FindObjectsOfType<PlanetData>();

        List<PlanetData> valid = new List<PlanetData>();

        foreach (PlanetData p in all)
        {
            if (p == ship.currentPlanet) continue;

            if (p.ownerEmpireIndex != ship.empireIndex)
                valid.Add(p);
        }

        if (valid.Count == 0) return null;

        return valid[Random.Range(0, valid.Count)];
    }
}