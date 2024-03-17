using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;

public class SpinButtonDisabler : NetworkBehaviour
{
    public override void Spawned()
    {
        base.Spawned();
        WheelSystem.OnWheelSpinFinish += DisableSpinButton;
    }
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        WheelSystem.OnWheelSpinFinish -= DisableSpinButton;
    }

    private void DisableSpinButton()
    {
        gameObject.SetActive(false);
    }
}
