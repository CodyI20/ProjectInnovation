using UnityEngine;
using Fusion;

[RequireComponent(typeof(RecipeManager))]
public class GenerateRandomRecipe : NetworkBehaviour
{
    [SerializeField] private Recipe[] recipes;
    private RecipeManager recipeManager;

    public override void Spawned()
    {
        recipeManager = GetComponent<RecipeManager>();
        Recipe randomRecipe = recipes[Random.Range(0, recipes.Length)];
        recipeManager.SetRecipe(randomRecipe);

        Debug.Log("Random Recipe: " + randomRecipe.recipeTitle);
    }
}
