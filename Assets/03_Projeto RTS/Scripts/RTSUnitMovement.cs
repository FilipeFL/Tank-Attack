using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class RTSUnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent = null;
    private Camera mainCamera;

    [SerializeField] private RTSTargeter targeter = null;

    [SerializeField] private float chaseRange = 10f;

    #region Server

    // A movimentação está acontecendo no server e
    // o client visualiza de volta a modunça de posição
    [Command]
    public void CmdMove(Vector3 position)
    {
        targeter.ClearTarget();
        
        if (NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            navMeshAgent.SetDestination(hit.position);
        }
    }

    [ServerCallback]
    public void Update()
    {
        RTSTargetable target = targeter.GetTarget();

        if (targeter.GetTarget() != null)
        {
            if((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)
            {
                navMeshAgent.SetDestination(target.transform.position);
            }
            else if (navMeshAgent.hasPath)
            {
                navMeshAgent.ResetPath();
            }

            return;
        }
        
        if (navMeshAgent.hasPath is true && (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance))
        {
            navMeshAgent.ResetPath();
        }
    }

    public override void OnStartServer()
    {
        RTSGameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        RTSGameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    private void ServerHandleGameOver()
    {
        navMeshAgent.ResetPath();
    }
    #endregion
}



