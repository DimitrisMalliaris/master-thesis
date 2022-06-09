using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node
{
    public Node Parent;
    public float Cost;
    public StateController States;
    public ActionBase Action;
    public int Depth;

    public Node(Node parent, float cost, StateController allStates, ActionBase action)
    {
        this.Parent = parent;
        this.Cost = cost;
        this.Action = action;
        this.States = allStates;
        if (parent == null)
            this.Depth = 1;
        else
            this.Depth = parent.Depth + 1;
    }
}

public class Planner
{
    private readonly Agent _Agent;

    private const int MaxDepth = 8;

    public Planner(Agent agent)
    {
        _Agent = agent;
    }

    public Queue<ActionBase> Plan(List<ActionBase> actions, List<State> goals, List<State> agentStates)
    {
        List<ActionBase> usableActions = new List<ActionBase>();
        foreach (ActionBase action in actions)
        {
            if (action.CanExecute(_Agent))
            {
                usableActions.Add(action);
            }
        }

        List<Node> leaves = new List<Node>();

        // Create state controller with agent states
        StateController currentStates = new StateController();
        currentStates.ModifyStates(agentStates);

        // Add world states not in currentStates dictionary
        List<State> worldStates = WorldManager.Instance.GetWorldStates();
        currentStates.ModifyStates(worldStates);

        // Debug
        if (SimulationManager.Instance.Mode == SimMode.Debug)
            DebugPrintStates(currentStates.States);

        if (GoalAchieved(goals, currentStates))
        {
            // Debug
            if (SimulationManager.Instance.Mode == SimMode.Debug)
                Debug.Log("<color=green>~ GOAL ALREADY ACHIEVED ~</color>");
            return null;
        }

        Node start = new Node(null, 0, currentStates, null);

        bool success = BuildGraph(start, leaves, usableActions, goals);

        if (!success)
        {
            // Debug
            if (SimulationManager.Instance.Mode == SimMode.Debug)
                Debug.Log("<color=red>~ NO PLAN ~</color>");
            return null;
        }

        Node cheapest = null;
        foreach (Node leaf in leaves)
        {
            if (cheapest == null)
            {
                cheapest = leaf;
            }
            else
            {
                if (leaf.Cost < cheapest.Cost)
                {
                    cheapest = leaf;
                }
            }
        }

        List<ActionBase> result = new List<ActionBase>();
        Node n = cheapest;
        while (n != null)
        {
            if (n.Action != null)
            {
                result.Insert(0, n.Action);
            }
            n = n.Parent;
        }

        Queue<ActionBase> queue = new Queue<ActionBase>();
        foreach (ActionBase action in result)
        {
            queue.Enqueue(action);
        }

        // Debug
        if (SimulationManager.Instance.Mode == SimMode.Debug)
        {
            string plan = "<color=ivory>The Plan is: ";
            foreach (ActionBase action in queue)
            {
                plan += action.name + ", ";
            }
            plan = plan.Trim();
            plan = plan.Trim(',');
            plan += "</color>";
            Debug.Log(plan);
        }

        return queue;
    }

    public bool BuildGraph(Node parent, List<Node> leaves, List<ActionBase> usableActions, List<State> goal)
    {
        bool foundPath = false;

        foreach (ActionBase action in usableActions)
        {
            // Check if current states meet the action's requirements
            if (action.Conditions.All(x=> parent.States.ContainsState(x.Key) != null))
            {
                if(SimulationManager.Instance.Mode == SimMode.Debug)
                    Debug.Log($"<color=green>Can do {action.name}</color>");

                // Clone parent current state but keep only persistent states
                StateController currentState = new StateController();
                currentState.ModifyStates(parent.States.States);

                // Add action effects to current states
                currentState.ModifyStates(action.Effects);

                // Create new child node
                Node node = new Node(parent, parent.Cost + action.GetCost(_Agent), currentState, action);

                // Check if goal is achieved or continue recursion
                if (GoalAchieved(goal, currentState))
                {
                    leaves.Add(node);
                    foundPath = true;

                    if (SimulationManager.Instance.Mode == SimMode.Debug)
                        Debug.Log($"<color=ivory>Leaf added to list of leaves.</color>");
                }
                else
                {
                    // DEPTH CHECK
                    if (node.Depth >= MaxDepth)
                        return false;

                    // Remove none persistent states inherited from parent node
                    currentState.States.RemoveAll(x=> !x.IsPersistent && parent.States.ContainsState(x.Key) != null);
                    foreach(State state in parent.States.States)
                    {
                        if (!state.IsPersistent)
                            currentState.RemoveState(state.Key);
                    }

                    // Debug
                    if (SimulationManager.Instance.Mode == SimMode.Debug)
                        DebugPrintStates(currentState.States);

                    // Continue to next child node
                    List<ActionBase> subSetOfActions = ActionSubset(usableActions, action);
                    bool found = BuildGraph(node, leaves, subSetOfActions, goal);
                    if (found)
                    {
                        foundPath = true;
                    }
                }
            }
            else
            {
                if (SimulationManager.Instance.Mode == SimMode.Debug)
                    Debug.Log($"<color=red>Cannot do {action.name}</color>");
            }
        }

        return foundPath;
    }

    private bool GoalAchieved(List<State> goalStates, StateController currentStateController)
    {
        foreach (State goalState in goalStates)
        {
            if (currentStateController.ContainsState(goalState.Key) == null)
                    return false;
        }

        return true;
    }

    private List<ActionBase> ActionSubset(List<ActionBase> actions, ActionBase actionToRemove)
    {
        List<ActionBase> subset = new List<ActionBase>();
        foreach (ActionBase action in actions)
        {
            if (!action.Equals(actionToRemove))
            {
                subset.Add(action);
            }
        }
        return subset;
    }

    private void DebugPrintStates(IEnumerable<State> states)
    {
        string debugLine = "<color=ivory>States: ";
        foreach (State state in states)
        {
            debugLine += state.Key + ", ";
        }
        debugLine = debugLine.Trim();
        debugLine = debugLine.Trim(',');
        debugLine += "</color>";
        Debug.Log(debugLine);
    }
}
