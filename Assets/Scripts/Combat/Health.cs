using System;
using Mirror;
using UnityEngine;

public class Health : NetworkBehaviour {
    [SerializeField] private int maxHealth = 100;

    [SyncVar] private int currentHealth;

    public event Action ServerOnDie;

    #region Server

    public override void OnStartServer() {
        currentHealth = maxHealth;
        Debug.Log($"setting starting health: {currentHealth}");
    }

    [Server]
    public void DealDamage(int amount) {
        Debug.Log("dealing damage");
        if (currentHealth == 0) return;
        Debug.Log($"health before hit: {currentHealth}");
        currentHealth = Math.Max(0, currentHealth - amount);
        Debug.Log($"health after hit: {currentHealth}");
        
        if (currentHealth != 0) return;
        
        ServerOnDie?.Invoke();
        Debug.Log("we died :(");
    }

    #endregion

    #region Client

    

    #endregion
}