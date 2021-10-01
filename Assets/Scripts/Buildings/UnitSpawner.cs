using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler {

    [SerializeField] private GameObject unitPrefab = null;
    [SerializeField] private Transform spawnLocation = null;
    
    #region Server

    [Command]
    private void CmdSpawnUnit() {
        GameObject unitInstance = Instantiate(unitPrefab, spawnLocation.position, spawnLocation.rotation);
        NetworkServer.Spawn(unitInstance, connectionToClient);
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