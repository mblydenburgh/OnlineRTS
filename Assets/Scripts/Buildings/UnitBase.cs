using System;
using Mirror;
using UnityEngine;

public class UnitBase : NetworkBehaviour {

    [SerializeField] private Health health = null;

    public static event Action<UnitBase> ServerOnBaseSpawned;
    public static event Action<UnitBase> ServerOnBaseDespawned;
    public static event Action<int> ServerOnPlayerDie;

    #region Server

    public override void OnStartServer() {
        health.ServerOnDie += HandleServerOnDie;
        ServerOnBaseSpawned?.Invoke(this);
    }

    public override void OnStopServer() {
        ServerOnBaseDespawned?.Invoke(this);
        health.ServerOnDie -= HandleServerOnDie;
    }

    [Server]
    private void HandleServerOnDie() {
        ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);
        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region Client



    #endregion

}