using Fusion;
using UnityEngine;

public class EnableIfPlayerTurn : NetworkBehaviour
{
    public override void Spawned()
    {
        base.Spawned();
        GameManager.Instance.OnPlayerTurnStart += RPC_OnPlayerTurnStart;
        //gameObject.SetActive(false);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_OnPlayerTurnStart(PlayerRef player)
    {
        gameObject.SetActive(player==Runner.LocalPlayer);
        Debug.Log($"Enabling {gameObject.name}; Player {player} is local player: {player==Runner.LocalPlayer}");
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        GameManager.Instance.OnPlayerTurnStart -= RPC_OnPlayerTurnStart;
    }
}
