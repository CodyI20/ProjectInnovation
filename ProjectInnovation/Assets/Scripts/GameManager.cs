using Fusion;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// This class is responsible for managing the match, it will handle the turns of the players and the time for each turn.
/// It manages the players in the match and will notify the players when it's their turn to play.
/// </summary>
public class GameManager : Singleton<GameManager>
{
    public event Action<PlayerRef> OnPlayerTurnStart;
    public event Action<PlayerRef> OnPlayerTurnEnd;
    public event Action<PlayerRef> OnPlayerWin;
    public static event Action OnMatchStart;

    [SerializeField, Tooltip("The amount of players that are needed to start the match.")] private int playersNeededToStart = 2;

    [SerializeField] private float timeForTurn = 10f;
    public float TimeForTurn { get { return timeForTurn; } }
    [HideInInspector][Networked] public float CurrentTurnTime { get; set; }
    [Networked] private TickTimer networkedCurrentTurnTime { get; set; }


    //Make a public get private set property for the playersInMatch list
    private List<PlayerRef> playersInMatch;
    [Networked] private int currentPlayerIndex { get; set; } = 0;
    [Networked] private bool canCheckTime { get; set; } = false;

    public override void Spawned()
    {
        base.Spawned();
        networkedCurrentTurnTime = TickTimer.None;
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_StartMatch()
    {
        playersInMatch = new List<PlayerRef>(Runner.ActivePlayers.ToList());
        if (playersInMatch.Count < playersNeededToStart)
        {
            Debug.LogError("Not enough players in the match!");
            return;
        }
        else
        {
            if (Runner.IsRunning)
                networkedCurrentTurnTime = TickTimer.CreateFromSeconds(Runner, timeForTurn);
            currentPlayerIndex = 0;
            RPC_StartPlayerTurn();
            Debug.Log("Match started!");
            OnMatchStart?.Invoke();
            canCheckTime = true;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_StartPlayerTurn()
    {
        OnPlayerTurnStart?.Invoke(playersInMatch[currentPlayerIndex]);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_CheckTurnTime()
    {
        if (!canCheckTime) return;
        RPC_SetCurrentTimeFloat();
        if (networkedCurrentTurnTime.Expired(Runner))
        {
            RPC_EndPlayerTurn();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_PlayerWin()
    {
        Debug.Log($"Player {playersInMatch[currentPlayerIndex]} won!");
        Time.timeScale = 0f;
        OnPlayerWin?.Invoke(playersInMatch[currentPlayerIndex]);
    }

    //public override void Render()
    //{
    //    base.Render();
    //    RPC_CheckTurnTime();

    //    //TEST
    //    if (Input.GetKeyDown(KeyCode.N))
    //    {
    //        OnPlayerWin?.Invoke(GetCurrentPlayer());
    //    }
    //}

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_EndPlayerTurn()
    {
        OnPlayerTurnEnd?.Invoke(playersInMatch[currentPlayerIndex]);
        currentPlayerIndex++;
        if (currentPlayerIndex >= playersInMatch.Count)
        {
            currentPlayerIndex = 0;
        }
        networkedCurrentTurnTime = TickTimer.CreateFromSeconds(Runner, timeForTurn);
        RPC_StartPlayerTurn();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        RPC_CheckTurnTime();
        //Debug.Log($"The current networked Value is: {networkedCurrentTurnTime.RemainingTime(Runner).GetValueOrDefault()}");
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_SetCurrentTimeFloat()
    {
        CurrentTurnTime = networkedCurrentTurnTime.RemainingTime(Runner).GetValueOrDefault();
    }

    public void EndMatch()
    {
        playersInMatch = null;
        currentPlayerIndex = 0;
    }


    public void AddPlayerToList(PlayerRef player)
    {
        playersInMatch.Add(player);
        Debug.Log($"Player added to list, new count of the playersInMatch list is: {playersInMatch.Count} ");
    }
    public PlayerRef GetCurrentPlayer()
    {
        return playersInMatch[currentPlayerIndex];
    }

    public List<PlayerRef> GetPlayersInMatch()
    {
        return playersInMatch;
    }


}