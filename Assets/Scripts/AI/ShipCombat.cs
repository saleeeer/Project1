using UnityEngine;

public class ShipCombat : MonoBehaviour
{
    [Header("Base Stats")]
    public float baseDamage = 5f;
    public float baseHealth = 20f;
    public float attackRate = 1f;
    public float attackRange = 2f;

    float currentHealth;
    float lastAttackTime;

    public GameObject explosionPrefab;

    ShipMovement movement;
    ShipAbility ability;

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

    public void TryAttack(
        ShipCombat target
    )
    {
        if (target == null)
            return;

        float effectiveAttackRate =
            attackRate;

        if (ability != null)
        {
            effectiveAttackRate *=
                ability.GetAttackRateMultiplier();
        }

        if (effectiveAttackRate <= 0f)
            return;

        float attackCooldown =
            1f / effectiveAttackRate;

        if (Time.time <
            lastAttackTime +
            attackCooldown)
        {
            return;
        }

        lastAttackTime =
            Time.time;

        float damage =
            CalculateDamage(target);

        target.TakeDamage(damage);
    }

    float CalculateDamage(
        ShipCombat target
    )
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

        // Power
        damage *=
            attackerStats.power;

        // Defense
        float defenseFactor =
            1f /
            Mathf.Max(
                0.1f,
                defenderStats.defense
            );

        damage *=
            defenseFactor;

        // Morale
        damage *=
            attackerStats
            .GetGlobalMultiplier();

        // Habilidad de la nave
        if (ability != null)
        {
            damage *=
                ability.GetDamageMultiplier();
        }

        return damage;
    }

    // ================= DAMAGE =================

    public void TakeDamage(
        float amount
    )
    {
        // Habilidad de evasión
        if (ability != null &&
            ability.TryEvadeDamage(
                amount))
        {
            return;
        }

        currentHealth -=
            amount;

        if (currentHealth <= 0f)
        {
            Die();
        }
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