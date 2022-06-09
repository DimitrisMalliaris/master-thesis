using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public enum ActionStatus
{
    NoAction,
    Activated,
    Invoked,
    Completed,
    Failed,
    Cancelled
}

[Serializable]
public enum HandSide
{
    Right,
    Left,
    Both
}

[RequireComponent(typeof(Agent))]
[RequireComponent(typeof(NavigationController))]
public class ActionController : MonoBehaviour
{
    public ActionBase CurrentAction;
    public GameObject CurrentTarget;
    Agent Agent;
    NavigationController NavigationController;
    AnimationController AnimationController;

    [SerializeField] bool CanUpdate = true;
    [SerializeField] bool IsInvoked = false;
    [SerializeField] float UpdateInterval = 1f;

    [SerializeField] Transform Hand;

    public ActionStatus Status = ActionStatus.NoAction;

    private void Awake()
    {
        Agent = GetComponent<Agent>();
        NavigationController = GetComponent<NavigationController>();
        AnimationController = GetComponent<AnimationController>();
    }

    private void Update()
    {
        if(CurrentTarget)
        {
            // Rotate
            Vector3 direction;
            if (CurrentTarget == this.gameObject)
                direction = (Camera.main.transform.position - transform.position).normalized;
            else
                direction = (CurrentTarget.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    /// <summary>
    /// CheckNavigation() is invoked on regular intervals equal to UpdateIntervals property. The first invoke happens in ActivateAction(). 
    /// </summary>
    private void CheckNavigation()
    {
        switch (NavigationController.NavStatus)
        {
            case NavStatus.Pending:
                return;
            case NavStatus.Success:
                // Upon Reaching Destination
                // Hold Items
                if (CurrentAction.ActionParams.HoldItemPrefab)
                    HoldInHand(CurrentAction.ActionParams.HoldItemPrefab, HandSide.Right);
                // Plays Action Animation
                Agent.AnimationController.PlayAnimation(CurrentAction.ActionParams.AnimParams);
                // & Invoke Action after action Duration
                Status = ActionStatus.Invoked;
                Invoke(nameof(InvokeAction), CurrentAction.ActionParams.Duration);
                NavigationController.NavStatus = NavStatus.Pending;
                break;
            case NavStatus.Failure:
                OnActionFailed();
                break;
        }
    }

    /// <summary>
    /// Called when by active action when it is activated. Sets new target gameobject for the agent.
    /// </summary>
    /// <param name="newTarget"></param>
    public void SetTarget(GameObject newTarget)
    {
        CurrentTarget = newTarget;

        // Debug
        if (SimulationManager.Instance.Mode == SimMode.Debug)
            Debug.Log($"<color=ivory>{(CurrentTarget == null ? "Cannot set target: No available Target." : $"Target set to: {CurrentTarget.name}.")}</color>");
    }

    /// <summary>
    /// Sets the active action, activates it and enables all components (NavigationController, AnimationController, etc.).
    /// </summary>
    /// <param name="action"></param>
    public void ActivateAction(ActionBase action)
    {
        CurrentAction = action;

        // Activate Action
        action.Activate(Agent);

        // Check if Target is set
        if (CurrentTarget == null)
        {
            OnActionFailed();
            return;
        }

        // Activate Navigation
        NavigationController.Target = CurrentTarget;
        NavigationController.StoppingDistance = CurrentAction.ActionParams.ActionRange;

        // Play Navigation Animation
        AnimationController.PlayAnimation(CurrentAction.NavigationParams.AnimParams);

        Status = ActionStatus.Activated;

        // Activate Navigation Updates
        InvokeRepeating(nameof(CheckNavigation), UpdateInterval, UpdateInterval);

        // Debug
        if (SimulationManager.Instance.Mode == SimMode.Debug)
            Debug.Log("<color=ivory>Action Activated</color>");
    }

    /// <summary>
    /// Invokes the active action. This method should be Invoked by the ActionController.
    /// </summary>
    public void InvokeAction()
    {
        // Debug
        if (SimulationManager.Instance.Mode == SimMode.Debug)
            Debug.Log("<color=ivory>Action Invoked</color>");

        // Invokes Action
        CurrentAction.OnActionInvoked(Agent);

        // Empty Hands
        EmptyHand();
    }

    /// <summary>
    /// Called by the active action when the action is completed.
    /// </summary>
    public void OnActionCompleted()
    {
        // Cancels all update invokes
        CancelInvoke();

        // Remember Persisting Effects
        Agent.StateController.ModifyStates(CurrentAction.Effects.Where(x=>x.IsPersistent));

        CurrentAction = null;

        Status = ActionStatus.Completed;

        // Debug
        if (SimulationManager.Instance.Mode == SimMode.Debug)
            Debug.Log("<color=green>Action Success</color>");
    }

    /// <summary>
    /// Called when the action fails.
    /// </summary>
    public void OnActionFailed()
    {
        // Cancels all update invokes
        CancelInvoke();

        // Stop Navigation
        NavigationController.CancelNavigation();

        // Stop Playing Animation
        Agent.AnimationController.StopAnimation();

        CurrentAction = null;

        Status = ActionStatus.Failed;

        // Debug
        if (SimulationManager.Instance.Mode == SimMode.Debug)
            Debug.Log("<color=red>Action Failure</color>");
    }

    /// <summary>
    /// Stops all components (NavigationController, AnimationController, etc.) and resets the ActionController.
    /// </summary>
    public void CancelAction()
    {
        // Cancels all update invokes
        CancelInvoke();

        // Stop Navigation
        NavigationController.CancelNavigation();

        // Stop Playing Animation
        AnimationController.StopAnimation();

        // Cancel Current Action
        if (CurrentAction)
        {
            CurrentAction.OnActionCancelled(Agent);
            CurrentAction = null;
            CurrentTarget = null;
        }

        Status = ActionStatus.Cancelled;

        // Debug
        if (SimulationManager.Instance.Mode == SimMode.Debug)
            Debug.Log("<color=yellow>Action Cancel</color>");
    }

    public void HoldInHand(GameObject gameObject, HandSide handSide)
    {
        if (Hand == null)
            return;

        Instantiate(gameObject, Hand);
    }
    public void EmptyHand()
    {
        if (Hand == null)
            return;

        foreach (Transform child in Hand)
            GameObject.Destroy(child.gameObject);
    }
}
