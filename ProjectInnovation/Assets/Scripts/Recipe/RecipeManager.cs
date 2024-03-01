using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class RecipeManager : NetworkBehaviour
{
    public HashSet<string> items { get; private set; } = new HashSet<string>();
    public string recipeTitle { get; private set; }

    public event Action OnRecipeSet;

    public void SetRecipe(Recipe recipe)
    {
        Debug.Log("OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO");
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
