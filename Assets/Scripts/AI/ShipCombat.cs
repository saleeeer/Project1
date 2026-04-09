using UnityEngine;

public class ShipCombat : MonoBehaviour
{
    public float baseHealth = 20f;
    public float currentHealth;

    public float baseDamage = 5f;

    [Header("Combat")]
    public float attackRange = 5f;
    public float attackCooldown = 1f;

    float lastAttackTime;

    ShipMovement movement;

    void Awake()
    {
        movement = GetComponent<ShipMovement>();
    }

    void Start()
    {
        currentHealth = GetMaxHealth();
    }

    float GetMaxHealth()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm == null) return baseHealth;

        EmpireStats stats = gm.GetEmpireTotalStats(movement.empireIndex);

        float value = baseHealth * stats.defense * stats.GetGlobalMultiplier();

        Debug.Log("HP calculada: " + value);

        return value;
    }

    float GetDamage()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm == null) return baseDamage;

        EmpireStats stats = gm.GetEmpireTotalStats(movement.empireIndex);

        float value = baseDamage * stats.power * stats.GetGlobalMultiplier();

        Debug.Log("Daño calculado: " + value);

        return value;
    }

    float GetAccuracy()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm == null) return 1f;

        return gm.GetEmpireTotalStats(movement.empireIndex).accuracy;
    }

    public bool CanAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown;
    }

    public void TryAttack(ShipCombat target)
    {
        if (target == null) return;

        float dist = Vector2.Distance(transform.position, target.transform.position);

        if (dist > attackRange) return;

        if (!CanAttack()) return;

        lastAttackTime = Time.time;

        float accuracy = GetAccuracy();

        if (Random.value > accuracy)
        {
            Debug.Log("Fallo ataque");
            return;
        }

        float dmg = GetDamage();

        Debug.Log("Ataque exitoso por " + dmg);

        target.TakeDamage(dmg);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}