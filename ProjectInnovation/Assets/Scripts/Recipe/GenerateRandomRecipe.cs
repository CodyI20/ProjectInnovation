using UnityEngine;
using Fusion;
using System.Collections.Generic;

[RequireComponent(typeof(RecipeManager))]
public class GenerateRandomRecipe : MonoBehaviour
{
    [SerializeField] private List<Recipe> recipes;
    private RecipeManager recipeManager;

    private void Start()
    {
        recipeManager = GetComponent<RecipeManager>();
        Recipe randomRecipe = recipes[Random.Range(0, recipes.Count)];
        recipeManager.SetRecipe(randomRecipe);

        Debug.Log("Random Recipe: " + randomRecipe.recipeTitle);
    }
}
