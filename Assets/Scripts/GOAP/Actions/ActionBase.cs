using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimationParameters
{
    public string Animation = "";
    public string DefaultAnimation = "Idle";
    public float Duration = 3f;
    public bool Loop = false;
}

[Serializable]
public class ActionParameters
{
    public string Name = "Action Name";
    public string TargetTag = "Target";
    public bool LivingTarget = false;
    public float ActionRange = 2f;
    public float Cost = 1f;
    public float Duration = 3f;
    public AnimationParameters AnimParams;
    public GameObject HoldItemPrefab;
    public bool DestroyTargetAfterComplete = false;
}

[Serializable]
public class NavigationParameters
{
    public float NavigationSpeed = 3.5f;
    public float CostFactor = 3.5f;
    public AnimationParameters AnimParams;
}

[CreateAssetMenu(menuName = "GOAP/Action/Basic Action")]
public class ActionBase : ScriptableObject
{
    public ActionParameters ActionParams;
    public NavigationParameters NavigationParams;
    public State[] Conditions;
    public State[] Effects;

    /// <summary>
    /// Determines if the action can be executed.
    /// </summary>
    public virtual bool CanExecute(Agent agent)
    {
        GameObject target = Utilities.GetAliveReachableGameObjectWithTagCloserToPosition(
            agent.NavigationController.NavMeshAgent,
            ActionParams.LivingTarget,
            ActionParams.TargetTag,
            ActionParams.ActionRange,
            true);
        return target != null;
    }
    /// <summary>
    /// Determines the cost of the action.
    /// </summary>
    public virtual float GetCost(Agent agent)
    {
        float targetDistance = Vector3.Distance(agent.transform.position, GetTarget(agent).transform.position);
        return ActionParams.Cost + targetDistance * NavigationParams.CostFactor;
    }
    /// <summary>
    /// Is called when the action is activated.
    /// </summary>
    public virtual void Activate(Agent agent) => agent.ActionController.SetTarget(GetTarget(agent));
    /// <summary>
    /// Is called when the action is invoked.
    /// </summary>
    public virtual void OnActionInvoked(Agent agent) => Complete(agent);
    /// <summary>
    /// Is called when the action is completed.
    /// </summary>
    public virtual void Complete(Agent agent)
    {
        if (ActionParams.DestroyTargetAfterComplete == true)
            GameObject.Destroy(agent.ActionController.CurrentTarget.gameObject);

        agent.ActionController.OnActionCompleted();
    }
    /// <summary>
    /// Is called when the action is cancelled.
    /// </summary>
    public virtual void OnActionCancelled(Agent agent) { }
    /// <summary>
    /// Returns the GameObject that is the Action's target.
    /// </summary>
    public virtual GameObject GetTarget(Agent agent)
    {
        var target = Utilities.GetAliveReachableGameObjectWithTagCloserToPosition(agent.NavigationController.NavMeshAgent, ActionParams.LivingTarget, ActionParams.TargetTag, ActionParams.ActionRange);
        return target;
    }
}
