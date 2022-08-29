using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System;

public class RTSNetworkManager : NetworkManager
{
    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    [SerializeField]
    private RTSGameOverHandler gameOverHandlerPrefab;

    [SerializeField]
    private GameObject unitSpawnerPrefab = null;

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        ClientOnDisconnected?.Invoke();
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Mapa_01"))
        {
            RTSGameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);

            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
        }
    }

    [Server]
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        GameObject unitSpawnerInstance = Instantiate(unitSpawnerPrefab,
           conn.identity.transform.position, conn.identity.transform.rotation);

        // De autoridade do objeto ao cliente que se conectou 
        // Faz o spawn para todos os clients
        NetworkServer.Spawn(unitSpawnerInstance, conn);

    }
}
