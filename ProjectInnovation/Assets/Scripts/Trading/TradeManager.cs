using Fusion;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TradeManager : Singleton<TradeManager>
{
    [SerializeField] private Image tradeSentItem1;
    [SerializeField] private Image tradeSentItem2;
    [SerializeField] private Image tradeReceivedItem1;
    [SerializeField] private Image tradeReceivedItem2;

    [SerializeField] private Transform playerBoxesParent;
    List<PlayerRef> playerRefs = new List<PlayerRef>();
    [SerializeField] private Button playerAcceptButton;
    public InventoryItem tradeItem { get; set; }
    public InventoryItem bankItem { get; set; }

    [SerializeField] private Button confirmTradeButton;

    public override void Spawned()
    {
        base.Spawned();
        confirmTradeButton.onClick.AddListener(AddItemsToUI);
        confirmTradeButton.interactable = false;
        playerRefs = Runner.ActivePlayers.ToList();
    }

    public override void Render()
    {
        base.Render();
        confirmTradeButton.interactable = tradeItem != null && bankItem != null;
    }

    private void AddItemsToUI()
    {
        if (tradeItem != null && bankItem != null)
        {
            tradeSentItem1.sprite = tradeItem.GetComponent<Image>().sprite;
            tradeSentItem2.sprite = bankItem.GetComponent<Image>().sprite;
            tradeReceivedItem1.sprite = tradeItem.GetComponent<Image>().sprite;
            tradeReceivedItem2.sprite = bankItem.GetComponent<Image>().sprite;
        }
        if (playerAcceptButton != null)
        {
            foreach (var player in playerRefs)
            {
                Instantiate(playerAcceptButton, playerBoxesParent);
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_AcceptTrade()
    {
        Debug.Log($"Player : {Runner.LocalPlayer.PlayerId} has accepted the trade");
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_DeclineTrade()
    {
        Debug.Log($"Player : {Runner.LocalPlayer.PlayerId} has declined the trade");
    }
}
