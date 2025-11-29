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
/// Contains 199 combination recipes.
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
    /// Initializes all 199 combination recipes from the icons database.
    /// </summary>
    private void InitializeDefaultRecipes()
    {
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
        recipes.Add(new RecipeData("combo_pizza", new List<string> { "grain", "local_fire_department" }, "local_pizza", "Pizza"));
        recipes.Add(new RecipeData("combo_fastfood", new List<string> { "bakery_dining", "pets" }, "fastfood", "Fast-food"));
        recipes.Add(new RecipeData("combo_rice", new List<string> { "grain", "water_drop", "local_fire_department" }, "rice_bowl", "Riz"));
        recipes.Add(new RecipeData("combo_soup", new List<string> { "water_drop", "nutrition", "local_fire_department" }, "soup_kitchen", "Soupe"));
        recipes.Add(new RecipeData("combo_breakfast", new List<string> { "egg", "bakery_dining" }, "breakfast_dining", "Petit-déjeuner"));
        recipes.Add(new RecipeData("combo_brunch", new List<string> { "breakfast_dining", "lunch_dining" }, "brunch_dining", "Brunch"));
        recipes.Add(new RecipeData("combo_tapas", new List<string> { "local_dining", "wine_bar" }, "tapas", "Tapas"));
        recipes.Add(new RecipeData("combo_bento", new List<string> { "rice_bowl", "local_dining" }, "bento", "Bento"));
        recipes.Add(new RecipeData("combo_coffee", new List<string> { "water_drop", "grain", "local_fire_department" }, "emoji_food_beverage", "Café"));
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
        recipes.Add(new RecipeData("combo_warehouse", new List<string> { "construction", "construction" }, "warehouse", "Entrepôt"));
        recipes.Add(new RecipeData("combo_store", new List<string> { "home", "hardware" }, "store", "Magasin"));
        recipes.Add(new RecipeData("combo_shop", new List<string> { "store", "local_dining" }, "shop", "Boutique"));
        recipes.Add(new RecipeData("combo_garage", new List<string> { "home", "directions_car" }, "garage_home", "Garage"));
        recipes.Add(new RecipeData("combo_barn", new List<string> { "forest", "agriculture" }, "barn", "Grange"));
        recipes.Add(new RecipeData("combo_silo", new List<string> { "construction", "grain" }, "silo", "Silo"));
        recipes.Add(new RecipeData("combo_villa", new List<string> { "home", "home", "park" }, "villa", "Villa"));
        recipes.Add(new RecipeData("combo_chalet", new List<string> { "cabin", "snow" }, "chalet", "Chalet"));
        recipes.Add(new RecipeData("combo_bungalow", new List<string> { "home", "park" }, "bungalow", "Bungalow"));
        recipes.Add(new RecipeData("combo_hospital", new List<string> { "home", "medical_services" }, "local_hospital", "Hôpital"));
        recipes.Add(new RecipeData("combo_library", new List<string> { "home", "school" }, "local_library", "Bibliothèque"));
        recipes.Add(new RecipeData("combo_mall", new List<string> { "store", "store", "store" }, "local_mall", "Centre Commercial"));
        recipes.Add(new RecipeData("combo_cinema", new List<string> { "home", "movie" }, "local_movies", "Cinéma"));
        recipes.Add(new RecipeData("combo_airport", new List<string> { "construction", "flight" }, "local_airport", "Aéroport"));
        recipes.Add(new RecipeData("combo_bank", new List<string> { "home", "savings" }, "local_atm", "Banque"));
        recipes.Add(new RecipeData("combo_museum", new List<string> { "home", "palette" }, "museum", "Musée"));
        recipes.Add(new RecipeData("combo_school", new List<string> { "home", "school" }, "school", "École"));
        recipes.Add(new RecipeData("combo_city", new List<string> { "apartment", "apartment", "apartment" }, "location_city", "Ville"));
        recipes.Add(new RecipeData("combo_corporate", new List<string> { "location_city", "work" }, "corporate_fare", "Siège Social"));
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
        recipes.Add(new RecipeData("combo_router", new List<string> { "developer_board", "wifi" }, "router", "Routeur"));
        recipes.Add(new RecipeData("combo_terminal", new List<string> { "developer_board", "code" }, "terminal", "Terminal"));
        recipes.Add(new RecipeData("combo_api", new List<string> { "code", "developer_board" }, "api", "API"));
        recipes.Add(new RecipeData("combo_sensors", new List<string> { "developer_board", "engineering" }, "sensors", "Capteurs"));
        recipes.Add(new RecipeData("combo_smart_display", new List<string> { "developer_board", "tv" }, "smart_display", "Écran Intelligent"));
        recipes.Add(new RecipeData("combo_thermostat", new List<string> { "sensors", "hvac" }, "device_thermostat", "Thermostat"));
        recipes.Add(new RecipeData("combo_solar", new List<string> { "wb_sunny", "developer_board" }, "solar_power", "Solaire"));
        recipes.Add(new RecipeData("combo_wind", new List<string> { "air", "precision_manufacturing" }, "wind_power", "Éolien"));
        recipes.Add(new RecipeData("combo_camera", new List<string> { "developer_board", "lens" }, "photo_camera", "Appareil Photo"));
        recipes.Add(new RecipeData("combo_video", new List<string> { "photo_camera", "movie" }, "videocam", "Caméra"));
        recipes.Add(new RecipeData("combo_electric_car", new List<string> { "bolt", "engineering" }, "electric_car", "Voiture Électrique"));
        recipes.Add(new RecipeData("combo_plane", new List<string> { "air", "engineering", "precision_manufacturing" }, "flight", "Avion"));
        recipes.Add(new RecipeData("combo_boat", new List<string> { "water_drop", "forest", "construction" }, "sailing", "Bateau"));
        recipes.Add(new RecipeData("combo_train", new List<string> { "precision_manufacturing", "precision_manufacturing", "construction" }, "train", "Train"));
        recipes.Add(new RecipeData("combo_helicopter", new List<string> { "flight", "air" }, "helicopter", "Hélicoptère"));
        recipes.Add(new RecipeData("combo_car", new List<string> { "precision_manufacturing", "engineering" }, "directions_car", "Voiture"));
        recipes.Add(new RecipeData("combo_bus", new List<string> { "directions_car", "directions_car" }, "directions_bus", "Bus"));
        recipes.Add(new RecipeData("combo_bike", new List<string> { "hardware", "hardware" }, "directions_bike", "Vélo"));
        recipes.Add(new RecipeData("combo_moto", new List<string> { "directions_bike", "precision_manufacturing" }, "motorcycle", "Moto"));
        recipes.Add(new RecipeData("combo_scooter", new List<string> { "directions_bike", "bolt" }, "electric_scooter", "Trottinette"));
        recipes.Add(new RecipeData("combo_skateboard", new List<string> { "forest", "hardware" }, "skateboarding", "Skateboard"));
        recipes.Add(new RecipeData("combo_subway", new List<string> { "train", "construction" }, "subway", "Métro"));
        recipes.Add(new RecipeData("combo_tram", new List<string> { "train", "bolt" }, "tram", "Tramway"));
        recipes.Add(new RecipeData("combo_cable_car", new List<string> { "construction", "air" }, "cable_car", "Téléphérique"));
        recipes.Add(new RecipeData("combo_truck", new List<string> { "directions_car", "construction" }, "local_shipping", "Camion"));
        recipes.Add(new RecipeData("combo_fire_truck", new List<string> { "local_shipping", "local_fire_department" }, "fire_truck", "Camion Pompier"));
        recipes.Add(new RecipeData("combo_rv", new List<string> { "local_shipping", "home" }, "rv_hookup", "Camping-car"));
        recipes.Add(new RecipeData("combo_tractor", new List<string> { "precision_manufacturing", "agriculture" }, "agriculture_machinery", "Tracteur"));
        recipes.Add(new RecipeData("combo_rocket", new List<string> { "factory", "engineering", "science" }, "rocket_launch", "Fusée"));
        recipes.Add(new RecipeData("combo_satellite", new List<string> { "rocket_launch", "developer_board" }, "satellite_alt", "Satellite"));
        recipes.Add(new RecipeData("combo_world", new List<string> { "satellite_alt", "satellite_alt" }, "public", "Monde"));
        recipes.Add(new RecipeData("combo_stars", new List<string> { "wb_sunny", "wb_sunny", "wb_sunny" }, "stars", "Étoiles"));
        recipes.Add(new RecipeData("combo_magic", new List<string> { "stars", "psychology" }, "auto_awesome", "Magie"));
        recipes.Add(new RecipeData("combo_infinite", new List<string> { "stars", "stars", "auto_awesome" }, "all_inclusive", "Infini"));
        recipes.Add(new RecipeData("combo_astronomy", new List<string> { "stars", "science" }, "astronomy", "Astronomie"));
        recipes.Add(new RecipeData("combo_orbit", new List<string> { "rocket_launch", "public" }, "orbit", "Orbite"));
        recipes.Add(new RecipeData("combo_exploration", new List<string> { "rocket_launch", "stars" }, "travel_explore", "Exploration"));
        recipes.Add(new RecipeData("combo_moon", new List<string> { "stars", "dark_mode" }, "dark_mode_outlined", "Lune"));
        recipes.Add(new RecipeData("combo_nightsky", new List<string> { "stars", "cloud" }, "bedtime", "Nuit Étoilée"));
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
        recipes.Add(new RecipeData("combo_storm", new List<string> { "cloud", "bolt", "water_drop" }, "storm", "Orage"));
        recipes.Add(new RecipeData("combo_rainbow", new List<string> { "water_drop", "wb_sunny" }, "rainbow", "Arc-en-ciel"));
        recipes.Add(new RecipeData("combo_fog", new List<string> { "cloud", "water_drop" }, "foggy", "Brouillard"));
        recipes.Add(new RecipeData("combo_snow", new List<string> { "ac_unit", "cloud" }, "snowy", "Neige"));
        recipes.Add(new RecipeData("combo_rain", new List<string> { "cloud", "water_drop" }, "rainy", "Pluie"));
        recipes.Add(new RecipeData("combo_thunderstorm", new List<string> { "storm", "bolt" }, "thunderstorm", "Tempête"));
        recipes.Add(new RecipeData("combo_cold", new List<string> { "ac_unit", "ac_unit" }, "severe_cold", "Froid Extrême"));
        recipes.Add(new RecipeData("combo_flood", new List<string> { "water_drop", "water_drop", "storm" }, "flood", "Inondation"));
        recipes.Add(new RecipeData("combo_earthquake", new List<string> { "landscape", "landscape", "bolt" }, "earthquake", "Tremblement"));
        recipes.Add(new RecipeData("combo_diamond", new List<string> { "landscape", "landscape", "local_fire_department" }, "diamond", "Diamant"));
        recipes.Add(new RecipeData("combo_recycling", new List<string> { "eco", "hardware" }, "recycling", "Recyclage"));
        recipes.Add(new RecipeData("combo_compost", new List<string> { "eco", "grass", "bug_report" }, "compost", "Compost"));
        recipes.Add(new RecipeData("combo_oil", new List<string> { "landscape", "landscape", "precision_manufacturing" }, "oil_barrel", "Pétrole"));
        recipes.Add(new RecipeData("combo_iron", new List<string> { "landscape", "local_fire_department", "precision_manufacturing" }, "iron", "Fer"));
        recipes.Add(new RecipeData("combo_coal", new List<string> { "forest", "local_fire_department", "terrain" }, "coal", "Charbon"));
        recipes.Add(new RecipeData("combo_sand", new List<string> { "landscape", "water_drop" }, "sand", "Sable"));
        recipes.Add(new RecipeData("combo_clay", new List<string> { "sand", "water_drop" }, "clay", "Argile"));
        recipes.Add(new RecipeData("combo_rock", new List<string> { "landscape", "landscape" }, "rock", "Roche"));
        recipes.Add(new RecipeData("combo_soil", new List<string> { "compost", "landscape" }, "soil", "Terre"));
        recipes.Add(new RecipeData("combo_seed", new List<string> { "spa", "grass" }, "seed", "Graine"));
        recipes.Add(new RecipeData("combo_sprout", new List<string> { "seed", "water_drop" }, "sprout", "Pousse"));
        recipes.Add(new RecipeData("combo_branch", new List<string> { "forest", "hardware" }, "branch", "Branche"));
        recipes.Add(new RecipeData("combo_trunk", new List<string> { "forest", "forest" }, "trunk", "Tronc"));
        recipes.Add(new RecipeData("combo_moss", new List<string> { "grass", "water_drop", "eco" }, "moss", "Mousse"));
        recipes.Add(new RecipeData("combo_hive", new List<string> { "bug_report", "forest" }, "hive", "Ruche"));
        recipes.Add(new RecipeData("combo_waves", new List<string> { "water_drop", "air" }, "waves", "Vagues"));
        recipes.Add(new RecipeData("combo_yard", new List<string> { "grass", "home" }, "yard", "Jardin"));
        recipes.Add(new RecipeData("combo_fence", new List<string> { "forest", "hardware" }, "fence", "Clôture"));
        recipes.Add(new RecipeData("combo_deck", new List<string> { "forest", "construction" }, "deck", "Terrasse"));
        recipes.Add(new RecipeData("combo_rabbit", new List<string> { "pets", "grass" }, "cruelty_free", "Lapin"));
        recipes.Add(new RecipeData("combo_fish", new List<string> { "pets", "water_drop" }, "phishing", "Poisson"));
        recipes.Add(new RecipeData("combo_butterfly", new List<string> { "bug_report", "spa" }, "flutter", "Papillon"));
        recipes.Add(new RecipeData("combo_bee", new List<string> { "bug_report", "spa", "spa" }, "emoji_nature", "Abeille"));
        recipes.Add(new RecipeData("combo_duck", new List<string> { "pets", "water_drop", "egg" }, "duck", "Canard"));
        recipes.Add(new RecipeData("combo_goat", new List<string> { "pets", "grass", "terrain" }, "goat", "Chèvre"));
        recipes.Add(new RecipeData("combo_turkey", new List<string> { "pets", "grain", "grain" }, "turkey", "Dinde"));
        recipes.Add(new RecipeData("combo_owl", new List<string> { "pets", "dark_mode" }, "owl", "Hibou"));
        recipes.Add(new RecipeData("combo_crow", new List<string> { "pets", "air" }, "crow", "Corneille"));
        recipes.Add(new RecipeData("combo_psychology", new List<string> { "pets", "spa", "eco" }, "psychology", "Esprit"));
        recipes.Add(new RecipeData("combo_meditation", new List<string> { "psychology", "park" }, "self_improvement", "Méditation"));
        recipes.Add(new RecipeData("combo_heart", new List<string> { "spa", "spa", "psychology" }, "favorite", "Coeur"));
        recipes.Add(new RecipeData("combo_idea", new List<string> { "psychology", "bolt" }, "lightbulb", "Idée"));
        recipes.Add(new RecipeData("combo_mood", new List<string> { "psychology", "wb_sunny" }, "mood", "Humeur"));
        recipes.Add(new RecipeData("combo_satisfied", new List<string> { "mood", "favorite" }, "sentiment_satisfied", "Satisfait"));
        recipes.Add(new RecipeData("combo_psychiatry", new List<string> { "psychology", "medical_services" }, "psychiatry", "Psychiatrie"));
        recipes.Add(new RecipeData("combo_trophy", new List<string> { "diamond", "workspace_premium" }, "emoji_events", "Trophée"));
        recipes.Add(new RecipeData("combo_medal", new List<string> { "emoji_events", "military_tech" }, "military_tech", "Médaille"));
        recipes.Add(new RecipeData("combo_bitcoin", new List<string> { "developer_board", "memory", "engineering" }, "currency_bitcoin", "Bitcoin"));
        recipes.Add(new RecipeData("combo_verified", new List<string> { "emoji_events", "check" }, "verified", "Vérifié"));
        recipes.Add(new RecipeData("combo_celebration", new List<string> { "emoji_events", "stars" }, "celebration", "Célébration"));
        recipes.Add(new RecipeData("combo_loyalty", new List<string> { "favorite", "emoji_events" }, "loyalty", "Fidélité"));
        recipes.Add(new RecipeData("combo_grade", new List<string> { "school", "emoji_events" }, "grade", "Note"));
        recipes.Add(new RecipeData("combo_chair", new List<string> { "forest", "hardware" }, "chair", "Chaise"));
        recipes.Add(new RecipeData("combo_table", new List<string> { "forest", "forest", "hardware" }, "table_restaurant", "Table"));
        recipes.Add(new RecipeData("combo_bed", new List<string> { "forest", "spa", "construction" }, "bed", "Lit"));
        recipes.Add(new RecipeData("combo_sofa", new List<string> { "bed", "chair" }, "weekend", "Canapé"));
        recipes.Add(new RecipeData("combo_desk", new List<string> { "table_restaurant", "work" }, "desk", "Bureau"));
        recipes.Add(new RecipeData("combo_door", new List<string> { "forest", "construction" }, "door_front", "Porte"));
        recipes.Add(new RecipeData("combo_window", new List<string> { "construction", "air" }, "window", "Fenêtre"));
        recipes.Add(new RecipeData("combo_stairs", new List<string> { "construction", "construction" }, "stairs", "Escaliers"));
        recipes.Add(new RecipeData("combo_soccer", new List<string> { "pets", "circle" }, "sports_soccer", "Football"));
        recipes.Add(new RecipeData("combo_basketball", new List<string> { "pets", "circle" }, "sports_basketball", "Basketball"));
        recipes.Add(new RecipeData("combo_tennis", new List<string> { "hardware", "circle" }, "sports_tennis", "Tennis"));
        recipes.Add(new RecipeData("combo_baseball", new List<string> { "hardware", "circle" }, "sports_baseball", "Baseball"));
        recipes.Add(new RecipeData("combo_golf", new List<string> { "hardware", "grass" }, "sports_golf", "Golf"));
        recipes.Add(new RecipeData("combo_pool", new List<string> { "water_drop", "construction" }, "pool", "Piscine"));
        recipes.Add(new RecipeData("combo_gym", new List<string> { "hardware", "construction" }, "fitness_center", "Gym"));
        recipes.Add(new RecipeData("combo_surfing", new List<string> { "water_drop", "forest" }, "surfing", "Surf"));
        recipes.Add(new RecipeData("combo_kayak", new List<string> { "water_drop", "forest", "hardware" }, "kayaking", "Kayak"));
        recipes.Add(new RecipeData("combo_snowboard", new List<string> { "ac_unit", "forest" }, "snowboarding", "Snowboard"));
        recipes.Add(new RecipeData("combo_piano", new List<string> { "forest", "hardware", "music_note" }, "piano", "Piano"));
        recipes.Add(new RecipeData("combo_guitar", new List<string> { "forest", "hardware" }, "guitar", "Guitare"));
        recipes.Add(new RecipeData("combo_radio", new List<string> { "developer_board", "music_note" }, "radio", "Radio"));
        recipes.Add(new RecipeData("combo_tv", new List<string> { "developer_board", "movie" }, "tv", "Télévision"));
        recipes.Add(new RecipeData("combo_theater", new List<string> { "home", "movie" }, "theaters", "Théâtre"));
        recipes.Add(new RecipeData("combo_album", new List<string> { "music_note", "music_note" }, "album", "Album"));
        recipes.Add(new RecipeData("combo_kitchen", new List<string> { "home", "local_fire_department" }, "kitchen", "Cuisine"));
        recipes.Add(new RecipeData("combo_microwave", new List<string> { "developer_board", "local_fire_department" }, "microwave", "Micro-ondes"));
        recipes.Add(new RecipeData("combo_blender", new List<string> { "precision_manufacturing", "local_fire_department" }, "blender", "Mixeur"));
        recipes.Add(new RecipeData("combo_coffee_maker", new List<string> { "precision_manufacturing", "water_drop" }, "coffee_maker", "Cafetière"));
        recipes.Add(new RecipeData("combo_grill", new List<string> { "hardware", "local_fire_department" }, "grill", "Barbecue"));
        recipes.Add(new RecipeData("combo_doctor", new List<string> { "science", "home" }, "medical_services", "Médecin"));
        recipes.Add(new RecipeData("combo_police", new List<string> { "home", "work" }, "local_police", "Policier"));
        recipes.Add(new RecipeData("combo_firefighter", new List<string> { "local_fire_department", "work" }, "firefighter", "Pompier"));
        recipes.Add(new RecipeData("combo_florist", new List<string> { "spa", "store" }, "local_florist", "Fleuriste"));
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
    
    /// <summary>
    /// Gets the total number of recipes.
    /// </summary>
    public int TotalRecipeCount => recipes.Count;
}
