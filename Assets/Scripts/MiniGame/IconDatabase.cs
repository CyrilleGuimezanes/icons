using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Database of available icons from Google Material Icons.
/// Provides icons organized by rarity for rewards and gameplay.
/// </summary>
public class IconDatabase : MonoBehaviour
{
    /// <summary>
    /// Singleton instance for global access.
    /// </summary>
    public static IconDatabase Instance { get; private set; }

    /// <summary>
    /// List of all available icons organized by rarity.
    /// </summary>
    [SerializeField]
    private List<IconEntry> allIcons = new List<IconEntry>();

    private Dictionary<IconRarity, List<IconEntry>> iconsByRarity;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDatabase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Initializes the icon database with Google Material Icons.
    /// </summary>
    private void InitializeDatabase()
    {
        allIcons.Clear();
        iconsByRarity = new Dictionary<IconRarity, List<IconEntry>>();

        // Initialize rarity lists
        foreach (IconRarity rarity in Enum.GetValues(typeof(IconRarity)))
        {
            iconsByRarity[rarity] = new List<IconEntry>();
        }

        // Common icons (resources, basic items) - Green
        AddIcon("grain", "Blé", IconRarity.Common);
        AddIcon("water_drop", "Eau", IconRarity.Common);
        AddIcon("landscape", "Pierre", IconRarity.Common);
        AddIcon("forest", "Bois", IconRarity.Common);
        AddIcon("grass", "Herbe", IconRarity.Common);
        AddIcon("park", "Parc", IconRarity.Common);
        AddIcon("eco", "Feuille", IconRarity.Common);
        AddIcon("nature", "Nature", IconRarity.Common);
        AddIcon("wb_sunny", "Soleil", IconRarity.Common);
        AddIcon("cloud", "Nuage", IconRarity.Common);
        AddIcon("air", "Air", IconRarity.Common);
        AddIcon("terrain", "Terrain", IconRarity.Common);
        AddIcon("pets", "Animal", IconRarity.Common);
        AddIcon("egg", "Oeuf", IconRarity.Common);
        AddIcon("nutrition", "Nutrition", IconRarity.Common);
        AddIcon("local_fire_department", "Feu", IconRarity.Common);
        AddIcon("ac_unit", "Glace", IconRarity.Common);
        AddIcon("spa", "Fleur", IconRarity.Common);
        AddIcon("bug_report", "Insecte", IconRarity.Common);
        AddIcon("catching_pokemon", "Filet", IconRarity.Common);
        AddIcon("recycling", "Recyclage", IconRarity.Common);
        AddIcon("compost", "Compost", IconRarity.Common);
        AddIcon("oil_barrel", "Pétrole", IconRarity.Common);
        AddIcon("iron", "Fer", IconRarity.Common);
        AddIcon("coal", "Charbon", IconRarity.Common);

        // Uncommon icons (tools, processed items) - Blue
        AddIcon("hardware", "Outil", IconRarity.Uncommon);
        AddIcon("handyman", "Bricoleur", IconRarity.Uncommon);
        AddIcon("construction", "Construction", IconRarity.Uncommon);
        AddIcon("carpenter", "Menuisier", IconRarity.Uncommon);
        AddIcon("agriculture", "Agriculture", IconRarity.Uncommon);
        AddIcon("bakery_dining", "Pain", IconRarity.Uncommon);
        AddIcon("restaurant", "Restaurant", IconRarity.Uncommon);
        AddIcon("local_dining", "Repas", IconRarity.Uncommon);
        AddIcon("lunch_dining", "Déjeuner", IconRarity.Uncommon);
        AddIcon("dinner_dining", "Dîner", IconRarity.Uncommon);
        AddIcon("cookie", "Cookie", IconRarity.Uncommon);
        AddIcon("cake", "Gâteau", IconRarity.Uncommon);
        AddIcon("icecream", "Glace", IconRarity.Uncommon);
        AddIcon("emoji_food_beverage", "Boisson", IconRarity.Uncommon);
        AddIcon("local_cafe", "Café", IconRarity.Uncommon);
        AddIcon("wine_bar", "Vin", IconRarity.Uncommon);
        AddIcon("liquor", "Liqueur", IconRarity.Uncommon);
        AddIcon("home", "Maison", IconRarity.Uncommon);
        AddIcon("cottage", "Cottage", IconRarity.Uncommon);
        AddIcon("cabin", "Cabane", IconRarity.Uncommon);
        AddIcon("plumbing", "Plomberie", IconRarity.Uncommon);
        AddIcon("electrical_services", "Électricité", IconRarity.Uncommon);
        AddIcon("set_meal", "Plateau Repas", IconRarity.Uncommon);
        AddIcon("ramen_dining", "Ramen", IconRarity.Uncommon);
        AddIcon("kebab_dining", "Kebab", IconRarity.Uncommon);

        // Rare icons (advanced machines, complex items) - Purple
        AddIcon("precision_manufacturing", "Machine", IconRarity.Rare);
        AddIcon("factory", "Usine", IconRarity.Rare);
        AddIcon("science", "Science", IconRarity.Rare);
        AddIcon("biotech", "Biotech", IconRarity.Rare);
        AddIcon("engineering", "Ingénierie", IconRarity.Rare);
        AddIcon("memory", "Mémoire", IconRarity.Rare);
        AddIcon("developer_board", "Circuit", IconRarity.Rare);
        AddIcon("smart_toy", "Robot", IconRarity.Rare);
        AddIcon("adb", "Android", IconRarity.Rare);
        AddIcon("castle", "Château", IconRarity.Rare);
        AddIcon("church", "Église", IconRarity.Rare);
        AddIcon("temple_buddhist", "Temple", IconRarity.Rare);
        AddIcon("stadium", "Stade", IconRarity.Rare);
        AddIcon("apartment", "Appartement", IconRarity.Rare);
        AddIcon("domain", "Domaine", IconRarity.Rare);
        AddIcon("diamond", "Diamant", IconRarity.Rare);
        AddIcon("currency_bitcoin", "Bitcoin", IconRarity.Rare);
        AddIcon("military_tech", "Médaille", IconRarity.Rare);
        AddIcon("workspace_premium", "Premium", IconRarity.Rare);
        AddIcon("emoji_events", "Trophée", IconRarity.Rare);
        AddIcon("electric_car", "Voiture Électrique", IconRarity.Rare);
        AddIcon("flight", "Avion", IconRarity.Rare);
        AddIcon("sailing", "Bateau", IconRarity.Rare);
        AddIcon("train", "Train", IconRarity.Rare);
        AddIcon("helicopter", "Hélicoptère", IconRarity.Rare);

        // Legendary icons (ultimate items) - Orange
        AddIcon("rocket_launch", "Fusée", IconRarity.Legendary);
        AddIcon("satellite_alt", "Satellite", IconRarity.Legendary);
        AddIcon("public", "Monde", IconRarity.Legendary);
        AddIcon("auto_awesome", "Magie", IconRarity.Legendary);
        AddIcon("stars", "Étoiles", IconRarity.Legendary);
        AddIcon("bolt", "Éclair", IconRarity.Legendary);
        AddIcon("flash_on", "Flash", IconRarity.Legendary);
        AddIcon("whatshot", "Flamme", IconRarity.Legendary);
        AddIcon("local_fire_station", "Feu Ardent", IconRarity.Legendary);
        AddIcon("volcano", "Volcan", IconRarity.Legendary);
        AddIcon("tsunami", "Tsunami", IconRarity.Legendary);
        AddIcon("tornado", "Tornade", IconRarity.Legendary);
        AddIcon("cyclone", "Cyclone", IconRarity.Legendary);
        AddIcon("flare", "Flare", IconRarity.Legendary);
        AddIcon("brightness_high", "Lumière", IconRarity.Legendary);
        AddIcon("all_inclusive", "Infini", IconRarity.Legendary);
        AddIcon("hub", "Hub", IconRarity.Legendary);
        AddIcon("psychology", "Esprit", IconRarity.Legendary);
        AddIcon("self_improvement", "Méditation", IconRarity.Legendary);
        AddIcon("favorite", "Coeur", IconRarity.Legendary);

        // Hidden mini-game reward icons (Legendary - only obtainable through hidden challenges)
        AddIcon("battery_0_bar", "Batterie Vide", IconRarity.Legendary);
        AddIcon("battery_charging_full", "Batterie Pleine", IconRarity.Legendary);
        AddIcon("volume_off", "Volume Off", IconRarity.Legendary);
        AddIcon("volume_up", "Volume Max", IconRarity.Legendary);
        AddIcon("stay_current_portrait", "Mode Portrait", IconRarity.Legendary);
        AddIcon("stay_current_landscape", "Mode Paysage", IconRarity.Legendary);
        AddIcon("airplanemode_active", "Mode Avion", IconRarity.Legendary);
        AddIcon("signal_cellular_alt", "Signal Fort", IconRarity.Legendary);
    }

