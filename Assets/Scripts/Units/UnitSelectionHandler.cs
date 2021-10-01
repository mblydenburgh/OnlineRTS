using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour {
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private RectTransform unitSelectionArea = null;
    private Camera mainCamera;
    private RTSPlayer player;
    private Vector2 dragStartPosition;
    public List<Unit> SelectedUnits { get; } = new List<Unit>();

    private void Start() {
        mainCamera = Camera.main;
    }

    private void Update() {
        if (player == null) {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        }

        if (Mouse.current.leftButton.wasPressedThisFrame) {
            StartSelectionArea();
        } else if (Mouse.current.leftButton.wasReleasedThisFrame) {
            ClearSelectionArea();
        } else if (Mouse.current.leftButton.isPressed) {
            UpdateSelectionArea();
        }
    }

    private void StartSelectionArea() {
        // If shift isn't held down, drop selected before reselect
        if (!Keyboard.current.leftShiftKey.isPressed) {
            foreach (var selectedUnit in SelectedUnits) {
                selectedUnit.Deselect();
            }

            SelectedUnits.Clear();    
        }
        // Else shift is held down, select more without de-selecting.
        // Turn on selection drag box & save its initial start point
        unitSelectionArea.gameObject.SetActive(true);
        dragStartPosition = Mouse.current.position.ReadValue();
        UpdateSelectionArea();
    }

    private void UpdateSelectionArea() {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        float dragWidth = mousePosition.x - dragStartPosition.x;
        float dragHeight = mousePosition.y - dragStartPosition.y;

        unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(dragWidth), Mathf.Abs(dragHeight));
        unitSelectionArea.anchoredPosition = dragStartPosition + new Vector2(dragWidth / 2, dragHeight / 2);
    }

    private void ClearSelectionArea() {
        // Clear drag selection box
        unitSelectionArea.gameObject.SetActive(false);

        // Didn't drag, selected 1 unit
        if (unitSelectionArea.sizeDelta.magnitude == 0) {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            // Check if raycast hits something
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) return;

            // Check if that something is a unit
            if (!hit.collider.TryGetComponent(out Unit unit)) return;

            // Check if we own that unit
            if (!unit.hasAuthority) return;

            // Add the selected units to the selected list
            SelectedUnits.Add(unit);

            // Call each of those unit's individual Select method to enable selection sprite
            foreach (var selectedUnit in SelectedUnits) {
                selectedUnit.Select();
            }

            return;
        }
        
        // Select all player units that are inside the box at time of closing
        Vector2 min = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2);
        Vector2 max = unitSelectionArea.anchoredPosition + (unitSelectionArea.sizeDelta / 2);

        foreach (var playerUnit in player.GetPlayerUnits()) {
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(playerUnit.transform.position);

            if (screenPosition.x > min.x && screenPosition.x < max.x && screenPosition.y > min.y &&
                screenPosition.y < max.y) {
                SelectedUnits.Add(playerUnit);
                playerUnit.Select();
            }
        }
    }
}