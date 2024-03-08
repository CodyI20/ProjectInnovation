using UnityEngine;
using UnityEngine.UI;
using Fusion;

[RequireComponent(typeof(Slider))]
public class PercentageIncrease : NetworkBehaviour
{
    [SerializeField] private CookingManager _cookingManager;
    [SerializeField] private RecipeManager _recipeManager;
    private Slider _slider;

    public override void Spawned()
    {
        base.Spawned();
        _slider = GetComponent<Slider>();
        RecipeUI.OnItemCrossedOut += RPC_UpdatePercentage;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_UpdatePercentage(PlayerRef player)
    {
        if(player != Runner.LocalPlayer)
            return;
        float percentageIncrease = 1.0f/_recipeManager.items.Count;
        if (_slider.value + percentageIncrease > 0.99 && _slider.value + percentageIncrease < 1)
        {
            _slider.value = 1;
            return;
        }
        if (_slider.value + percentageIncrease <= 1)
            _slider.value += percentageIncrease;
        else
            _slider.value = 1;

    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        RecipeUI.OnItemCrossedOut -= RPC_UpdatePercentage;
    }
}
