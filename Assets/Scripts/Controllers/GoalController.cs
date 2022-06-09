using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public enum GoalStatus
{
    NoGoal,
    GoalChanged,
    Failure
}

[Serializable]
public class GoalState
{
    GoalController goalController;
    public GoalBase Goal;
    public float Priority;
    [SerializeField] Image imageFill;

    public GoalState(GoalBase goal, GoalController goalController)
    {
        this.goalController = goalController;
        Goal = goal;
        Priority = goal.BasePriority;

        if (String.IsNullOrEmpty(goal.StatusBarTag))
            return;

        var statusGameObjects = GameObject.FindGameObjectsWithTag("Status");

        var taggedGameObject = statusGameObjects.FirstOrDefault(x => x.GetComponent<StatusBase>().StatusTag == Goal.StatusBarTag);

        if (!taggedGameObject)
            return;

        var status = taggedGameObject.GetComponent<StatusBase>();
        status.PointOfNeed = 1 - Goal.UpperThreshold/100;
        status.PointOfSatisfaction = 1 - Goal.LowerThreshold/100;
        imageFill = status.current;
    }

    public void UpdateUI()
    {
        if (!imageFill)
            return;

        imageFill.fillAmount = (Goal.MaxPriority - Priority) / Goal.MaxPriority;
        if(imageFill.fillAmount <= Goal.DamageHealthThreshHold / 100)
        {
            if (imageFill.color != Color.red)
            {
                imageFill.color = Color.red;

                if(Goal.DamagesHealth)
                    goalController.HealingFactor --;
            }
        }
        else
        {
            if (imageFill.color != Color.green)
            {
                imageFill.color = Color.green;

                if (Goal.DamagesHealth)
                    goalController.HealingFactor ++;
            }
        }
    }
}

[RequireComponent(typeof(Agent))]
public class GoalController : MonoBehaviour
{
    [SerializeField] Agent Agent;
    [SerializeField] List<GoalState> GoalStates;
    [SerializeField] float GoalUpdateInterval = 1f;
    [SerializeField] float HealthUpdateInterval = 2f;
    GoalState ActiveGoalState { get; set; } = null;

    public float HealingFactor = 0f;

    void Awake()
    {
        Agent = GetComponent<Agent>();

        GoalStates = new List<GoalState>();
        foreach (GoalBase goal in Agent.Goals)
            GoalStates.Add(new GoalState(goal, this));

        InvokeRepeating(nameof(UpdatePriorities), GoalUpdateInterval, GoalUpdateInterval);
        InvokeRepeating(nameof(ChangeAgentHealth), HealthUpdateInterval, HealthUpdateInterval);
    }

    void FixedUpdate()
    {
        // Select active goal based on priority
        GoalState newGoal = null;

        // Update goal priority
        foreach (GoalState goalState in GoalStates)
        {
            goalState.Priority = goalState.Goal.GetPriority(Agent, goalState.Priority);

            Debug.Log("State " + goalState.Goal.name + " updated.");

            if(newGoal == null 
                || (goalState.Priority > goalState.Goal.UpperThreshold
                && goalState.Goal.Priority > newGoal.Goal.Priority)
                || (goalState.Goal.Priority == newGoal.Goal.Priority
                && goalState.Priority > newGoal.Priority))
            {
                newGoal = goalState;
            }

            // Update UI
            goalState.UpdateUI();
        }

        if (Agent.ActiveGoal == newGoal.Goal)
            return;

        // Update Active Goal
        Agent.OnActiveGoalChanged(newGoal.Goal);
    }

    void UpdatePriorities()
    {
        List<State> statesToRemove = new List<State>();

        foreach(var state in Agent.StateController.States)
        {
            foreach (var goalState in GoalStates)
            {
                if (goalState.Goal.subGoals.Any(x => x.Key == state.Key))
                {
                    goalState.Priority = goalState.Goal.BasePriority;
                    statesToRemove.Add(state);
                    break;
                }
            }
        }

        foreach(var state in statesToRemove)
        {
            Agent.StateController.RemoveState(state.Key);
        }
    }

    void ChangeAgentHealth()
    {
        if (HealingFactor > 0)
            Agent.HealthSystem.Heal(HealingFactor);
        else if (HealingFactor < 0)
            Agent.HealthSystem.TakeDamage(-HealingFactor);
    }
}
