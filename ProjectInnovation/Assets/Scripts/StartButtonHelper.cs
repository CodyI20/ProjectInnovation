using UnityEngine;
using Fusion;

public class StartButtonHelper : NetworkBehaviour
{
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_StartMatch()
    {
        GameManager.Instance.RPC_StartMatch();
    }
}
