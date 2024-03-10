using CookingEnums;
using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TradeInventory : NetworkBehaviour
{
    [SerializeField] private Inventory inventory;
    private Dictionary<RawIngredients, int> tradeIngredients;
    [SerializeField] private List<InventoryItem> tradeItems;
    private void Awake()
    {
        if (inventory == null)
        {
            Debug.LogError("Inventory is not assigned to the TradeInventory script. Please assign it in the inspector.");
        }
        tradeIngredients = new Dictionary<RawIngredients, int>();
    }
    private void OnEnable()
    {
        Debug.Log("Enabled TradeInventory");
        AddTradeItems();
    }
    public override void Spawned()
    {
        base.Spawned();
        inventory.OnIngredientAdded += AddTradeIngredients;
        CookingManager.OnCookingFinishedd += RemoveTradeIngredients;
    }
    private void AddTradeItems()
    {
        if (inventory.GetIngredients() != null)
        {
            foreach (var ingredient in inventory.GetIngredients())
            {
                if (ingredient.Value > 0)
                {
                    AddTradeIngredients(ingredient.Key, 1);
                }
            }
        }
    }
    public void AddTradeIngredients(RawIngredients ingredient, int q)
    {
        foreach (var item in tradeItems)
        {
            if (item.Item == ingredient)
            {
                if (tradeIngredients.ContainsKey(ingredient))
                {
                    Debug.Log("Increasing trade ingredient: " + ingredient);
                    tradeIngredients[ingredient]++;
                }
                else
                {
                    Debug.Log("Creating inventory ingredient: " + ingredient);
                    tradeIngredients[ingredient] = 1;
                    Instantiate(tradeItems.Find(x => x.Item == ingredient), gameObject.transform);
                }
                break;
            }
        }
    }

    public void RemoveTradeIngredients(RawIngredients ingredient)
    {
        if (tradeIngredients.ContainsKey(ingredient))
        {
            Debug.Log("Decreasing trade ingredient: " + ingredient);
            tradeIngredients[ingredient]--;
            if (tradeIngredients[ingredient] == 0)
            {
                Debug.Log("Removing trade ingredient: " + ingredient);
                tradeIngredients.Remove(ingredient);
                Destroy(tradeItems.Find(x => x.Item == ingredient).gameObject);
            }
        }
    }

    public Inventory GetInventory()
    {
        return inventory;
    }
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        inventory.OnIngredientAdded -= AddTradeIngredients;
        CookingManager.OnCookingFinishedd -= RemoveTradeIngredients;
    }
}
