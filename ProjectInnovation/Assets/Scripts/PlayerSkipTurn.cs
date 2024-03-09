using UnityEngine;

public class PlayerSkipTurn : MonoBehaviour
{
    public void SkipTurn()
    {
        GameManager.Instance.RPC_EndPlayerTurn();
    }
}
