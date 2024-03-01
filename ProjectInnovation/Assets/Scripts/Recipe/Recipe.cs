using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "New Recipe", menuName = "Recipe")]
public class Recipe : ScriptableObject
{
    public string recipeTitle;
    public string[] items;
    public Image dishVisual;

#if UNITY_EDITOR
    [ExecuteInEditMode]
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            recipeTitle = name;
        }
    }
#endif
}
