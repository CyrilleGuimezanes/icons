using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Represents a production recipe (plant or manufactured good).
/// Contains the required ingredients, result, and production time.
/// </summary>
[Serializable]
public class ProductionData
{
    /// <summary>
    /// Unique identifier for this production recipe.
    /// </summary>
    public string id;

    /// <summary>
    /// Display name for the production.
    /// </summary>
    public string displayName;

    /// <summary>
    /// List of icon IDs required as ingredients for this production.
    /// </summary>
    public List<string> ingredients = new List<string>();

    /// <summary>
    /// The icon ID of the result when this production is complete.
    /// </summary>
    public string result;

    /// <summary>
    /// Base production time in seconds (10-30 seconds as per game design).
    /// </summary>
    public float baseProductionTime;

    /// <summary>
    /// Type of production (Plant or ManufacturedGood).
    /// </summary>
    public ProductionType productionType;

    /// <summary>
    /// Whether this production has been discovered/unlocked by the player.
    /// </summary>
    public bool isDiscovered;

    public ProductionData() { }

    public ProductionData(string id, string displayName, List<string> ingredients, string result, float baseProductionTime, ProductionType productionType)
    {
        this.id = id;
        this.displayName = displayName;
        this.ingredients = ingredients ?? new List<string>();
        this.result = result;
        this.baseProductionTime = Mathf.Clamp(baseProductionTime, 10f, 30f);
        this.productionType = productionType;
        this.isDiscovered = false;
    }

    /// <summary>
    /// Checks if the given ingredients match this production recipe.
    /// </summary>
    /// <param name="providedIngredients">List of icon IDs to check</param>
    /// <returns>True if the ingredients match this production</returns>
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

    /// <summary>
    /// Gets the production time for a given multiplier.
    /// </summary>
    /// <param name="multiplier">The quantity multiplier (x1, x5, x10, x100)</param>
    /// <returns>Total production time in seconds</returns>
    public float GetProductionTime(int multiplier)
    {
        return baseProductionTime * multiplier;
    }
}

/// <summary>
/// Types of production available in the game.
/// </summary>
public enum ProductionType
{
    /// <summary>
    /// Plant-based production (potager) - growing crops.
    /// </summary>
    Plant = 0,

    /// <summary>
    /// Manufactured goods (industrie) - processing materials.
    /// </summary>
    ManufacturedGood = 1
}
