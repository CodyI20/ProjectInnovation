using Fusion;
using TMPro;
using UnityEngine;

public class TradeTimer : NetworkBehaviour
{
    [Networked] private string tradeTimeLeft { get; set; } // Synced timer value
    [SerializeField] private TextMeshProUGUI timerText;

    public void UpdateTextTimer(float updatedTime)
    {
        tradeTimeLeft = updatedTime.ToString("F0");
        RPC_UpdateTimerText();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_UpdateTimerText()
    {
        timerText.text = tradeTimeLeft;
    }
}
