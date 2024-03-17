
public class TradeDeclineButton : TradeButton
{
    protected override void RPC_TradeButtonClicked()
    {
        base.RPC_TradeButtonClicked();
        TradeManager.Instance.RPC_DeclineTrade();
    }
}
