using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages all production recipes and active productions in the game.
/// Singleton that persists across scenes.
/// </summary>
public class ProductionManager : MonoBehaviour
{
    private const string SAVE_KEY_PRODUCTIONS = "ActiveProductions";
    private const string SAVE_KEY_DISCOVERED = "DiscoveredProductions";
    private const int MAX_PRODUCTION_SLOTS = 5;

    /// <summary>
    /// Singleton instance for global access.
    /// </summary>
    public static ProductionManager Instance { get; private set; }

    /// <summary>
    /// Event triggered when a production is started.
    /// </summary>
    public event Action<int, ActiveProduction> OnProductionStarted;

    /// <summary>
    /// Event triggered when a production is completed.
    /// </summary>
    public event Action<int, ActiveProduction> OnProductionCompleted;

    /// <summary>
    /// Event triggered when a production is cancelled.
    /// </summary>
    public event Action<int> OnProductionCancelled;

    /// <summary>
    /// Event triggered when active productions change.
    /// </summary>
    public event Action OnProductionsChanged;

    [SerializeField]
    private List<ProductionData> availableProductions = new List<ProductionData>();

    [SerializeField]
    private List<ActiveProduction> activeProductions = new List<ActiveProduction>();

    [SerializeField]
    private List<string> discoveredProductionIds = new List<string>();

    // Dictionary for O(1) active production lookup by slot index
    private Dictionary<int, ActiveProduction> activeProductionsBySlot = new Dictionary<int, ActiveProduction>();

