
public class TradeAcceptButton : TradeButton
{
    protected override void RPC_TradeButtonClicked()
    {
        base.RPC_TradeButtonClicked();
        TradeManager.Instance.RPC_AcceptTrade();
    }
}
