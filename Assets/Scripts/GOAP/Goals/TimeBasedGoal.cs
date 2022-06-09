using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GOAP/Goals/Time-Based Goal")]
public class TimeBasedGoal : GoalBase
{
    [SerializeField] float DayFactor = 1f;
    [SerializeField] float NightFactor = 1f;

    public override float GetPriority(Agent agent, float previousPriority)
    {
        if (DayNightManager.Instance == null)
            return previousPriority;

        if (DayNightManager.Instance.Status == DayStatus.Day)
            return previousPriority + PriorityIncreaseRate * DayFactor / DayNightManager.MinutesInDay;

        if (DayNightManager.Instance.Status == DayStatus.Night)
            return previousPriority + PriorityIncreaseRate * NightFactor / DayNightManager.MinutesInDay;

        return previousPriority;
    }
}
