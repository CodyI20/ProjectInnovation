using UnityEngine;

public class EnableButtonOnTradeEnd : MonoBehaviour
{
    private void OnEnable()
    {
        TradeManager.Instance.OnTradeEnded += EnableButton;
    }
    private void OnDestroy()
    {
        TradeManager.Instance.OnTradeEnded -= EnableButton;
    }

    private void EnableButton()
    {
        gameObject.SetActive(true);
    }
}
