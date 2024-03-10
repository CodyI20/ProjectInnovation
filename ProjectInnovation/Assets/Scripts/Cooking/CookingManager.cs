using Fusion;
using System;
using System.Collections;
using UnityEngine;
using CookingEnums;

public class CookingManager : NetworkBehaviour
{
    public event Action OnCookingStarted;
    public static event Action<InventoryItem, CookingProcess> OnCookingFinished;
    public static event Action<RawIngredients> OnCookingFinishedd;
    public event Action OnCookingInterrupted;

    private InventoryItem inventoryItem;

    public override void Spawned()
    {
        base.Spawned();
        GameManager.Instance.OnPlayerTurnEnd += InterruptCooking;
        InventoryItem.OnInventoryItemClicked += GetInventoryItem;
    }

    [HideInInspector][Networked] public CookingState _cookingState { get; private set; } = CookingState.Idle; //TODO check if it works with private set, if not, remove it

    public void GetInventoryItem(InventoryItem inventoryItem)
    {
        this.inventoryItem = inventoryItem;
    }

    public void StartCooking(CookingProcess cookingProcess)
    {
        _cookingState = CookingState.Cooking;
        OnCookingStarted?.Invoke();
        StartCoroutine(Cooking(cookingProcess));
    }

    private void InterruptCooking(PlayerRef currentPlayer)
    {
        if(_cookingState == CookingState.Cooking && Runner.LocalPlayer == currentPlayer)
        {
            _cookingState = CookingState.Interrupted;
            StopCoroutine("Cooking");
            OnCookingInterrupted?.Invoke();
        }
    }

    private IEnumerator Cooking(CookingProcess cookingProcess)
    {
        if(_cookingState == CookingState.Interrupted)
        {
            yield break;
        }
        yield return new WaitForSeconds(cookingProcess.CookingTime);
        _cookingState = CookingState.Done;
        OnCookingFinished?.Invoke(inventoryItem, cookingProcess);
        OnCookingFinishedd?.Invoke(inventoryItem.Item);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        GameManager.Instance.OnPlayerTurnEnd -= InterruptCooking;
        InventoryItem.OnInventoryItemClicked -= GetInventoryItem;
    }

}