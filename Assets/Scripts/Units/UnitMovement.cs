using System;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour {
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private float chaseRange = 1f;

    private Camera mainCamera;

    #region Server

    [ServerCallback]
    private void Update() {
        // Get current target
        Targetable target = targeter.GetTarget();
        
        // If there is one, chase logic
        if (target != null) {
            if ((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange) {
                // Chase
                agent.SetDestination(target.transform.position);
            } else if (agent.hasPath) {
                // Stop chasing
                agent.ResetPath();
            }
            return;
        }
        
        if (!agent.hasPath) return;
        if (agent.remainingDistance > agent.stoppingDistance) return;
        
        agent.ResetPath();
    }

    [Command]
    public void CmdMove(Vector3 position) {
        targeter.ClearTarget();
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) return;
        
        agent.SetDestination(hit.position);
    }

    #endregion
    
}
