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
    ShipMovement currentTarget;

    float combatSpeedMultiplier = 0.5f;

    bool isCapturing = false;


    // ================= UNITY =================

    void Awake()
    {
        combat = GetComponent<ShipCombat>();

        if (combat == null)
            combat = gameObject.AddComponent<ShipCombat>();
    }


    void Start()
    {
        StartCoroutine(AssignStartingPlanet());

        if (GameManager.Instance != null)
            GameManager.Instance.RegisterShip(this);
    }


    IEnumerator AssignStartingPlanet()
    {
        yield return null;

        PlanetData[] planets = FindObjectsOfType<PlanetData>();

        float minDist = Mathf.Infinity;
        PlanetData closest = null;

        foreach (PlanetData p in planets)
        {
            float dist =
                Vector2.Distance(
                    transform.position,
                    p.transform.position
                );

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

        if (isPlayerControlled)
            HandleInput();

        if (isOrbiting)
            Orbit();
        else
            Move();
    }


    // ================= COMBATE =================

    void HandleCombat()
    {
        currentTarget = null;

        float closestDistance = Mathf.Infinity;

        if (GameManager.Instance == null)
            return;

        foreach (ShipMovement other in GameManager.Instance.allShips)
        {
            if (other == this)
                continue;

            if (other.empireIndex == empireIndex)
                continue;

            float dist =
                Vector2.Distance(
                    transform.position,
                    other.transform.position
                );

            if (dist <= combat.attackRange)
            {
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    currentTarget = other;
                }

                ShipCombat enemyCombat =
                    other.GetComponent<ShipCombat>();

                if (enemyCombat != null)
                {
                    combat.TryAttack(enemyCombat);
                }
            }
        }

        if (currentTarget != null)
        {
            LookAt(currentTarget.transform.position);
        }
    }


    void LookAt(Vector3 target)
    {
        Vector3 dir =
            (target - transform.position).normalized;

        if (dir == Vector3.zero)
            return;

        float angle =
            Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        transform.rotation =
            Quaternion.Euler(0, 0, angle);
    }


    // ================= INPUT =================

    void HandleInput()
    {
        // CLICK DERECHO
        if (Input.GetMouseButtonDown(1))
        {
            if (Camera.main == null)
                return;

            Vector2 mouseWorld =
                Camera.main.ScreenToWorldPoint(
                    Input.mousePosition
                );

            RaycastHit2D hit =
                Physics2D.Raycast(
                    mouseWorld,
                    Vector2.zero
                );

            if (hit.collider == null)
                return;

            PlanetData planet =
                hit.collider.GetComponent<PlanetData>();

            if (planet == null)
            {
                planet =
                    hit.collider.GetComponentInParent<PlanetData>();
            }

            if (planet != null)
            {
                SetTarget(planet);
            }
        }
    }


    // ================= MOVIMIENTO =================

    public void SetTarget(PlanetData newTarget)
    {
        if (currentPlanet == null)
            return;

        if (newTarget == null)
            return;

        targetPlanet = newTarget;

        path.Clear();
        path.Add(newTarget);

        currentIndex = 0;

        isOrbiting = false;

        // Si estaba capturando otro planeta,
        // cancelamos esa captura.
        if (isCapturing)
        {
            StopCoroutine(nameof(CaptureRoutine));
            isCapturing = false;
        }
    }


    void Move()
    {
        if (path == null || path.Count == 0)
            return;

        if (currentIndex >= path.Count)
            return;

        PlanetData targetNode =
            path[currentIndex];

        if (targetNode == null)
            return;

        Vector3 targetPos =
            targetNode.transform.position;

        Vector3 direction =
            (targetPos - transform.position).normalized;

        if (direction != Vector3.zero &&
            currentTarget == null)
        {
            float angle =
                Mathf.Atan2(
                    direction.y,
                    direction.x
                ) * Mathf.Rad2Deg;

            transform.rotation =
                Quaternion.Euler(0, 0, angle);
        }

        float currentSpeed = speed;

        if (currentTarget != null)
        {
            currentSpeed *= combatSpeedMultiplier;
        }

        transform.position =
            Vector3.MoveTowards(
                transform.position,
                targetPos,
                currentSpeed * Time.deltaTime
            );

        if (Vector3.Distance(
                transform.position,
                targetPos) < 0.1f)
        {
            currentPlanet = targetNode;

            isOrbiting = true;

            targetPlanet = null;

            path.Clear();

            SnapToOrbit(currentPlanet);

            if (!isCapturing)
            {
                StartCoroutine(CaptureRoutine());
            }
        }
    }


    // ================= CAPTURA =================

    IEnumerator CaptureRoutine()
    {
        if (currentPlanet == null)
            yield break;

        isCapturing = true;

        float timer = 0f;

        while (timer < captureTime)
        {
            // Si la nave dejó el planeta,
            // cancelamos la captura.
            if (currentPlanet == null)
            {
                isCapturing = false;
                yield break;
            }

            timer += Time.deltaTime;

            yield return null;
        }

        if (currentPlanet != null)
        {
            currentPlanet.SetOwner(empireIndex);

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(
                    AudioManager.Instance.planetCaptured
                );
            }
        }

        isCapturing = false;
    }


    // ================= ORBITA =================

    void Orbit()
    {
        if (currentPlanet == null)
            return;

        float currentOrbitSpeed =
            orbitSpeed;

        if (currentTarget != null)
        {
            currentOrbitSpeed *=
                combatSpeedMultiplier;
        }

        orbitAngle +=
            currentOrbitSpeed *
            Time.deltaTime;

        float rad =
            orbitAngle *
            Mathf.Deg2Rad;

        Vector3 offset =
            new Vector3(
                Mathf.Cos(rad),
                Mathf.Sin(rad),
                0
            ) * orbitDistance;

        transform.position =
            currentPlanet.transform.position +
            offset;

        if (currentTarget != null)
        {
            LookAt(
                currentTarget.transform.position
            );
        }
        else
        {
            Vector3 direction =
                new Vector3(
                    -Mathf.Sin(rad),
                    Mathf.Cos(rad),
                    0
                );

            float angle =
                Mathf.Atan2(
                    direction.y,
                    direction.x
                ) * Mathf.Rad2Deg;

            transform.rotation =
                Quaternion.Euler(
                    0,
                    0,
                    angle
                );
        }
    }


    // ================= ORBITA INICIAL =================

    void SnapToOrbit(PlanetData planet)
    {
        if (planet == null)
            return;

        Vector2 dir =
            transform.position -
            planet.transform.position;

        if (dir.magnitude < 0.01f)
        {
            dir =
                Random.insideUnitCircle
                .normalized;
        }
        else
        {
            dir = dir.normalized;
        }

        orbitAngle =
            Mathf.Atan2(
                dir.y,
                dir.x
            ) * Mathf.Rad2Deg;

        transform.position =
            planet.transform.position +
            (Vector3)(dir * orbitDistance);
    }


    // ================= DESTROY =================

    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UnregisterShip(this);
        }
    }
}