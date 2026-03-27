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

    [Header("Stop Distance")]
    public float stopOffset = 0.2f;

    [Header("Orbit")]
    public float orbitSpeed = 50f;

    public PlanetData currentPlanet;
    public PlanetData targetPlanet;

    public List<PlanetData> path = new List<PlanetData>();

    int currentIndex = 0;

    public bool isOrbiting = false;
    float orbitAngle = 0f;

    void Start()
    {
        StartCoroutine(AssignStartingPlanetDelayed());
    }

    IEnumerator AssignStartingPlanetDelayed()
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
            SnapToOrbitFromCurrentPosition(currentPlanet);
            isOrbiting = true;
        }
    }

    void Update()
    {
        if (isPlayerControlled)
        {
            HandleInput();
        }

        if (isOrbiting)
        {
            OrbitPlanet();
        }
        else
        {
            MoveAlongPath();
        }
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mouseWorld, Vector2.zero);

            if (hit.collider != null)
            {
                PlanetData clickedPlanet = hit.collider.GetComponent<PlanetData>();

                if (clickedPlanet != null)
                {
                    SetTarget(clickedPlanet);
                }
            }
        }
    }

    public void SetTarget(PlanetData newTarget)
    {
        if (currentPlanet == null || newTarget == null) return;

        targetPlanet = newTarget;

        path = FindPath(currentPlanet, targetPlanet);

        if (path.Count > 0 && path[0] == currentPlanet)
        {
            path.RemoveAt(0);
        }

        currentIndex = 0;
        isOrbiting = false;
    }

    void MoveAlongPath()
    {
        if (path == null || currentIndex >= path.Count)
        {
            isOrbiting = true;
            return;
        }

        PlanetData targetPlanetNode = path[currentIndex];

        Vector3 planetCenter = targetPlanetNode.transform.position;

        CircleCollider2D col = targetPlanetNode.GetComponent<CircleCollider2D>();

        float stopDistance = 0.5f;

        if (col != null)
        {
            stopDistance = col.radius * targetPlanetNode.transform.localScale.x + stopOffset;
        }

        Vector3 direction = (planetCenter - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        Vector3 targetPos = planetCenter - direction * stopDistance;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            currentPlanet = targetPlanetNode;
            currentIndex++;

            // 🔥 CAPTURA DE PLANETA
            currentPlanet.SetOwner(empireIndex);

            if (currentIndex >= path.Count)
            {
                isOrbiting = true;
                SnapToOrbitFromCurrentPosition(currentPlanet);
            }
        }
    }

    void OrbitPlanet()
    {
        if (currentPlanet == null) return;

        orbitAngle += orbitSpeed * Time.deltaTime;

        CircleCollider2D col = currentPlanet.GetComponent<CircleCollider2D>();

        float radius = 1f;

        if (col != null)
            radius = col.radius * currentPlanet.transform.localScale.x + stopOffset;

        float x = Mathf.Cos(orbitAngle * Mathf.Deg2Rad) * radius;
        float y = Mathf.Sin(orbitAngle * Mathf.Deg2Rad) * radius;

        Vector3 orbitPos = currentPlanet.transform.position + new Vector3(x, y, 0);

        Vector3 direction = (orbitPos - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        transform.position = orbitPos;
    }

    void SnapToOrbitFromCurrentPosition(PlanetData planet)
    {
        CircleCollider2D col = planet.GetComponent<CircleCollider2D>();

        float radius = 1f;

        if (col != null)
            radius = col.radius * planet.transform.localScale.x + stopOffset;

        Vector2 dir = (transform.position - planet.transform.position).normalized;

        orbitAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        transform.position = planet.transform.position + (Vector3)(dir * radius);
    }

    List<PlanetData> FindPath(PlanetData start, PlanetData goal)
    {
        Queue<PlanetData> queue = new Queue<PlanetData>();
        Dictionary<PlanetData, PlanetData> cameFrom = new Dictionary<PlanetData, PlanetData>();

        queue.Enqueue(start);
        cameFrom[start] = null;

        int myEmpire = empireIndex;

        while (queue.Count > 0)
        {
            PlanetData current = queue.Dequeue();

            if (current == goal)
                break;

            foreach (PlanetData neighbor in current.neighbors)
            {
                // 🔥 permitir neutrales o propios
                if (neighbor.ownerEmpireIndex != -1 && neighbor.ownerEmpireIndex != myEmpire)
                    continue;

                if (!cameFrom.ContainsKey(neighbor))
                {
                    queue.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }

        List<PlanetData> path = new List<PlanetData>();
        PlanetData temp = goal;

        if (!cameFrom.ContainsKey(goal))
            return path;

        while (temp != null)
        {
            path.Add(temp);
            temp = cameFrom[temp];
        }

        path.Reverse();

        return path;
    }
}