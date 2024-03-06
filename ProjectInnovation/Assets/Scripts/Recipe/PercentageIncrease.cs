using UnityEngine;
using UnityEngine.UI;
using Fusion;

[RequireComponent(typeof(Slider))]
public class PercentageIncrease : NetworkBehaviour
{
    [SerializeField] private CookingManager _cookingManager;
    private Slider _slider;

    public override void Spawned()
    {
        base.Spawned();
        _slider = GetComponent<Slider>();
        CookingManager.OnCookingFinishedd += RPC_UpdatePercentage;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_UpdatePercentage()
    {
        if (_slider.value + 0.1f <= 1)
            _slider.value += 0.1f;
        else
            _slider.value = 1;
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        CookingManager.OnCookingFinishedd -= RPC_UpdatePercentage;
    }
}
