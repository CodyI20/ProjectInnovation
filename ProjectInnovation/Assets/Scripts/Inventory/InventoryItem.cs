using CookingEnums;
using Fusion;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        if(itemType == ItemType.Trade)
        {
            button.onClick.AddListener(AddTradeItemToTrade);
        }
        if(itemType == ItemType.Bank)
        {
            button.onClick.AddListener(AddBankItemToTrade);
        }
    }

    private void AddTradeItemToTrade()
    {
        TradeManager.Instance.tradeItem = this;
    }
    private void AddBankItemToTrade()
    {
        TradeManager.Instance.bankItem = this;
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
    }

}
