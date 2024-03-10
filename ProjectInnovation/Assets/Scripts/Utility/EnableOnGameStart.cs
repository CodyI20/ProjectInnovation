using Fusion;

public class EnableOnGameStart : NetworkBehaviour
{
    public override void Spawned()
    {
        GameManager.OnMatchStart += RPC_Enable;
        gameObject.SetActive(false);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        GameManager.OnMatchStart -= RPC_Enable;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_Enable()
    {   
        gameObject.SetActive(true);
    }
}
