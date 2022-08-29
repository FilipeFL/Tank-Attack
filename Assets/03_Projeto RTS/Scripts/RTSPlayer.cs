using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] private List<RTSUnit> myUnits = new List<RTSUnit>();

    public List<RTSUnit> GetMyUnits()
    {
        return myUnits;
    }

    #region Server
    public override void OnStartServer()
    {
        RTSUnit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        RTSUnit.ServerOnUnitDeSpawned += ServerHandleUnitDespawned;
    }
    public override void OnStopServer()
    {
        RTSUnit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        RTSUnit.ServerOnUnitDeSpawned -= ServerHandleUnitDespawned;
    }
    public void ServerHandleUnitSpawned(RTSUnit unit)
    {
        if(unit.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }
        myUnits.Add(unit);
    }
    public void ServerHandleUnitDespawned(RTSUnit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }
        myUnits.Remove(unit);
    }
    #endregion

    #region Client
    public override void OnStartAuthority()
    {
        RTSUnit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        RTSUnit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
    }
    public override void OnStopClient()
    {
        if (hasAuthority is true)
        {
            RTSUnit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
            RTSUnit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        }
    }
    public void AuthorityHandleUnitSpawned(RTSUnit unit)
    {        
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }
        if (isClientOnly is true && hasAuthority is true)
        {
            myUnits.Add(unit);
        }
    }
    public void AuthorityHandleUnitDespawned(RTSUnit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }
        if (isClientOnly is true && hasAuthority is true)
        {
            myUnits.Add(unit);
        }
    }
    #endregion
}
