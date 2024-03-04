using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class TradingSystem : NetworkBehaviour
{   
    //public PlayerManager playerManager;
    private PlayerRef player1;
    private PlayerRef player2;
    private bool isTrading = false;

    private bool isTradeAcceptedByPlayer1 = false;
    private bool isTradeAcceptedByPlayer2 = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)) {
            OnClickTradeButton();
        }
        if (Input.GetKeyDown(KeyCode.B)) {
            AcceptTrade();
        }
        if (Input.GetKeyDown(KeyCode.C)) {
            CancelTrade();
        }
    }

    public void OnClickTradeButton()
    {
        if (!Runner.IsRunning) { //Check if these are right??
            return;
        }

        if (isTrading) {
            CancelTrade();
            return;
        }

        PlayerRef localPlayer = Runner.LocalPlayer;

        if (player1 == null) {
            player1 = localPlayer;
        } else if (player1 == localPlayer) {
            player1 = player2;
            player2 = new PlayerRef();
        } else if (player2 == null) {
            player2 = localPlayer;
        } else if (player2 == localPlayer) {
            player2 = new PlayerRef();
        }

        RPC_InitiateTrade(player1, player2);
        //photonView.RPC("RPC_InitiateTrade", RpcTarget.All, player1ActorNumber, player2ActorNumber);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_InitiateTrade(PlayerRef player1, PlayerRef player2)
    {
        if (player1 == null || player2 == null) {
            return;
        }
        isTrading = true;

        

        // Display trade UI for both players
        // For example, show trade panel, offer buttons, etc.
    }

    public void AcceptTrade()
    {
        if (!isTrading) {
            return;
        }
        if (!Runner.IsRunning) {
            return;
        }

        PlayerRef localPlayer = Runner.LocalPlayer;
        bool isAccepted;

        if (localPlayer == player1) {
            isAccepted = !isTradeAcceptedByPlayer1;
        } else if (localPlayer == player2) {
            isAccepted = !isTradeAcceptedByPlayer2;
        } else {
            return;
        }

        //photonView.RPC("RPC_AcceptTrade", RpcTarget.All, localPlayer.ActorNumber, isAccepted);
        RPC_AcceptTrade(localPlayer, isAccepted);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_AcceptTrade(PlayerRef playerActorNumber, bool isAccepted)
    {
        if (player1 == playerActorNumber) {
            this.isTradeAcceptedByPlayer1 = isAccepted;
        } else if (player2 == playerActorNumber) {
            this.isTradeAcceptedByPlayer2 = isAccepted;
        }

        if (isTradeAcceptedByPlayer1 && !isTradeAcceptedByPlayer2) {
            Debug.Log("Trade accepted by " + player1);
        } else if (!isTradeAcceptedByPlayer1 && isTradeAcceptedByPlayer2) {
            Debug.Log("Trade accepted by " + player2);
        } else if (isTradeAcceptedByPlayer1 && isTradeAcceptedByPlayer2) {
            Debug.Log("Trade accepted by both players");
        } else {
            Debug.Log("Trade not accepted by both players");
        }

        if (!isTradeAcceptedByPlayer1 || !isTradeAcceptedByPlayer2) {
            return;
        }

        Debug.Log("Trade accepted between " + player1 + " and " + player2);
        
        // Inventory player1Inventory = playerManager.GetPlayerObject(player1.ActorNumber).GetComponent<Inventory>();
        // Inventory player2Inventory = playerManager.GetPlayerObject(player2.ActorNumber).GetComponent<Inventory>();

        // if (player1Inventory.HasTradingIngredients() && player2Inventory.HasTradingIngredients()) {
        //     player1Inventory.TradeIngredients(player2Inventory.GetIngredients());
        //     player2Inventory.TradeIngredients(player1Inventory.GetIngredients());
        // } else {
        //     CancelTrade();
        //     return;
        // }

        // Handle trade execution
        // For example, exchange items, update inventories, etc.
        
        Debug.Log("Trade performed between " + player1 + " and " + player2);
        
        
        isTrading = false;
        player1 = new PlayerRef();
        player2 = new PlayerRef();
        

    }

    public void CancelTrade()
    {
        // Notify both players that the trade is cancelled
        RPC_CancelTrade();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_CancelTrade()
    {
        // Handle trade cancellation
        // For example, close trade UI, reset trade variables, etc.

        Debug.Log("Trade cancelled between " + player1 + " and " + player2);
        isTrading = false;
        player1 = new PlayerRef();
        player2 = new PlayerRef();
    }


}
