using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour {
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;
    [SerializeField] private UnitMovement unitMovement = null;

    public UnitMovement GetUnitMovement() {
        return unitMovement;
    }

    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;
    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;

    #region Server

    public override void OnStartServer() {
        ServerOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopServer() {
        ServerOnUnitDespawned?.Invoke(this);
    }

    #endregion

    #region Client

    [Client]
    public override void OnStartClient() {
        if (!isClientOnly || !hasAuthority) return;

        AuthorityOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient() {
        if (!isClientOnly || !hasAuthority) return;

        AuthorityOnUnitDespawned?.Invoke(this);
    }

    public void Select() {
        if (!hasAuthority) return;

        onSelected?.Invoke();
    }

    [Client]
    public void Deselect() {
        if (!hasAuthority) return;

        onDeselected?.Invoke();
    }

    #endregion
}