    // Timer to avoid checking production completion every frame
    private float lastCompletionCheckTime;
    private const float COMPLETION_CHECK_INTERVAL = 0.5f; // Check twice per second

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDefaultProductions();
            Load();
            RebuildSlotDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        UpdateActiveProductions();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Save();
        }
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    /// <summary>
    /// Initializes default production recipes.
    /// </summary>
    private void InitializeDefaultProductions()
    {
        // Plant productions (Potager)
        availableProductions.Add(new ProductionData(
            "wheat_production",
            "Blé",
            new List<string> { "seed_wheat" },
            "wheat",
            15f,
            ProductionType.Plant
        ));

        availableProductions.Add(new ProductionData(
            "tomato_production",
            "Tomate",
            new List<string> { "seed_tomato" },
            "tomato",
            20f,
            ProductionType.Plant
        ));

        availableProductions.Add(new ProductionData(
            "carrot_production",
            "Carotte",
            new List<string> { "seed_carrot" },
            "carrot",
            18f,
            ProductionType.Plant
        ));

        // Manufactured goods (Industrie)
        availableProductions.Add(new ProductionData(
            "flour_production",
            "Farine",
            new List<string> { "wheat" },
            "flour",
            12f,
            ProductionType.ManufacturedGood
        ));

        availableProductions.Add(new ProductionData(
            "planks_production",
            "Planches",
            new List<string> { "wood" },
            "planks",
            15f,
            ProductionType.ManufacturedGood
        ));

        availableProductions.Add(new ProductionData(
            "metal_plates_production",
            "Plaques métalliques",
            new List<string> { "ore" },
            "metal_plates",
            25f,
            ProductionType.ManufacturedGood
        ));

        availableProductions.Add(new ProductionData(
            "bread_production",
            "Pain",
            new List<string> { "flour", "water" },
            "bread",
            20f,
            ProductionType.ManufacturedGood
        ));

        // Mark some as discovered by default for testing
        DiscoverProduction("wheat_production");
        DiscoverProduction("flour_production");
    }

    /// <summary>
    /// Updates all active productions, checking for completion.
    /// Uses a check interval to avoid unnecessary iterations every frame.
    /// </summary>
    private void UpdateActiveProductions()
    {
        // Only check periodically to reduce overhead
        if (Time.time - lastCompletionCheckTime < COMPLETION_CHECK_INTERVAL)
        {
            return;
        }
        lastCompletionCheckTime = Time.time;

        bool hasChanges = false;

        for (int i = activeProductions.Count - 1; i >= 0; i--)
        {
            var production = activeProductions[i];
            if (production != null && production.IsComplete())
            {
                CompleteProduction(production.slotIndex);
                hasChanges = true;
            }
        }

        if (hasChanges)
        {
            OnProductionsChanged?.Invoke();
        }
    }

    /// <summary>
    /// Gets all discovered production recipes.
    /// </summary>
    /// <returns>List of discovered production recipes</returns>
    public List<ProductionData> GetDiscoveredProductions()
    {
        return availableProductions.FindAll(p => discoveredProductionIds.Contains(p.id));
    }

    /// <summary>
    /// Gets all discovered productions of a specific type.
    /// </summary>
    /// <param name="type">The production type to filter by</param>
    /// <returns>List of discovered productions of the specified type</returns>
    public List<ProductionData> GetDiscoveredProductionsByType(ProductionType type)
    {
        return availableProductions.FindAll(p => 
            discoveredProductionIds.Contains(p.id) && p.productionType == type);
    }

    /// <summary>
    /// Gets a production recipe by its ID.
    /// </summary>
    /// <param name="productionId">The production ID</param>
    /// <returns>The production data, or null if not found</returns>
    public ProductionData GetProduction(string productionId)
    {
        return availableProductions.Find(p => p.id == productionId);
    }

    /// <summary>
    /// Discovers a production recipe, making it available for use.
    /// </summary>
    /// <param name="productionId">The production ID to discover</param>
    /// <returns>True if newly discovered, false if already discovered</returns>
    public bool DiscoverProduction(string productionId)
    {
        if (string.IsNullOrEmpty(productionId))
        {
            return false;
        }

        if (discoveredProductionIds.Contains(productionId))
        {
            return false;
        }

        var production = availableProductions.Find(p => p.id == productionId);
        if (production != null)
        {
            discoveredProductionIds.Add(productionId);
            production.isDiscovered = true;
            Save();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Starts a production in the specified slot.
    /// </summary>
    /// <param name="slotIndex">The slot index (0-4)</param>
    /// <param name="productionId">The production recipe ID</param>
    /// <param name="multiplier">The quantity multiplier</param>
    /// <returns>True if production started successfully</returns>
    public bool StartProduction(int slotIndex, string productionId, int multiplier)
    {
        if (slotIndex < 0 || slotIndex >= MAX_PRODUCTION_SLOTS)
        {
            return false;
        }

        // Check if slot is already in use
        if (GetActiveProduction(slotIndex) != null)
        {
            return false;
        }

        var production = GetProduction(productionId);
        if (production == null)
        {
            return false;
        }

        // Check if player has required ingredients
        if (PlayerInventory.Instance == null)
        {
            return false;
        }

        foreach (var ingredient in production.ingredients)
        {
            if (!PlayerInventory.Instance.HasIcon(ingredient, multiplier))
            {
                return false;
            }
        }

        // Consume ingredients
        foreach (var ingredient in production.ingredients)
        {
            PlayerInventory.Instance.RemoveIcon(ingredient, multiplier);
        }

        // Create active production
        float productionTime = production.GetProductionTime(multiplier);
        var activeProduction = new ActiveProduction(
            slotIndex,
            productionId,
            multiplier,
            Time.time,
            productionTime
        );

        activeProductions.Add(activeProduction);
        activeProductionsBySlot[slotIndex] = activeProduction;
        Save();

        OnProductionStarted?.Invoke(slotIndex, activeProduction);
        OnProductionsChanged?.Invoke();

        return true;
    }

    /// <summary>
    /// Cancels a production in the specified slot, returning ingredients.
    /// </summary>
    /// <param name="slotIndex">The slot index to cancel</param>
    /// <returns>True if production was cancelled</returns>
    public bool CancelProduction(int slotIndex)
    {
        var activeProduction = GetActiveProduction(slotIndex);
        if (activeProduction == null)
        {
            return false;
        }

        var production = GetProduction(activeProduction.productionId);
        if (production != null && PlayerInventory.Instance != null)
        {
            // Return ingredients to player
            foreach (var ingredient in production.ingredients)
            {
                PlayerInventory.Instance.AddIcon(ingredient, activeProduction.multiplier);
            }
        }

        activeProductions.Remove(activeProduction);
        activeProductionsBySlot.Remove(slotIndex);
        Save();

        OnProductionCancelled?.Invoke(slotIndex);
        OnProductionsChanged?.Invoke();

        return true;
    }

    /// <summary>
    /// Completes a production in the specified slot, granting the result.
    /// </summary>
    /// <param name="slotIndex">The slot index to complete</param>
    private void CompleteProduction(int slotIndex)
    {
        var activeProduction = GetActiveProduction(slotIndex);
        if (activeProduction == null)
        {
            return;
        }

        var production = GetProduction(activeProduction.productionId);
        if (production != null && PlayerInventory.Instance != null)
        {
            // Grant result to player
            PlayerInventory.Instance.AddIcon(production.result, activeProduction.multiplier);
        }

        OnProductionCompleted?.Invoke(slotIndex, activeProduction);

        activeProductions.Remove(activeProduction);
        activeProductionsBySlot.Remove(slotIndex);
        Save();
    }

    /// <summary>
    /// Gets the active production in a specific slot.
    /// </summary>
    /// <param name="slotIndex">The slot index to check</param>
    /// <returns>The active production, or null if slot is empty</returns>
    public ActiveProduction GetActiveProduction(int slotIndex)
    {
        activeProductionsBySlot.TryGetValue(slotIndex, out var production);
        return production;
    }

    /// <summary>
    /// Rebuilds the slot dictionary from the active productions list.
    /// Called after loading or modifying productions.
    /// </summary>
    private void RebuildSlotDictionary()
    {
        activeProductionsBySlot.Clear();
        foreach (var production in activeProductions)
        {
            activeProductionsBySlot[production.slotIndex] = production;
        }
    }

    /// <summary>
    /// Gets all active productions.
    /// </summary>
    /// <returns>List of all active productions</returns>
    public List<ActiveProduction> GetAllActiveProductions()
    {
        return new List<ActiveProduction>(activeProductions);
    }

    /// <summary>
    /// Gets the number of active production slots.
    /// </summary>
    /// <returns>Number of slots currently in use</returns>
    public int GetActiveSlotCount()
    {
        return activeProductions.Count;
    }

    /// <summary>
    /// Gets the maximum number of production slots.
    /// </summary>
    public int MaxProductionSlots => MAX_PRODUCTION_SLOTS;

    /// <summary>
    /// Checks if there are any available production slots.
    /// </summary>
    /// <returns>True if at least one slot is available</returns>
    public bool HasAvailableSlot()
    {
        return activeProductions.Count < MAX_PRODUCTION_SLOTS;
    }

    /// <summary>
    /// Saves production state to persistent storage.
    /// </summary>
    public void Save()
    {
        // Update saved elapsed time for each active production before saving
        foreach (var production in activeProductions)
        {
            production.savedElapsedTime = production.GetElapsedTime();
        }

        // Save active productions
        var productionsSaveData = new ActiveProductionsSaveData { productions = activeProductions };
        string productionsJson = JsonUtility.ToJson(productionsSaveData);
        PlayerPrefs.SetString(SAVE_KEY_PRODUCTIONS, productionsJson);

        // Save discovered productions
        var discoveredSaveData = new DiscoveredProductionsSaveData { discoveredIds = discoveredProductionIds };
        string discoveredJson = JsonUtility.ToJson(discoveredSaveData);
        PlayerPrefs.SetString(SAVE_KEY_DISCOVERED, discoveredJson);

        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads production state from persistent storage.
    /// Note: Productions resume from their saved progress. This design choice means
    /// productions do not progress while the game is closed. For offline progression,
    /// consider using DateTime-based tracking instead of Time.time.
    /// </summary>
    public void Load()
    {
        // Load active productions
        if (PlayerPrefs.HasKey(SAVE_KEY_PRODUCTIONS))
        {
            string json = PlayerPrefs.GetString(SAVE_KEY_PRODUCTIONS);
            if (!string.IsNullOrEmpty(json))
            {
                var saveData = JsonUtility.FromJson<ActiveProductionsSaveData>(json);
                if (saveData != null && saveData.productions != null)
                {
                    activeProductions = saveData.productions;

                    // Recalculate start time based on saved elapsed time
                    // This preserves progress within the session but doesn't account for offline time
                    float currentTime = Time.time;
                    foreach (var production in activeProductions)
                    {
                        // Set startTime so that GetElapsedTime() returns savedElapsedTime
                        production.startTime = currentTime - production.savedElapsedTime;
                    }
                }
            }
        }

        // Load discovered productions
        if (PlayerPrefs.HasKey(SAVE_KEY_DISCOVERED))
        {
            string json = PlayerPrefs.GetString(SAVE_KEY_DISCOVERED);
            if (!string.IsNullOrEmpty(json))
            {
                var saveData = JsonUtility.FromJson<DiscoveredProductionsSaveData>(json);
                if (saveData != null && saveData.discoveredIds != null)
                {
                    discoveredProductionIds = saveData.discoveredIds;

                    // Update discovered state in available productions
                    foreach (var production in availableProductions)
                    {
                        production.isDiscovered = discoveredProductionIds.Contains(production.id);
                    }
                }
            }
        }
    }
}

/// <summary>
/// Represents an active (in-progress) production.
/// </summary>
[Serializable]
public class ActiveProduction
{
    /// <summary>
    /// The slot index where this production is running.
    /// </summary>
    public int slotIndex;

    /// <summary>
    /// The production recipe ID.
    /// </summary>
    public string productionId;

    /// <summary>
    /// The quantity multiplier.
    /// </summary>
    public int multiplier;

    /// <summary>
    /// The time when production started (Time.time).
    /// </summary>
    public float startTime;

    /// <summary>
    /// The total time required for this production.
    /// </summary>
    public float totalTime;

    /// <summary>
    /// Elapsed time at the time of saving (used for persistence).
    /// </summary>
    public float savedElapsedTime;

    public ActiveProduction() { }

    public ActiveProduction(int slotIndex, string productionId, int multiplier, float startTime, float totalTime)
    {
        this.slotIndex = slotIndex;
        this.productionId = productionId;
        this.multiplier = multiplier;
        this.startTime = startTime;
        this.totalTime = totalTime;
        this.savedElapsedTime = 0f;
    }

    /// <summary>
    /// Gets the elapsed time since production started.
    /// </summary>
    /// <returns>Elapsed time in seconds</returns>
    public float GetElapsedTime()
    {
        return Time.time - startTime;
    }

    /// <summary>
    /// Gets the remaining time until production is complete.
    /// </summary>
    /// <returns>Remaining time in seconds</returns>
    public float GetRemainingTime()
    {
        return Mathf.Max(0, totalTime - GetElapsedTime());
    }

    /// <summary>
    /// Gets the progress as a value between 0 and 1.
    /// </summary>
    /// <returns>Progress value (0 = just started, 1 = complete)</returns>
    public float GetProgress()
    {
        if (totalTime <= 0)
        {
            return 1f;
        }
        return Mathf.Clamp01(GetElapsedTime() / totalTime);
    }

    /// <summary>
    /// Checks if the production is complete.
    /// </summary>
    /// <returns>True if production is complete</returns>
    public bool IsComplete()
    {
        return GetElapsedTime() >= totalTime;
    }
}

/// <summary>
/// Serializable container for saving active productions.
/// </summary>
[Serializable]
public class ActiveProductionsSaveData
{
    public List<ActiveProduction> productions = new List<ActiveProduction>();
}

/// <summary>
/// Serializable container for saving discovered production IDs.
/// </summary>
[Serializable]
public class DiscoveredProductionsSaveData
{
    public List<string> discoveredIds = new List<string>();
}
