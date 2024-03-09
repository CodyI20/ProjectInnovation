using UnityEngine;
using UnityEngine.UI;
using Fusion;

[RequireComponent(typeof(Slider))]
public class SliderValueNetworked : NetworkBehaviour
{
    private Slider _slider;

    [Networked, OnChangedRender(nameof(UpdateSliderValue))] private float _sliderValue { get; set; }

    public override void Spawned()
    {
        base.Spawned();
    }

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    public void UpdateSliderValue()
    {
        _sliderValue = _slider.value;
        _slider.value = _sliderValue;
    }

    public override void Render()
    {
        base.Render();
        UpdateSliderValue();
    }
}
