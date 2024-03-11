using CookingEnums;
using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : Singleton<Inventory>
{
    public event Action<RawIngredients, int> OnIngredientAdded;

    [SerializeField] private WheelSystem wheelSystem;
    [SerializeField] private GameObject cookingMethodsTab;
    [SerializeField] private List<InventoryItem> inventoryItem;
    [SerializeField] private CookingManager cookingManager;
    private Dictionary<RawIngredients, int> ingredients;
    private Dictionary<PreparedIngredients, int> preparedIngredients;
    private Dictionary<RawIngredients, int> tradeIngredients;

    /**
     * Constructors
     */

    public override void Spawned()
    {
        Debug.Log("Inventory Spawned");
        if (cookingManager == null)
        {
            Debug.LogError("Cooking Manager is not assigned to the Inventory script. Please assign it in the inspector.");
        }
        ingredients = new Dictionary<RawIngredients, int>();
        preparedIngredients = new Dictionary<PreparedIngredients, int>();
        if (wheelSystem != null)
            wheelSystem.GetInventory(this);
        CookingManager.OnCookingFinishedd += RemoveIngredient;
        TradeManager.Instance.OnTradeFinished += AddIngredient;
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        CookingManager.OnCookingFinishedd -= RemoveIngredient;
        TradeManager.Instance.OnTradeFinished -= AddIngredient;
    }


    /**
     * Trading Functions
     */

    public void TradeIngredients(Dictionary<RawIngredients, int> traderItems)
    {
        foreach (KeyValuePair<RawIngredients, int> item in tradeIngredients)
        {
            RemoveTradeIngredient(item.Key, item.Value);
            RemoveIngredient(item.Key, item.Value);
        }
        foreach (KeyValuePair<RawIngredients, int> item in traderItems)
        {
            AddIngredient(item.Key, item.Value);
        }
    }

    public bool HasTradingIngredients()
    {
        foreach (KeyValuePair<RawIngredients, int> item in tradeIngredients)
        {
            if (!HasIngredient(item.Key, item.Value))
                return false;
        }
        return true;
    }



    /**
     * Basic Inventory Management Functions
     */

    // Ingredients

    public void AddIngredient(RawIngredients ingredient)
    {
        if (ingredients.ContainsKey(ingredient))
        {
            Debug.Log("Increasing inventory ingredient: " + ingredient);
            ingredients[ingredient]++;
        }
        else
        {
            Debug.Log("Creating inventory ingredient: " + ingredient);
            ingredients[ingredient] = 1;
            Instantiate(inventoryItem.Find(x => x.Item == ingredient), gameObject.transform);
        }
        OnIngredientAdded?.Invoke(ingredient, 1);
    }

    public void AddIngredient(RawIngredients ingredient, int quantity)
    {
        if (ingredients.ContainsKey(ingredient))
        {
            Debug.Log("Increasing inventory ingredient: " + ingredient);
            ingredients[ingredient] += quantity;
        }
        else
        {
            Debug.Log("Creating inventory ingredient: " + ingredient);
            ingredients[ingredient] = quantity;
            Instantiate(inventoryItem.Find(x => x.Item == ingredient), gameObject.transform);
        }
        OnIngredientAdded?.Invoke(ingredient, quantity);
    }

    public void RemoveIngredient(RawIngredients ingredient)
    {
        Debug.Log("Removing ingredient: " + ingredient);
        if (ingredients.ContainsKey(ingredient))
            ingredients[ingredient]--;
        if (ingredients[ingredient] <= 0)
            ingredients.Remove(ingredient);
    }

    public void RemoveIngredient(RawIngredients ingredient, int quantity)
    {
        if (ingredients.ContainsKey(ingredient))
            ingredients[ingredient] -= quantity;
        if (ingredients[ingredient] <= 0)
            ingredients.Remove(ingredient);
    }

    // Prepared Ingredients

    public void AddPreparedIngredient(PreparedIngredients ingredient)
    {
        if (preparedIngredients.ContainsKey(ingredient))
            preparedIngredients[ingredient]++;
        else
            preparedIngredients[ingredient] = 1;
    }

    public void AddPreparedIngredient(PreparedIngredients ingredient, int quantity)
    {
        if (preparedIngredients.ContainsKey(ingredient))
            preparedIngredients[ingredient] += quantity;
        else
            preparedIngredients[ingredient] = quantity;
    }

    public void RemovePreparedIngredient(PreparedIngredients ingredient)
    {
        if (preparedIngredients.ContainsKey(ingredient))
            preparedIngredients[ingredient]--;
        if (preparedIngredients[ingredient] <= 0)
            preparedIngredients.Remove(ingredient);
    }

    public void RemovePreparedIngredient(PreparedIngredients ingredient, int quantity)
    {
        if (preparedIngredients.ContainsKey(ingredient))
            preparedIngredients[ingredient] -= quantity;
        if (preparedIngredients[ingredient] <= 0)
            preparedIngredients.Remove(ingredient);
    }

    // Trade Ingredients

    public void AddTradeIngredient(RawIngredients ingredient)
    {
        if (tradeIngredients.ContainsKey(ingredient))
            tradeIngredients[ingredient]++;
        else
            tradeIngredients[ingredient] = 1;
    }

    public void AddTradeIngredient(RawIngredients ingredient, int quantity)
    {
        if (tradeIngredients.ContainsKey(ingredient))
            tradeIngredients[ingredient] += quantity;
        else
            tradeIngredients[ingredient] = quantity;
    }

    public void RemoveTradeIngredient(RawIngredients ingredient)
    {
        if (tradeIngredients.ContainsKey(ingredient))
            tradeIngredients[ingredient]--;
        if (tradeIngredients[ingredient] <= 0)
            tradeIngredients.Remove(ingredient);
    }

    public void RemoveTradeIngredient(RawIngredients ingredient, int quantity)
    {
        if (tradeIngredients.ContainsKey(ingredient))
            tradeIngredients[ingredient] -= quantity;
        if (tradeIngredients[ingredient] <= 0)
            tradeIngredients.Remove(ingredient);
    }

    /**
     * Getters
     */

    public Dictionary<RawIngredients, int> GetIngredients()
    {
        return ingredients;
    }

    public Dictionary<PreparedIngredients, int> GetPreparedIngredients()
    {
        return preparedIngredients;
    }

    public Dictionary<RawIngredients, int> GetTradeIngredients()
    {
        return tradeIngredients;
    }

    public bool HasIngredient(RawIngredients ingredient)
    {
        return ingredients.ContainsKey(ingredient);
    }

    public bool HasIngredient(RawIngredients ingredient, int quantity)
    {
        return ingredients.ContainsKey(ingredient) && ingredients[ingredient] >= quantity;
    }

    public bool HasPreparedIngredient(PreparedIngredients ingredient)
    {
        return preparedIngredients.ContainsKey(ingredient);
    }

    public bool HasPreparedIngredient(PreparedIngredients ingredient, int quantity)
    {
        return preparedIngredients.ContainsKey(ingredient) && preparedIngredients[ingredient] >= quantity;
    }

    public bool HasIngredients(Dictionary<RawIngredients, int> ingredients)
    {
        foreach (KeyValuePair<RawIngredients, int> ingredient in ingredients)
            if (!HasIngredient(ingredient.Key, ingredient.Value))
                return false;
        return true;
    }

    public bool HasPreparedIngredients(Dictionary<PreparedIngredients, int> ingredients)
    {
        foreach (KeyValuePair<PreparedIngredients, int> ingredient in ingredients)
            if (!HasPreparedIngredient(ingredient.Key, ingredient.Value))
                return false;
        return true;
    }

    public GameObject GetCookingMethodsTab()
    {
        return cookingMethodsTab;
    }

    public CookingManager GetCookingManager()
    {
        return cookingManager;
    }

    /**
     * Debugging Functions
     */

    public void Clear()
    {
        ingredients.Clear();
        preparedIngredients.Clear();
        tradeIngredients.Clear();
    }

    public void Print()
    {
        Debug.Log("Inventory: ");
        foreach (KeyValuePair<RawIngredients, int> ingredient in ingredients)
            Debug.Log("Raw: " + ingredient.Key);
        foreach (KeyValuePair<PreparedIngredients, int> ingredient in preparedIngredients)
            Debug.Log("Prepared: " + ingredient.Key);
    }

    /**
     * Photon Functions
     */

    //public void UpdateInventory()
    //{
    //    ExitGames.Client.Photon.Hashtable inventory = new ExitGames.Client.Photon.Hashtable();
    //    inventory.Add("Ingredients", ingredients);
    //    inventory.Add("PreparedIngredients", preparedIngredients);
    //    Runner.LocalPlayer.SetCustomProperties(inventory);
    //}

    //[Rpc(RpcSources.All, RpcTargets.All)]
    //public override void RPC_OnPlayerPropertiesUpdate(PlayerRef targetPlayer)
    //{
    //    // Check if the properties were updated for the local player
    //    if (targetPlayer != null && targetPlayer == Runner.LocalPlayer)
    //    {
    //        // Update local inventory based on received data
    //        foreach (var key in changedProps.Keys)
    //        {
    //            if (key.Equals("Ingredients"))
    //            {
    //                ingredients = (Dictionary<string, int>)changedProps[key];
    //            }
    //            else if (key.Equals("PreparedIngredients"))
    //            {
    //                preparedIngredients = (Dictionary<string, int>)changedProps[key];
    //            }
    //        }
    //    }
    //}
}
