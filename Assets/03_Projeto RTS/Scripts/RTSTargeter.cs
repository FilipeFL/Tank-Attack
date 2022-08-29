using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class RTSTargeter : NetworkBehaviour
{
    [SerializeField] private RTSTargetable target;

    public RTSTargetable GetTarget()
    {
        return target;
    }

    [Command]
    public void CmdSetTarget(GameObject targetGameObject)
    {
        if(targetGameObject.TryGetComponent<RTSTargetable>(out RTSTargetable target) is true)
        {
            this.target = target;
        }
    }

    #region Server
    [Server]
    public void ClearTarget()
    {
        target = null;
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
        ClearTarget();
    }
    #endregion
}
