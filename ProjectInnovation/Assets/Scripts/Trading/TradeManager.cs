using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using System.Linq;

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
        if(tradeItem != null && bankItem != null)
        {
            tradeSentItem1.sprite = tradeItem.GetComponent<Image>().sprite;
            tradeSentItem2.sprite = bankItem.GetComponent<Image>().sprite;
            tradeReceivedItem1.sprite = tradeItem.GetComponent<Image>().sprite;
            tradeReceivedItem2.sprite = bankItem.GetComponent<Image>().sprite;
        }
    }
}
