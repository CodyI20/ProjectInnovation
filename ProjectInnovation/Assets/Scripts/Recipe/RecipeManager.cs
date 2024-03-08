using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using System;
using CookingEnums;

public class RecipeManager : MonoBehaviour
{
    public HashSet<RecipeRequirements> items { get; private set; } = new HashSet<RecipeRequirements>();
    public FinalDishes recipeTitle { get; private set; }
    public Sprite recipeVisual { get; private set;}

    public event Action OnRecipeSet;

    public void SetRecipe(Recipe recipe)
    {
        //Set the recipe title
        recipeTitle = recipe.recipeTitle;
        //Set the recipe image
        recipeVisual = recipe.dishVisual;
        // Add all the items to the hashset
        foreach(var item in recipe.recipeRequirements)
        {
            items.Add(item);
        }
        OnRecipeSet?.Invoke();
    }


}
