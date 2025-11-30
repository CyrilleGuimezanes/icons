using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages up to 3 game slots for player saves.
/// Handles slot creation, deletion, switching, and persistence.
/// </summary>
public class GameSlotsManager : MonoBehaviour
{
    private const string SAVE_KEY = "GameSlots";
    private const string CURRENT_SLOT_KEY = "CurrentSlotIndex";
    private const int MAX_SLOTS = 3;

    /// <summary>
    /// Singleton instance for global access.
    /// </summary>
    public static GameSlotsManager Instance { get; private set; }

    /// <summary>
    /// Event triggered when a slot is selected/changed.
    /// </summary>
    public event Action<int> OnSlotChanged;

    /// <summary>
    /// Event triggered when slots data is updated.
    /// </summary>
    public event Action OnSlotsUpdated;

    [SerializeField]
    private GameSlotsData slotsData = new GameSlotsData();

    private int currentSlotIndex = -1;
    private float sessionStartTime;

    /// <summary>
    /// Gets the currently selected slot index.
    /// </summary>
    public int CurrentSlotIndex => currentSlotIndex;

    /// <summary>
    /// Gets the currently active slot data, or null if none selected.
    /// </summary>
    public GameSlotData CurrentSlot
    {
        get
        {
            if (currentSlotIndex >= 0 && currentSlotIndex < slotsData.slots.Count)
            {
                return slotsData.slots[currentSlotIndex];
            }
            return null;
        }
    }

    /// <summary>
    /// Gets whether a slot is currently active.
    /// </summary>
    public bool HasActiveSlot => currentSlotIndex >= 0 && CurrentSlot != null && CurrentSlot.isActive;

    private void Awake()
    {
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

    private void Start()
    {
        sessionStartTime = Time.realtimeSinceStartup;
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            UpdatePlayTime();
            Save();
        }
        else
        {
            sessionStartTime = Time.realtimeSinceStartup;
        }
    }

    private void OnApplicationQuit()
    {
        UpdatePlayTime();
        Save();
    }

    private void UpdatePlayTime()
    {
        if (HasActiveSlot)
        {
            float elapsed = Time.realtimeSinceStartup - sessionStartTime;
            CurrentSlot.AddPlayTime((long)elapsed);
            CurrentSlot.UpdateLastPlayed();
            sessionStartTime = Time.realtimeSinceStartup;
        }
    }

    /// <summary>
    /// Gets all game slots.
    /// </summary>
    /// <returns>List of all slots.</returns>
    public List<GameSlotData> GetAllSlots()
    {
        return new List<GameSlotData>(slotsData.slots);
    }

    /// <summary>
    /// Gets a specific slot by index.
    /// </summary>
    /// <param name="index">Slot index (0-2).</param>
    /// <returns>The slot data or null if invalid.</returns>
    public GameSlotData GetSlot(int index)
    {
        if (index >= 0 && index < slotsData.slots.Count)
        {
            return slotsData.slots[index];
        }
        return null;
    }

    /// <summary>
    /// Creates a new game in the specified slot.
    /// </summary>
    /// <param name="slotIndex">The slot index (0-2).</param>
    /// <param name="slotName">The name for the slot.</param>
    /// <returns>True if creation was successful.</returns>
    public bool CreateNewSlot(int slotIndex, string slotName)
    {
        if (slotIndex < 0 || slotIndex >= MAX_SLOTS)
        {
            return false;
        }

        // Ensure we have enough slots in the list
        while (slotsData.slots.Count <= slotIndex)
        {
            slotsData.slots.Add(new GameSlotData());
        }

        // Clear any existing data for this slot
        ClearSlotData(slotIndex);

        // Create new slot
        slotsData.slots[slotIndex] = new GameSlotData(slotIndex, slotName);
        
        Save();
        OnSlotsUpdated?.Invoke();
        return true;
    }

    /// <summary>
    /// Renames a slot.
    /// </summary>
    /// <param name="slotIndex">The slot index.</param>
    /// <param name="newName">The new name.</param>
    /// <returns>True if successful.</returns>
    public bool RenameSlot(int slotIndex, string newName)
    {
        var slot = GetSlot(slotIndex);
        if (slot == null || !slot.isActive)
        {
            return false;
        }

        slot.slotName = string.IsNullOrEmpty(newName) ? $"Game {slotIndex + 1}" : newName;
        Save();
        OnSlotsUpdated?.Invoke();
        return true;
    }

