using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButtonDisabler : NetworkBehaviour
{
    private void OnEnable()
    {
        GameManager.Instance.OnMatchStart += DisableStartButton;
    }
    private void OnDisable()
    {
        GameManager.Instance.OnMatchStart -= DisableStartButton;
    }


    void DisableStartButton()
    {
        Destroy(gameObject);
    }

}
