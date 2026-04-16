using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipMovement : MonoBehaviour
{
    [Header("Type")]
    public ShipType shipType;

    [Header("Stats")]
    public float speed = 10f;

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

    int currentIndex = 0;

    public bool isOrbiting = false;
    float orbitAngle;

    ShipCombat combat;

    bool isFighting = false;
    bool isCapturing = false;

    void Awake()
    {
        combat = GetComponent<ShipCombat>();

        if (combat == null)
            combat = gameObject.AddComponent<ShipCombat>();
    }

    void Start()
    {
        ApplyShipTypeStats();
        StartCoroutine(AssignStartingPlanet());
    }

    void ApplyShipTypeStats()
    {
        switch (shipType)
        {
            case ShipType.Fighter:
                speed *= 1.5f;
                combat.baseDamage *= 0.8f;
                combat.baseHealth *= 0.8f;
                break;

            case ShipType.Bomber:
                speed *= 0.7f;
                combat.baseDamage *= 2f;
                combat.baseHealth *= 1.5f;
                break;

            case ShipType.Commander:
                speed *= 0.9f;
                combat.baseDamage *= 0.5f;
                combat.baseHealth *= 2f;
                break;
        }

        Debug.Log("Speed final: " + speed);
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
        if (isFighting) return;

        if (isPlayerControlled)
            HandleInput();

        if (isOrbiting)
            Orbit();
        else
            Move();
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

    // ================= MOVIMIENTO =================

    public void SetTarget(PlanetData newTarget)
    {
        if (currentPlanet == null || newTarget == null) return;

        targetPlanet = newTarget;

        path = new List<PlanetData>() { newTarget };

        currentIndex = 0;
        isOrbiting = false;
    }

    void Move()
    {
        if (path == null || path.Count == 0) return;

        PlanetData targetNode = path[currentIndex];

        Vector3 targetPos = targetNode.transform.position;

        Vector3 direction = (targetPos - transform.position).normalized;

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

            isOrbiting = true;
            targetPlanet = null;

            SnapToOrbit(currentPlanet);

            // 🔥 INICIAR CAPTURA
            if (!isCapturing)
                StartCoroutine(CaptureRoutine());
        }
    }

    // ================= CAPTURA =================

    IEnumerator CaptureRoutine()
    {
        isCapturing = true;

        Debug.Log("Intentando capturar " + currentPlanet.name);

        float timer = 0f;

        while (timer < captureTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // 🔥 YA NO BLOQUEAMOS POR ENEMIGOS
        currentPlanet.SetOwner(empireIndex);

        Debug.Log("✅ Planeta conquistado por imperio " + empireIndex);

        isCapturing = false;
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

    void OnDestroy()
    {
        GameManager gm = FindObjectOfType<GameManager>();

        if (gm != null)
        {
            gm.UnregisterShip(empireIndex);
        }
    }
}