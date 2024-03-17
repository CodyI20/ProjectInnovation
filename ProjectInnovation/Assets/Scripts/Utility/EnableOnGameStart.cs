using Fusion;

public class EnableOnGameStart : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(EnableObject))]private bool _isGameStarted { get; set; } = false;
    public override void Spawned()
    {
        if (!_isGameStarted)
        {
            gameObject.SetActive(false);
        }
        GameManager.OnMatchStart += EnableObject;
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        GameManager.OnMatchStart -= EnableObject;
    }
    
    void EnableObject()
    {
        RPC_Enable();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_Enable()
    {
        _isGameStarted = true;
        gameObject.SetActive(true);
    }
}
