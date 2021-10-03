using System;
using System.Collections.Generic;
using Mirror;

public class GameOverHandler : NetworkBehaviour {

    private List<UnitBase> bases = new List<UnitBase>();

    public static event Action ServerOnGameOver;
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
        bases.Add(unitBase);
    }
    
    [Server]
    private void HandleBaseDespawned(UnitBase unitBase) {
        bases.Remove(unitBase);
        if (bases.Count == 1) {
            int winner = bases[0].connectionToClient.connectionId;
            RpcGameOver($"Player {winner}");
        }
        ServerOnGameOver?.Invoke();
    }

    #endregion

    #region Client

    [ClientRpc]
    private void RpcGameOver(string winnerName) {
        ClientOnGameOver?.Invoke(winnerName);
    }

    #endregion
}