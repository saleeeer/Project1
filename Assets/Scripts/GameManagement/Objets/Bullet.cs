using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 5f;

    Transform target;
    ShipCombat shooter;

    public void Init(
        Transform targetTransform,
        float dmg,
        ShipCombat shooterCombat)
    {
        target =
            targetTransform;

        damage =
            dmg;

        shooter =
            shooterCombat;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position =
            Vector3.MoveTowards(
                transform.position,
                target.position,
                speed *
                Time.deltaTime
            );

        if ((transform.position -
             target.position).sqrMagnitude <
            0.01f)
        {
            ShipCombat combat =
                target.GetComponent<ShipCombat>();

            if (combat != null)
            {
                ShipAbility shooterAbility =
                    shooter != null
                    ? shooter.GetComponent<ShipAbility>()
                    : null;

                if (shooterAbility != null)
                {
                    shooterAbility.OnBulletHit(
                        combat,
                        damage
                    );
                }

                combat.TakeDamage(
                    damage
                );
            }

            Destroy(gameObject);
        }
    }
}