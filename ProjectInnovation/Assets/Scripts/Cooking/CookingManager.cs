using Fusion;
using System;
using System.Collections;
using UnityEngine;

public class CookingManager : NetworkBehaviour
{
    public event Action OnCookingStarted;
    public event Action OnCookingFinished;

    public override void Spawned()
    {
        base.Spawned();
    }

    public void StartCooking(CookingProcess cookingProcess)
    {
        OnCookingStarted?.Invoke();
        StartCoroutine(Cooking(cookingProcess));
    }

    private IEnumerator Cooking(CookingProcess cookingProcess)
    {
        Debug.Log("Cooking started");
        yield return new WaitForSeconds(cookingProcess.CookingTime);
        Debug.Log("Cooking finished");
        OnCookingFinished?.Invoke();
    }

}