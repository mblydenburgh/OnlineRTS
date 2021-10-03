using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour {
    [SerializeField] private List<Unit> playerUnits = new List<Unit>();

    public List<Unit> GetPlayerUnits() {
        return playerUnits;
    }
    
    #region Server

    public override void OnStartServer() {
        Unit.ServerOnUnitSpawned += ServerHandlerUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandlerUnitDespawned;
    }

    public override void OnStopServer() {
        Unit.ServerOnUnitSpawned -= ServerHandlerUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandlerUnitDespawned;
    }

    private void ServerHandlerUnitSpawned(Unit unit) {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
        playerUnits.Add(unit);
    }

    private void ServerHandlerUnitDespawned(Unit unit) {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
        playerUnits.Remove(unit);
    }

    #endregion

    #region Client

    public override void OnStartAuthority() {
        if (NetworkServer.active) return;
        Unit.AuthorityOnUnitSpawned += AuthorityHandlerUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandlerUnitDespawned;
    }

    public override void OnStopClient() {
        if (!isClientOnly || !hasAuthority) return;
        Unit.AuthorityOnUnitSpawned -= AuthorityHandlerUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandlerUnitDespawned;
    }

    private void AuthorityHandlerUnitSpawned(Unit unit) {
        playerUnits.Add(unit);
    }
    
    private void AuthorityHandlerUnitDespawned(Unit unit) {
        playerUnits.Remove(unit);
    }

    #endregion
}