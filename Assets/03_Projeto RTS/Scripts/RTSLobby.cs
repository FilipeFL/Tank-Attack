using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class RTSLobby : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUI = null;
    
    // Start is called before the first frame update
    private void Start()
    {
        RTSNetworkManager.ClientOnConnected += HandleClientConnected;
    }

    private void OnDestroy()
    {
        RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
    }

    private void HandleClientConnected()
    {
        lobbyUI.SetActive(true);
    }

    public void LeaveLobby()
    {
        if(NetworkServer.active is true && NetworkClient.isConnected is true)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();

            SceneManager.LoadScene(0);
        }
    }
}
