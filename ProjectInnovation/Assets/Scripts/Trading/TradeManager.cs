using CookingEnums;
using Fusion;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TradeManager : Singleton<TradeManager>
{
    public event System.Action<RawIngredients> OnTradeComplete;
    public event System.Action OnTradeEnded;
    [SerializeField] private Image tradeSentItem1;
    [SerializeField] private Image tradeSentItem2;
    [SerializeField] private Image tradeReceivedItem1;
    [SerializeField] private Image tradeReceivedItem2;
    [SerializeField] private TradeTimer tradeTimer;

    [SerializeField] private GameObject mainTradeUI;
    [SerializeField] private GameObject tradeSentUI;
    [SerializeField] private GameObject tradeReceivedUI;

    [SerializeField] private Transform playerBoxesParent;
    List<PlayerRef> playerRefs = new List<PlayerRef>();
    [SerializeField] private Button playerAcceptButton;
    private InventoryItem tradeItem;
    private InventoryItem bankItem;
    [HideInInspector][Networked, OnChangedRender(nameof(RPC_SetCurrentTimeFloat))] public float CurrentTradeTimeLeft { get; set; }
    [Networked] public TickTimer networkedCurrentTradeTime { get; set; }

    public PlayerRef tradeInitiator { get; set; }

    [SerializeField] private Button confirmTradeButton;
    [Header("TRADE TIMER")]
    [SerializeField, Tooltip("The maximum time for a trade to be completed.")] private float tradeTime = 20f;

    public override void Spawned()
    {
        base.Spawned();
        confirmTradeButton.onClick.AddListener(RPC_EnableTheMainTradeUI);
        confirmTradeButton.onClick.AddListener(AddItemsToUISent);
        confirmTradeButton.onClick.AddListener(RPC_AddItemsToUIReceived);
        InventoryItem.OnBankItemClicked += RPC_AddBankItem;
        InventoryItem.OnTradeItemClicked += RPC_AddTradeItem;
        confirmTradeButton.interactable = false;
        networkedCurrentTradeTime = TickTimer.None;
        playerRefs = Runner.ActivePlayers.ToList();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_AddTradeItem(InventoryItem item)
    {
        tradeItem = item;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_AddBankItem(InventoryItem item)
    {
        bankItem = item;
    }

    public InventoryItem GetTradeItem()
    {
        return tradeItem;
    }
    public InventoryItem GetBankItem()
    {
        return bankItem;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_SetCurrentTimeFloat()
    {
        CurrentTradeTimeLeft = networkedCurrentTradeTime.RemainingTime(Runner).GetValueOrDefault();
        tradeTimer.UpdateTextTimer(CurrentTradeTimeLeft);
    }

    void CheckTurnTime()
    {
        RPC_SetCurrentTimeFloat();
        if (networkedCurrentTradeTime.Expired(Runner))
        {
            RPC_DeclineTrade();
        }
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        CheckTurnTime();
    }
    public override void Render()
    {
        base.Render();
        confirmTradeButton.interactable = tradeItem != null && bankItem != null;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_TradeEndActions()
    {
        OnTradeEnded?.Invoke();
        networkedCurrentTradeTime = TickTimer.None;
        tradeReceivedUI.SetActive(false);
        tradeSentUI.SetActive(false);
        mainTradeUI.SetActive(false);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_EnableTheMainTradeUI()
    {
        if (mainTradeUI != null)
        {
            mainTradeUI.SetActive(true);
            if (Runner.IsRunning)
                networkedCurrentTradeTime = TickTimer.CreateFromSeconds(Runner, tradeTime);
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
        if (tradeInitiator == Runner.LocalPlayer)
            return;
        if (tradeItem != null && bankItem != null)
        {
            Debug.Log("Adding items to UI for the receiver of the trade...");
            tradeReceivedItem1.sprite = bankItem.GetComponent<Image>().sprite;
            tradeReceivedItem2.sprite = tradeItem.GetComponent<Image>().sprite;
        }
        tradeReceivedUI.SetActive(true);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_AcceptTrade()
    {
        if (tradeInitiator == Runner.LocalPlayer)
        {
            Debug.Log($"Player : {Runner.LocalPlayer.PlayerId} has accepted the trade");
            OnTradeComplete?.Invoke(bankItem.Item);
            tradeInitiator = PlayerRef.None;
            tradeReceivedUI.SetActive(false);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_DeclineTrade()
    {
        Debug.Log($"Player : {Runner.LocalPlayer.PlayerId} has declined the trade");
        RPC_TradeEndActions();
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        confirmTradeButton.onClick.RemoveListener(RPC_EnableTheMainTradeUI);
        confirmTradeButton.onClick.RemoveListener(AddItemsToUISent);
        confirmTradeButton.onClick.RemoveListener(RPC_AddItemsToUIReceived);
        InventoryItem.OnTradeItemClicked -= RPC_AddTradeItem;
        InventoryItem.OnBankItemClicked -= RPC_AddBankItem;
    }
}
