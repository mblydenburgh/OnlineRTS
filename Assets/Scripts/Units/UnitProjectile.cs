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
    
    [Server]
    private void OnTriggerEnter(Collider other) {
        Debug.Log("projectile hit collider");
        if (other.TryGetComponent(out NetworkIdentity networkIdentity)) {
            Debug.Log("projectile hit a network identity");
            if (networkIdentity.connectionToClient == connectionToClient) return;

            if (other.TryGetComponent(out Health health)) {
                Debug.Log("projectile hit health component");
                health.DealDamage(projectileDamage);
            }
        }
        Debug.Log("calling to destroying projectile");
        DestroySelf();
    }
    
    [Server]
    private void DestroySelf() {
        Debug.Log("destroying projectile");
        NetworkServer.Destroy(gameObject);
    }
}