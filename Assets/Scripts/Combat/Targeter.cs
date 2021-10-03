using Mirror;
using UnityEngine;

public class Targeter : NetworkBehaviour {

    private Targetable target;

    public Targetable GetTarget() {
        return target;
    }

    #region Server

    public override void OnStartServer() {
        GameOverHandler.ServerOnGameOver += ServerHandleOnGameOver;
    }

    public override void OnStopServer() {
        GameOverHandler.ServerOnGameOver += ServerHandleOnGameOver;
    }

    [Command]
    public void CmdSetTarget(GameObject targetGameObject) {
        if (!targetGameObject.TryGetComponent(out Targetable newTarget)) return;
        target = newTarget;
    }

    [Server]
    public void ClearTarget() {
        target = null;
    }

    private void ServerHandleOnGameOver() {
        ClearTarget();
    }

    #endregion

}