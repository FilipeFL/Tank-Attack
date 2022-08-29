using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class RTSHealth : NetworkBehaviour
{    
    [SerializeField] private int maxHealth = 100;

    [SyncVar(hook = nameof(HandleHealthUpdate)), SerializeField]
    private int currentHealth = 100;

    public event Action ServerOnDie;
    public event Action ClientOnDie;
    public event Action<int, int> ClientOnHealthUpdate;

    #region Server
    public override void OnStartServer()
    {
        currentHealth = maxHealth;
        RTSUnitBase.ServerOnPlayerDie += ServerHandlePlayerDie;
    }

    public override void OnStopServer()
    {
        RTSUnitBase.ServerOnPlayerDie -= ServerHandlePlayerDie;
    }

    private void ServerHandlePlayerDie(int conId)
    {
        conId = connectionToClient.connectionId;
        dealDamage(conId);
    }

    [Server]
    public void dealDamage(int damage)
    {
        if(currentHealth > 0)
        {
            currentHealth = Mathf.Max(currentHealth - damage, 0);

            if(currentHealth == 0)
            {
                ServerOnDie?.Invoke();

                Debug.Log("Morri");
            }
        }
    }
    #endregion


    #region Client
    private void HandleHealthUpdate(int oldHealth, int newHealth)
    {
        ClientOnHealthUpdate?.Invoke(newHealth, maxHealth);
    }
    #endregion
}
