using UnityEngine;

[System.Serializable]
public class EmpireStats
{
    public float power = 1f;
    public float defense = 1f;
    public float accuracy = 1f;
    public float morale = 1f; // autoestima
    public float intelligence = 1f;

    // 🔥 MULTIPLICADOR GLOBAL (por autoestima)
    public float GetGlobalMultiplier()
    {
        return Mathf.Clamp(morale, 0.5f, 2f);
    }
}