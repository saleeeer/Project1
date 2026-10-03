using UnityEngine;

public class FighterOverdrive : ShipAbility
{
    [Header("Overdrive")]
    public float speedMultiplier = 1.35f;
    public float attackRateMultiplier = 1.20f;
    public float damageMultiplier = 1.15f;

    ShipMovement movement;

    bool wasInCombat = false;

    void Awake()
    {
        movement = GetComponent<ShipMovement>();
    }

    void Update()
    {
        if (movement == null)
            return;

        bool inCombat =
            movement.HasCombatTarget;

        if (inCombat && !wasInCombat)
        {
            Debug.Log(
                name + " activó OVERDRIVE"
            );
        }

        if (!inCombat && wasInCombat)
        {
            Debug.Log(
                name + " desactivó OVERDRIVE"
            );
        }

        wasInCombat = inCombat;
    }

    public override float GetSpeedMultiplier()
    {
        if (movement != null &&
            movement.HasCombatTarget)
        {
            return speedMultiplier;
        }

        return 1f;
    }

    public override float GetAttackRateMultiplier()
    {
        if (movement != null &&
            movement.HasCombatTarget)
        {
            return attackRateMultiplier;
        }

        return 1f;
    }

    public override float GetDamageMultiplier()
    {
        if (movement != null &&
            movement.HasCombatTarget)
        {
            return damageMultiplier;
        }

        return 1f;
    }
}
