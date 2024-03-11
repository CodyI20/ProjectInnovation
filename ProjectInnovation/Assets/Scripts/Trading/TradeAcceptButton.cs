
public class TradeAcceptButton : TradeButton
{
    protected override void TradeButtonClicked()
    {
        base.TradeButtonClicked();
        TradeManager.Instance.RPC_AcceptTrade();
    }
}
