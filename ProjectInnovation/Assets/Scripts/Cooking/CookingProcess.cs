using UnityEngine;

[CreateAssetMenu(fileName = "CookingProcess", menuName = "Cooking/CookingProcess")]
public class CookingProcess : ScriptableObject
{
    [Header("Cooking Process")]
    public float CookingTime;
    public CookingEnums.CookingType CookingType;

    [Header("Cooking FX")]
    public AnimationClip CookingAnimation;
    public AudioClip CookingSound;
}
