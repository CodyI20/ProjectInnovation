using UnityEngine;
using Fusion;

public class PlayerSkipTurn : NetworkBehaviour
{
    public void SkipTurn()
    {
        GameManager.Instance.EndPlayerTurn();
    }
}
