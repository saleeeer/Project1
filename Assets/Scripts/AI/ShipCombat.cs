using UnityEngine;

public class ShipCombat : MonoBehaviour
{
    [Header("Base Stats")]
    public float baseDamage = 5f;
    public float baseHealth = 20f;
    public float attackRate = 1f;
    public float attackRange = 2f;

    [Header("Bullet")]
    public Bullet bulletPrefab;
    public Transform firePoint;

    [Header("Death")]
    public GameObject explosionPrefab;

    float currentHealth;
    float lastAttackTime;

    ShipMovement movement;
    ShipAbility ability;

    float empEndTime;
    float empSpeedMultiplier = 1f;
    float empAttackRateMultiplier = 1f;

    void Awake()
    {
        movement =
            GetComponent<ShipMovement>();

        ability =
            GetComponent<ShipAbility>();

        currentHealth =
            baseHealth;
    }

    // ================= COMBAT =================


    public void TryAttack(ShipCombat target)
    {
        if (target == null)
            return;

        float rate = attackRate;

        if (ability != null)
            rate *= ability.GetAttackRateMultiplier();

        rate *= GetAllyAttackRateMultiplier();
        rate *= GetExternalAttackRateMultiplier();

        // Evitar divisiones por cero o cadencias negativas.
        rate = Mathf.Max(0.01f, rate);

        // Esperar el intervalo de ataque correspondiente.
        if (Time.time - lastAttackTime < 1f / rate)
            return;

        lastAttackTime = Time.time;

        float damage = CalculateDamage(target);

        // Mantener las habilidades de ataque especial existentes.
        if (ability != null &&
            ability.TrySpecialAttack(target, damage))
        {
            return;
        }

        // Ataque normal: un solo proyectil.
        SpawnBullet(target, damage);

        // El Bomber puede activar aleatoriamente su cadencia rápida.
        BomberMissileBarrage bomberAbility =
            GetComponent<BomberMissileBarrage>();

        if (bomberAbility != null)
            bomberAbility.TryActivateRapidFire();
    }


    public void SpawnBullet(
        ShipCombat target,
        float damage)
    {
        if (bulletPrefab == null ||
            target == null)
        {
            return;
        }

        Transform spawnPoint =
            firePoint != null
            ? firePoint
            : transform;

        Bullet bullet =
            Instantiate(
                bulletPrefab,
                spawnPoint.position,
                spawnPoint.rotation
            );

        bullet.Init(
            target.transform,
            damage,
            this
        );
    }

    float CalculateDamage(
        ShipCombat target)
    {
        if (movement == null ||
            GameManager.Instance == null)
        {
            return baseDamage;
        }

        EmpireStats attackerStats =
            GameManager.Instance
            .GetEmpireTotalStats(
                movement.empireIndex
            );

        EmpireStats defenderStats =
            GameManager.Instance
            .GetEmpireTotalStats(
                target.movement.empireIndex
            );

        float damage =
            baseDamage;

        damage *=
            attackerStats.power;

        float defenseFactor =
            1f /
            Mathf.Max(
                0.1f,
                defenderStats.defense
            );

        damage *=
            defenseFactor;

        damage *=
            attackerStats.GetGlobalMultiplier();

        if (ability != null)
        {
            damage *=
                ability.GetDamageMultiplier();
        }

        damage *=
            GetAllyDamageMultiplier();

        return damage;
    }

    // ================= DAMAGE =================

    public void TakeDamage(
        float amount)
    {
        if (ability != null &&
            ability.TryEvadeDamage(amount))
        {
            return;
        }

        amount *=
            GetIncomingDamageMultiplier();

        currentHealth -=
            amount;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    // ================= ALLY BUFFS =================

    float GetAllyDamageMultiplier()
    {
        if (GameManager.Instance == null)
            return 1f;

        float bestMultiplier = 1f;

        foreach (
            ShipMovement ship
            in GameManager.Instance.allShips)
        {
            if (ship == null ||
                ship == movement)
            {
                continue;
            }

            if (ship.empireIndex !=
                movement.empireIndex)
            {
                continue;
            }

            ShipAbility allyAbility =
                ship.GetComponent<ShipAbility>();

            if (allyAbility == null)
                continue;

            float multiplier =
                allyAbility
                .GetAllyDamageMultiplier(
                    transform.position
                );

            bestMultiplier =
                Mathf.Max(
                    bestMultiplier,
                    multiplier
                );
        }

        return bestMultiplier;
    }

    float GetAllyAttackRateMultiplier()
    {
        if (GameManager.Instance == null)
            return 1f;

        float bestMultiplier = 1f;

        foreach (
            ShipMovement ship
            in GameManager.Instance.allShips)
        {
            if (ship == null ||
                ship == movement)
            {
                continue;
            }

            if (ship.empireIndex !=
                movement.empireIndex)
            {
                continue;
            }

            ShipAbility allyAbility =
                ship.GetComponent<ShipAbility>();

            if (allyAbility == null)
                continue;

            float multiplier =
                allyAbility
                .GetAllyAttackRateMultiplier(
                    transform.position
                );

            bestMultiplier =
                Mathf.Max(
                    bestMultiplier,
                    multiplier
                );
        }

        return bestMultiplier;
    }

    float GetIncomingDamageMultiplier()
    {
        if (GameManager.Instance == null)
            return 1f;

        float bestMultiplier = 1f;

        foreach (
            ShipMovement ship
            in GameManager.Instance.allShips)
        {
            if (ship == null ||
                ship == movement)
            {
                continue;
            }

            if (ship.empireIndex !=
                movement.empireIndex)
            {
                continue;
            }

            ShipAbility allyAbility =
                ship.GetComponent<ShipAbility>();

            if (allyAbility == null)
                continue;

            float multiplier =
                allyAbility
                .GetIncomingDamageMultiplier(
                    transform.position
                );

            bestMultiplier =
                Mathf.Min(
                    bestMultiplier,
                    multiplier
                );
        }

        return bestMultiplier;
    }

    // ================= EMP =================

    public void ApplyEMP(
        float duration,
        float speedMultiplier,
        float attackRateMultiplier)
    {
        empEndTime =
            Mathf.Max(
                empEndTime,
                Time.time + duration
            );

        empSpeedMultiplier =
            Mathf.Min(
                empSpeedMultiplier,
                speedMultiplier
            );

        empAttackRateMultiplier =
            Mathf.Min(
                empAttackRateMultiplier,
                attackRateMultiplier
            );
    }

    public float GetExternalSpeedMultiplier()
    {
        if (Time.time >= empEndTime)
        {
            empSpeedMultiplier = 1f;
            empAttackRateMultiplier = 1f;

            return 1f;
        }

        return empSpeedMultiplier;
    }

    float GetExternalAttackRateMultiplier()
    {
        if (Time.time >= empEndTime)
        {
            empSpeedMultiplier = 1f;
            empAttackRateMultiplier = 1f;

            return 1f;
        }

        return empAttackRateMultiplier;
    }

    // ================= DEATH =================

    void Die()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(
                AudioManager.Instance.shipDestroyed
            );
        }

        if (explosionPrefab != null)
        {
            Instantiate(
                explosionPrefab,
                transform.position,
                Quaternion.identity
            );
        }

        Destroy(gameObject);
    }

    // ================= UTILS =================

    public float GetHealthPercent()
    {
        if (baseHealth <= 0)
            return 0f;

        return currentHealth /
               baseHealth;
    }
}