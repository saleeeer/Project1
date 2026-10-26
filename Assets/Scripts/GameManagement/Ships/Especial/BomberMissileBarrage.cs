
using UnityEngine;
using System.Collections;

public class BomberMissileBarrage : ShipAbility
{
    [Header("Rapid Fire")]
    [Range(0f, 1f)]
    public float activationChance = 0.25f;

    public float attackRateMultiplier = 1.5f;
    public float duration = 2f;

    private bool rapidFireActive = false;
    private Coroutine rapidFireCoroutine;
    private AbilityFeedback feedback;

    void Awake()
    {
        feedback = GetComponent<AbilityFeedback>();
    }

    public override float GetAttackRateMultiplier()
    {
        return rapidFireActive ? attackRateMultiplier : 1f;
    }

    public override void OnBulletHit(ShipCombat target, float damage)
    {
        // No uso el impacto para activar la habilidad.
        // La activación se decide al iniciar cada ataque.
    }

    public bool TryActivateRapidFire()
    {
        if (rapidFireActive || Random.value > activationChance)
            return false;

        if (rapidFireCoroutine != null)
            StopCoroutine(rapidFireCoroutine);

        rapidFireCoroutine = StartCoroutine(RapidFireRoutine());
        return true;
    }

    private IEnumerator RapidFireRoutine()
    {
        rapidFireActive = true;

        if (feedback != null)
            feedback.PlayOneShot();

        yield return new WaitForSeconds(duration);

        rapidFireActive = false;
        rapidFireCoroutine = null;
    }
}
