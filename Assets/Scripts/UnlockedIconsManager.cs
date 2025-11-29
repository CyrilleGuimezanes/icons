using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages the icons unlock system.
/// Tracks which icons have been discovered/unlocked by the player.
/// When a player finds or creates an icon, it becomes unlocked and visible in collections.
/// Persists unlock data across game sessions using PlayerPrefs.
/// </summary>
public class UnlockedIconsManager : MonoBehaviour
{
    private const string SAVE_KEY = "UnlockedIcons";

    /// <summary>
    /// Singleton instance for global access.
    /// </summary>
    public static UnlockedIconsManager Instance { get; private set; }

    /// <summary>
    /// Event triggered when a new icon is unlocked.
    /// </summary>
    public event Action<string> OnIconUnlocked;

    [SerializeField]
    private UnlockedIconsData unlockedData = new UnlockedIconsData();

    private void Awake()
    {
        // Singleton pattern with persistence across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        // Subscribe to inventory changes to auto-unlock icons
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnInventoryChanged += OnInventoryChanged;
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from inventory changes
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnInventoryChanged -= OnInventoryChanged;
        }
    }

    private void Start()
    {
        // Retry subscription if PlayerInventory wasn't ready in OnEnable
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnInventoryChanged -= OnInventoryChanged;
            PlayerInventory.Instance.OnInventoryChanged += OnInventoryChanged;
        }
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
    /// Called when the inventory changes - unlocks any new icons found.
    /// </summary>
    private void OnInventoryChanged()
    {
        if (PlayerInventory.Instance == null) return;

        List<InventoryItem> items = PlayerInventory.Instance.GetAllItems();
        foreach (var item in items)
        {
            UnlockIcon(item.iconId);
        }
    }

    /// <summary>
    /// Unlocks an icon, making it visible in the collection.
    /// Called when a player finds or creates an icon.
    /// </summary>
    /// <param name="iconId">The unique identifier of the icon to unlock.</param>
    /// <returns>True if the icon was newly unlocked, false if already unlocked.</returns>
    public bool UnlockIcon(string iconId)
    {
        if (string.IsNullOrEmpty(iconId))
        {
            return false;
        }

        if (unlockedData.unlockedIconIds.Contains(iconId))
        {
            return false; // Already unlocked
        }

        unlockedData.unlockedIconIds.Add(iconId);
        Save();
        OnIconUnlocked?.Invoke(iconId);
        return true;
    }

    /// <summary>
    /// Checks if an icon has been unlocked.
    /// </summary>
    /// <param name="iconId">The unique identifier of the icon.</param>
    /// <returns>True if the icon is unlocked, false otherwise.</returns>
    public bool IsIconUnlocked(string iconId)
    {
        if (string.IsNullOrEmpty(iconId))
        {
            return false;
        }

        return unlockedData.unlockedIconIds.Contains(iconId);
    }

    /// <summary>
    /// Gets a read-only list of all unlocked icon IDs.
    /// </summary>
    /// <returns>A list of all unlocked icon IDs.</returns>
    public List<string> GetAllUnlockedIconIds()
    {
        return new List<string>(unlockedData.unlockedIconIds);
    }

    /// <summary>
    /// Gets the total number of unlocked icons.
    /// </summary>
    /// <returns>The count of unlocked icons.</returns>
    public int GetUnlockedIconCount()
    {
        return unlockedData.unlockedIconIds.Count;
    }

    /// <summary>
    /// Clears all unlock data (for testing or reset purposes).
    /// </summary>
    public void ClearUnlockData()
    {
        unlockedData.unlockedIconIds.Clear();
        Save();
    }

    /// <summary>
    /// Saves the unlock data to persistent storage.
    /// </summary>
    public void Save()
    {
        string json = JsonUtility.ToJson(unlockedData);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads the unlock data from persistent storage.
    /// </summary>
    public void Load()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            if (!string.IsNullOrEmpty(json))
            {
                unlockedData = JsonUtility.FromJson<UnlockedIconsData>(json);
                if (unlockedData == null)
                {
                    unlockedData = new UnlockedIconsData();
                }
            }
        }
    }
}

/// <summary>
/// Serializable container for unlocked icon IDs.
/// Used for JSON serialization of the unlock data.
/// </summary>
[Serializable]
public class UnlockedIconsData
{
    public List<string> unlockedIconIds = new List<string>();
}
