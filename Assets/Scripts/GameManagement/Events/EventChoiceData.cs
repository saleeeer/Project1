using UnityEngine;

[System.Serializable]
public class EventChoiceData
{
    public string choiceText;

    [Header("Required Stats")]
    public float requiredPower = 0f;
    public float requiredDefense = 0f;
    public float requiredIntelligence = 0f;
    public float requiredMorale = 0f;

    [Header("Success Rewards")]
    public int rewardCredits = 0;

    public float rewardPower = 0f;
    public float rewardDefense = 0f;
    public float rewardAccuracy = 0f;
    public float rewardMorale = 0f;
    public float rewardIntelligence = 0f;

    [Header("Failure Penalties")]
    public int penaltyCredits = 0;

    public float penaltyPower = 0f;
    public float penaltyDefense = 0f;
    public float penaltyAccuracy = 0f;
    public float penaltyMorale = 0f;
    public float penaltyIntelligence = 0f;

    [TextArea]
    public string successMessage;

    [TextArea]
    public string failureMessage;
}