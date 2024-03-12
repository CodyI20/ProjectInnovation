using UnityEngine;
using UnityEngine.UI;
using Fusion;

[RequireComponent(typeof(Button))]
public class TradeButton : MonoBehaviour
{
    public static event System.Action<TradeButton> OnTradeButtonClicked;
    protected Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.interactable = false;
    }

    private void OnEnable()
    {
        button.onClick.AddListener(TradeButtonClicked);
        if (Inventory.Instance != null)
        {
            SetButtonInteractable();
        }
    }

    private void SetButtonInteractable()
    {
        if(Inventory.Instance.GetIngredients() == null)
        {
            return;
        }
        foreach (var item in Inventory.Instance.GetIngredients())
        {
            if (item.Key == TradeManager.Instance.GetBankItem().Item && item.Value > 0)
            {
                button.interactable = true;
                break;
            }
        }
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(TradeButtonClicked);
    }

    protected virtual void TradeButtonClicked()
    {
        OnTradeButtonClicked?.Invoke(this);
    }
}
