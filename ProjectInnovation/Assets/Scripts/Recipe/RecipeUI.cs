using UnityEngine;
using Fusion;
using TMPro;

public class RecipeUI : NetworkBehaviour
{
    [SerializeField] private TMP_Text recipeTitleText;
    [SerializeField] private GameObject recipePanel;
    [SerializeField] private RecipeManager recipeManager;

    public override void Spawned()
    {
        base.Spawned();
        // Add all the items to the hashset
        recipeManager.OnRecipeSet += AddTextItems;
    }

    private void AddTextItems()
    {
        foreach (var item in recipeManager.items)
        {
            Debug.Log("Adding item: " + item);
            Instantiate(new GameObject(), recipePanel.transform, false).AddComponent<TextMeshProUGUI>().SetText(item);
        }
        SetRecipeTitle();
    }

    private void SetRecipeTitle()
    {
        recipeTitleText.text = recipeManager.recipeTitle;
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        recipeManager.OnRecipeSet -= AddTextItems;
    }
}
