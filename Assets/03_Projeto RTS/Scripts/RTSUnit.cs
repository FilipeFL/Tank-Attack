using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RTSUnit : NetworkBehaviour
{
    [SerializeField] private RTSHealth health;
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeSelected = null;
    [SerializeField] private RTSUnitMovement unitMovement = null;
    [SerializeField] private RTSTargeter targeter = null;

    public static event Action<RTSUnit> ServerOnUnitSpawned;
    public static event Action<RTSUnit> ServerOnUnitDeSpawned;

    public static event Action<RTSUnit> AuthorityOnUnitSpawned;
    public static event Action<RTSUnit> AuthorityOnUnitDespawned;

    public RTSTargeter GetTargeter()
    {
        return targeter;
    }

    #region Server
    public override void OnStartServer()
    {
        ServerOnUnitSpawned?.Invoke(this);
        health.ServerOnDie += ServerHandleDeath;
    }
    public override void OnStopServer()
    {
        ServerOnUnitDeSpawned?.Invoke(this);
        health.ServerOnDie -= ServerHandleDeath;
    }

    [Server]
    private void ServerHandleDeath()
    {
        FindObjectOfType<AudioManager>().Play("Death");
        NetworkServer.Destroy(gameObject);
    }
    #endregion

    #region Client
    public override void OnStartAuthority()
    {
        AuthorityOnUnitSpawned?.Invoke(this);
    }
    public override void OnStopClient()
    {
        if (hasAuthority is true)
        {
            AuthorityOnUnitDespawned?.Invoke(this);
        }
    }
    #endregion

    public RTSUnitMovement GetRTSUnitMovement()
    {
        return unitMovement;
    }


    [Client]
    public void DoSelect()
    {
        if (hasAuthority is true)
        {
            onSelected?.Invoke();
        }
    }

    [Client]
    public void DoDeSelect()
    {
        if (hasAuthority is true)
        {
            onDeSelected?.Invoke();
        }
    }
}
