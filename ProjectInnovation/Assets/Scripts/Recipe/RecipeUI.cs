using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;
using CookingEnums;
using System;

public class RecipeUI : NetworkBehaviour
{
    public static event Action<PlayerRef> OnItemCrossedOut;
    [SerializeField] private TMP_Text recipeTitleText;
    [SerializeField] private Image recipeImage;
    [SerializeField] private GameObject recipePanel;
    [SerializeField] private RecipeManager recipeManager;

    private List<TextMeshProUGUI> _texts = new List<TextMeshProUGUI>();
    [SerializeField] private GameObject textItemPrefab;

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

            // Instantiate the prefab instead of a new empty GameObject.
            GameObject textItem = Instantiate(textItemPrefab, recipePanel.transform, false);

            // Access the TextMeshProUGUI component from the instantiated prefab.
            TextMeshProUGUI textPart = textItem.GetComponent<TextMeshProUGUI>();

            // Customize the textPart properties if needed.
            textPart.text = item.cookingProcess.CookingType.ToString() + "ed " + item.rawIngredient.ToString();
            textPart.fontSize = 20;
            textPart.color = Color.black;
        }
        AddToTextsList();
        SetRecipeTitle();
        SetRecipeImage();
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
            foreach(var recipeItem in recipeManager.items)
            {
                if(textItem.text.Contains(item.Item.ToString()) && item.Item.ToString() == recipeItem.rawIngredient.ToString() && process == recipeItem.cookingProcess)
                {
                    textItem.fontStyle = FontStyles.Strikethrough;
                    OnItemCrossedOut?.Invoke(Runner.LocalPlayer);
                }
            }
        }
    }

    private void SetRecipeTitle()
    {
        recipeTitleText.text = recipeManager.recipeTitle.ToString();
    }

    private void SetRecipeImage()
    {
        recipeImage.sprite = recipeManager.recipeVisual;
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        recipeManager.OnRecipeSet -= AddTextItems;
        CookingManager.OnCookingFinished -= CrossOutItem;
    }
}
