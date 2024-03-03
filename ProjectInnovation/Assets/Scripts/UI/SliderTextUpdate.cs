using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(TextMeshProUGUI))]
public class SliderTextUpdate : NetworkBehaviour
{
    [Tooltip("Make sure that the slider value is from 0 to 1! Very important!!!")] [SerializeField] private Slider _slider;
    private TextMeshProUGUI _percentageText;

    private void Awake()
    {
        _percentageText = GetComponent<TextMeshProUGUI>();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_UpdateText()
    {
        _percentageText.text = $"{_slider.value * 100}%";
    }

    public override void Render()
    {
        base.Render();
        //RPC_UpdateText();
    }
}
