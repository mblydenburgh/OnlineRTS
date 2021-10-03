using Mirror;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour {
    [SerializeField] private Rigidbody rb = null;
    [SerializeField] private float destroyAfterSeconds = 5f;
    [SerializeField] private float launchForce = 10f;
    [SerializeField] private int projectileDamage = 20;

    private void Start() {
        rb.velocity = transform.forward * launchForce;
    }

    public override void OnStartServer() {
        // When instantiated, call destroy self after x seconds
        Invoke(nameof(DestroySelf), destroyAfterSeconds);
    }
    
    
    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out NetworkIdentity networkIdentity)) {
            if (networkIdentity.connectionToClient == connectionToClient) return;

            if (other.TryGetComponent(out Health health)) {
                health.DealDamage(projectileDamage);
            }
        }
        DestroySelf();
    }
    
    [Server]
    private void DestroySelf() {
        NetworkServer.Destroy(gameObject);
    }
}