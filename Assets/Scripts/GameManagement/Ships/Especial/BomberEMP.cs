using UnityEngine;

public class BomberEMP : ShipAbility
{
    [Header("EMP")]
    public float duration = 3f;

    [Range(0.1f, 1f)]
    public float speedMultiplier = 0.5f;

    [Range(0.1f, 1f)]
    public float attackRateMultiplier = 0.5f;

    public GameObject empImpactPrefab;

    public override void OnBulletHit(
        ShipCombat target,
        float damage
    )
    {
        if (target == null)
            return;

        if (empImpactPrefab != null)
        {
            GameObject effect =
                Instantiate(
                    empImpactPrefab,
                    target.transform.position,
                    Quaternion.identity
                );

            Destroy(
                effect,
                3f
            );
        }

        target.ApplyEMP(
            duration,
            speedMultiplier,
            attackRateMultiplier
        );
    }
}