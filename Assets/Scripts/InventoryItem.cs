using System;

/// <summary>
/// Represents a single inventory item with an icon ID and quantity owned.
/// </summary>
[Serializable]
public class InventoryItem
{
    /// <summary>
    /// The unique identifier of the icon.
    /// </summary>
    public string iconId;

    /// <summary>
    /// The quantity of this icon owned by the player.
    /// </summary>
    public int quantity;

    public InventoryItem(string iconId, int quantity)
    {
        this.iconId = iconId;
        this.quantity = quantity;
    }
}
