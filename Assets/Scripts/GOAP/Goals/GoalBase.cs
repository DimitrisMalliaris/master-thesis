using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "GOAP/Goals/Basic Goal")]
public class GoalBase : ScriptableObject
{
    public string GoalName = "Goal";
    public float Priority = 1f;
    [Range(1f,100f)] public float UpperThreshold = 1f;
    [Range(1f, 100f)] public float LowerThreshold = 1f;
    public float BasePriority = 0f;
    public float PriorityIncreaseRate = 1f;
    public List<State> subGoals;
    public string StatusBarTag;
    public float MaxPriority = 100f;
    public bool DamagesHealth = false;
    public float DamageHealthThreshHold = 25f;

    /// <summary>
    /// Returns the goal's new priority.
    /// </summary>
    public virtual float GetPriority(Agent agent, float previousPriority)
    {
        float newPriority = Mathf.Min(previousPriority + PriorityIncreaseRate * 0.0167f, MaxPriority);

        return newPriority;
    }
}
