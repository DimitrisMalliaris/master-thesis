using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(ActionController))]
[RequireComponent(typeof(NavigationController))]
[RequireComponent(typeof(AnimationController))]
public class Agent : MonoBehaviour
{
    public HealthSystem HealthSystem;

    // Controllers
    public ActionController ActionController;
    public NavigationController NavigationController;
    public AnimationController AnimationController;
    public StateController StateController;
    [SerializeField] State[] StartingStates;

    // Planner
    [SerializeField] private Planner Planner;
    [SerializeField] public List<ActionBase> Actions;
    [SerializeField] private Queue<ActionBase> ActionQueue;

    [SerializeField] public List<GoalBase> Goals;


    public bool DebugMode = false;

    public GoalBase ActiveGoal;

    void Start()
    {
        // Precache Controllers
        ActionController = GetComponent<ActionController>();
        NavigationController = GetComponent<NavigationController>();
        AnimationController = GetComponent<AnimationController>();
        
        // Init Agent's states
        StateController = new StateController();
        StateController.ModifyStates(StartingStates);

        // Precache Systems
        HealthSystem = GetComponent<HealthSystem>();

        // Initialize Planner
        Planner = new Planner(this);
    }

    internal void OnActiveGoalChanged(GoalBase newGoal)
    {
        ActionController.CancelAction();

        ActiveGoal = newGoal;
        ActionQueue = null;

        // Debug
        if (SimulationManager.Instance.Mode == SimMode.Debug)
            Debug.Log($"<color=ivory>Active Goal changed to: {newGoal.GoalName}</color>");
    }

    void Update()
    {
        // Is Agent alive?
        if (!HealthSystem.IsAlive)
            return;

        // Has active Goal
        if (!ActiveGoal)
            return;

        // If ActionQueue is null or empty or action failed and not action activated
        if (((ActionQueue == null || ActionQueue.Count <= 0) 
            && ActionController.Status != ActionStatus.Activated 
            && ActionController.Status != ActionStatus.Invoked) 
            || ActionController.Status == ActionStatus.Failed)
        {
            ActionQueue = Planner.Plan(Actions, ActiveGoal.subGoals, StateController.States);
            ActionController.Status = ActionStatus.NoAction;
            return;
        }

        // Debug Set Action
        if (ActionController.Status == ActionStatus.NoAction || ActionController.Status == ActionStatus.Completed)
            SetAction(ActionQueue.Dequeue());
    }

    public void SetAction(ActionBase action)
    {
        ActionController.ActivateAction(action);
    }

    public void CancelAction()
    {
        // Cancel Action
        ActionController.CancelAction();
    }

    public void Die()
    {
        CancelAction();
        Destroy(gameObject, 1f);
    }

    public void Memorize(IEnumerable<State> states)
    {
        if (DebugMode)
            Debug.Log($"{name} is memorizing {states.Count()} states.");

        StateController.ModifyStates(states);
    }

}
