using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class TestIncreasePercentage : NetworkBehaviour
{
    [SerializeField] private Slider _slider;
    //[Networked] private float _percentage { get; set; }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_UpdatePercentage(float percentage)
    {
        if (_slider.value + percentage <= 1)
            _slider.value += percentage;
        else
            _slider.value = 1;
    }
}
