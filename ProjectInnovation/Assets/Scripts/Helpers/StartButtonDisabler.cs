using Fusion;

public class StartButtonDisabler : NetworkBehaviour
{
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_DisableStartButton()
    {
        Destroy(gameObject);
    }

}
