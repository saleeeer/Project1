using UnityEngine;

public class ShipCombat : MonoBehaviour
{
    public float maxHealth = 20f;
    public float currentHealth;

    public float damage = 5f;

    [Header("Combat")]
    public float attackRange = 5f;
    public float attackCooldown = 1f;

    float lastAttackTime;

    [Header("Visual")]
    public GameObject bulletPrefab;

    void Awake()
    {
        currentHealth = maxHealth;
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

        Shoot(target);
    }

    void Shoot(ShipCombat target)
    {
        if (bulletPrefab == null)
        {
            target.TakeDamage(damage);
            return;
        }

        GameObject bullet = Instantiate(
            bulletPrefab,
            transform.position,
            Quaternion.identity
        );

        Bullet b = bullet.GetComponent<Bullet>();

        if (b != null)
        {
            b.Init(target.transform, damage);
        }
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