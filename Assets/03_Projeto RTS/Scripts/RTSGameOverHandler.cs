using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class RTSGameOverHandler : NetworkBehaviour
{
    [SerializeField] List<RTSUnitBase> bases = new List<RTSUnitBase>();

    public static event Action ServerOnGameOver;
    public static event Action<string> ClientOnGameOver;

    #region Server
    public override void OnStartServer()
    {
        RTSUnitBase.ServerOnBaseSpawned += ServerHandleBaseSpawned;

        RTSUnitBase.ServerOnBaseDespawned += ServerHandleBaseDespawned;
    }

    public override void OnStopServer()
    {
        RTSUnitBase.ServerOnBaseSpawned -= ServerHandleBaseSpawned;

        RTSUnitBase.ServerOnBaseDespawned -= ServerHandleBaseDespawned;
    }

    [Server]
    private void ServerHandleBaseSpawned(RTSUnitBase unitBase)
    {
        bases.Add(unitBase);
    }

    private void ServerHandleBaseDespawned(RTSUnitBase unitBase)
    {
        bases.Remove(unitBase);

        if(bases.Count == 1)
        {
            int playerID = bases[0].connectionToClient.connectionId;

            RpcGameOver("{Jogador {playerId}}");

            ServerOnGameOver?.Invoke();
        }
    }
    #endregion

    #region Client
    [ClientRpc]
    private void  RpcGameOver(string winner)
    {
        ClientOnGameOver?.Invoke(winner);
    }

    public override void OnStartClient()
    {
        RTSUnitBase.ClientOnBaseSpawned += ClientHandleBaseSpawned;

        RTSUnitBase.ClientOnBaseDespawned += ClientHandleBaseDespawned;
    }

    public override void OnStopClient()
    {
        RTSUnitBase.ClientOnBaseSpawned -= ClientHandleBaseSpawned;

        RTSUnitBase.ClientOnBaseDespawned -= ClientHandleBaseDespawned;
    }

    [Client]
    private void ClientHandleBaseSpawned(RTSUnitBase unitBase)
    {
        bases.Add(unitBase);
    }

    private void ClientHandleBaseDespawned(RTSUnitBase unitBase)
    {
        bases.Remove(unitBase);

        if (bases.Count == 1)
        {
            int playerID = bases[0].connectionToClient.connectionId;

            RpcGameOver("{Jogador {playerId}}");

            ServerOnGameOver?.Invoke();
        }
    }
    #endregion
}
