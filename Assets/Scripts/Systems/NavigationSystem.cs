using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class NavigationSystem2 : MonoBehaviour
{
    [SerializeField] private Agent _Agent;
    [SerializeField] private NavMeshAgent _NavMeshAgent;

    [SerializeField] private Vector3 _Destination;
    [SerializeField] private bool _DestinationSet = false;
    [SerializeField] private float _StoppingDistance = 0f;

    [SerializeField] private GameObject _Target;
    [SerializeField] private bool _IsTrackingTarget = false;

    [HideInInspector]
    public UnityEvent NavigationSuccessful;
    [HideInInspector]
    public UnityEvent NavigationUnsuccessful;

    private void Start()
    {
        _Agent = GetComponent<Agent>();
        _NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if destination is set
        if (!_DestinationSet)
        {
            if (_NavMeshAgent.hasPath)
                _NavMeshAgent.ResetPath();

            return;
        }

        // Check if destination is reached
        if(Vector3.Distance(_Agent.transform.position, _Destination) <= _StoppingDistance)
        {
            OnDestinationReached();

            return;
        }

        // Check if destination is unreachable
        if (_NavMeshAgent.isPathStale && !CanReachDestination(_Destination, _StoppingDistance))
        {
            OnDestinationUnreachable();

            return;
        }

        // If tracking target set new target position
        if (_IsTrackingTarget)
        {
            if (_Target == null)
            {
                OnDestinationUnreachable();

                return;
            }

            Vector3 targetPos = _Target.transform.position;

            // If target changed position
            if (Vector3.Distance(targetPos, _Destination) != 0.5f)
                SetDestination(_Target.transform.position, _StoppingDistance);
        }
    }

    public void OnDestinationReached()
    {
        if (_Agent.DebugMode)
            Debug.Log("Destination reached.");

        // Notify Agent
        NavigationSuccessful?.Invoke();

        // Set booleans
        OnNavigationReset();
    }

    public void OnDestinationChanged()
    {
        // Set booleans
        _DestinationSet = true;
    }

    public void OnDestinationUnreachable()
    {
        if (_Agent.DebugMode)
            Debug.Log("Destination unreacheable.");

        // Cancel Navigation
        OnNavigationCancel();

        // Notify Agent
        NavigationUnsuccessful?.Invoke();
    }

    public void OnNavigationCancel()
    {
        if (_Agent.DebugMode)
            Debug.Log("Navigation cancelled.");

        OnNavigationReset();
    }

    public void OnNavigationReset()
    {
        if (_Agent.DebugMode)
            Debug.Log("Navigation system reset.");

        // Reset all navigation parameters
        _DestinationSet = false;
        _Target = null;
        _IsTrackingTarget = false;

        _NavMeshAgent.ResetPath();
    }

    public void SetTargetToTrack(GameObject target, float stoppingDistance)
    {
        // Change tracking params
        _Target = target;
        _IsTrackingTarget = true;
        _StoppingDistance = stoppingDistance;

        // Start Navigating
        SetDestination(_Target.transform.position, _StoppingDistance);
    }

    public void SetDestination(Vector3 destination, float stoppingDistance)
    {
        // Check if destination is reachable
        if (!CanReachDestination(destination, stoppingDistance))
        {
            OnDestinationUnreachable();
            return;
        }

        // Set target destination
        _NavMeshAgent.SetDestination(destination);
        _Destination = destination;
        _StoppingDistance = stoppingDistance;

        OnDestinationChanged();
    }

    public bool CanReachDestination(Vector3 destination, float stoppingDistance)
    {
        NavMeshPath path = new NavMeshPath();

        _NavMeshAgent.CalculatePath(destination, path);

        switch (path.status)
        {
            case NavMeshPathStatus.PathComplete:
                return true;
            case NavMeshPathStatus.PathInvalid:
                return false;
            case NavMeshPathStatus.PathPartial:
                {
                    var lastWaypoint = path.corners[path.corners.Length - 1];

                    if (Vector3.Distance(destination, lastWaypoint) <= stoppingDistance)
                        return true;
                    else
                        return false;
                }
            default:
                return false;
        }
    }

}
