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

    [Header("Capture")]
    public float captureTime = 3f;

    public PlanetData currentPlanet;
    public PlanetData targetPlanet;

    public List<PlanetData> path = new List<PlanetData>();
    int currentPathIndex = 0;

    public bool isOrbiting = false;
    float orbitAngle;

    ShipCombat combat;

    void Awake()
    {
        combat = GetComponent<ShipCombat>();

        if (combat == null)
            combat = gameObject.AddComponent<ShipCombat>();
    }

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

        // 🔥 COMBATE SIN BLOQUEAR MOVIMIENTO
        HandleCombat();
    }

    // ================= INPUT =================

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

    // ================= PATHFINDING =================

    public void SetTarget(PlanetData newTarget)
    {
        if (currentPlanet == null || newTarget == null) return;

        path = FindPath(currentPlanet, newTarget);

        if (path == null || path.Count == 0) return;

        currentPathIndex = 0;
        isOrbiting = false;
    }

    List<PlanetData> FindPath(PlanetData start, PlanetData goal)
    {
        Queue<PlanetData> queue = new Queue<PlanetData>();
        Dictionary<PlanetData, PlanetData> cameFrom = new Dictionary<PlanetData, PlanetData>();

        queue.Enqueue(start);
        cameFrom[start] = null;

        while (queue.Count > 0)
        {
            PlanetData current = queue.Dequeue();

            if (current == goal)
                break;

            foreach (PlanetData neighbor in current.neighbors)
            {
                if (!cameFrom.ContainsKey(neighbor))
                {
                    queue.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }

        if (!cameFrom.ContainsKey(goal))
            return null;

        List<PlanetData> result = new List<PlanetData>();
        PlanetData temp = goal;

        while (temp != null)
        {
            result.Add(temp);
            temp = cameFrom[temp];
        }

        result.Reverse();

        if (result.Count > 0)
            result.RemoveAt(0);

        return result;
    }

    // ================= MOVIMIENTO =================

    void Move()
    {
        if (path == null || currentPathIndex >= path.Count) return;

        PlanetData targetNode = path[currentPathIndex];

        Vector3 direction = (targetNode.transform.position - transform.position).normalized;

        // 🔥 ROTACIÓN HACIA DONDE VA
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetNode.transform.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetNode.transform.position) < 0.1f)
        {
            currentPlanet = targetNode;
            currentPathIndex++;

            if (currentPathIndex >= path.Count)
            {
                isOrbiting = true;
                SnapToOrbit(currentPlanet);
                StartCoroutine(CaptureRoutine());
            }
        }
    }

    // ================= CAPTURA =================

    IEnumerator CaptureRoutine()
    {
        float timer = 0f;

        while (timer < captureTime)
        {
            if (EnemyOnPlanet())
                yield break;

            timer += Time.deltaTime;
            yield return null;
        }

        if (!EnemyOnPlanet())
        {
            currentPlanet.SetOwner(empireIndex);
        }
    }

    bool EnemyOnPlanet()
    {
        ShipMovement[] ships = FindObjectsOfType<ShipMovement>();

        foreach (ShipMovement s in ships)
        {
            if (s == this) continue;

            if (s.currentPlanet == currentPlanet &&
                s.empireIndex != empireIndex)
            {
                return true;
            }
        }

        return false;
    }

    // ================= ORBITA =================

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

        transform.position = currentPlanet.transform.position + offset;

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

        if (dir.magnitude < 0.01f)
            dir = Random.insideUnitCircle.normalized;
        else
            dir = dir.normalized;

        orbitAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        transform.position = planet.transform.position + (Vector3)(dir * orbitDistance);
    }

    // ================= COMBATE =================

    void HandleCombat()
    {
        if (currentPlanet == null) return;

        ShipMovement[] ships = FindObjectsOfType<ShipMovement>();

        foreach (ShipMovement other in ships)
        {
            if (other == this) continue;

            // 🔥 SOLO SI MISMO PLANETA
            if (other.currentPlanet == currentPlanet &&
                other.empireIndex != empireIndex)
            {
                ShipCombat enemyCombat = other.GetComponent<ShipCombat>();

                if (enemyCombat != null)
                {
                    // 🔥 MIRAR AL ENEMIGO
                    Vector3 dir = (other.transform.position - transform.position).normalized;

                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0, 0, angle);

                    combat.TryAttack(enemyCombat);
                }
            }
        }
    }
}