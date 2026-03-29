using UnityEngine;

public class ShipCombat : MonoBehaviour
{
    public int maxHealth = 10;
    public int currentHealth;

    public int damage = 2;

    ShipMovement movement;

    void Awake()
    {
        currentHealth = maxHealth;
        movement = GetComponent<ShipMovement>();
    }

    public void Attack(ShipCombat target)
    {
        if (target == null) return;

        target.TakeDamage(damage);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}