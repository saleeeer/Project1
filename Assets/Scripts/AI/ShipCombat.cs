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

    ShipMovement movement;

    [Header("Bullet")]
    public Bullet bulletPrefab;
    public Transform firePoint;

    public GameObject explosionPrefab;

    void Awake()
    {
        movement = GetComponent<ShipMovement>();
        currentHealth = baseHealth;
    }

    // ================= COMBAT =================

    public void TryAttack(ShipCombat target)
    {
        if (target == null) return;

        if (Time.time < lastAttackTime + (1f / attackRate))
            return;

        lastAttackTime = Time.time;

        float damage = CalculateDamage(target);

        Transform spawnPoint = firePoint != null ? firePoint : transform;

        Bullet b = Instantiate(
            bulletPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        Debug.Log($"{name} dispara a {target.name}");

        b.Init(target.transform, damage);
    }

    float CalculateDamage(ShipCombat target)
    {
        if (movement == null || GameManager.Instance == null)
            return baseDamage;

        EmpireStats attackerStats = GameManager.Instance.GetEmpireTotalStats(movement.empireIndex);
        EmpireStats defenderStats = GameManager.Instance.GetEmpireTotalStats(target.movement.empireIndex);

        float damage = baseDamage;

        //aplicar power
        damage *= attackerStats.power;

        //aplicar defense
        float defenseFactor = 1f / Mathf.Max(0.1f, defenderStats.defense);
        damage *= defenseFactor;

        //morale como multiplicador global
        damage *= attackerStats.GetGlobalMultiplier();

        return damage;
    }

    // ================= DAMAGE =================

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }



    void Die()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(
                AudioManager.Instance.shipDestroyed
            );
        }

        Instantiate(
    explosionPrefab,
    transform.position,
    Quaternion.identity);

        Destroy(gameObject);
    }

    // ================= UTILS =================

    public float GetHealthPercent()
    {
        if (baseHealth <= 0) return 0f;
        return currentHealth / baseHealth;
    }
}