using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GoToOrigin : MonoBehaviour
{
    void Start()
    {
        GetComponent<NavMeshAgent>().SetDestination(new Vector3(0, 0, 0));
    }
}
