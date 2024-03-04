using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using CookingEnums;

public class RecipeManager : NetworkBehaviour
{
    public HashSet<RawIngredients> items { get; private set; } = new HashSet<RawIngredients>();
    public FinalDishes recipeTitle { get; private set; }

    public event Action OnRecipeSet;

    public void SetRecipe(Recipe recipe)
    {
        //Set the recipe title
        recipeTitle = recipe.recipeTitle;
        // Add all the items to the hashset
        foreach(var item in recipe.items)
        {
            items.Add(item);
        }
        OnRecipeSet?.Invoke();
    }


}
