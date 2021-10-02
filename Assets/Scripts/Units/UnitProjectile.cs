using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour {
    [SerializeField] private Rigidbody rigidbody = null;
    [SerializeField] private float destroyAfterSeconds = 5f;
    [SerializeField] private float launchForce = 10f;

    private void Start() {
        rigidbody.velocity = transform.forward * launchForce;
    }

    public override void OnStartServer() {
        // When instantiated, call destroy self after x seconds
        Invoke(nameof(DestroySelf), destroyAfterSeconds);
    }

    [Server]
    private void DestroySelf() {
        NetworkServer.Destroy(gameObject);
    }
}