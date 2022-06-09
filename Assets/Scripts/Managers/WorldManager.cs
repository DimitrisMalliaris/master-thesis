using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StateController))]
public class WorldManager : MonoBehaviour
{
    private static WorldManager _Instance;
    public static WorldManager Instance { get => _Instance; }

    private StateController _WorldStates;

    // Start is called before the first frame update
    void Start()
    {
        _Instance = this;
        _WorldStates = new StateController();
    }

    public List<State> GetWorldStates()
    {
        if (_WorldStates != null)
            return _WorldStates.States;
        else
            return new List<State>();
    }
}
