using UnityEngine;

public class FighterEvasion : ShipAbility
{
    [Header("Evasion")]
    [Range(0f, 1f)]
    public float evadeChance = 0.20f;

    [Header("Sprite Feedback")]
    public Color evadeColor = Color.white;

    [Range(0f, 1f)]
    public float evadeAlpha = 0.5f;

    public float feedbackDuration = 0.25f;

    AbilityFeedback feedback;

    void Awake()
    {
        feedback =
            GetComponent<AbilityFeedback>();
    }

    public override bool TryEvadeDamage(
        float damage
    )
    {
        bool evade =
            Random.value < evadeChance;

        if (evade)
        {
            if (feedback != null)
            {
                feedback.PlaySpriteFlash(
                    evadeColor,
                    evadeAlpha,
                    feedbackDuration
                );
            }
        }

        return evade;
    }
}