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
    }

    void Start()
    {
        if (feedback != null)
            feedback.SetPersistent(true);
    }

    public override float GetIncomingDamageMultiplier(Vector3 allyPosition)
    {
        float distance = Vector2.Distance(transform.position, allyPosition);

        if (distance <= shieldRadius)
        {
            return incomingDamageMultiplier;
        }

        return 1f;
    }
}