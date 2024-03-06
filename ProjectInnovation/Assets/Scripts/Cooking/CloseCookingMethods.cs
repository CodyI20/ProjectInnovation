using System;
using UnityEngine;

public class CloseCookingMethods : MonoBehaviour
{
    [SerializeField] private CookingManager _cookingManager;
    private void OnEnable()
    {
        CookingManager.OnCookingFinished += CloseCooking;
    }
    private void OnDisable()
    {
        CookingManager.OnCookingFinished -= CloseCooking;
    }

    private void CloseCooking(InventoryItem item, CookingProcess process)
    {
        gameObject.SetActive(false);
    }
}
