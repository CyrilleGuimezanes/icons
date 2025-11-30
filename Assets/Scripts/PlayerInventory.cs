using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages the player's inventory of icons.
/// Provides methods to add, remove, and query icons.
/// Persists inventory data across game sessions using PlayerPrefs.
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    private const string SAVE_KEY_BASE = "PlayerInventory";

    /// <summary>
    /// Singleton instance for global access.
    /// </summary>
    public static PlayerInventory Instance { get; private set; }

    /// <summary>
    /// Gets the current save key with slot suffix.
    /// </summary>
    private string SaveKey
    {
        get
        {
            if (GameSlotsManager.Instance != null)
            {
                return SAVE_KEY_BASE + GameSlotsManager.Instance.GetCurrentSlotSuffix();
            }
            return SAVE_KEY_BASE;
        }
    }

    /// <summary>
    /// Event triggered when the inventory changes.
    /// </summary>
    public event Action OnInventoryChanged;

    [SerializeField]
    private InventoryData inventoryData = new InventoryData();

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
    /// Adds a specified quantity of an icon to the inventory.
    /// Also unlocks the icon if it wasn't already unlocked.
    /// </summary>
    /// <param name="iconId">The unique identifier of the icon.</param>
    /// <param name="quantity">The quantity to add (must be positive).</param>
    public void AddIcon(string iconId, int quantity = 1)
    {
        if (string.IsNullOrEmpty(iconId) || quantity <= 0)
        {
            return;
        }

        InventoryItem existingItem = inventoryData.items.Find(item => item.iconId == iconId);
        
        if (existingItem != null)
        {
            existingItem.quantity += quantity;
        }
        else
        {
            inventoryData.items.Add(new InventoryItem(iconId, quantity));
        }

        // Unlock the icon when it's added to the inventory
        if (UnlockedIconsManager.Instance != null)
        {
            UnlockedIconsManager.Instance.UnlockIcon(iconId);
        }

        Save();
        OnInventoryChanged?.Invoke();
    }

    /// <summary>
    /// Removes a specified quantity of an icon from the inventory.
    /// </summary>
    /// <param name="iconId">The unique identifier of the icon.</param>
    /// <param name="quantity">The quantity to remove (must be positive).</param>
    /// <returns>True if the removal was successful, false if insufficient quantity.</returns>
    public bool RemoveIcon(string iconId, int quantity = 1)
    {
        if (string.IsNullOrEmpty(iconId) || quantity <= 0)
        {
            return false;
        }

        InventoryItem existingItem = inventoryData.items.Find(item => item.iconId == iconId);
        
        if (existingItem == null || existingItem.quantity < quantity)
        {
            return false;
        }

        existingItem.quantity -= quantity;
        
        if (existingItem.quantity <= 0)
        {
            inventoryData.items.Remove(existingItem);
        }

        Save();
        OnInventoryChanged?.Invoke();
        return true;
    }

    /// <summary>
    /// Gets the quantity of a specific icon in the inventory.
    /// </summary>
    /// <param name="iconId">The unique identifier of the icon.</param>
    /// <returns>The quantity owned, or 0 if not in inventory.</returns>
    public int GetIconQuantity(string iconId)
    {
        if (string.IsNullOrEmpty(iconId))
        {
            return 0;
        }

        InventoryItem item = inventoryData.items.Find(i => i.iconId == iconId);
        return item?.quantity ?? 0;
    }

    /// <summary>
    /// Checks if the player has at least the specified quantity of an icon.
    /// </summary>
    /// <param name="iconId">The unique identifier of the icon.</param>
    /// <param name="quantity">The required quantity.</param>
    /// <returns>True if the player has enough of the icon.</returns>
    public bool HasIcon(string iconId, int quantity = 1)
    {
        return GetIconQuantity(iconId) >= quantity;
    }

    /// <summary>
    /// Gets a read-only list of all items in the inventory.
    /// </summary>
    /// <returns>A list of all inventory items.</returns>
    public List<InventoryItem> GetAllItems()
    {
        return new List<InventoryItem>(inventoryData.items);
    }

    /// <summary>
    /// Gets the total number of unique icons in the inventory.
    /// </summary>
    /// <returns>The count of unique icons.</returns>
    public int GetUniqueIconCount()
    {
        return inventoryData.items.Count;
    }

    /// <summary>
    /// Gets the total quantity of all icons in the inventory.
    /// </summary>
    /// <returns>The total quantity of all icons.</returns>
    public int GetTotalIconCount()
    {
        int total = 0;
        foreach (var item in inventoryData.items)
        {
            total += item.quantity;
        }
        return total;
    }

    /// <summary>
    /// Clears all items from the inventory.
    /// </summary>
    public void ClearInventory()
    {
        inventoryData.items.Clear();
        Save();
        OnInventoryChanged?.Invoke();
    }

    /// <summary>
    /// Saves the inventory data to persistent storage.
    /// </summary>
    public void Save()
    {
        string json = JsonUtility.ToJson(inventoryData);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads the inventory data from persistent storage.
    /// </summary>
    public void Load()
    {
        bool hasExistingData = inventoryData.items.Count > 0;
        
        if (PlayerPrefs.HasKey(SaveKey))
        {
            string json = PlayerPrefs.GetString(SaveKey);
            if (!string.IsNullOrEmpty(json))
            {
                inventoryData = JsonUtility.FromJson<InventoryData>(json);
                if (inventoryData == null)
                {
                    inventoryData = new InventoryData();
                }
            }
        }
        else
        {
            inventoryData = new InventoryData();
        }
        
        // Only notify if data actually changed or if there was previous data
        if (hasExistingData || inventoryData.items.Count > 0)
        {
            OnInventoryChanged?.Invoke();
        }
    }
}

/// <summary>
/// Serializable container for inventory items.
/// Used for JSON serialization of the inventory.
/// </summary>
[Serializable]
public class InventoryData
{
    public List<InventoryItem> items = new List<InventoryItem>();
}
