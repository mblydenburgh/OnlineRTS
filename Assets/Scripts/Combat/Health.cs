using System;
using Mirror;
using UnityEngine;

public class Health : NetworkBehaviour {
    [SerializeField] private int maxHealth = 100;

    [SyncVar(hook = nameof(HandleHealthChanged))] private int currentHealth;

    public event Action ServerOnDie;
    public event Action<int, int> ClientOnHealthChanged;

    #region Server

    public override void OnStartServer() {
        currentHealth = maxHealth;
        UnitBase.ServerOnPlayerDie += ServerHandlePlayerDie;
    }

    public override void OnStopServer() {
        UnitBase.ServerOnPlayerDie -= ServerHandlePlayerDie;
    }

    [Server]
    public void DealDamage(int amount) {
        if (currentHealth == 0) return;
        currentHealth = Math.Max(0, currentHealth - amount);
        
        if (currentHealth != 0) return;
        ServerOnDie?.Invoke();
    }

    private void ServerHandlePlayerDie(int playerId) {
        if (connectionToClient.connectionId != playerId) return;
        DealDamage(maxHealth);
    }

    #endregion

    #region Client

    private void HandleHealthChanged(int oldHealth, int newHealth) {
        ClientOnHealthChanged?.Invoke(newHealth, maxHealth);
    }

    #endregion
}