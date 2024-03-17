using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffOnTurnEnd : NetworkBehaviour
{
    public override void Spawned()
    {
        base.Spawned();
        GameManager.Instance.OnPlayerTurnEnd += DisableGameObject;
    }
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        GameManager.Instance.OnPlayerTurnEnd -= DisableGameObject;
    }
    private void DisableGameObject(PlayerRef player)
    {
        gameObject.SetActive(false);
    }
}
