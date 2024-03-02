using UnityEngine;

public class CookingActionPanel : MonoBehaviour
{
    [SerializeField] private CookingManager cookingManager;
    [SerializeField] private Animator cookingAnimator;
    private CookingEnums.CookingType cookingType;

    private void Awake()
    {
        if(cookingManager == null)
        {
            Debug.LogError("CookingManager is not assigned to CookingActionPanel");
            return;
        }
        if(cookingAnimator == null)
        {
            Debug.LogError("CookingAnimator is not assigned to CookingActionPanel");
            return;
        }
    }

    private void OnEnable()
    {
        cookingManager.OnCookingStarted += CookingStarted;
        cookingManager.OnCookingFinished += CookingFinished;
    }

    private void OnDisable()
    {
        cookingManager.OnCookingStarted -= CookingStarted;
        cookingManager.OnCookingFinished -= CookingFinished;
    }

    //Use this method when setting the animation type (with an event like button press)
    public void SetCookingAction(CookingEnums.CookingType cookingType)
    {
        this.cookingType = cookingType;
    }

    void CookingStarted()
    {
        Debug.Log("Cooking started");
        cookingAnimator.SetBool(cookingType.ToString(),true);
    }

    public void CookingFinished()
    {
        Debug.Log("Cooking finished");
        cookingAnimator.SetBool(cookingType.ToString(),false);
        gameObject.SetActive(false);
    }
}
