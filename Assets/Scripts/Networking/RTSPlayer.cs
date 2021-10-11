using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour {
    [SerializeField] private List<Building> buildings = new List<Building>();

    [SyncVar(hook = "ClientHandleResourcesUpdated")] private int resources = 500;
    
    private List<Unit> playerUnits = new List<Unit>();
    private List<Building> playerBuildings = new List<Building>();

    public event Action<int> ClientOnResourcesUpdated;

    public List<Unit> GetPlayerUnits() {
        return playerUnits;
    }

    public List<Building> GetPlayerBuildings() {
        return playerBuildings;
    }

    public int GetPlayerResources() {
        return resources;
    }

    [Server]
    public void SetPlayerResources(int newResources) {
        resources = newResources;
    }

    #region Server

    public override void OnStartServer() {
        Unit.ServerOnUnitSpawned += ServerHandlerUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandlerUnitDespawned;
        Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;
    }

    public override void OnStopServer() {
        Unit.ServerOnUnitSpawned -= ServerHandlerUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandlerUnitDespawned;
        Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
    }

    [Command]
    public void CmdTryPlaceBuilding(int buildingId, Vector3 location) {
        Building buildingToPlace = null;
        foreach (var building in buildings) {
            if (building.GetId() == buildingId) {
                buildingToPlace = building;
                break;
            }
        }

        if (buildingToPlace == null) return;
        GameObject buildingInstance =
            Instantiate(buildingToPlace.gameObject, location, buildingToPlace.transform.rotation);
        NetworkServer.Spawn(buildingInstance, connectionToClient);
    }

    private void ServerHandlerUnitSpawned(Unit unit) {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
        playerUnits.Add(unit);
    }

    private void ServerHandlerUnitDespawned(Unit unit) {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
        playerUnits.Remove(unit);
    }

    private void ServerHandleBuildingSpawned(Building building) {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;
        playerBuildings.Add(building);
    }

    private void ServerHandleBuildingDespawned(Building building) {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;
        playerBuildings.Remove(building);
    }

    #endregion

    #region Client

    public override void OnStartAuthority() {
        if (NetworkServer.active) return;
        Unit.AuthorityOnUnitSpawned += AuthorityHandlerUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandlerUnitDespawned;
        Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
    }

    public override void OnStopClient() {
        if (!isClientOnly || !hasAuthority) return;
        Unit.AuthorityOnUnitSpawned -= AuthorityHandlerUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandlerUnitDespawned;
        Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
    }
    
    private void ClientHandleResourcesUpdated(int oldResources, int newResources) {
        ClientOnResourcesUpdated?.Invoke(newResources);
    }

    private void AuthorityHandlerUnitSpawned(Unit unit) {
        playerUnits.Add(unit);
    }

    private void AuthorityHandlerUnitDespawned(Unit unit) {
        playerUnits.Remove(unit);
    }

    private void AuthorityHandleBuildingSpawned(Building building) {
        playerBuildings.Add(building);
    }

    private void AuthorityHandleBuildingDespawned(Building building) {
        playerBuildings.Remove(building);
    }

    #endregion
}