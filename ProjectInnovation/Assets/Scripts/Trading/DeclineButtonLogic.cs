using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;

public class DeclineButtonLogic : NetworkBehaviour
{
    Button button;

    public override void Spawned()
    {
        base.Spawned();
        button = GetComponent<Button>();
        button.onClick.AddListener(NotifyTradeInitiator);
    }

    private void NotifyTradeInitiator()
    {
        Debug.Log("Will not trade...");
        TradeManager.Instance.NotifyTradeInitiator(false);
    }
}
