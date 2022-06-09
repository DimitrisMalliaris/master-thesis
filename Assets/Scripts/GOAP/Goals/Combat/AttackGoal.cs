using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GOAP/Goals/Attack Goal")]
public class AttackGoal : GoalBase
{
    [SerializeField] string enemyTag = "Enemy";
    [SerializeField] float detectionRange = 10f;

    public override float GetPriority(Agent agent, float previousPriority)
    {
        if (IsEnemyNear(agent))
            return float.MaxValue;
        else
            return float.MinValue;
    }

    private bool IsEnemyNear(Agent agent)
    {
        Bounds bounds = new Bounds(agent.transform.position, Vector3.one * detectionRange);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        foreach (var enemy in enemies)
        {
            if (bounds.Contains(enemy.transform.position))
                return true;
        }

        return false;
    }
}
