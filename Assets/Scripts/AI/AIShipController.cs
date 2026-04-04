using UnityEngine;
using System.Collections;

public class AIShipController : MonoBehaviour
{
    ShipMovement ship;

    public float waitBeforeAttack = 2f;

    bool isThinking = false;

    void Start()
    {
        ship = GetComponent<ShipMovement>();
    }

    void Update()
    {
        if (ship == null || ship.isPlayerControlled) return;

        if (ship.isOrbiting && !isThinking)
            StartCoroutine(Think());
    }

    IEnumerator Think()
    {
        isThinking = true;

        yield return new WaitForSeconds(waitBeforeAttack);

        PlanetData target = ChooseTarget();

        if (target != null)
            ship.SetTarget(target);

        yield return new WaitForSeconds(0.5f);

        isThinking = false;
    }

    PlanetData ChooseTarget()
    {
        var neighbors = ship.currentPlanet.neighbors;

        foreach (PlanetData p in neighbors)
        {
            if (p.ownerEmpireIndex == -1)
                return p;
        }

        foreach (PlanetData p in neighbors)
        {
            if (p.ownerEmpireIndex != ship.empireIndex)
                return p;
        }

        return null;
    }
}