using UnityEngine;

public class FighterOverdrive : ShipAbility
{
    [Header("Overdrive")]
    public float speedMultiplier = 1.35f;
    public float attackRateMultiplier = 1.20f;
    public float damageMultiplier = 1.15f;

    [Header("Sprite Feedback")]
    public Color overdriveColor = Color.cyan;
    [Range(0f, 1f)]
    public float overdriveAlpha = 1f;

    AbilityFeedback feedback;

    ShipMovement movement;

    bool wasInCombat = false;

    void Awake()
    {
        movement = GetComponent<ShipMovement>();
        feedback = GetComponent<AbilityFeedback>();
    }

    void Update()
    {
        if (movement == null)
            return;

        bool inCombat =
            movement.HasCombatTarget;

        if (feedback != null && inCombat != wasInCombat)
        {
            feedback.SetSpriteFeedback(
                inCombat,
                overdriveColor,
                overdriveAlpha
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
