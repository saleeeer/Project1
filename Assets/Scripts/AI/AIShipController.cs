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

        if (ship.isOrbiting)
        {
            PlanetData target = FindTargetPlanet();

            if (target != null)
            {
                ship.SetTarget(target);
            }
        }
    }

    PlanetData FindTargetPlanet()
    {
        if (ship.currentPlanet == null) return null;

        List<PlanetData> options = new List<PlanetData>();

        foreach (PlanetData p in ship.currentPlanet.neighbors)
        {
            // 🔥 atacar cualquier planeta que no sea propio
            if (p.ownerEmpireIndex != ship.empireIndex)
            {
                options.Add(p);
            }
        }

        if (options.Count == 0) return null;

        return options[Random.Range(0, options.Count)];
    }
}