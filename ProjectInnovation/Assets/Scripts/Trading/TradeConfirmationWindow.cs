// TradeConfirmationWindow.cs
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class TradeConfirmationWindow : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI tradeDetailsText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button declineButton;

    public event Action<PlayerRef, bool> OnResponse;
    public event Action<PlayerRef, PlayerRef> OnFinalize;

    private PlayerRef initiatingPlayer;

    public void SetTradeDetails(PlayerRef initiatingPlayer, InventoryItem initiatingItem, PlayerRef[] otherPlayers, InventoryItem[] otherItems)
    {
        this.initiatingPlayer = initiatingPlayer;

        // Display trade details on the window
        // Modify this based on your UI layout
        tradeDetailsText.text = $"Trade initiated by {initiatingPlayer}:\n";
        tradeDetailsText.text += $"{initiatingItem.Item} for:\n";

        for (int i = 0; i < otherPlayers.Length; i++)
        {
            tradeDetailsText.text += $"{otherPlayers[i]}: {otherItems[i].Item}\n";
        }

        // Add listeners to buttons
        acceptButton.onClick.AddListener(() => OnResponse?.Invoke(initiatingPlayer, true));
        declineButton.onClick.AddListener(() => OnResponse?.Invoke(initiatingPlayer, false));
    }

    public void ShowToOtherPlayers(PlayerRef[] otherPlayers)
    {
        // Display the trade confirmation window to other players
        // You can modify this based on your UI implementation
        // (e.g., show a pop-up, enable a canvas, etc.)
        gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        // Cleanup button listeners
        acceptButton.onClick.RemoveAllListeners();
        declineButton.onClick.RemoveAllListeners();
    }
}
