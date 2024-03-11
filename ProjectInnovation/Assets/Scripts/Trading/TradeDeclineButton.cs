
public class TradeDeclineButton : TradeButton
{
    protected override void TradeButtonClicked()
    {
        base.TradeButtonClicked();
        TradeManager.Instance.RPC_DeclineTrade();
    }
}
