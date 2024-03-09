using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class EnableOnGameStart : NetworkBehaviour
{
    public override void Spawned()
    {
        GameManager.OnMatchStart += Enable;
        gameObject.SetActive(false);
    }
  
    void Enable()
    {
        gameObject.SetActive(true);
        GameManager.OnMatchStart -= Enable;
    }
}
