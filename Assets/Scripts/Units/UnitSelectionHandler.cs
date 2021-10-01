using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour {
    [SerializeField] private LayerMask layerMask;
    private Camera mainCamera;
    public List<Unit> SelectedUnits { get; } = new List<Unit>();

    private void Start() {
        mainCamera = Camera.main;
    }

    private void Update() {
        if (Mouse.current.leftButton.wasPressedThisFrame) {
            StartSelectionArea();
        } else if (Mouse.current.leftButton.wasReleasedThisFrame) {
            ClearSelectionArea();
        }
    }

    private void StartSelectionArea() {
        foreach (var selectedUnit in SelectedUnits) {
            selectedUnit.Deselect();
        }

        SelectedUnits.Clear();
    }

    private void ClearSelectionArea() {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        // Check if raycast hits something
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) {
            return;
        }

        // Check if that something is a unit
        if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) {
            return;
        }

        // Check if we own that unit
        if (!unit.hasAuthority) {
            return;
        }

        // Add the selected units to the selected list
        SelectedUnits.Add(unit);

        // Call each of those unit's individual Select method to enable selection sprite
        foreach (var selectedUnit in SelectedUnits) {
            selectedUnit.Select();
        }
    }
}