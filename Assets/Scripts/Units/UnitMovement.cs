using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour {
    [SerializeField] private NavMeshAgent agent = null;

    // private Camera mainCamera;

    #region Server

    [Command]
    public void CmdMove(Vector3 position) {
        Debug.Log("Processing move command");
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) {
            Debug.Log("Not a valid navmesh hit");
            return;
        }
        agent.SetDestination(hit.position);
    }

    #endregion

    // #region Client
    //
    // public override void OnStartAuthority() {
    //     mainCamera = Camera.main;
    // }
    //
    // [ClientCallback]
    // private void Update() {
    //     // Check if we have authority to move object
    //     if (!hasAuthority) { return; }
    //     //If we clicked r mouse button
    //     if (!Mouse.current.rightButton.wasPressedThisFrame) { return; }
    //     // If we clicked someone in the navmesh
    //     Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
    //     if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) { return; }
    //     
    //     CmdMove(hit.point);
    // }
    //
    // #endregion
}
