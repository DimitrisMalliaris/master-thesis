using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class Utilities
{
    public static GameObject GetAliveReachableGameObjectWithTagCloserToPosition(NavMeshAgent navMeshAgent, bool livingTarget, string tag, float range, bool first = false)
    {
        float minDistance = float.MaxValue;
        GameObject bestCandidateTarget = null;
        NavMeshPath path = new NavMeshPath();

        foreach (GameObject candidateTarget in GameObject.FindGameObjectsWithTag(tag))
        {
            if (livingTarget)
            {
                HealthSystem healthSystem = candidateTarget.GetComponent<HealthSystem>();

                // if target has health and is alive
                if (!healthSystem || !healthSystem.IsAlive)
                    continue;
            }

            // if there is a valid path
            if (!navMeshAgent.CalculatePath(candidateTarget.transform.position, path))
                continue;

            // if last valid corner of the path distance from target is within action range
            if (Vector3.Distance(candidateTarget.transform.position, path.corners[path.corners.Length - 1]) > range)
                continue;

            // if returns first target found
            if (first)
                return candidateTarget;

            // if there is no previous candidate
            if (!bestCandidateTarget)
            {
                minDistance = Vector3.Distance(navMeshAgent.transform.position, candidateTarget.transform.position);
                bestCandidateTarget = candidateTarget;
                continue;
            }

            // if minimum distance smaller or equal to candidate's distance
            if (minDistance <= Vector3.Distance(navMeshAgent.transform.position, candidateTarget.transform.position))
                continue;

            // replace candidate
            minDistance = Vector3.Distance(navMeshAgent.transform.position, candidateTarget.transform.position);
            bestCandidateTarget = candidateTarget;

        }

        return bestCandidateTarget;
    }

    public static void DrawPath(NavMeshPath path)
    {
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
        }
    }
}
