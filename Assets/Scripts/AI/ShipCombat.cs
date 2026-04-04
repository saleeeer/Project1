using UnityEngine;

public class ShipCombat : MonoBehaviour
{
    public float maxHealth = 20f;
    public float currentHealth;

    public float damage = 5f;

    public float attackRange = 5f;
    public float attackCooldown = 1f;

    float lastAttackTime;

    public GameObject bulletPrefab;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TryAttack(ShipCombat target)
    {
        if (target == null) return;

        float dist = Vector2.Distance(transform.position, target.transform.position);
        if (dist > attackRange) return;

        if (Time.time < lastAttackTime + attackCooldown) return;

        lastAttackTime = Time.time;

        if (bulletPrefab == null)
        {
            target.TakeDamage(damage);
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        Bullet b = bullet.GetComponent<Bullet>();
        if (b != null)
            b.Init(target.transform, damage);
    }

    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
            Destroy(gameObject);
    }
}