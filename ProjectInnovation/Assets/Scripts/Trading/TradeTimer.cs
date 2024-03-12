using Fusion;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TradeTimer : NetworkBehaviour
{
    [Networked] private bool isTimerActive { get; set; } = false;
    private TMP_Text timerText { get; set; }

    public override void Spawned()
    {
        base.Spawned();
        timerText = GetComponent<TMP_Text>();
        isTimerActive = true;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_UpdateTimerText(float updatedTime)
    {
        timerText.text = updatedTime.ToString("F0");
    }

    public override void Render()
    {
        if (isTimerActive)
        {
            RPC_UpdateTimerText(TradeManager.Instance.CurrentTradeTimeLeft);
        }
    }
}
