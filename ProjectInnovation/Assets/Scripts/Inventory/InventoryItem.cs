using UnityEngine;
using CookingEnums;
using TMPro;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Button))]
public class InventoryItem : MonoBehaviour
{
    public static event Action<RawIngredients> OnIngredientDeleted;

    private Inventory inventory;
    private Button button;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private RawIngredients item;


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
        inventory = GetComponentInParent<Inventory>();
        inventory.OnIngredientAdded += AddIngredient;
        inventory.GetCookingManager().OnCookingFinished += RemoveIngredient;
        button.onClick.AddListener(OpenMethodsTab);
    }

    private void OpenMethodsTab()
    {
        inventory.GetCookingMethodsTab().SetActive(true);
    }

    private void AddIngredient(RawIngredients ingredient, int amount)
    {
        if (ingredient != Item) return;
        this.amount += amount;
        amountText.text = this.amount.ToString();
    }

    private void RemoveIngredient()
    {
        OnIngredientDeleted?.Invoke(item);
        this.amount -= 1;
        amountText.text = this.amount.ToString();
        if(this.amount <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        inventory.OnIngredientAdded -= AddIngredient;
        inventory.GetCookingManager().OnCookingFinished -= RemoveIngredient;
    }

}
