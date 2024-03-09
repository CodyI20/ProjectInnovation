using UnityEngine;
using UnityEngine.UI;
using Fusion;

[RequireComponent(typeof(Slider))]
public class PercentageIncrease : NetworkBehaviour
{
    [SerializeField] private CookingManager _cookingManager;
    [SerializeField] private RecipeManager _recipeManager;
    private Slider _slider;
    [Networked, OnChangedRender(nameof(ChangeActualSlider))] private float _sliderValue { get; set; }

    public override void Spawned()
    {
        base.Spawned();
        _slider = GetComponent<Slider>();
        RecipeUI.OnItemCrossedOut += UpdatePercentage;
    }

    private void Update()
    {
        if(HasStateAuthority && Input.GetKeyDown(KeyCode.P))
        {
            UpdatePercentage(Runner.LocalPlayer);
        }
    }

    public void UpdatePercentage(PlayerRef player)
    {
        float percentageIncrease = 1.0f/_recipeManager.items.Count;
        if (_sliderValue + percentageIncrease > 0.99 && _sliderValue + percentageIncrease < 1)
        {
            _sliderValue = 1;
            return;
        }
        if (_sliderValue + percentageIncrease <= 1)
            _sliderValue += percentageIncrease;
        else
            _sliderValue = 1;

    }

    void ChangeActualSlider()
    {
        _slider.value = _sliderValue;
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        RecipeUI.OnItemCrossedOut -= UpdatePercentage;
    }
}
