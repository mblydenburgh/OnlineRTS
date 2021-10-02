using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.Mathematics;
using UnityEngine;

public class UnitFiring : NetworkBehaviour {
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private GameObject projectilePrefab = null;
    [SerializeField] private Transform projectileSpawnPoint = null;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackRate = 1f;
    [SerializeField] private float rotationSpeed = 10f;

    private float lastTimeAttacked;

    [ServerCallback]
    private void Update() {
        Targetable target = targeter.GetTarget();
        if (target == null) return;

        // Check if in range
        if (!CanFireAtTarget()) return;

        // If in range, rotate toward target
        Quaternion targetRotation =
            Quaternion.LookRotation(target.transform.position - transform.position);
        transform.rotation =
            Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // If allowed to fire due to time since last attacked
        if (Time.time > (1 / attackRate) + lastTimeAttacked) {
            var projectileSpawnPosition = projectileSpawnPoint.position;
            // Get angle at which to fire at target
            Quaternion projectileRotation =
                Quaternion.LookRotation(target.GetAimLocation().position - projectileSpawnPosition);
            // Instantiate projectile instance
            GameObject projectileInstance =
                Instantiate(projectilePrefab, projectileSpawnPosition, projectileRotation);
            NetworkServer.Spawn(projectileInstance, connectionToClient);
            // Reset time since last attacked
            lastTimeAttacked = Time.time;
        }
    }

    [Server]
    private bool CanFireAtTarget() {
        return (targeter.GetTarget().transform.position - transform.position).sqrMagnitude <= attackRange * attackRange;
    }
}