using UnityEngine;

public class CommanderBattleAura : ShipAbility
{
    [Header("Battle Aura")]
    public float auraRadius = 6f;

    public float damageMultiplier = 1.20f;
    public float attackRateMultiplier = 1.15f;

    AbilityFeedback feedback;

    void Awake()
    {
        feedback = GetComponent<AbilityFeedback>();
        Debug.Log("[AURA] AbilityFeedback encontrado: " + (feedback != null));
    }

    void Start()
    {
        if (feedback != null)
            feedback.SetPersistent(true);
    }

    public override float GetAllyDamageMultiplier(
        Vector3 allyPosition
    )
    {
        float distance =
            Vector2.Distance(
                transform.position,
                allyPosition
            );

        if (distance <= auraRadius)
            return damageMultiplier;

        return 1f;
    }

    public override float GetAllyAttackRateMultiplier(Vector3 allyPosition)
    {
        float distance = Vector2.Distance(transform.position, allyPosition);

        if (distance <= auraRadius)
        {
            Debug.Log(
                "[AURA] " + name +
                " aumenta la velocidad de ataque de un aliado. Distancia: " +
                distance.ToString("F2") +
                " | Multiplicador: " +
                attackRateMultiplier
            );

            return attackRateMultiplier;
        }

        return 1f;
    }
}