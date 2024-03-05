using UnityEngine;

public class CloseCookingMethods : MonoBehaviour
{
    [SerializeField] private CookingManager _cookingManager;
    private void OnEnable()
    {
        _cookingManager.OnCookingFinished += CloseCooking;
    }
    private void OnDisable()
    {
        _cookingManager.OnCookingFinished -= CloseCooking;
    }
    private void CloseCooking()
    {
        gameObject.SetActive(false);
    }
}
