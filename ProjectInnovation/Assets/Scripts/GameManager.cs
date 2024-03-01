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
    public static event Action OnMatchStart;

    [SerializeField, Tooltip("The amount of players that are needed to start the match.")] private int playersNeededToStart = 2;

    [SerializeField] private float timeForTurn = 10f;
    private float currentTurnTime;
    public float TimeForTurn { get { return timeForTurn; } }
    public float CurrentTurnTime { get { return currentTurnTime; } }

    private List<PlayerRef> playersInMatch;
    private int currentPlayerIndex;
    private bool canCheckTime = false;

    public void StartMatch()
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
        currentTurnTime = timeForTurn;
        OnPlayerTurnStart?.Invoke(playersInMatch[currentPlayerIndex]);
    }

    void CheckTurnTime()
    {
        if(!canCheckTime) return;
        if (currentTurnTime > 0)
        {
            currentTurnTime -= Time.deltaTime;
        }
        else
        {
            EndPlayerTurn();
        }
    }

    private void Update()
    {
        CheckTurnTime();
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