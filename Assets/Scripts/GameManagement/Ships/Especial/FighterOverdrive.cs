
using UnityEngine;

public class FighterOverdrive : ShipAbility
{
    [Header("Overdrive")]
    public float speedMultiplier = 1.35f;
    public float attackRateMultiplier = 1.20f;
    public float damageMultiplier = 1.15f;

    [Header("Activation Chance")]
    [Range(0f, 1f)]
    public float activationChance = 0.30f;

    [Header("Sprite Feedback")]
    public Color overdriveColor = Color.cyan;

    [Range(0f, 1f)]
    public float overdriveAlpha = 1f;

    AbilityFeedback feedback;
    ShipMovement movement;

    bool wasInCombat = false;
    bool overdriveActive = false;

    void Awake()
    {
        movement = GetComponent<ShipMovement>();
        feedback = GetComponent<AbilityFeedback>();
    }

    void Update()
    {
        if (movement == null)
            return;

        bool inCombat = movement.HasCombatTarget;

        if (inCombat && !wasInCombat)
        {
            overdriveActive = Random.value < activationChance;

            if (overdriveActive)
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFX(
                        AudioManager.Instance.overdriveSound
                    );
                }
            }
        }

        if (!inCombat)
        {
            overdriveActive = false;
        }

        if (inCombat != wasInCombat && feedback != null)
        {
            feedback.SetSpriteFeedback(
                inCombat && overdriveActive,
                overdriveColor,
                overdriveAlpha
            );
        }

        wasInCombat = inCombat;
    }

    public override float GetSpeedMultiplier()
    {
        return overdriveActive ? speedMultiplier : 1f;
    }

    public override float GetAttackRateMultiplier()
    {
        return overdriveActive ? attackRateMultiplier : 1f;
    }

    public override float GetDamageMultiplier()
    {
        return overdriveActive ? damageMultiplier : 1f;
    }
}
