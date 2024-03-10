using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;

public class MultiplayerChat : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI messages;
    [SerializeField] private InputField inputField;
    [SerializeField] private TextMeshProUGUI username;


    public void CallMessageRpc()
    {
        string message = inputField.text;
        RPC_SendMessage(username.text, message);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SendMessage(string username, string message, RpcInfo rpcInfo = default)
    {
        messages.text += $"{username}: {message}\n";
    }
    
}
