using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using CookingEnums;

public class RecipeUI : NetworkBehaviour
{
    [SerializeField] private TMP_Text recipeTitleText;
    [SerializeField] private GameObject recipePanel;
    [SerializeField] private RecipeManager recipeManager;

    private List<TextMeshProUGUI> _texts = new List<TextMeshProUGUI>();

    public override void Spawned()
    {
        base.Spawned();
        // Add all the items to the hashset
        recipeManager.OnRecipeSet += AddTextItems;
        CookingManager.OnCookingFinished += CrossOutItem;
    }

    private void AddTextItems()
    {
        foreach (var item in recipeManager.items)
        {
            Debug.Log("Adding item: " + item);
            Instantiate(new GameObject(), recipePanel.transform, false).AddComponent<TextMeshProUGUI>().SetText(item.rawIngredient.ToString());
        }
        AddToTextsList();
        SetRecipeTitle();
    }

    //This functions will be used in aiding the crossing out logic by loading all the text items into a list
    private void AddToTextsList()
    {
        foreach(var textItem in recipePanel.GetComponentsInChildren<TextMeshProUGUI>())
        {
            _texts.Add(textItem);
        }
    }

    private void CrossOutItem(InventoryItem item, CookingProcess process)
    {
        foreach(var textItem in _texts)
        {
            if(textItem.text == item.Item.ToString())
            {
                textItem.fontStyle = FontStyles.Strikethrough;
                break;
            }
        }
    }

    private void SetRecipeTitle()
    {
        recipeTitleText.text = recipeManager.recipeTitle.ToString();
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        recipeManager.OnRecipeSet -= AddTextItems;
        CookingManager.OnCookingFinished -= CrossOutItem;
    }
}
