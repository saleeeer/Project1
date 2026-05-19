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

    [Header("Failure Penalties")]
    public int penaltyCredits = 0;

    [TextArea]
    public string successMessage;

    [TextArea]
    public string failureMessage;
}