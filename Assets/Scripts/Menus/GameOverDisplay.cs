using Mirror;
using TMPro;
using UnityEngine;

public class GameOverDisplay : MonoBehaviour {
    [SerializeField] private TMP_Text winnerText = null;
    [SerializeField] private GameObject gameOverDisplayParent = null;
    
    private void Start() {
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy() {
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void ClientHandleGameOver(string winner) {
        winnerText.text = $"{winner} has won the game.";
        gameOverDisplayParent.SetActive(true);
    }

    public void LeaveGame() {
        if (NetworkServer.active && NetworkClient.isConnected) {
            // stop hosting
            NetworkManager.singleton.StopHost();
        } else {
            // stop client
            NetworkManager.singleton.StopClient();
        }
    }
}