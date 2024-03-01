using UnityEngine;
using Fusion;

public class EnableIfPlayerTurn : NetworkBehaviour
{
    public override void Spawned()
    {
        base.Spawned();
        GameManager.Instance.OnPlayerTurnStart += RPC_OnPlayerTurnStart;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_OnPlayerTurnStart(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        GameManager.Instance.OnPlayerTurnStart -= RPC_OnPlayerTurnStart;
    }
}
