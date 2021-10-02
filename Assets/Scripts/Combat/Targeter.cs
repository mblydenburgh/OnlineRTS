using Mirror;
using UnityEngine;

public class Targeter : NetworkBehaviour {

    [SerializeField] private Targetable target = null;

    #region Server

    [Command]
    public void CmdSetTarget(GameObject targetGameObject) {
        Debug.Log("Attempting to set target");
        if (!targetGameObject.TryGetComponent(out Targetable newTarget)) {
            Debug.Log("Game object doesnt have targetable");
            return;
        }
        Debug.Log("Setting target");
        target = newTarget;
    }

    [Server]
    public void ClearTarget() {
        Debug.Log("Clearing target");
        target = null;
    }

    #endregion

    #region Client
    
    

    #endregion
    
}