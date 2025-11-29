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
    /// Initializes default recipes from the icons database.
    /// </summary>
    private void InitializeDefaultRecipes()
    {
        // Food & Cooking Recipes
        recipes.Add(new RecipeData("combo_bread", new List<string> { "grain", "precision_manufacturing" }, "bakery_dining", "Pain"));
        recipes.Add(new RecipeData("combo_flour", new List<string> { "grain", "hardware" }, "nutrition", "Farine"));
        recipes.Add(new RecipeData("combo_agriculture", new List<string> { "water_drop", "grain" }, "agriculture", "Agriculture"));
        recipes.Add(new RecipeData("combo_cookie", new List<string> { "grain", "egg" }, "cookie", "Cookie"));
        recipes.Add(new RecipeData("combo_cake", new List<string> { "grain", "egg", "nutrition" }, "cake", "Gâteau"));
        recipes.Add(new RecipeData("combo_icecream", new List<string> { "ac_unit", "nutrition" }, "icecream", "Glace"));
        recipes.Add(new RecipeData("combo_meal", new List<string> { "bakery_dining", "nutrition" }, "local_dining", "Repas"));
        recipes.Add(new RecipeData("combo_lunch", new List<string> { "local_dining", "wb_sunny" }, "lunch_dining", "Déjeuner"));
        recipes.Add(new RecipeData("combo_dinner", new List<string> { "local_dining", "cloud" }, "dinner_dining", "Dîner"));
        recipes.Add(new RecipeData("combo_wine", new List<string> { "water_drop", "spa", "wb_sunny" }, "wine_bar", "Vin"));
        recipes.Add(new RecipeData("combo_liquor", new List<string> { "wine_bar", "local_fire_department" }, "liquor", "Liqueur"));
        recipes.Add(new RecipeData("combo_ramen", new List<string> { "water_drop", "grain", "local_fire_department" }, "ramen_dining", "Ramen"));
        recipes.Add(new RecipeData("combo_kebab", new List<string> { "pets", "local_fire_department", "hardware" }, "kebab_dining", "Kebab"));
        recipes.Add(new RecipeData("combo_set_meal", new List<string> { "lunch_dining", "dinner_dining" }, "set_meal", "Plateau Repas"));

        // Building Recipes
        recipes.Add(new RecipeData("combo_construction", new List<string> { "forest", "hardware" }, "construction", "Construction"));
        recipes.Add(new RecipeData("combo_cottage", new List<string> { "forest", "construction" }, "cottage", "Cottage"));
        recipes.Add(new RecipeData("combo_home", new List<string> { "construction", "landscape" }, "home", "Maison"));
        recipes.Add(new RecipeData("combo_cabin", new List<string> { "forest", "forest", "hardware" }, "cabin", "Cabane"));
        recipes.Add(new RecipeData("combo_restaurant", new List<string> { "home", "local_dining" }, "restaurant", "Restaurant"));
        recipes.Add(new RecipeData("combo_cafe", new List<string> { "home", "emoji_food_beverage" }, "local_cafe", "Café"));
        recipes.Add(new RecipeData("combo_castle", new List<string> { "home", "home", "landscape" }, "castle", "Château"));
        recipes.Add(new RecipeData("combo_church", new List<string> { "home", "self_improvement" }, "church", "Église"));
        recipes.Add(new RecipeData("combo_temple", new List<string> { "home", "psychology" }, "temple_buddhist", "Temple"));
        recipes.Add(new RecipeData("combo_stadium", new List<string> { "construction", "construction", "construction" }, "stadium", "Stade"));
        recipes.Add(new RecipeData("combo_apartment", new List<string> { "home", "home", "construction" }, "apartment", "Appartement"));
        recipes.Add(new RecipeData("combo_domain", new List<string> { "castle", "park" }, "domain", "Domaine"));

        // Technology & Engineering Recipes
        recipes.Add(new RecipeData("combo_science", new List<string> { "engineering", "biotech" }, "science", "Science"));
        recipes.Add(new RecipeData("combo_biotech", new List<string> { "eco", "precision_manufacturing" }, "biotech", "Biotech"));
        recipes.Add(new RecipeData("combo_factory", new List<string> { "construction", "precision_manufacturing" }, "factory", "Usine"));
        recipes.Add(new RecipeData("combo_engineering", new List<string> { "hardware", "precision_manufacturing" }, "engineering", "Ingénierie"));
        recipes.Add(new RecipeData("combo_memory", new List<string> { "developer_board", "bolt" }, "memory", "Mémoire"));
        recipes.Add(new RecipeData("combo_circuit", new List<string> { "precision_manufacturing", "bolt" }, "developer_board", "Circuit"));
        recipes.Add(new RecipeData("combo_robot", new List<string> { "developer_board", "memory", "engineering" }, "smart_toy", "Robot"));
        recipes.Add(new RecipeData("combo_android", new List<string> { "smart_toy", "memory" }, "adb", "Android"));
        recipes.Add(new RecipeData("combo_hub", new List<string> { "developer_board", "developer_board", "memory" }, "hub", "Hub"));
        recipes.Add(new RecipeData("combo_plumbing", new List<string> { "water_drop", "hardware" }, "plumbing", "Plomberie"));
        recipes.Add(new RecipeData("combo_electrical", new List<string> { "bolt", "hardware" }, "electrical_services", "Électricité"));

        // Vehicle Recipes
        recipes.Add(new RecipeData("combo_electric_car", new List<string> { "bolt", "engineering" }, "electric_car", "Voiture Électrique"));
        recipes.Add(new RecipeData("combo_plane", new List<string> { "air", "engineering", "precision_manufacturing" }, "flight", "Avion"));
        recipes.Add(new RecipeData("combo_boat", new List<string> { "water_drop", "forest", "construction" }, "sailing", "Bateau"));
        recipes.Add(new RecipeData("combo_train", new List<string> { "precision_manufacturing", "precision_manufacturing", "construction" }, "train", "Train"));
        recipes.Add(new RecipeData("combo_helicopter", new List<string> { "flight", "air" }, "helicopter", "Hélicoptère"));

        // Space & Legendary Recipes
        recipes.Add(new RecipeData("combo_rocket", new List<string> { "factory", "engineering", "science" }, "rocket_launch", "Fusée"));
        recipes.Add(new RecipeData("combo_satellite", new List<string> { "rocket_launch", "developer_board" }, "satellite_alt", "Satellite"));
        recipes.Add(new RecipeData("combo_world", new List<string> { "satellite_alt", "satellite_alt" }, "public", "Monde"));
        recipes.Add(new RecipeData("combo_stars", new List<string> { "wb_sunny", "wb_sunny", "wb_sunny" }, "stars", "Étoiles"));
        recipes.Add(new RecipeData("combo_magic", new List<string> { "stars", "psychology" }, "auto_awesome", "Magie"));
        recipes.Add(new RecipeData("combo_infinite", new List<string> { "stars", "stars", "auto_awesome" }, "all_inclusive", "Infini"));

        // Element & Weather Recipes
        recipes.Add(new RecipeData("combo_bolt", new List<string> { "cloud", "local_fire_department" }, "bolt", "Éclair"));
        recipes.Add(new RecipeData("combo_flash", new List<string> { "bolt", "bolt" }, "flash_on", "Flash"));
        recipes.Add(new RecipeData("combo_flame", new List<string> { "local_fire_department", "air" }, "whatshot", "Flamme"));
        recipes.Add(new RecipeData("combo_fire_station", new List<string> { "whatshot", "home" }, "local_fire_station", "Feu Ardent"));
        recipes.Add(new RecipeData("combo_volcano", new List<string> { "landscape", "whatshot", "local_fire_department" }, "volcano", "Volcan"));
        recipes.Add(new RecipeData("combo_tsunami", new List<string> { "water_drop", "water_drop", "water_drop" }, "tsunami", "Tsunami"));
        recipes.Add(new RecipeData("combo_tornado", new List<string> { "air", "air", "cloud" }, "tornado", "Tornade"));
        recipes.Add(new RecipeData("combo_cyclone", new List<string> { "tornado", "water_drop" }, "cyclone", "Cyclone"));
        recipes.Add(new RecipeData("combo_flare", new List<string> { "wb_sunny", "whatshot" }, "flare", "Flare"));
        recipes.Add(new RecipeData("combo_light", new List<string> { "wb_sunny", "flash_on" }, "brightness_high", "Lumière"));

        // Resource & Nature Recipes
        recipes.Add(new RecipeData("combo_diamond", new List<string> { "landscape", "landscape", "local_fire_department" }, "diamond", "Diamant"));
        recipes.Add(new RecipeData("combo_recycling", new List<string> { "eco", "hardware" }, "recycling", "Recyclage"));
        recipes.Add(new RecipeData("combo_compost", new List<string> { "eco", "grass", "bug_report" }, "compost", "Compost"));
        recipes.Add(new RecipeData("combo_oil", new List<string> { "landscape", "landscape", "precision_manufacturing" }, "oil_barrel", "Pétrole"));
        recipes.Add(new RecipeData("combo_iron", new List<string> { "landscape", "local_fire_department", "precision_manufacturing" }, "iron", "Fer"));
        recipes.Add(new RecipeData("combo_coal", new List<string> { "forest", "local_fire_department", "terrain" }, "coal", "Charbon"));

        // Mind & Spirit Recipes
        recipes.Add(new RecipeData("combo_psychology", new List<string> { "pets", "spa", "eco" }, "psychology", "Esprit"));
        recipes.Add(new RecipeData("combo_meditation", new List<string> { "psychology", "park" }, "self_improvement", "Méditation"));

        // Achievement Recipes
        recipes.Add(new RecipeData("combo_trophy", new List<string> { "diamond", "workspace_premium" }, "emoji_events", "Trophée"));
        recipes.Add(new RecipeData("combo_medal", new List<string> { "emoji_events", "military_tech" }, "military_tech", "Médaille"));
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
