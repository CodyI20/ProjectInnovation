using UnityEngine;
using Fusion;

public class PlayerSkipTurn : NetworkBehaviour
{
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SkipTurn()
    {
        GameManager.Instance.RPC_EndPlayerTurn();
    }
}
