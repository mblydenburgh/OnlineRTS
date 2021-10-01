using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommanderPublisher : MonoBehaviour {

    [SerializeField] private UnitSelectionHandler unitSelectionHandler = null;
    [SerializeField] private LayerMask layerMask;
    
    private Camera mainCamera;
    
    private void Start() {
        mainCamera = Camera.main;
    }

    private void Update() {
        if (!Mouse.current.rightButton.wasPressedThisFrame) { return; }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) { return; }

        TryMove(hit.point);
    }

    private void TryMove(Vector3 point) {
        foreach (var selectedUnit in unitSelectionHandler.SelectedUnits) {
            selectedUnit.GetUnitMovement().CmdMove(point);
        }
    }
}