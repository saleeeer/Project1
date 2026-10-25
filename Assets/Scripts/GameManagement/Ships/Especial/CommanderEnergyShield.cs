using UnityEngine;

public class CommanderEnergyShield : ShipAbility
{
    [Header("Energy Shield")]
    public float shieldRadius = 6f;

    [Range(0.1f, 1f)]
    public float incomingDamageMultiplier = 0.65f;

    AbilityFeedback feedback;

    void Awake()
    {
        feedback = GetComponent<AbilityFeedback>();

        Debug.Log(
            "[ESCUDO] " + name +
            " | AbilityFeedback encontrado: " +
            (feedback != null)
        );

        if (feedback != null)
        {
            feedback.SetPersistent(true);
            Debug.Log("[ESCUDO] Activando feedback persistente");
        }
    }

    public override float GetIncomingDamageMultiplier(Vector3 allyPosition)
    {
        float distance = Vector2.Distance(transform.position, allyPosition);

        if (distance <= shieldRadius)
        {
            Debug.Log(
                "[ESCUDO] " + name +
                " protege a un aliado. Distancia: " +
                distance.ToString("F2") +
                " | Multiplicador: " +
                incomingDamageMultiplier
            );

            return incomingDamageMultiplier;
        }

        return 1f;
    }
}