    /// <summary>
    /// Selects a slot to use.
    /// </summary>
    /// <param name="slotIndex">The slot index to select.</param>
    /// <returns>True if selection was successful.</returns>
    public bool SelectSlot(int slotIndex)
    {
        var slot = GetSlot(slotIndex);
        if (slot == null || !slot.isActive)
        {
            return false;
        }

        // Save current slot's play time
        UpdatePlayTime();

        currentSlotIndex = slotIndex;
        sessionStartTime = Time.realtimeSinceStartup;
        slot.UpdateLastPlayed();
        
        SaveCurrentSlotIndex();
        OnSlotChanged?.Invoke(currentSlotIndex);
        return true;
    }

    /// <summary>
    /// Deletes a slot and clears its data.
    /// </summary>
    /// <param name="slotIndex">The slot index to delete.</param>
    /// <returns>True if deletion was successful.</returns>
    public bool DeleteSlot(int slotIndex)
    {
        var slot = GetSlot(slotIndex);
        if (slot == null)
        {
            return false;
        }

        // Clear game data for this slot
        ClearSlotData(slotIndex);

        // Reset the slot
        slotsData.slots[slotIndex] = new GameSlotData();
        slotsData.slots[slotIndex].slotIndex = slotIndex;

        // If this was the current slot, deselect it
        if (currentSlotIndex == slotIndex)
        {
            currentSlotIndex = -1;
            SaveCurrentSlotIndex();
        }

        Save();
        OnSlotsUpdated?.Invoke();
        return true;
    }

    /// <summary>
    /// Updates the unlocked icons count for the current slot.
    /// </summary>
    /// <param name="count">The new count.</param>
    public void UpdateUnlockedIconsCount(int count)
    {
        if (HasActiveSlot)
        {
            CurrentSlot.unlockedIconsCount = count;
            Save();
        }
    }

    /// <summary>
    /// Clears game-specific data for a slot.
    /// </summary>
    private void ClearSlotData(int slotIndex)
    {
        // Clear slot-specific PlayerPrefs keys
        string suffix = GetSlotSuffix(slotIndex);
        
        PlayerPrefs.DeleteKey("PlayerInventory" + suffix);
        PlayerPrefs.DeleteKey("CurrencyData" + suffix);
        PlayerPrefs.DeleteKey("UnlockedIcons" + suffix);
        PlayerPrefs.DeleteKey("ProductionData" + suffix);
        
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Gets the suffix for slot-specific keys.
    /// </summary>
    /// <param name="slotIndex">The slot index.</param>
    /// <returns>The key suffix.</returns>
    public static string GetSlotSuffix(int slotIndex)
    {
        if (slotIndex < 0) return "";
        return "_Slot" + slotIndex;
    }

    /// <summary>
    /// Gets the save key suffix for the current slot.
    /// </summary>
    /// <returns>The current slot suffix.</returns>
    public string GetCurrentSlotSuffix()
    {
        return GetSlotSuffix(currentSlotIndex);
    }

    /// <summary>
    /// Saves the slots data.
    /// </summary>
    public void Save()
    {
        string json = JsonUtility.ToJson(slotsData);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Saves the current slot index.
    /// </summary>
    private void SaveCurrentSlotIndex()
    {
        PlayerPrefs.SetInt(CURRENT_SLOT_KEY, currentSlotIndex);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads the slots data.
    /// </summary>
    public void Load()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            if (!string.IsNullOrEmpty(json))
            {
                slotsData = JsonUtility.FromJson<GameSlotsData>(json);
                if (slotsData == null)
                {
                    slotsData = new GameSlotsData();
                }
            }
        }

        // Initialize slots if needed
        while (slotsData.slots.Count < MAX_SLOTS)
        {
            var slot = new GameSlotData();
            slot.slotIndex = slotsData.slots.Count;
            slotsData.slots.Add(slot);
        }

        // Load current slot index
        if (PlayerPrefs.HasKey(CURRENT_SLOT_KEY))
        {
            currentSlotIndex = PlayerPrefs.GetInt(CURRENT_SLOT_KEY, -1);
        }
    }

    /// <summary>
    /// Checks if any slots have active games.
    /// </summary>
    /// <returns>True if at least one slot is active.</returns>
    public bool HasAnyActiveSlot()
    {
        foreach (var slot in slotsData.slots)
        {
            if (slot.isActive)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Gets the count of active slots.
    /// </summary>
    /// <returns>Number of active slots.</returns>
    public int GetActiveSlotCount()
    {
        int count = 0;
        foreach (var slot in slotsData.slots)
        {
            if (slot.isActive)
            {
                count++;
            }
        }
        return count;
    }
}

/// <summary>
/// Serializable container for game slots.
/// </summary>
[Serializable]
public class GameSlotsData
{
    public List<GameSlotData> slots = new List<GameSlotData>();
}