    private void AddIcon(string id, string displayName, IconRarity rarity)
    {
        var entry = new IconEntry(id, displayName, rarity);
        allIcons.Add(entry);
        iconsByRarity[rarity].Add(entry);
    }

    /// <summary>
    /// Gets a random icon based on rarity weights.
    /// Common: 60%, Uncommon: 25%, Rare: 12%, Legendary: 3%
    /// </summary>
    public IconEntry GetRandomIcon()
    {
        float roll = UnityEngine.Random.Range(0f, 100f);
        IconRarity rarity;

        if (roll < 60f)
            rarity = IconRarity.Common;
        else if (roll < 85f)
            rarity = IconRarity.Uncommon;
        else if (roll < 97f)
            rarity = IconRarity.Rare;
        else
            rarity = IconRarity.Legendary;

        return GetRandomIconByRarity(rarity);
    }

    /// <summary>
    /// Gets a random icon of the specified rarity.
    /// </summary>
    public IconEntry GetRandomIconByRarity(IconRarity rarity)
    {
        if (!iconsByRarity.ContainsKey(rarity) || iconsByRarity[rarity].Count == 0)
        {
            // Fallback to common if the rarity list is empty
            rarity = IconRarity.Common;
        }

        var list = iconsByRarity[rarity];
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    /// <summary>
    /// Gets multiple random icons for gameplay.
    /// </summary>
    public List<IconEntry> GetRandomIcons(int count, bool allowDuplicates = false)
    {
        List<IconEntry> result = new List<IconEntry>();
        List<IconEntry> available = new List<IconEntry>(allIcons);

        for (int i = 0; i < count && (allowDuplicates || available.Count > 0); i++)
        {
            int index = UnityEngine.Random.Range(0, available.Count);
            result.Add(available[index]);

            if (!allowDuplicates)
            {
                available.RemoveAt(index);
            }
        }

        return result;
    }

    /// <summary>
    /// Gets an icon by its ID.
    /// </summary>
    public IconEntry GetIconById(string iconId)
    {
        return allIcons.Find(i => i.id == iconId);
    }

    /// <summary>
    /// Gets all icons of a specific rarity.
    /// </summary>
    public List<IconEntry> GetIconsByRarity(IconRarity rarity)
    {
        if (iconsByRarity.ContainsKey(rarity))
        {
            return new List<IconEntry>(iconsByRarity[rarity]);
        }
        return new List<IconEntry>();
    }

    /// <summary>
    /// Gets the total count of icons in the database.
    /// </summary>
    public int TotalIconCount => allIcons.Count;

    /// <summary>
    /// Gets all icons in the database.
    /// </summary>
    public List<IconEntry> GetAllIcons()
    {
        return new List<IconEntry>(allIcons);
    }
}

/// <summary>
/// Represents a single icon entry in the database.
/// </summary>
[Serializable]
public class IconEntry
{
    public string id;
    public string displayName;
    public IconRarity rarity;

    public IconEntry(string id, string displayName, IconRarity rarity)
    {
        this.id = id;
        this.displayName = displayName;
        this.rarity = rarity;
    }

    /// <summary>
    /// Gets the color associated with this icon's rarity.
    /// </summary>
    public Color GetRarityColor()
    {
        return rarity switch
        {
            IconRarity.Common => new Color(0.2f, 0.8f, 0.2f),     // Green
            IconRarity.Uncommon => new Color(0.2f, 0.6f, 1f),    // Blue
            IconRarity.Rare => new Color(0.6f, 0.2f, 0.8f),      // Purple
            IconRarity.Legendary => new Color(1f, 0.6f, 0.2f),   // Orange
            _ => Color.white
        };
    }
}
