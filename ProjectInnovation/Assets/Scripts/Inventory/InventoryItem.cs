using UnityEngine;
using CookingEnums;
using TMPro;

public class InventoryItem : MonoBehaviour
{
    private Inventory inventory;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private RawIngredients item;
    public RawIngredients Item
    {
        get { return item; }
    }
    public int amount { get; private set; }

    private void OnEnable()
    {
        inventory = GetComponentInParent<Inventory>();
        inventory.OnIngredientAdded += AddIngredient;
    }

    private void AddIngredient(RawIngredients ingredient, int amount)
    {
        if (ingredient != Item) return;
        this.amount += amount;
        amountText.text = this.amount.ToString();
    }

    private void OnDisable()
    {
        inventory.OnIngredientAdded -= AddIngredient;
    }

}
