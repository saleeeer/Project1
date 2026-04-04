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

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            ShipCombat combat = target.GetComponent<ShipCombat>();

            if (combat != null)
                combat.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}