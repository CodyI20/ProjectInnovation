using UnityEngine;
using TMPro;
using Fusion;

public class TurnTimerUI : NetworkBehaviour
{
    [Networked] private bool isTimerActive { get; set; } = false;
    private float currentTurnTime;

    [Networked] public NetworkObject TurnTimer { get; set; }
    private TMP_Text timerText { get; set; }


    public override void Spawned()
    {
        base.Spawned();
        timerText = GetComponent<TMP_Text>();
        gameObject.SetActive(false);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SetGameObjectActive()
    {
        Debug.Log("SetGameObjectActive!!!!!!!!!!!!!!!!!!!!");
        isTimerActive = true;
        gameObject.SetActive(true);
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
            RPC_UpdateTimerText(GameManager.Instance.CurrentTurnTime);
        }
    }
}
