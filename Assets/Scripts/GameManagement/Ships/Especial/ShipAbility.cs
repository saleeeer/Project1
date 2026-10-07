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

    // Devuelve true si la habilidad reemplaza el ataque normal.
    public virtual bool TrySpecialAttack(
        ShipCombat target,
        float damage)
    {
        return false;
    }

    // Se ejecuta cuando un proyectil de esta nave impacta.
    public virtual void OnBulletHit(
        ShipCombat target,
        float damage)
    {
    }

    // Buffs para aliados cercanos.
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

    // Reducción de daño para aliados cercanos.
    public virtual float GetIncomingDamageMultiplier(
        Vector3 allyPosition)
    {
        return 1f;
    }
}