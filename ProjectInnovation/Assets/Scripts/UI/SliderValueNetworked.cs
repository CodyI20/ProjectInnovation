using UnityEngine;
using UnityEngine.UI;
using Fusion;

[RequireComponent(typeof(Slider))]
public class SliderValueNetworked : NetworkBehaviour
{
    private ChangeDetector _changeDetector;
    private Slider _slider;

    [Networked] private float _sliderValue { get; set; }

    public override void Spawned()
    {
        base.Spawned();
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SnapshotTo, false);
    }

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_UpdateSliderValue()
    {
        _sliderValue = _slider.value;
        _slider.value = _sliderValue;
    }

    public override void Render()
    {
        base.Render();
        RPC_UpdateSliderValue();
        foreach(var change in _changeDetector.DetectChanges(this))
        {
            if (change == nameof(_sliderValue))
            {
                _slider.value = _sliderValue;
            }
        }
    }
}
