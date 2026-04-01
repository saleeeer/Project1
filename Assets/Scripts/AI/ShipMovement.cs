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

    int currentIndex = 0;

    public bool isOrbiting = false;
    float orbitAngle;

    ShipCombat combat;

    bool isCapturing = false;

    // 🔥 COMBATE TIPO DUELO
    bool isFighting = false;
    ShipMovement currentEnemy;

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
        HandleCombat();

        if (isFighting) return; // 🔥 frena todo durante combate

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

            StartCoroutine(CaptureRoutine());
        }
    }

    // ================= CAPTURA =================

    IEnumerator CaptureRoutine()
    {
        isCapturing = true;

        float timer = 0f;

        while (timer < captureTime)
        {
            if (EnemyOnPlanet())
            {
                isCapturing = false;
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        if (!EnemyOnPlanet())
        {
            currentPlanet.SetOwner(empireIndex);
        }

        isCapturing = false;
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

    void RecheckCapture()
    {
        if (!EnemyOnPlanet())
        {
            currentPlanet.SetOwner(empireIndex);
        }
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

        Vector3 orbitPos = currentPlanet.transform.position + offset;

        transform.position = orbitPos;

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

    // ================= COMBATE TIPO DUELO =================

    void HandleCombat()
    {
        if (isFighting) return;

        ShipMovement[] ships = FindObjectsOfType<ShipMovement>();

        foreach (ShipMovement other in ships)
        {
            if (other == this) continue;
            if (other.empireIndex == empireIndex) continue;

            float dist = Vector2.Distance(transform.position, other.transform.position);

            if (dist < combat.attackRange)
            {
                StartCoroutine(FightRoutine(other));
                break;
            }
        }
    }

    IEnumerator FightRoutine(ShipMovement enemy)
    {
        isFighting = true;
        currentEnemy = enemy;

        ShipCombat enemyCombat = enemy.GetComponent<ShipCombat>();

        while (enemy != null && enemyCombat != null && combat != null)
        {
            LookAt(enemy.transform.position);
            enemy.LookAt(transform.position);

            combat.TryAttack(enemyCombat);

            if (enemyCombat != null)
                enemyCombat.TryAttack(combat);

            yield return null;
        }

        isFighting = false;
        currentEnemy = null;
    }

    void LookAt(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnDestroy()
    {
        if (currentPlanet != null)
        {
            RecheckCapture();
        }
    }
}