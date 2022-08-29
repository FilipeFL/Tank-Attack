using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RTSMainMenu : MonoBehaviour
{
    [SerializeField] private GameObject panelMenu = null;
    [SerializeField] private TMP_InputField enderecoHost = null;
    [SerializeField] private Button joinButton = null;

    private void OnEnable()
    {
        RTSNetworkManager.ClientOnConnected += HandleClientConnected;
        RTSNetworkManager.ClientOnDisconnected += HandleClientDisconnected;
    }

    private void OnDisable()
    {
        RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
        RTSNetworkManager.ClientOnDisconnected -= HandleClientDisconnected;
    }

    public void HostLobby()
    {
        panelMenu.SetActive(false);

        NetworkManager.singleton.StartHost();
    }

    public void JoinServer()
    {
        string enderecoServer = enderecoHost.text;
        NetworkManager.singleton.networkAddress = enderecoServer;

        NetworkManager.singleton.StartClient();

        joinButton.interactable = false;
    }

    public void HandleClientConnected()
    {
        joinButton.interactable = true;
        gameObject.SetActive(false);
        panelMenu.SetActive(false);
    }

    public void HandleClientDisconnected()
    {
        joinButton.interactable = true;
    }
}
