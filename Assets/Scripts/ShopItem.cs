using UnityEngine;
using System;

/// <summary>
/// Represents an item available for purchase in the shop.
/// Can be an individual icon or other purchasable content.
/// </summary>
[Serializable]
public class ShopItem
{
    /// <summary>
    /// The icon ID of the item being sold.
    /// </summary>
    public string iconId;

    /// <summary>
    /// Display name of the item.
    /// </summary>
    public string displayName;

    /// <summary>
    /// Price in coins for this item.
    /// </summary>
    public int price;

    /// <summary>
    /// Rarity of the icon (affects price and availability).
    /// </summary>
    public IconRarity rarity;

    public ShopItem() { }

    public ShopItem(string iconId, string displayName, int price, IconRarity rarity)
    {
        this.iconId = iconId;
        this.displayName = displayName;
        this.price = price;
        this.rarity = rarity;
    }

    /// <summary>
    /// Creates a ShopItem from an IconEntry.
    /// Price is calculated based on rarity.
    /// </summary>
    public static ShopItem FromIconEntry(IconEntry icon)
    {
        int price = GetPriceForRarity(icon.rarity);
        return new ShopItem(icon.id, icon.displayName, price, icon.rarity);
    }

    /// <summary>
    /// Gets the base price for an icon based on its rarity.
    /// </summary>
    public static int GetPriceForRarity(IconRarity rarity)
    {
        return rarity switch
        {
            IconRarity.Common => 5,
            IconRarity.Uncommon => 15,
            IconRarity.Rare => 50,
            IconRarity.Legendary => 200,
            _ => 10
        };
    }
}
