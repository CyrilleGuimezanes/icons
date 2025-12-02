using UnityEngine;

/// <summary>
/// Main game initialization manager.
/// Ensures all game systems are initialized in the correct order.
/// 
/// USAGE: Attachez uniquement ce script à un GameObject vide dans votre scène.
/// Il créera automatiquement tous les managers nécessaires au démarrage du jeu.
/// 
/// Managers créés automatiquement (8 au total):
/// 1. GameSlotsManager - Gestion des 3 slots de sauvegarde
/// 2. IconDatabase - Base de données des 439 icônes
/// 3. UnlockedIconsManager - Suivi des icônes débloquées
/// 4. PlayerInventory - Inventaire du joueur
/// 5. CurrencyManager - Gestion des pièces/monnaie
/// 6. ProductionManager - Gestion des productions et recettes
/// 7. ShopManager - Gestion de la boutique
/// 8. HiddenMiniGameManager - Mini-jeux cachés (batterie, volume, etc.)
/// </summary>
public class GameInitializer : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Active la création automatique de tous les managers au démarrage")]
    [SerializeField] private bool createManagers = true;

    [Header("Managers optionnels")]
    [Tooltip("Créer le AdRewardManager pour les publicités récompensées")]
    [SerializeField] private bool createAdRewardManager = false;

    private void Awake()
    {
        if (createManagers)
        {
            InitializeManagers();
        }
    }

    /// <summary>
    /// Initialise tous les managers du jeu dans l'ordre correct.
    /// L'ordre est important car certains managers dépendent d'autres.
    /// </summary>
    private void InitializeManagers()
    {
        // 1. GameSlotsManager - DOIT être créé en premier (les autres managers en dépendent pour les sauvegardes)
        CreateManager<GameSlotsManager>("GameSlotsManager");

        // 2. IconDatabase - Base de données des icônes (nécessaire pour ShopManager et autres)
        CreateManager<IconDatabase>("IconDatabase");

        // 3. UnlockedIconsManager - Suivi des icônes débloquées
        CreateManager<UnlockedIconsManager>("UnlockedIconsManager");

        // 4. PlayerInventory - Inventaire du joueur
        CreateManager<PlayerInventory>("PlayerInventory");

        // 5. CurrencyManager - Gestion de la monnaie
        CreateManager<CurrencyManager>("CurrencyManager");

        // 6. ProductionManager - Gestion des productions
        CreateManager<ProductionManager>("ProductionManager");

        // 7. ShopManager - Gestion de la boutique
        CreateManager<ShopManager>("ShopManager");

        // 8. HiddenMiniGameManager - Mini-jeux cachés
        CreateManager<HiddenMiniGameManager>("HiddenMiniGameManager");

        // Managers optionnels
        if (createAdRewardManager)
        {
            CreateManager<AdRewardManager>("AdRewardManager");
        }

        Debug.Log("[GameInitializer] Tous les managers ont été initialisés avec succès!");
    }

    /// <summary>
    /// Crée un manager singleton s'il n'existe pas déjà.
    /// </summary>
    /// <typeparam name="T">Type du manager (MonoBehaviour)</typeparam>
    /// <param name="managerName">Nom du GameObject à créer</param>
    private void CreateManager<T>(string managerName) where T : MonoBehaviour
    {
        // Vérifie si le manager existe déjà dans la scène
        if (FindAnyObjectByType<T>() == null)
        {
            GameObject managerObj = new GameObject(managerName);
            managerObj.AddComponent<T>();
            Debug.Log($"[GameInitializer] {managerName} créé");
        }
        else
        {
            Debug.Log($"[GameInitializer] {managerName} existe déjà");
        }
    }
}
