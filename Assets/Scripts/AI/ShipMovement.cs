using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipMovement : MonoBehaviour
{
    public float speed = 5f;

    [Header("Control")]
    public bool isPlayerControlled = false;

    [Header("Empire")]
    public int empireIndex;

    [Header("Orbit")]
    public float orbitDistance = 2f;
    public float orbitSpeed = 50f;

    public PlanetData currentPlanet;
    public PlanetData targetPlanet;

    public List<PlanetData> path = new List<PlanetData>();

    int currentIndex = 0;

    public bool isOrbiting = false;
    float orbitAngle;

    void Start()
    {
        StartCoroutine(AssignStartingPlanet());
    }

    IEnumerator AssignStartingPlanet()
    {
        yield return null;

        PlanetData[] planets = FindObjectsOfType<PlanetData>();

        float minDist = Mathf.Infinity;
        PlanetData closest = null;

        foreach (PlanetData p in planets)
        {
            float dist = Vector2.Distance(transform.position, p.transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                closest = p;
            }
        }

        currentPlanet = closest;

        if (currentPlanet != null)
        {
            SnapToOrbit(currentPlanet);
            isOrbiting = true;
        }
    }

    void Update()
    {
        if (isPlayerControlled)
            HandleInput();

        if (isOrbiting)
            Orbit();
        else
            Move();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mouseWorld, Vector2.zero);

            if (hit.collider != null)
            {
                PlanetData p = hit.collider.GetComponent<PlanetData>();

                if (p != null)
                    SetTarget(p);
            }
        }
    }

    public void SetTarget(PlanetData newTarget)
    {
        if (currentPlanet == null || newTarget == null) return;

        targetPlanet = newTarget;

        path = new List<PlanetData>();
        path.Add(newTarget);

        currentIndex = 0;
        isOrbiting = false;
    }

    void Move()
    {
        if (path == null || path.Count == 0) return;

        PlanetData targetNode = path[currentIndex];

        Vector3 targetPos = targetNode.transform.position;

        Vector3 direction = (targetPos - transform.position).normalized;

        // ROTACIÓN AL MOVERSE
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            currentPlanet = targetNode;

            currentPlanet.SetOwner(empireIndex);

            isOrbiting = true;
            targetPlanet = null;

            SnapToOrbit(currentPlanet);
        }
    }

    void Orbit()
    {
        if (currentPlanet == null) return;

        orbitAngle += orbitSpeed * Time.deltaTime;

        float rad = orbitAngle * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(
            Mathf.Cos(rad),
            Mathf.Sin(rad),
            0
        ) * orbitDistance;

        Vector3 orbitPos = currentPlanet.transform.position + offset;

        transform.position = orbitPos;

        // ROTACIÓN EN ÓRBITA
        Vector3 direction = new Vector3(
            -Mathf.Sin(rad),
            Mathf.Cos(rad),
            0
        );

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void SnapToOrbit(PlanetData planet)
    {
        Vector2 dir = (transform.position - planet.transform.position);

        // 🔥 SI ESTÁ EN EL CENTRO → dirección random
        if (dir.magnitude < 0.01f)
        {
            dir = Random.insideUnitCircle.normalized;
        }
        else
        {
            dir = dir.normalized;
        }

        orbitAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        transform.position = planet.transform.position + (Vector3)(dir * orbitDistance);
    }
}