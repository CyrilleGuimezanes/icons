using UnityEngine;
using System;

/// <summary>
/// Represents an icon that can be collected by the player.
/// Contains the icon's identifier and rarity level.
/// </summary>
[Serializable]
public class IconData
{
    /// <summary>
    /// Unique identifier for the icon.
    /// </summary>
    public string id;

    /// <summary>
    /// Display name of the icon.
    /// </summary>
    public string displayName;

    /// <summary>
    /// Rarity level of the icon (0 = Common, 1 = Uncommon, 2 = Rare, 3 = Legendary).
    /// </summary>
    public IconRarity rarity;

    public IconData(string id, string displayName, IconRarity rarity)
    {
        this.id = id;
        this.displayName = displayName;
        this.rarity = rarity;
    }
}

/// <summary>
/// Enumeration of icon rarity levels matching the game design document.
/// </summary>
public enum IconRarity
{
    Common = 0,      // Green
    Uncommon = 1,    // Blue
    Rare = 2,        // Purple
    Legendary = 3    // Orange
}
