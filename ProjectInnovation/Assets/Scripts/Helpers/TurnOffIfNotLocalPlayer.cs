using Fusion;

public class TurnOffIfNotLocalPlayer : NetworkBehaviour
{
    public override void Spawned()
    {
        base.Spawned();
        RPC_TurnOnOff(Runner.LocalPlayer);
    }
    //This method is called when the object is spawned and makes it so that only the local player can see the object
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_TurnOnOff(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
