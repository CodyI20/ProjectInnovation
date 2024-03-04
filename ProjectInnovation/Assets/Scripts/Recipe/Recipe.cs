using UnityEngine;
using UnityEngine.UI;
using CookingEnums;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Recipe")]
public class Recipe : ScriptableObject
{
    [Tooltip("Make sure the asset name is the same as the Recipe Title in order to avoid confusion")] public FinalDishes recipeTitle;
    public RawIngredients[] items;
    public Image dishVisual;
}
