using UnityEngine;
using Fusion;
using TMPro;

public class VictoryScreenLogic : NetworkBehaviour
{
    private TextMeshProUGUI victoryText;

    private void Awake()
    {
        victoryText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public override void Spawned()
    {
        base.Spawned();
        GameManager.Instance.OnPlayerWin += RPC_PlayerWinActions;
        gameObject.SetActive(false);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_PlayerWinActions(PlayerRef player)
    {
        victoryText.text = $"Player {player} won!";
        gameObject.SetActive(true);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        GameManager.Instance.OnPlayerWin -= RPC_PlayerWinActions;
    }
}
