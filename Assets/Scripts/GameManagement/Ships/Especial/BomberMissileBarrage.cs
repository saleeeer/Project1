using UnityEngine;

public class BomberMissileBarrage : ShipAbility
{
    [Header("Missile Barrage")]
    public int missileCount = 3;

    [Range(0.1f, 1f)]
    public float damagePerMissile = 0.45f;

    public float attackRateMultiplier = 0.8f;

    AbilityFeedback feedback;

    void Awake()
    {
        feedback =
            GetComponent<AbilityFeedback>();
    }

    public override float GetAttackRateMultiplier()
    {
        return attackRateMultiplier;
    }

    public override bool TrySpecialAttack(
        ShipCombat target,
        float damage
    )
    {
        if (target == null)
            return false;

        if (feedback != null)
        {
            feedback.PlayOneShot();
        }

        for (int i = 0;
            i < missileCount;
            i++)
        {
            ShipCombat combat =
                GetComponent<ShipCombat>();

            if (combat != null)
            {
                combat.SpawnBullet(
                    target,
                    damage *
                    damagePerMissile
                );
            }
        }

        return true;
    }
}