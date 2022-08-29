using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RTSUnitCommandGiver : MonoBehaviour
{
    [SerializeField] private RTSUnitSelectionHandler unitSelectionHandler;
    [SerializeField] private LayerMask layerMask = new LayerMask();

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        RTSGameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    public void OnDestroy()
    {
        RTSGameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    private void ClientHandleGameOver(string winnerName)
    {
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame == false)
        {
            return;
        }
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask) == false)
        {
            return;
        }

        if(hit.collider.TryGetComponent<RTSTargetable>(out RTSTargetable target) is true)
        {
            if(target.hasAuthority is false)
            {
                TryTarget(target);
                return;
            }
        }

        TryMove(hit.point);
    }

    private void TryTarget(RTSTargetable target)
    {
        foreach(RTSUnit unit in unitSelectionHandler.GetSelectedUnits())
        {
            unit.GetTargeter().CmdSetTarget(target.gameObject);
        }
    }

    private void TryMove(Vector3 point)
    {
       foreach(RTSUnit unit in unitSelectionHandler.GetSelectedUnits())
       {
            unit.GetRTSUnitMovement().CmdMove(point);
       }
    }
}
