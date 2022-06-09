using System;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public enum NavStatus
{
    Pending,
    Success,
    Failure
}

[RequireComponent(typeof(NavMeshAgent))]
public class NavigationController : MonoBehaviour
{
    // Pre-cached Navigation Settings
    public NavMeshAgent NavMeshAgent;
    public float StoppingDistance = 2f;
    [SerializeField] float UpdateInterval = 1f;

    // Navigation Data
    public GameObject Target;
    [SerializeField] bool DestinationSet = false;
    public NavStatus NavStatus = NavStatus.Pending;

    void Start()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // Is Target Null? || Is Path Pending?
        if (Target == null || NavMeshAgent.pathPending)
            return;

        // Is Target Within Reach?
        if (DestinationSet && Vector3.Distance(transform.position, Target.transform.position) <= StoppingDistance)
        {
            OnNavigationSuccessful();
            return;
        }

        if (!DestinationSet)
            SetDestination();

        // Debug
        if (SimulationManager.Instance.Mode == SimMode.Debug)
        {
            if (!NavMeshAgent.pathPending)
                Utilities.DrawPath(NavMeshAgent.path);
        }
    }

    /// <summary>
    /// Method is called when target destination is within stopping distance. Resets NavMeshAgent.
    /// </summary>
    private void OnNavigationSuccessful()
    {
        // Set Navigation Status
        NavStatus = NavStatus.Success;

        // Reset
        Target = null;
        CancelNavigation();

        // Debug
        if (SimulationManager.Instance.Mode == SimMode.Debug)
            Debug.Log("<color=green>Navigation Success.</color>");
    }

    /// <summary>
    /// Method is called when there is no available valid path to reach target destination. Resets NavMeshAgent.
    /// </summary>
    private void OnNavigationFailed()
    {
        // Set Navigation Status
        NavStatus = NavStatus.Failure;

        // Reset
        Target = null;
        CancelNavigation();

        // Debug
        if (SimulationManager.Instance.Mode == SimMode.Debug)
            Debug.Log("<color=red>Navigation Failed.</color>");
    }

    /// <summary>
    /// Resets NavMeshAgent's path.
    /// </summary>
    public void CancelNavigation()
    {
        // Clear the current path
        NavMeshAgent.ResetPath();

        // Cancel all path update invokes
        CancelInvoke();

        // Default values
        DestinationSet = false;

        // Debug
        if (SimulationManager.Instance.Mode == SimMode.Debug)
            Debug.Log("<color=yellow>Navigation Cancelled.</color>");
    }

    /// <summary>
    /// Method is called on Update when the DestinationSet property is set to false. The method then, invokes itself on regular intervals equal to UpdateInterval property.
    /// </summary>
    private void SetDestination()
    {
        CancelNavigation();

        Vector3 destination = Target.transform.position;
        NavMeshPath path = new NavMeshPath();
        // find nearest spot on navmesh and move there
        if (NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path) 
            && Vector3.Distance(path.corners[path.corners.Length-1], destination) <= StoppingDistance)
        {
            NavMeshAgent.SetPath(path);

            NavStatus = NavStatus.Pending;

            DestinationSet = true;

            // Invoke next path update
            Invoke(nameof(SetDestination), UpdateInterval);

            // Debug
            if (SimulationManager.Instance.Mode == SimMode.Debug)
                Debug.Log($"<color=Ivory>Destination Set: {path.corners[path.corners.Length-1]}</color>");

        }
        else
        {
            OnNavigationFailed();
        }
    }
}