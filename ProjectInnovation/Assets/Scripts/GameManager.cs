using Fusion;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Fusion.NetworkBehaviour;


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
    [HideInInspector][Networked] public float CurrentTurnTime { get; private set;}


    //Make a public get private set property for the playersInMatch list
    private List<PlayerRef> playersInMatch;
    private int currentPlayerIndex = 0;
    [Networked] private bool canCheckTime { get; set; } = false;

    public override void Spawned()
    {
        base.Spawned();

    }
    [Rpc(RpcSources.All, RpcTargets.All)]
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
            currentPlayerIndex = 0;
            StartPlayerTurn();
            Debug.Log("Match started!");
            OnMatchStart?.Invoke();
            canCheckTime = true;
        }
    }

    public void StartPlayerTurn()
    {
        CurrentTurnTime = timeForTurn;
        OnPlayerTurnStart?.Invoke(playersInMatch[currentPlayerIndex]);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_CheckTurnTime()
    {
        if(!canCheckTime) return;
        if (CurrentTurnTime > 0)
        {
            CurrentTurnTime -= Time.deltaTime;
        }
        else
        {
            EndPlayerTurn();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_PlayerWin()
    {
        Debug.Log($"Player {playersInMatch[currentPlayerIndex]} won!");
        Time.timeScale = 0f;
        OnPlayerWin?.Invoke(playersInMatch[currentPlayerIndex]);
    }

    public override void Render()
    {
        base.Render();
        RPC_CheckTurnTime();

        //TEST
        if (Input.GetKeyDown(KeyCode.N))
        {
            OnPlayerWin?.Invoke(GetCurrentPlayer());
        }
    }

    public void EndPlayerTurn()
    {
        OnPlayerTurnEnd?.Invoke(playersInMatch[currentPlayerIndex]);
        currentPlayerIndex++;
        if (currentPlayerIndex >= playersInMatch.Count)
        {
            currentPlayerIndex = 0;
        }
        StartPlayerTurn();
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
