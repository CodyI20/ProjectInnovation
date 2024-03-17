using Fusion;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TradeButton : NetworkBehaviour
{
    public static event System.Action<TradeButton> OnTradeButtonClicked;
    protected Button button;
    [HideInInspector][Networked] public bool isButtonInteractable { get; private set; } = false;

    public override void Spawned()
    {
        base.Spawned();
        button = GetComponent<Button>();
        button.interactable = false;
        isButtonInteractable = false;
        button.onClick.AddListener(RPC_TradeButtonClicked);
        if (Inventory.Instance != null)
        {
            RPC_SetButtonInteractable();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_SetButtonInteractable()
    {
        if (Inventory.Instance.GetIngredients() == null)
        {
            //Do something here to automatically decline the trade!!!
            return;
        }
        foreach (var item in Inventory.Instance.GetIngredients())
        {
            if (item.Key == TradeManager.Instance.GetBankItem().Item && item.Value > 0)
            {
                isButtonInteractable = true;
                button.interactable = true;
                break;
            }
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        button.onClick.RemoveListener(RPC_TradeButtonClicked);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    protected virtual void RPC_TradeButtonClicked()
    {
        OnTradeButtonClicked?.Invoke(this);
    }
}
