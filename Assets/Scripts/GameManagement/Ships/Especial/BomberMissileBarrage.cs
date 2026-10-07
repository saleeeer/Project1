using UnityEngine;

public class BomberMissileBarrage : ShipAbility
{
    [Header("Missile Barrage")]
    public int missileCount = 3;

    [Range(0.1f, 1f)]
    public float damagePerMissile = 0.45f;

    public float attackRateMultiplier = 0.8f;

    public override float GetAttackRateMultiplier()
    {
        return attackRateMultiplier;
    }

    public override bool TrySpecialAttack(
        ShipCombat target,
        float damage)
    {
        if (target == null)
            return false;

        for (int i = 0; i < missileCount; i++)
        {
            combatFallback(target, damage * damagePerMissile);
        }

        return true;
    }

    void combatFallback(
        ShipCombat target,
        float damage)
    {
        ShipCombat combat =
            GetComponent<ShipCombat>();

        if (combat != null)
        {
            combat.SpawnBullet(
                target,
                damage
            );
        }
    }
}