using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class MultiplayerChat : NetworkBehaviour
{
    public Text messages;
    public InputField inputField;
    public InputField usernameInput;
    public string username = "Player";

    public void SetUsername()
    {
        messages.text += $"{username} changed their name to {usernameInput.text}\n";
        username = usernameInput.text;
    }

    public void CallMessageRpc()
    {
        string message = inputField.text;
        RPC_SendMessage(username, message);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SendMessage(string username, string message, RpcInfo rpcInfo = default)
    {
        messages.text += $"{username}: {message}\n";
    }
    
}
