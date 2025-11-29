using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Database of available icons from Google Material Icons.
/// Contains 439 icons organized by rarity for rewards and gameplay.
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
    /// Total: 439 icons (138 Common, 119 Uncommon, 104 Rare, 78 Legendary)
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

        // Common icons (138 icons) - Green
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
        AddIcon("yard", "Jardin", IconRarity.Common);
        AddIcon("waves", "Vagues", IconRarity.Common);
        AddIcon("sunny", "Ensoleillé", IconRarity.Common);
        AddIcon("cloudy", "Nuageux", IconRarity.Common);
        AddIcon("rainy", "Pluvieux", IconRarity.Common);
        AddIcon("snowy", "Neigeux", IconRarity.Common);
        AddIcon("foggy", "Brumeux", IconRarity.Common);
        AddIcon("storm", "Orage", IconRarity.Common);
        AddIcon("rainbow", "Arc-en-ciel", IconRarity.Common);
        AddIcon("north", "Nord", IconRarity.Common);
        AddIcon("south", "Sud", IconRarity.Common);
        AddIcon("east", "Est", IconRarity.Common);
        AddIcon("west", "Ouest", IconRarity.Common);
        AddIcon("explore", "Explorer", IconRarity.Common);
        AddIcon("hiking", "Randonnée", IconRarity.Common);
        AddIcon("cruelty_free", "Lapin", IconRarity.Common);
        AddIcon("phishing", "Poisson", IconRarity.Common);
        AddIcon("flutter", "Papillon", IconRarity.Common);
        AddIcon("pest_control", "Rat", IconRarity.Common);
        AddIcon("emoji_nature", "Abeille", IconRarity.Common);
        AddIcon("hive", "Ruche", IconRarity.Common);
        AddIcon("raven", "Corbeau", IconRarity.Common);
        AddIcon("owl", "Hibou", IconRarity.Common);
        AddIcon("crow", "Corneille", IconRarity.Common);
        AddIcon("duck", "Canard", IconRarity.Common);
        AddIcon("goat", "Chèvre", IconRarity.Common);
        AddIcon("turkey", "Dinde", IconRarity.Common);
        AddIcon("texture", "Texture", IconRarity.Common);
        AddIcon("grain_outlined", "Grain", IconRarity.Common);
        AddIcon("sand", "Sable", IconRarity.Common);
        AddIcon("rock", "Roche", IconRarity.Common);
        AddIcon("clay", "Argile", IconRarity.Common);
        AddIcon("soil", "Terre", IconRarity.Common);
        AddIcon("leaf", "Feuille Simple", IconRarity.Common);
        AddIcon("branch", "Branche", IconRarity.Common);
        AddIcon("trunk", "Tronc", IconRarity.Common);
        AddIcon("seed", "Graine", IconRarity.Common);
        AddIcon("sprout", "Pousse", IconRarity.Common);
        AddIcon("moss", "Mousse", IconRarity.Common);
        AddIcon("build", "Construire", IconRarity.Common);
        AddIcon("handyman_outlined", "Bricoleur", IconRarity.Common);
        AddIcon("pan_tool", "Outil Main", IconRarity.Common);
        AddIcon("gesture", "Geste", IconRarity.Common);
        AddIcon("touch_app", "Toucher", IconRarity.Common);
        AddIcon("swipe", "Glisser", IconRarity.Common);
        AddIcon("schedule", "Horaire", IconRarity.Common);
        AddIcon("timer", "Minuteur", IconRarity.Common);
        AddIcon("hourglass_empty", "Sablier", IconRarity.Common);
        AddIcon("hourglass_full", "Sablier Plein", IconRarity.Common);
        AddIcon("update", "Mise à jour", IconRarity.Common);
        AddIcon("history", "Historique", IconRarity.Common);
        AddIcon("circle", "Cercle", IconRarity.Common);
        AddIcon("square", "Carré", IconRarity.Common);
        AddIcon("change_history", "Triangle", IconRarity.Common);
        AddIcon("pentagon", "Pentagone", IconRarity.Common);
        AddIcon("hexagon", "Hexagone", IconRarity.Common);
        AddIcon("star_outline", "Étoile Vide", IconRarity.Common);
        AddIcon("palette", "Palette", IconRarity.Common);
        AddIcon("brush", "Pinceau", IconRarity.Common);
        AddIcon("colorize", "Coloriser", IconRarity.Common);
        AddIcon("format_paint", "Peinture", IconRarity.Common);
        AddIcon("edit", "Éditer", IconRarity.Common);
        AddIcon("draw", "Dessiner", IconRarity.Common);
        AddIcon("add", "Ajouter", IconRarity.Common);
        AddIcon("remove", "Retirer", IconRarity.Common);
        AddIcon("done", "Fait", IconRarity.Common);
        AddIcon("close", "Fermer", IconRarity.Common);
        AddIcon("check", "Vérifier", IconRarity.Common);
        AddIcon("clear", "Effacer", IconRarity.Common);
        AddIcon("inventory_2", "Inventaire", IconRarity.Common);
        AddIcon("inbox", "Boîte", IconRarity.Common);
        AddIcon("archive", "Archive", IconRarity.Common);
        AddIcon("folder", "Dossier", IconRarity.Common);
        AddIcon("shopping_bag", "Sac", IconRarity.Common);
        AddIcon("backpack", "Sac à dos", IconRarity.Common);
        AddIcon("north_east", "Nord-Est", IconRarity.Common);
        AddIcon("north_west", "Nord-Ouest", IconRarity.Common);
        AddIcon("south_east", "Sud-Est", IconRarity.Common);
        AddIcon("south_west", "Sud-Ouest", IconRarity.Common);
        AddIcon("arrow_upward", "Haut", IconRarity.Common);
        AddIcon("arrow_downward", "Bas", IconRarity.Common);
        AddIcon("arrow_forward", "Avant", IconRarity.Common);
        AddIcon("arrow_back", "Arrière", IconRarity.Common);
        AddIcon("volume_down", "Volume Bas", IconRarity.Common);
        AddIcon("volume_mute", "Muet", IconRarity.Common);
        AddIcon("mic", "Micro", IconRarity.Common);
        AddIcon("speaker", "Haut-parleur", IconRarity.Common);
        AddIcon("light_mode", "Mode Clair", IconRarity.Common);
        AddIcon("dark_mode", "Mode Sombre", IconRarity.Common);
        AddIcon("brightness_low", "Luminosité Basse", IconRarity.Common);
        AddIcon("brightness_medium", "Luminosité Moyenne", IconRarity.Common);
        AddIcon("looks_one", "Un", IconRarity.Common);
        AddIcon("looks_two", "Deux", IconRarity.Common);
        AddIcon("looks_3", "Trois", IconRarity.Common);
        AddIcon("looks_4", "Quatre", IconRarity.Common);
        AddIcon("looks_5", "Cinq", IconRarity.Common);
        AddIcon("looks_6", "Six", IconRarity.Common);
        AddIcon("straighten", "Règle", IconRarity.Common);
        AddIcon("square_foot", "Mètre Carré", IconRarity.Common);
        AddIcon("height", "Hauteur", IconRarity.Common);
        AddIcon("width", "Largeur", IconRarity.Common);
        AddIcon("battery_std", "Batterie", IconRarity.Common);
        AddIcon("power", "Énergie", IconRarity.Common);
        AddIcon("flash_auto", "Flash Auto", IconRarity.Common);
        AddIcon("highlight", "Surbrillance", IconRarity.Common);
        AddIcon("mood", "Humeur", IconRarity.Common);
        AddIcon("sentiment_satisfied", "Satisfait", IconRarity.Common);
        AddIcon("sentiment_dissatisfied", "Insatisfait", IconRarity.Common);
        AddIcon("sentiment_neutral", "Neutre", IconRarity.Common);
        AddIcon("fiber_manual_record", "Point", IconRarity.Common);
        AddIcon("lens", "Lentille", IconRarity.Common);
        AddIcon("crop_square", "Carré Crop", IconRarity.Common);
        AddIcon("vignette", "Vignette", IconRarity.Common);
        AddIcon("paid", "Pièce", IconRarity.Common);  // Coin icon for in-game currency

        // Uncommon icons (119 icons) - Blue
        AddIcon("hardware", "Outil", IconRarity.Uncommon);
        AddIcon("handyman", "Bricoleur", IconRarity.Uncommon);
        AddIcon("construction", "Construction", IconRarity.Uncommon);
        AddIcon("carpenter", "Menuisier", IconRarity.Uncommon);
        AddIcon("agriculture", "Agriculture", IconRarity.Uncommon);
        AddIcon("plumbing", "Plomberie", IconRarity.Uncommon);
        AddIcon("electrical_services", "Électricité", IconRarity.Uncommon);
        AddIcon("architecture", "Architecture", IconRarity.Uncommon);
        AddIcon("design_services", "Design", IconRarity.Uncommon);
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
        AddIcon("set_meal", "Plateau Repas", IconRarity.Uncommon);
        AddIcon("ramen_dining", "Ramen", IconRarity.Uncommon);
        AddIcon("kebab_dining", "Kebab", IconRarity.Uncommon);
        AddIcon("local_pizza", "Pizza", IconRarity.Uncommon);
        AddIcon("fastfood", "Fast-food", IconRarity.Uncommon);
        AddIcon("rice_bowl", "Bol de Riz", IconRarity.Uncommon);
        AddIcon("tapas", "Tapas", IconRarity.Uncommon);
        AddIcon("bento", "Bento", IconRarity.Uncommon);
        AddIcon("soup_kitchen", "Soupe", IconRarity.Uncommon);
        AddIcon("breakfast_dining", "Petit-déjeuner", IconRarity.Uncommon);
        AddIcon("brunch_dining", "Brunch", IconRarity.Uncommon);
        AddIcon("home", "Maison", IconRarity.Uncommon);
        AddIcon("cottage", "Cottage", IconRarity.Uncommon);
        AddIcon("cabin", "Cabane", IconRarity.Uncommon);
        AddIcon("apartment_outlined", "Appartement Simple", IconRarity.Uncommon);
        AddIcon("house", "Maison Simple", IconRarity.Uncommon);
        AddIcon("bungalow", "Bungalow", IconRarity.Uncommon);
        AddIcon("chalet", "Chalet", IconRarity.Uncommon);
        AddIcon("villa", "Villa", IconRarity.Uncommon);
        AddIcon("warehouse", "Entrepôt", IconRarity.Uncommon);
        AddIcon("store", "Magasin", IconRarity.Uncommon);
        AddIcon("storefront", "Vitrine", IconRarity.Uncommon);
        AddIcon("shop", "Boutique", IconRarity.Uncommon);
        AddIcon("garage_home", "Garage", IconRarity.Uncommon);
        AddIcon("shed", "Abri", IconRarity.Uncommon);
        AddIcon("barn", "Grange", IconRarity.Uncommon);
        AddIcon("silo", "Silo", IconRarity.Uncommon);
        AddIcon("engineering_outlined", "Ingénieur", IconRarity.Uncommon);
        AddIcon("medical_services", "Médecin", IconRarity.Uncommon);
        AddIcon("local_police", "Policier", IconRarity.Uncommon);
        AddIcon("firefighter", "Pompier", IconRarity.Uncommon);
        AddIcon("school", "École", IconRarity.Uncommon);
        AddIcon("work", "Travail", IconRarity.Uncommon);
        AddIcon("business", "Affaires", IconRarity.Uncommon);
        AddIcon("checkroom", "Vestiaire", IconRarity.Uncommon);
        AddIcon("dry_cleaning", "Nettoyage", IconRarity.Uncommon);
        AddIcon("iron_outlined", "Fer à repasser", IconRarity.Uncommon);
        AddIcon("watch", "Montre", IconRarity.Uncommon);
        AddIcon("diamond_outlined", "Diamant Brut", IconRarity.Uncommon);
        AddIcon("directions_car", "Voiture", IconRarity.Uncommon);
        AddIcon("directions_bus", "Bus", IconRarity.Uncommon);
        AddIcon("directions_bike", "Vélo", IconRarity.Uncommon);
        AddIcon("directions_walk", "Marche", IconRarity.Uncommon);
        AddIcon("directions_boat", "Bateau Simple", IconRarity.Uncommon);
        AddIcon("motorcycle", "Moto", IconRarity.Uncommon);
        AddIcon("pedal_bike", "Vélo Pédale", IconRarity.Uncommon);
        AddIcon("electric_scooter", "Trottinette", IconRarity.Uncommon);
        AddIcon("skateboarding", "Skateboard", IconRarity.Uncommon);
        AddIcon("snowboarding", "Snowboard", IconRarity.Uncommon);
        AddIcon("surfing", "Surf", IconRarity.Uncommon);
        AddIcon("kayaking", "Kayak", IconRarity.Uncommon);
        AddIcon("chair", "Chaise", IconRarity.Uncommon);
        AddIcon("table_restaurant", "Table", IconRarity.Uncommon);
        AddIcon("bed", "Lit", IconRarity.Uncommon);
        AddIcon("weekend", "Canapé", IconRarity.Uncommon);
        AddIcon("desk", "Bureau", IconRarity.Uncommon);
        AddIcon("door_front", "Porte", IconRarity.Uncommon);
        AddIcon("window", "Fenêtre", IconRarity.Uncommon);
        AddIcon("stairs", "Escaliers", IconRarity.Uncommon);
        AddIcon("king_bed", "Grand Lit", IconRarity.Uncommon);
        AddIcon("single_bed", "Lit Simple", IconRarity.Uncommon);
        AddIcon("kitchen", "Cuisine", IconRarity.Uncommon);
        AddIcon("countertops", "Comptoir", IconRarity.Uncommon);
        AddIcon("microwave", "Micro-ondes", IconRarity.Uncommon);
        AddIcon("blender", "Mixeur", IconRarity.Uncommon);
        AddIcon("coffee_maker", "Cafetière", IconRarity.Uncommon);
        AddIcon("soup", "Soupe", IconRarity.Uncommon);
        AddIcon("sports_soccer", "Football", IconRarity.Uncommon);
        AddIcon("sports_basketball", "Basketball", IconRarity.Uncommon);
        AddIcon("sports_tennis", "Tennis", IconRarity.Uncommon);
        AddIcon("sports_baseball", "Baseball", IconRarity.Uncommon);
        AddIcon("sports_golf", "Golf", IconRarity.Uncommon);
        AddIcon("sports_hockey", "Hockey", IconRarity.Uncommon);
        AddIcon("sports_volleyball", "Volleyball", IconRarity.Uncommon);
        AddIcon("sports_handball", "Handball", IconRarity.Uncommon);
        AddIcon("sports_rugby", "Rugby", IconRarity.Uncommon);
        AddIcon("sports_cricket", "Cricket", IconRarity.Uncommon);
        AddIcon("sports_kabaddi", "Kabaddi", IconRarity.Uncommon);
        AddIcon("sports_martial_arts", "Arts Martiaux", IconRarity.Uncommon);
        AddIcon("pool", "Piscine", IconRarity.Uncommon);
        AddIcon("fitness_center", "Gym", IconRarity.Uncommon);
        AddIcon("self_improvement_outlined", "Yoga", IconRarity.Uncommon);
        AddIcon("music_note", "Note", IconRarity.Uncommon);
        AddIcon("audiotrack", "Piste Audio", IconRarity.Uncommon);
        AddIcon("piano", "Piano", IconRarity.Uncommon);
        AddIcon("guitar", "Guitare", IconRarity.Uncommon);
        AddIcon("album", "Album", IconRarity.Uncommon);
        AddIcon("radio", "Radio", IconRarity.Uncommon);
        AddIcon("tv", "Télévision", IconRarity.Uncommon);
        AddIcon("videocam", "Caméra", IconRarity.Uncommon);
        AddIcon("movie", "Film", IconRarity.Uncommon);
        AddIcon("theaters", "Théâtre", IconRarity.Uncommon);
        AddIcon("local_florist", "Fleuriste", IconRarity.Uncommon);
        AddIcon("yard_outlined", "Cour", IconRarity.Uncommon);
        AddIcon("fence", "Clôture", IconRarity.Uncommon);
        AddIcon("deck", "Terrasse", IconRarity.Uncommon);
        AddIcon("grill", "Barbecue", IconRarity.Uncommon);
        AddIcon("outdoor_grill", "Grill Extérieur", IconRarity.Uncommon);

        // Rare icons (104 icons) - Purple
        AddIcon("precision_manufacturing", "Machine", IconRarity.Rare);
        AddIcon("factory", "Usine", IconRarity.Rare);
        AddIcon("science", "Science", IconRarity.Rare);
        AddIcon("biotech", "Biotech", IconRarity.Rare);
        AddIcon("engineering", "Ingénierie", IconRarity.Rare);
        AddIcon("memory", "Mémoire", IconRarity.Rare);
        AddIcon("developer_board", "Circuit", IconRarity.Rare);
        AddIcon("smart_toy", "Robot", IconRarity.Rare);
        AddIcon("adb", "Android", IconRarity.Rare);
        AddIcon("hub", "Hub", IconRarity.Rare);
        AddIcon("router", "Routeur", IconRarity.Rare);
        AddIcon("dns", "DNS", IconRarity.Rare);
        AddIcon("terminal", "Terminal", IconRarity.Rare);
        AddIcon("code", "Code", IconRarity.Rare);
        AddIcon("data_object", "Données", IconRarity.Rare);
        AddIcon("storage", "Stockage", IconRarity.Rare);
        AddIcon("cloud_upload", "Cloud Upload", IconRarity.Rare);
        AddIcon("cloud_download", "Cloud Download", IconRarity.Rare);
        AddIcon("api", "API", IconRarity.Rare);
        AddIcon("webhook", "Webhook", IconRarity.Rare);
        AddIcon("smart_display", "Écran Intelligent", IconRarity.Rare);
        AddIcon("smart_screen", "Smart Screen", IconRarity.Rare);
        AddIcon("cast", "Cast", IconRarity.Rare);
        AddIcon("sensors", "Capteurs", IconRarity.Rare);
        AddIcon("device_thermostat", "Thermostat", IconRarity.Rare);
        AddIcon("nest_cam_outdoor", "Caméra Extérieure", IconRarity.Rare);
        AddIcon("castle", "Château", IconRarity.Rare);
        AddIcon("church", "Église", IconRarity.Rare);
        AddIcon("temple_buddhist", "Temple", IconRarity.Rare);
        AddIcon("stadium", "Stade", IconRarity.Rare);
        AddIcon("apartment", "Appartement", IconRarity.Rare);
        AddIcon("domain", "Domaine", IconRarity.Rare);
        AddIcon("corporate_fare", "Siège Social", IconRarity.Rare);
        AddIcon("location_city", "Ville", IconRarity.Rare);
        AddIcon("local_hospital", "Hôpital", IconRarity.Rare);
        AddIcon("local_library", "Bibliothèque", IconRarity.Rare);
        AddIcon("local_mall", "Centre Commercial", IconRarity.Rare);
        AddIcon("local_movies", "Cinéma", IconRarity.Rare);
        AddIcon("local_airport", "Aéroport", IconRarity.Rare);
        AddIcon("local_atm", "Banque", IconRarity.Rare);
        AddIcon("museum", "Musée", IconRarity.Rare);
        AddIcon("account_balance", "Tribunal", IconRarity.Rare);
        AddIcon("synagogue", "Synagogue", IconRarity.Rare);
        AddIcon("mosque", "Mosquée", IconRarity.Rare);
        AddIcon("temple_hindu", "Temple Hindou", IconRarity.Rare);
        AddIcon("fort", "Fort", IconRarity.Rare);
        AddIcon("electric_car", "Voiture Électrique", IconRarity.Rare);
        AddIcon("flight", "Avion", IconRarity.Rare);
        AddIcon("sailing", "Bateau", IconRarity.Rare);
        AddIcon("train", "Train", IconRarity.Rare);
        AddIcon("helicopter", "Hélicoptère", IconRarity.Rare);
        AddIcon("rocket", "Fusée Simple", IconRarity.Rare);
        AddIcon("directions_railway", "Train Rapide", IconRarity.Rare);
        AddIcon("tram", "Tramway", IconRarity.Rare);
        AddIcon("subway", "Métro", IconRarity.Rare);
        AddIcon("cable_car", "Téléphérique", IconRarity.Rare);
        AddIcon("local_shipping", "Camion", IconRarity.Rare);
        AddIcon("fire_truck", "Camion Pompier", IconRarity.Rare);
        AddIcon("airport_shuttle", "Navette", IconRarity.Rare);
        AddIcon("rv_hookup", "Camping-car", IconRarity.Rare);
        AddIcon("two_wheeler", "Deux-roues", IconRarity.Rare);
        AddIcon("agriculture_machinery", "Tracteur", IconRarity.Rare);
        AddIcon("diamond", "Diamant", IconRarity.Rare);
        AddIcon("currency_bitcoin", "Bitcoin", IconRarity.Rare);
        AddIcon("paid", "Payé", IconRarity.Rare);
        AddIcon("savings", "Épargne", IconRarity.Rare);
        AddIcon("account_balance_wallet", "Portefeuille", IconRarity.Rare);
        AddIcon("credit_card", "Carte Crédit", IconRarity.Rare);
        AddIcon("currency_exchange", "Change", IconRarity.Rare);
        AddIcon("euro", "Euro", IconRarity.Rare);
        AddIcon("attach_money", "Dollar", IconRarity.Rare);
        AddIcon("currency_yen", "Yen", IconRarity.Rare);
        AddIcon("currency_pound", "Livre", IconRarity.Rare);
        AddIcon("currency_rupee", "Roupie", IconRarity.Rare);
        AddIcon("currency_franc", "Franc", IconRarity.Rare);
        AddIcon("currency_lira", "Lire", IconRarity.Rare);
        AddIcon("military_tech", "Médaille", IconRarity.Rare);
        AddIcon("workspace_premium", "Premium", IconRarity.Rare);
        AddIcon("emoji_events", "Trophée", IconRarity.Rare);
        AddIcon("grade", "Note", IconRarity.Rare);
        AddIcon("verified", "Vérifié", IconRarity.Rare);
        AddIcon("new_releases", "Nouveau", IconRarity.Rare);
        AddIcon("stars_outlined", "Étoiles Rating", IconRarity.Rare);
        AddIcon("thumb_up", "Pouce Haut", IconRarity.Rare);
        AddIcon("celebration", "Célébration", IconRarity.Rare);
        AddIcon("loyalty", "Fidélité", IconRarity.Rare);
        AddIcon("biotech_outlined", "Biologie", IconRarity.Rare);
        AddIcon("medication", "Médicament", IconRarity.Rare);
        AddIcon("vaccines", "Vaccin", IconRarity.Rare);
        AddIcon("science_outlined", "Laboratoire", IconRarity.Rare);
        AddIcon("psychology_outlined", "Psychologie", IconRarity.Rare);
        AddIcon("psychiatry", "Psychiatrie", IconRarity.Rare);
        AddIcon("health_and_safety", "Santé Sécurité", IconRarity.Rare);
        AddIcon("solar_power", "Solaire", IconRarity.Rare);
        AddIcon("wind_power", "Éolien", IconRarity.Rare);
        AddIcon("electric_bolt", "Électricité", IconRarity.Rare);
        AddIcon("gas_meter", "Gaz", IconRarity.Rare);
        AddIcon("water_heater", "Chauffe-eau", IconRarity.Rare);
        AddIcon("hvac", "Climatisation", IconRarity.Rare);
        AddIcon("museum_outlined", "Galerie", IconRarity.Rare);
        AddIcon("palette_outlined", "Art", IconRarity.Rare);
        AddIcon("photo_camera", "Appareil Photo", IconRarity.Rare);
        AddIcon("camera", "Caméra Pro", IconRarity.Rare);
        AddIcon("videocam_outlined", "Vidéo Pro", IconRarity.Rare);

        // Legendary icons (78 icons) - Orange
        AddIcon("rocket_launch", "Fusée", IconRarity.Legendary);
        AddIcon("satellite_alt", "Satellite", IconRarity.Legendary);
        AddIcon("public", "Monde", IconRarity.Legendary);
        AddIcon("stars", "Étoiles", IconRarity.Legendary);
        AddIcon("flare", "Flare", IconRarity.Legendary);
        AddIcon("brightness_high", "Lumière", IconRarity.Legendary);
        AddIcon("nightlight", "Veilleuse", IconRarity.Legendary);
        AddIcon("dark_mode_outlined", "Lune", IconRarity.Legendary);
        AddIcon("bedtime", "Nuit Étoilée", IconRarity.Legendary);
        AddIcon("astronomy", "Astronomie", IconRarity.Legendary);
        AddIcon("orbit", "Orbite", IconRarity.Legendary);
        AddIcon("travel_explore", "Exploration", IconRarity.Legendary);
        AddIcon("bolt", "Éclair", IconRarity.Legendary);
        AddIcon("flash_on", "Flash", IconRarity.Legendary);
        AddIcon("whatshot", "Flamme", IconRarity.Legendary);
        AddIcon("local_fire_station", "Feu Ardent", IconRarity.Legendary);
        AddIcon("thunderstorm", "Tempête", IconRarity.Legendary);
        AddIcon("severe_cold", "Froid Extrême", IconRarity.Legendary);
        AddIcon("thermostat_auto", "Température Auto", IconRarity.Legendary);
        AddIcon("device_hub", "Hub Énergie", IconRarity.Legendary);
        AddIcon("volcano", "Volcan", IconRarity.Legendary);
        AddIcon("tsunami", "Tsunami", IconRarity.Legendary);
        AddIcon("tornado", "Tornade", IconRarity.Legendary);
        AddIcon("cyclone", "Cyclone", IconRarity.Legendary);
        AddIcon("flood", "Inondation", IconRarity.Legendary);
        AddIcon("landslide", "Glissement", IconRarity.Legendary);
        AddIcon("earthquake", "Tremblement", IconRarity.Legendary);
        AddIcon("auto_awesome", "Magie", IconRarity.Legendary);
        AddIcon("all_inclusive", "Infini", IconRarity.Legendary);
        AddIcon("psychology", "Esprit", IconRarity.Legendary);
        AddIcon("self_improvement", "Méditation", IconRarity.Legendary);
        AddIcon("favorite", "Coeur", IconRarity.Legendary);
        AddIcon("auto_fix_high", "Auto-Fix", IconRarity.Legendary);
        AddIcon("auto_awesome_mosaic", "Mosaïque Magique", IconRarity.Legendary);
        AddIcon("auto_awesome_motion", "Mouvement Magique", IconRarity.Legendary);
        AddIcon("blur_on", "Flou Magique", IconRarity.Legendary);
        AddIcon("lens_blur", "Lentille Magique", IconRarity.Legendary);
        AddIcon("tips_and_updates", "Conseils Magiques", IconRarity.Legendary);
        AddIcon("lightbulb", "Idée", IconRarity.Legendary);
        AddIcon("emoji_objects", "Ampoule", IconRarity.Legendary);
        AddIcon("memory_alt", "Super Mémoire", IconRarity.Legendary);
        AddIcon("precision_manufacturing_outlined", "Super Machine", IconRarity.Legendary);
        AddIcon("hub_outlined", "Super Hub", IconRarity.Legendary);
        AddIcon("router_outlined", "Super Routeur", IconRarity.Legendary);
        AddIcon("cell_tower", "Tour Cellulaire", IconRarity.Legendary);
        AddIcon("broadcast_on_personal", "Broadcast", IconRarity.Legendary);
        AddIcon("settings_input_antenna", "Antenne", IconRarity.Legendary);
        AddIcon("satellite", "Satellite Simple", IconRarity.Legendary);
        AddIcon("castle_outlined", "Palais", IconRarity.Legendary);
        AddIcon("church_outlined", "Cathédrale", IconRarity.Legendary);
        AddIcon("fort_outlined", "Forteresse", IconRarity.Legendary);
        AddIcon("domain_outlined", "Empire", IconRarity.Legendary);
        AddIcon("location_city_outlined", "Métropole", IconRarity.Legendary);
        AddIcon("holiday_village", "Village Vacances", IconRarity.Legendary);
        AddIcon("pets_outlined", "Animal Légendaire", IconRarity.Legendary);
        AddIcon("cruelty_free_outlined", "Créature", IconRarity.Legendary);
        AddIcon("power_settings_new", "Puissance Max", IconRarity.Legendary);
        AddIcon("offline_bolt", "Éclair Offline", IconRarity.Legendary);
        AddIcon("electrical_services_outlined", "Super Électricité", IconRarity.Legendary);
        AddIcon("workspace_premium_outlined", "Couronne", IconRarity.Legendary);
        AddIcon("verified_user", "Utilisateur Vérifié", IconRarity.Legendary);
        AddIcon("shield", "Bouclier", IconRarity.Legendary);
        AddIcon("security", "Sécurité Max", IconRarity.Legendary);
        AddIcon("admin_panel_settings", "Admin", IconRarity.Legendary);
        AddIcon("supervisor_account", "Superviseur", IconRarity.Legendary);
        AddIcon("timeline", "Timeline", IconRarity.Legendary);
        AddIcon("timer_outlined", "Temps Infini", IconRarity.Legendary);
        AddIcon("restore", "Restaurer", IconRarity.Legendary);
        AddIcon("next_plan", "Futur", IconRarity.Legendary);
        AddIcon("event_repeat", "Répétition", IconRarity.Legendary);
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
