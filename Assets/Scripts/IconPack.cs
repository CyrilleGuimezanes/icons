using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Represents a pack of random icons that can be purchased.
/// Supports both in-game currency and real money (IAP) purchases.
/// </summary>
[Serializable]
public class IconPack
{
    /// <summary>
    /// Unique identifier for this pack.
    /// </summary>
    public string packId;

    /// <summary>
    /// Display name of the pack.
    /// </summary>
    public string displayName;

    /// <summary>
    /// Number of icons in this pack.
    /// </summary>
    public int iconCount;

    /// <summary>
    /// Price in real money (for IAP). Format: "$X.XX"
    /// </summary>
    public string realMoneyPrice;

    /// <summary>
    /// Whether this pack requires real money purchase (IAP).
    /// </summary>
    public bool isIAP;

    /// <summary>
    /// Icon ID used to represent this pack visually.
    /// </summary>
    public string packIconId;

    /// <summary>
    /// Description of the pack contents.
    /// </summary>
    public string description;

    public IconPack() { }

    public IconPack(string packId, string displayName, int iconCount, string realMoneyPrice, bool isIAP, string packIconId, string description)
    {
        this.packId = packId;
        this.displayName = displayName;
        this.iconCount = iconCount;
        this.realMoneyPrice = realMoneyPrice;
        this.isIAP = isIAP;
        this.packIconId = packIconId;
        this.description = description;
    }

    /// <summary>
    /// Generates the contents of this pack based on rarity weights.
    /// </summary>
    public List<IconEntry> GeneratePackContents()
    {
        if (IconDatabase.Instance == null)
        {
            Debug.LogWarning("IconDatabase not available for pack generation");
            return new List<IconEntry>();
        }

        return IconDatabase.Instance.GetRandomIcons(iconCount, allowDuplicates: true);
    }
}

/// <summary>
/// Defines the available icon packs in the game.
/// </summary>
public static class IconPacks
{
    /// <summary>
    /// Small pack: 5 random icons.
    /// </summary>
    public static readonly IconPack SmallPack = new IconPack(
        "pack_small",
        "Petit Pack",
        5,
        "$0.99",
        true,
        "inventory_2",
        "5 icônes aléatoires"
    );

    /// <summary>
    /// Big pack: 15 random icons.
    /// </summary>
    public static readonly IconPack BigPack = new IconPack(
        "pack_big",
        "Grand Pack",
        15,
        "$2.99",
        true,
        "all_inbox",
        "15 icônes aléatoires"
    );

    /// <summary>
    /// Huge pack: 50 random icons.
    /// </summary>
    public static readonly IconPack HugePack = new IconPack(
        "pack_huge",
        "Énorme Pack",
        50,
        "$4.99",
        true,
        "move_to_inbox",
        "50 icônes aléatoires"
    );

    /// <summary>
    /// Gets all available packs.
    /// </summary>
    public static IconPack[] GetAllPacks()
    {
        return new IconPack[] { SmallPack, BigPack, HugePack };
    }
}
