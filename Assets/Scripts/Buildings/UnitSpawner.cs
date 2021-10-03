using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler {

    [SerializeField] private GameObject unitPrefab = null;
    [SerializeField] private Transform spawnLocation = null;
    [SerializeField] private Health health = null;
    
    #region Server

    public override void OnStartServer() {
        health.ServerOnDie += ServerHandleDie;
    }

    public override void OnStopServer() {
        health.ServerOnDie -= ServerHandleDie;
    }

    [Command]
    private void CmdSpawnUnit() {
        GameObject unitInstance = Instantiate(unitPrefab, spawnLocation.position, spawnLocation.rotation);
        NetworkServer.Spawn(unitInstance, connectionToClient);
    }

    [Server]
    private void ServerHandleDie() {
        // NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region Client

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) { return; }
        if (!hasAuthority) { return; }
        CmdSpawnUnit();
    }

    #endregion
}