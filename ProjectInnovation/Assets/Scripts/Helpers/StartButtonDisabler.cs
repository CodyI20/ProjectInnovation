using Fusion;

public class StartButtonDisabler : NetworkBehaviour
{
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_DisableStartButton()
    {
        Destroy(gameObject);
    }

}
