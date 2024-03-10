using CookingEnums;
using Fusion;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Button))]
public class InventoryItem : NetworkBehaviour
{
    public static event Action<InventoryItem> OnInventoryItemClicked;

    private Inventory inventory;
    private TradeInventory tradeInventory;
    private Button button;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private RawIngredients item;
    [SerializeField] private ItemType itemType;
    [SerializeField] private UnityEvent OnTradeInitiated;

    public enum ItemType
    {
        Inventory,
        Trade,
        Bank
    }

    public RawIngredients Item
    {
        get { return item; }
    }
    public int amount { get; private set; } = 0;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        CookingManager.OnCookingFinished += RemoveIngredient;
        if (inventory == null)
        {
            if (itemType == ItemType.Inventory)
            {
                inventory = GetComponentInParent<Inventory>();
                inventory.OnIngredientAdded += AddIngredient;
            }
        }
        if (itemType == ItemType.Inventory)
        {
            button.onClick.AddListener(OpenMethodsTab);
        }
        else if (itemType == ItemType.Bank)
        {
            button.onClick.AddListener(AddToTradeBank);
        }
        else
        {
            button.onClick.AddListener(AddToTradeTrade);
        }
    }

    public override void Spawned()
    {
        base.Spawned();
    }

    private void AddToTradeBank()
    {
        Debug.Log("Adding to trade bank");
        TradeManager.Instance.bankItem = this;
    }
    private void AddToTradeTrade()
    {
        Debug.Log($"Name of the item: {item}, script: {this})");
        TradeManager.Instance.tradeItem = this;
    }
    private void OpenTradeConfirmation()
    {
        OnTradeInitiated?.Invoke();
    }

    private void OpenMethodsTab()
    {
        inventory.GetCookingMethodsTab().SetActive(true);
        OnInventoryItemClicked?.Invoke(this);
    }

    private void AddIngredient(RawIngredients ingredient, int amount)
    {
        if (ingredient != Item) return;
        this.amount += amount;
        amountText.text = this.amount.ToString();
    }

    private void RemoveIngredient(InventoryItem item, CookingProcess process)
    {
        //OnIngredientDeleted?.Invoke(this.item);
        if (Item == item.Item)
        {
            this.amount -= 1;
            amountText.text = this.amount.ToString();
            if (this.amount <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        CookingManager.OnCookingFinished -= RemoveIngredient;
        if (inventory != null)
            inventory.OnIngredientAdded -= AddIngredient;
        button.onClick.RemoveAllListeners();
    }

}
