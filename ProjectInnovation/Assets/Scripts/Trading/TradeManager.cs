using CookingEnums;
using Fusion;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TradeManager : Singleton<TradeManager>
{
    public event System.Action<RawIngredients> OnTradeFinished;
    [SerializeField] private Image tradeSentItem1;
    [SerializeField] private Image tradeSentItem2;
    [SerializeField] private Image tradeReceivedItem1;
    [SerializeField] private Image tradeReceivedItem2;

    [SerializeField] private GameObject mainTradeUI;
    [SerializeField] private GameObject tradeSentUI;
    [SerializeField] private GameObject tradeReceivedUI;

    [SerializeField] private Transform playerBoxesParent;
    List<PlayerRef> playerRefs = new List<PlayerRef>();
    [SerializeField] private Button playerAcceptButton;
    public InventoryItem tradeItem { get; set;}
    public InventoryItem bankItem { get; set;}
    public PlayerRef tradeInitiator { get; set; }

    [SerializeField] private Button confirmTradeButton;

    public override void Spawned()
    {
        base.Spawned();
        confirmTradeButton.onClick.AddListener(EnableTheMainTradeUI);
        confirmTradeButton.onClick.AddListener(AddItemsToUISent);
        confirmTradeButton.onClick.AddListener(RPC_AddItemsToUIReceived);
        confirmTradeButton.interactable = false;
        playerRefs = Runner.ActivePlayers.ToList();
    }


    public InventoryItem GetTradeItem()
    {
        return tradeItem;
    }
    public InventoryItem GetBankItem()
    {
        return bankItem;
    }

    public override void Render()
    {
        base.Render();
        confirmTradeButton.interactable = tradeItem != null && bankItem != null;
    }

    private void EnableTheMainTradeUI()
    {
        if(mainTradeUI != null)
        {
            mainTradeUI.SetActive(true);
        }
    }

    private void AddItemsToUISent()
    {
        Debug.Log($"Adding items to UI for the sender of the trade: {tradeInitiator}...");
        if (tradeItem != null && bankItem != null)
        {
            tradeSentItem1.sprite = tradeItem.GetComponent<Image>().sprite;
            tradeSentItem2.sprite = bankItem.GetComponent<Image>().sprite;
        }
        tradeInitiator = Runner.LocalPlayer;
        if (playerAcceptButton != null)
        {
            foreach (var player in playerRefs)
            {
                Instantiate(playerAcceptButton, playerBoxesParent);
            }
        }
        tradeSentUI.SetActive(true);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_AddItemsToUIReceived()
    {
        Debug.Log($"Adding items to UI for the receivers of the trade...");
        if(tradeItem != null && bankItem != null)
        {
            tradeReceivedItem1.sprite = bankItem.GetComponent<Image>().sprite;
            tradeReceivedItem2.sprite = tradeItem.GetComponent<Image>().sprite;
        }
        if(tradeInitiator != Runner.LocalPlayer)
        {
            tradeReceivedUI.SetActive(true);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_AcceptTrade()
    {
        if (tradeInitiator == Runner.LocalPlayer)
        {
            Debug.Log($"Player : {Runner.LocalPlayer.PlayerId} has accepted the trade");
            OnTradeFinished?.Invoke(bankItem.Item);
            tradeInitiator = PlayerRef.None;
            tradeReceivedUI.SetActive(false);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_DeclineTrade()
    {
        Debug.Log($"Player : {Runner.LocalPlayer.PlayerId} has declined the trade");
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        confirmTradeButton.onClick.RemoveListener(EnableTheMainTradeUI);
        confirmTradeButton.onClick.RemoveListener(AddItemsToUISent);
        confirmTradeButton.onClick.RemoveListener(RPC_AddItemsToUIReceived);
    }
}
