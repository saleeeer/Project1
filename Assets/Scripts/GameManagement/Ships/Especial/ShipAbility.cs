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

    public virtual bool TrySpecialAttack(
        ShipCombat target,
        float damage)
    {
        return false;
    }

    public virtual void OnBulletHit(
        ShipCombat target,
        float damage)
    {
    }

    public virtual float GetAllyDamageMultiplier(
        Vector3 allyPosition)
    {
        return 1f;
    }

    public virtual float GetAllyAttackRateMultiplier(
        Vector3 allyPosition)
    {
        return 1f;
    }

    public virtual float GetIncomingDamageMultiplier(
        Vector3 allyPosition)
    {
        return 1f;
    }
}