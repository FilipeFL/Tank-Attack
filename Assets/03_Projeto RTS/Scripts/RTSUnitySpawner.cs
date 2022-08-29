using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RTSUnitySpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField]
    private RTSHealth health;
    [SerializeField]
    private GameObject unitPrefab = null;
    [SerializeField]
    private Transform unitSpawnPosition = null;

    #region Server
    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDeath;
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDeath;
    }

    [Server]
    private void ServerHandleDeath()
    {
        FindObjectOfType<AudioManager>().Play("Death");
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    public void CmdSpawnUnit()
    {
        GameObject unitInstance = Instantiate(unitPrefab, 
            unitSpawnPosition.position, unitSpawnPosition.rotation);

        // De autoridade do objeto ao cliente que se conectou 
        NetworkServer.Spawn(unitInstance, connectionToClient);
    }
    #endregion

    #region Client
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Entrei no OnPointerClick");
        if (hasAuthority is false)
        {
            return;
        }
        Debug.Log("Tenho autoridade no OnPointerClick");
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        CmdSpawnUnit();
    }
    #endregion

}
