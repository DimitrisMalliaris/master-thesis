using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GOAP/Action/Attack Action")]
public class AttackAction : ActionBase
{
    [SerializeField] float damage = 20f;

    public override void Complete(Agent agent)
    {
        var targetHealth = agent.ActionController.CurrentTarget.GetComponent<HealthSystem>();
        targetHealth.TakeDamage(damage);
        base.Complete(agent);
    }
}
