using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class RTSUnitBase : NetworkBehaviour
{
    [SerializeField] private RTSHealth health;

    public static event Action<int> ServerOnPlayerDie;
    public static event Action<int> ClientOnPlayerDie;
    public static event Action<RTSUnitBase> ServerOnBaseSpawned;
    public static event Action<RTSUnitBase> ServerOnBaseDespawned;
    public static event Action<RTSUnitBase> ClientOnBaseSpawned;
    public static event Action<RTSUnitBase> ClientOnBaseDespawned;

    #region Server
    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDie;

        ServerOnBaseSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnBaseDespawned?.Invoke(this);

        health.ServerOnDie -= ServerHandleDie;
    }

    [Server]
    private void ServerHandleDie()
    {
        ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);
        NetworkServer.Destroy(gameObject);
    }
    #endregion

    #region Client
    public override void OnStartClient()
    {
        health.ClientOnDie += ServerHandleDie;

        ClientOnBaseSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        ServerOnBaseDespawned?.Invoke(this);

        health.ClientOnDie -= ServerHandleDie;
    }

    [Server]
    private void ClientHandleDie()
    {
        ClientOnPlayerDie?.Invoke(connectionToClient.connectionId);
        NetworkServer.Destroy(gameObject);
    }
    #endregion
}
