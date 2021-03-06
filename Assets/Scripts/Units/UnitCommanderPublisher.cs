using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommanderPublisher : MonoBehaviour {
    [SerializeField] private UnitSelectionHandler unitSelectionHandler = null;
    [SerializeField] private LayerMask layerMask;

    private Camera mainCamera;

    private void Start() {
        mainCamera = Camera.main;
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy() {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
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
        foreach (var unit in unitSelectionHandler.SelectedUnits) {
            Targeter unitTargeter = unit.GetTargeter();
            unitTargeter.CmdSetTarget(target.gameObject);
        }
    }

    private void ClientHandleGameOver(string name) {
        enabled = false;
    }
}