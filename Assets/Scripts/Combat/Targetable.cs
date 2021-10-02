using Mirror;
using UnityEngine;

public class Targetable : NetworkBehaviour {

    [SerializeField] private Transform aimLocation = null;

    public Transform GetAimLocation() {
        return aimLocation;
    }

}