using Mirror;
using TMPro;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour {
    [SerializeField] private TMP_Text displayNameText = null;
    [SerializeField] private Renderer displayColor = null;
    [SyncVar(hook = nameof(HandleDisplayNameUpdate))] [SerializeField] private string playerName = "DefaultPlayer";
    [SyncVar(hook = nameof(HandleDisplayColorUpdate))] [SerializeField] private Color playerColor;
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    #region Server
    [Command]
    private void CmdSetDisplayName(string newName) {
        if (newName.Length <= 1 || newName.Length > 10) return;
        // Do server validation here
        RpcLogDisplayName(newName);
        SetDisplayName(newName);

    }

    [ClientRpc]
    private void RpcLogDisplayName(string logName) {
        Debug.Log($"{logName} says hello to the world.");
    } 
    
    [Server]
    public void SetDisplayName(string newPlayerName) {
        this.playerName = newPlayerName;
    }

    [Server]
    public void SetColor(Color color) {
        this.playerColor = color;
    }

    #endregion

    #region Client
    private void HandleDisplayColorUpdate(Color oldColor, Color newColor) {
        displayColor.material.SetColor(BaseColor, newColor);
    }

    private void HandleDisplayNameUpdate(string oldName, string newName) {
        displayNameText.text = newName;
    }

    [ContextMenu("Set Player Name")]
    private void SetPlayerName() {
        CmdSetDisplayName("Bitch ass nigga");
    }
    #endregion
    
}