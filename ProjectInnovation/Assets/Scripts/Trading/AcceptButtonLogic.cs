using UnityEngine;
using Fusion;
using UnityEngine.UI;

public class AcceptButtonLogic : NetworkBehaviour
{
    public InventoryItem itemNeeded { get; set;}
    [SerializeField] private Inventory inventory;
    Button button;

    public override void Spawned()
    {
        base.Spawned();
        button = GetComponent<Button>();
        button.interactable = false;
        foreach (var item in inventory.GetIngredients())
        {
            if(item.Key == itemNeeded.Item && item.Value > 0)
            {
                button.interactable = true;
                break;
            }
        }
        if (button.interactable)
        {
            button.onClick.AddListener(NotifyTradeInitiator);
        }
    }

    private void NotifyTradeInitiator()
    {
        Debug.Log("Will trade...");
        TradeManager.Instance.NotifyTradeInitiator(true);
    }


}
