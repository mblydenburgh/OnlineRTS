using System;
using Mirror;
using UnityEngine;

public class ResourceGenerator : NetworkBehaviour {

    [SerializeField] private Health health = null;
    [SerializeField] private int resourcePerTick = 10;
    [SerializeField] private float tickInterval = 5f;

    private float timer;
    private RTSPlayer player;

    public override void OnStartServer() {
        timer = tickInterval;
        player = connectionToClient.identity.GetComponent<RTSPlayer>();

        health.ServerOnDie += ServerHandleDie;
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer() {
        health.ServerOnDie -= ServerHandleDie;
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    #region Server

    [ServerCallback]
    private void Update() {
        timer -= Time.deltaTime;
        if (timer <= 0) {
            player.SetPlayerResources(player.GetPlayerResources() + resourcePerTick);
            timer += tickInterval;
        }
    }

    private void ServerHandleDie() {
        NetworkServer.Destroy(gameObject);
    }

    private void ServerHandleGameOver() {
        enabled = false;
    }

    #endregion
}