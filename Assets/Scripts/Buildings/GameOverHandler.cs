using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour {

    private List<UnitBase> bases = new List<UnitBase>();

    public static event Action<string> ClientOnGameOver;

    #region Server

    public override void OnStartServer() {
        UnitBase.ServerOnBaseSpawned += HandleBaseSpawned;
        UnitBase.ServerOnBaseDespawned += HandleBaseDespawned;
    }

    public override void OnStopServer() {
        UnitBase.ServerOnBaseSpawned -= HandleBaseSpawned;
        UnitBase.ServerOnBaseDespawned -= HandleBaseDespawned;
    }

    [Server]
    private void HandleBaseSpawned(UnitBase unitBase) {
        Debug.Log("Adding base");
        bases.Add(unitBase);
        Debug.Log($"Total bases {bases.Count}");
    }
    
    [Server]
    private void HandleBaseDespawned(UnitBase unitBase) {
        Debug.Log("Removing bases");
        bases.Remove(unitBase);
        Debug.Log($"Base count: {bases.Count}");
        if (bases.Count == 1) {
            int winner = bases[0].connectionToClient.connectionId;
            RpcGameOver($"Player {winner}");
        }
    }

    #endregion

    #region Client

    [ClientRpc]
    private void RpcGameOver(string winnerName) {
        ClientOnGameOver?.Invoke(winnerName);
    }

    #endregion
}