using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Represents a recipe for combining icons.
/// Contains the required ingredients and the resulting icon.
/// </summary>
[Serializable]
public class RecipeData
{
    /// <summary>
    /// Unique identifier for this recipe.
    /// </summary>
    public string id;
    
    /// <summary>
    /// List of icon IDs required for this recipe.
    /// Order does not matter.
    /// </summary>
    public List<string> ingredients = new List<string>();
    
    /// <summary>
    /// The icon ID of the result when this recipe is crafted.
    /// </summary>
    public string result;
    
    /// <summary>
    /// Optional display name for the recipe.
    /// </summary>
    public string displayName;
    
    /// <summary>
    /// Whether this recipe has been discovered by the player.
    /// </summary>
    public bool isDiscovered;
    
    public RecipeData() { }
    
    public RecipeData(string id, List<string> ingredients, string result, string displayName = null)
    {
        this.id = id;
        this.ingredients = ingredients ?? new List<string>();
        this.result = result;
        this.displayName = displayName ?? result;
        this.isDiscovered = false;
    }
    
    /// <summary>
    /// Checks if the given ingredients match this recipe.
    /// </summary>
    /// <param name="providedIngredients">List of icon IDs to check</param>
    /// <returns>True if the ingredients match this recipe</returns>
    public bool MatchesIngredients(List<string> providedIngredients)
    {
        if (providedIngredients == null || providedIngredients.Count != ingredients.Count)
        {
            return false;
        }
        
        // Create sorted copies to compare
        var sortedRecipe = new List<string>(ingredients);
        var sortedProvided = new List<string>(providedIngredients);
        
        sortedRecipe.Sort();
        sortedProvided.Sort();
        
        for (int i = 0; i < sortedRecipe.Count; i++)
        {
            if (sortedRecipe[i] != sortedProvided[i])
            {
                return false;
            }
        }
        
        return true;
    }
}

/// <summary>
/// Manages all recipes in the game.
/// Placeholder implementation - recipes will be populated later.
/// </summary>
public class RecipeManager : MonoBehaviour
{
    public static RecipeManager Instance { get; private set; }
    
    [SerializeField]
    private List<RecipeData> recipes = new List<RecipeData>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDefaultRecipes();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Initializes default recipes.
    /// This is a placeholder - recipes will be configured later.
    /// </summary>
    private void InitializeDefaultRecipes()
    {
        // Placeholder: Add some example recipes
        // These will be replaced with actual game recipes later
        
        // Example: wheat + machine = flour
        // recipes.Add(new RecipeData(
        //     "wheat_flour",
        //     new List<string> { "wheat", "machine" },
        //     "flour",
        //     "Farine"
        // ));
    }
    
    /// <summary>
    /// Tries to find a recipe matching the given ingredients.
    /// </summary>
    /// <param name="ingredients">List of icon IDs to check</param>
    /// <returns>The matching recipe, or null if no match</returns>
    public RecipeData FindMatchingRecipe(List<string> ingredients)
    {
        foreach (var recipe in recipes)
        {
            if (recipe.MatchesIngredients(ingredients))
            {
                return recipe;
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// Adds a new recipe to the manager.
    /// </summary>
    /// <param name="recipe">The recipe to add</param>
    public void AddRecipe(RecipeData recipe)
    {
        if (recipe != null && !string.IsNullOrEmpty(recipe.id))
        {
            recipes.Add(recipe);
        }
    }
    
    /// <summary>
    /// Gets all discovered recipes.
    /// </summary>
    public List<RecipeData> GetDiscoveredRecipes()
    {
        return recipes.FindAll(r => r.isDiscovered);
    }
    
    /// <summary>
    /// Marks a recipe as discovered.
    /// </summary>
    /// <param name="recipeId">The recipe ID to mark as discovered</param>
    public void DiscoverRecipe(string recipeId)
    {
        var recipe = recipes.Find(r => r.id == recipeId);
        if (recipe != null)
        {
            recipe.isDiscovered = true;
        }
    }
}
