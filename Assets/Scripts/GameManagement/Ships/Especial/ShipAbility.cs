using UnityEngine;

public class ShipAbility : MonoBehaviour
{
    public virtual float GetSpeedMultiplier()
    {
        return 1f;
    }

    public virtual float GetAttackRateMultiplier()
    {
        return 1f;
    }

    public virtual float GetDamageMultiplier()
    {
        return 1f;
    }

    public virtual bool TryEvadeDamage(float damage)
    {
        return false;
    }
}