using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class State
{
    public string Key;
    public int Value;
    public bool IsPersistent;

    public State(string key, int value, bool isPersistent = false, bool isReusable = false)
    {
        this.Key = key;
        this.Value = value;
        this.IsPersistent = isPersistent;
    }
}

[Serializable]
public class StateController
{
    public List<State> States;

    public StateController()
    {
        States = new List<State>();
    }

    public void ModifyStates(IEnumerable<State> states)
    {
        foreach (State state in states)
        {
            ModifyState(state.Key, state.Value, state.IsPersistent);
        }
    }

    public void ModifyState(string key, int value, bool isPersistent)
    {
        State existingState = ContainsState(key);
        if (existingState == null)
            AddState(key, value, isPersistent);
        else
        {
            existingState.Value += value;

            if (existingState.Value == 0)
                States.Remove(existingState);
        }
    }

    private void AddState(string key, int value, bool isPersistent)
    {
        States.Add(new State(key, value, isPersistent));
    }

    public void RemoveState(string key)
    {
        State existingState = ContainsState(key);
        States.Remove(existingState);
    }

    public State ContainsState(string key)
    {
        return States.FirstOrDefault(x => x.Key == key);
    }

    public Dictionary<string, int> GetStates()
    {
        Dictionary<string, int> beliefs = new Dictionary<string, int>();

        foreach (State belief in States)
            beliefs.Add(belief.Key, belief.Value);

        return beliefs;
    }
}
