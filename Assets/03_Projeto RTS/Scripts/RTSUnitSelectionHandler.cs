using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class RTSUnitSelectionHandler : MonoBehaviour
{
    [SerializeField] private RectTransform unitSelectionArea = null;
    private RTSPlayer player;
    private Vector2 startPosition;

    [SerializeField] private LayerMask layerMask = new LayerMask();
    private Camera mainCamera;
    [SerializeField]
    public List<RTSUnit> selectedUnits = new List<RTSUnit>();

    private void OnDestroy()
    {
        RTSUnit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;

        RTSGameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    private void AuthorityHandleUnitDespawned(RTSUnit unit)
    {
        selectedUnits.Remove(unit);
    }

    public List<RTSUnit> GetSelectedUnits()
    {
        return selectedUnits;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        
        RTSUnit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;

        RTSGameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void ClientHandleGameOver(string winnerName)
    {
        enabled = false;
    }

    private void Update()
    {
        if(player == null)
        {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Inicia área de seleção
            ClearSelectionArea();  

        }

        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            // Processar área de seleção
            ProcessSelectionArea();
        }

        else if (Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionArea();
        }
    }

    private void UpdateSelectionArea()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        float areaWidth = mousePosition.x - startPosition.x;
        float areaHeight = mousePosition.y - startPosition.y;

        unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));

        unitSelectionArea.anchoredPosition = startPosition + new Vector2(areaWidth / 2, areaHeight / 2);
    }

    private void ClearSelectionArea()
    {        
        if(Keyboard.current.leftShiftKey.isPressed is false)
        {
            foreach (RTSUnit aUnit in selectedUnits)
            {
                aUnit.DoDeSelect();
            }

            selectedUnits.Clear();
        }

        unitSelectionArea.gameObject.SetActive(true);
        startPosition = Mouse.current.position.ReadValue();
        
        UpdateSelectionArea();
    }

    private void ProcessSelectionArea()
    {
        unitSelectionArea.gameObject.SetActive(false);

        if (unitSelectionArea.sizeDelta.magnitude == 0)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask) == false)
            {
                return;
            }

            // Verifica se o clique NÃO foi em uma Unit
            RTSUnit unit;
            if (hit.collider.TryGetComponent<RTSUnit>(out unit) == false)
            {
                return;
            }

            if (unit.hasAuthority == false)
            {
                return;
            }

            selectedUnits.Add(unit);

            foreach (RTSUnit aUnit in selectedUnits)
            {
                aUnit.DoSelect();
            }

            return;
        }

        Vector2 min = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2);
        Vector2 max = unitSelectionArea.anchoredPosition + (unitSelectionArea.sizeDelta / 2);

        foreach(RTSUnit unit in player.GetMyUnits())
        {
            if (selectedUnits.Contains(unit) is false)
            {
                Vector3 unitScreenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);

                if (unitScreenPosition.x > min.x && unitScreenPosition.x < max.x && unitScreenPosition.y > min.y && unitScreenPosition.y < max.y)
                {
                    selectedUnits.Add(unit);
                    unit.DoSelect();
                }
            }
        }

    }
}
