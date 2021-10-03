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
    }

    [Server]
    public void DealDamage(int amount) {
        Debug.Log("dealing damage");
        if (currentHealth == 0) return;
        currentHealth = Math.Max(0, currentHealth - amount);
        Debug.Log($"health after hit: {currentHealth}");
        
        if (currentHealth != 0) return;
        
        ServerOnDie?.Invoke();
        Debug.Log("we died :(");
    }

    #endregion

    #region Client

    private void HandleHealthChanged(int oldHealth, int newHealth) {
        ClientOnHealthChanged?.Invoke(newHealth, maxHealth);
    }

    #endregion
}