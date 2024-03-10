using CookingEnums;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class InventoryItem : MonoBehaviour
{
    public static event Action<InventoryItem> OnItemClicked;

    private Inventory inventory;
    private Button button;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private RawIngredients item;
    [SerializeField] private ItemType itemType;

    public enum ItemType
    {
        Inventory,
        Trade
    }


    private void Awake()
    {
        button = GetComponent<Button>();
    }
    public RawIngredients Item
    {
        get { return item; }
    }
    public int amount { get; private set; }

    private void OnEnable()
    {
        CookingManager.OnCookingFinished += RemoveIngredient;
        if (itemType == ItemType.Inventory)
        {
            inventory = GetComponentInParent<Inventory>();
            inventory.OnIngredientAdded += AddIngredient;
            button.onClick.AddListener(OpenMethodsTab);
        }
    }

    private void OpenMethodsTab()
    {
        if (itemType == ItemType.Inventory)
            inventory.GetCookingMethodsTab().SetActive(true);
        OnItemClicked?.Invoke(this);
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

    private void OnDisable()
    {
        if (itemType == ItemType.Inventory)
            inventory.OnIngredientAdded -= AddIngredient;
        CookingManager.OnCookingFinished -= RemoveIngredient;
    }

}
