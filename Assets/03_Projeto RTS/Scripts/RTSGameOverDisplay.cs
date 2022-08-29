using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RTSGameOverDisplay : MonoBehaviour
{
    [SerializeField] private GameObject gameOverDisplayObject;
    [SerializeField] private TMP_Text winnerNameText;
    
    // Start is called before the first frame update
    private void Start()
    {
        RTSGameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        RTSGameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    public void LeaveGame()
    {
        if(NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
    }

    private void ClientHandleGameOver(string winnerName)
    {
        winnerNameText.text = $"{winnerName} venceu!";

        gameOverDisplayObject.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LeaveGame();
        }
    }
}
