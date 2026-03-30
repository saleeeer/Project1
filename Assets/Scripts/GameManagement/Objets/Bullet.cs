using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 5f;

    Transform target;

    public void Init(Transform targetTransform, float dmg)
    {
        target = targetTransform;
        damage = dmg;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = (target.position - transform.position).normalized;

        transform.position += direction * speed * Time.deltaTime;

        // Rotación del proyectil
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Impacto
        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            ShipCombat combat = target.GetComponent<ShipCombat>();

            if (combat != null)
            {
                combat.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}