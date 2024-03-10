// TradeManager.cs
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using System.Linq;
using Unity.VisualScripting;

public class TradeManager : Singleton<TradeManager>
{
    [SerializeField] private GameObject tradeConfirmationPrefab;
    private Dictionary<PlayerRef, bool> tradeResponses = new Dictionary<PlayerRef, bool>();
    List<PlayerRef> playersInGame = new List<PlayerRef>();

    [SerializeField] private Transform tradeItemsParentSent;
    [SerializeField] private Transform tradeItemsParentReceived;
    [SerializeField] private Transform playerBoxesParent;
    public InventoryItem tradeItem { get; set; }
    public InventoryItem bankItem { get; set; }
    [SerializeField] private Button tradeButton;
    [SerializeField] private Button acceptButton;

    private void OnEnable()
    {
        tradeButton.onClick.AddListener(InstantiateItemsOnUI);
    }

    public override void Spawned()
    {
        tradeButton.interactable = false;
        acceptButton.interactable = false;
        playersInGame = Runner.ActivePlayers.ToList();
    }

    public override void Render()
    {
        base.Render();
        tradeButton.interactable = tradeItem != null && bankItem != null;
    }

    public void NotifyTradeInitiator(bool willItTrade)
    {
        tradeResponses[Runner.LocalPlayer] = willItTrade;
    }

    public void InstantiateItemsOnUI()
    {
        Instantiate(tradeItem, tradeItemsParentSent);
        Instantiate(bankItem, tradeItemsParentSent);
        Instantiate(tradeItem, tradeItemsParentReceived);
        Instantiate(bankItem, tradeItemsParentReceived);
        foreach (var player in playersInGame)
        {
            Instantiate(acceptButton, playerBoxesParent);
        }
    }

    public void InitiateTrade(PlayerRef initiatingPlayer, InventoryItem initiatingItem, PlayerRef[] otherPlayers, InventoryItem[] otherItems)
    {
        // Create a trade confirmation window
        GameObject tradeConfirmation = Instantiate(tradeConfirmationPrefab, transform);
        TradeConfirmationWindow confirmationWindow = tradeConfirmation.GetComponent<TradeConfirmationWindow>();

        // Set trade details
        confirmationWindow.SetTradeDetails(initiatingPlayer, initiatingItem, otherPlayers, otherItems);

        // Handle trade responses
        confirmationWindow.OnResponse += (player, response) => tradeResponses[player] = response;

        // Handle trade finalization
        confirmationWindow.OnFinalize += FinalizeTrade;

        // Show the confirmation window to other players
        confirmationWindow.ShowToOtherPlayers(otherPlayers);
    }

    private void FinalizeTrade(PlayerRef initiatingPlayer, PlayerRef finalizingPlayer)
    {
        if (tradeResponses.ContainsKey(initiatingPlayer) && tradeResponses[initiatingPlayer])
        {
            // Finalize the trade logic here
            Debug.Log("Trade finalized!");
        }
        else
        {
            // Trade declined
            Debug.Log("Trade declined!");
        }

        // Cleanup and reset trade responses
        tradeResponses.Clear();
    }
}
