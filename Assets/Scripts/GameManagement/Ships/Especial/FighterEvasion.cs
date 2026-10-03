using UnityEngine;

public class FighterEvasion : ShipAbility
{
    [Header("Evasion")]
    [Range(0f, 1f)]
    public float evadeChance = 0.20f;

    public override bool TryEvadeDamage(float damage)
    {
        bool evade =
            Random.value < evadeChance;

        if (evade)
        {
            Debug.Log(
                name + " ESQUIVÓ un disparo"
            );
        }

        return evade;
    }
}