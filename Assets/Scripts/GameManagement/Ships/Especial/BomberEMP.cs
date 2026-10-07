using UnityEngine;

public class BomberEMP : ShipAbility
{
    [Header("EMP")]
    public float duration = 3f;

    [Range(0.1f, 1f)]
    public float speedMultiplier = 0.5f;

    [Range(0.1f, 1f)]
    public float attackRateMultiplier = 0.5f;

    public override void OnBulletHit(
        ShipCombat target,
        float damage)
    {
        if (target == null)
            return;

        target.ApplyEMP(
            duration,
            speedMultiplier,
            attackRateMultiplier
        );
    }
}