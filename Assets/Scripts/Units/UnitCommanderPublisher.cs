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
        // If the right mouse button wasn't pressed, dont do anything
        if (!Mouse.current.rightButton.wasPressedThisFrame) return;

        // If it was press, do a raycast
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) return;

        // If we hit something targetable, check if we own it. If true, move. If false, try to target
        if (hit.collider.TryGetComponent(out Targetable target)) {
            if (target.hasAuthority) {
                TryMove(hit.point);
                return;
            }

            Debug.Log("Hit enemy target");
            TryTarget(target);
            return;
        }

        // If not targetable, just move
        TryMove(hit.point);
    }

    private void TryMove(Vector3 point) {
        foreach (var selectedUnit in unitSelectionHandler.SelectedUnits) {
            selectedUnit.GetUnitMovement().CmdMove(point);
        }
    }

    private void TryTarget(Targetable target) {
        Debug.Log("Trying to target with selected units");
        foreach (var unit in unitSelectionHandler.SelectedUnits) {
            Targeter unitTargeter = unit.GetTargeter();
            Debug.Log("Got unit targeter");
            unitTargeter.CmdSetTarget(target.gameObject);
        }
    }
}