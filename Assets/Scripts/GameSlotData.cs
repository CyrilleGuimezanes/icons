using System;

/// <summary>
/// Data class to store information about a game slot.
/// Includes slot name, statistics, and timestamps.
/// </summary>
[Serializable]
public class GameSlotData
{
    /// <summary>
    /// Unique identifier for the slot (0, 1, or 2).
    /// </summary>
    public int slotIndex;

    /// <summary>
    /// User-defined name for the slot.
    /// </summary>
    public string slotName;

    /// <summary>
    /// Number of icons unlocked in this save.
    /// </summary>
    public int unlockedIconsCount;

    /// <summary>
    /// Total game play duration in seconds.
    /// </summary>
    public long totalPlayTimeSeconds;

    /// <summary>
    /// Timestamp when the slot was created (Unix time).
    /// </summary>
    public long creationTimestamp;

    /// <summary>
    /// Timestamp of last played session (Unix time).
    /// </summary>
    public long lastPlayedTimestamp;

    /// <summary>
    /// Whether this slot has been initialized with a game.
    /// </summary>
    public bool isActive;

    /// <summary>
    /// Creates an empty slot data.
    /// </summary>
    public GameSlotData()
    {
        slotIndex = -1;
        slotName = "";
        unlockedIconsCount = 0;
        totalPlayTimeSeconds = 0;
        creationTimestamp = 0;
        lastPlayedTimestamp = 0;
        isActive = false;
    }

    /// <summary>
    /// Creates a new active slot with the given name.
    /// </summary>
    /// <param name="index">The slot index.</param>
    /// <param name="name">The slot name.</param>
    public GameSlotData(int index, string name)
    {
        slotIndex = index;
        slotName = string.IsNullOrEmpty(name) ? $"Game {index + 1}" : name;
        unlockedIconsCount = 0;
        totalPlayTimeSeconds = 0;
        creationTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        lastPlayedTimestamp = creationTimestamp;
        isActive = true;
    }

    /// <summary>
    /// Gets the formatted play time string.
    /// </summary>
    /// <returns>Formatted string like "2h 30m" or "45m".</returns>
    public string GetFormattedPlayTime()
    {
        TimeSpan time = TimeSpan.FromSeconds(totalPlayTimeSeconds);
        
        if (time.TotalHours >= 1)
        {
            return $"{(int)time.TotalHours}h {time.Minutes}m";
        }
        else if (time.TotalMinutes >= 1)
        {
            return $"{(int)time.TotalMinutes}m";
        }
        else
        {
            return "< 1m";
        }
    }

    /// <summary>
    /// Updates the last played timestamp to now.
    /// </summary>
    public void UpdateLastPlayed()
    {
        lastPlayedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    /// <summary>
    /// Adds play time to the total.
    /// </summary>
    /// <param name="seconds">Seconds to add.</param>
    public void AddPlayTime(long seconds)
    {
        totalPlayTimeSeconds += seconds;
    }
}
