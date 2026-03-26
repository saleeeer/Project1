using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ShipMovement))]
public class AIShipController : MonoBehaviour
{
    ShipMovement movement;

    [Header("AI Settings")]
    public float decisionDelay = 2f;

    GalaxyGenerator galaxy;

    void Start()
    {
        movement = GetComponent<ShipMovement>();

        galaxy = FindObjectOfType<GalaxyGenerator>();

        StartCoroutine(AILoop());
    }

    IEnumerator AILoop()
    {
        // esperar a que todo esté inicializado
        yield return new WaitForSeconds(1f);

        while (true)
        {
            yield return new WaitForSeconds(decisionDelay);

            TryChooseNewTarget();
        }
    }

    void TryChooseNewTarget()
    {
        // si está moviéndose, no hacer nada
        if (movement.path != null && movement.path.Count > 0 && !IsIdle())
            return;

        if (movement.currentPlanet == null)
            return;

        PlanetData target = ChooseRandomPlanet();

        if (target != null && target != movement.currentPlanet)
        {
            movement.SetTarget(target);
        }
    }

    PlanetData ChooseRandomPlanet()
    {
        if (galaxy == null || galaxy.allPlanets.Count == 0)
            return null;

        int attempts = 10;

        while (attempts > 0)
        {
            PlanetData randomPlanet = galaxy.allPlanets[Random.Range(0, galaxy.allPlanets.Count)];

            if (randomPlanet != movement.currentPlanet)
                return randomPlanet;

            attempts--;
        }

        return null;
    }

    bool IsIdle()
    {
        return movement.path == null || movement.path.Count == 0 || movement.currentPlanet == movement.targetPlanet;
    }
}