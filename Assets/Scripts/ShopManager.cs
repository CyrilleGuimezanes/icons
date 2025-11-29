using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages the shop/market system.
/// Handles icon purchases with coins and IAP pack purchases.
/// </summary>
public class ShopManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance for global access.
    /// </summary>
    public static ShopManager Instance { get; private set; }

    /// <summary>
    /// Event triggered when an icon is purchased.
    /// </summary>
    public event Action<string> OnIconPurchased;

    /// <summary>
    /// Event triggered when a pack is purchased.
    /// </summary>
    public event Action<IconPack, List<IconEntry>> OnPackPurchased;

    /// <summary>
    /// Event triggered when a purchase fails due to insufficient funds.
    /// </summary>
    public event Action OnPurchaseFailed;

    private List<ShopItem> availableItems = new List<ShopItem>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        RefreshShopItems();
    }

    /// <summary>
    /// Refreshes the list of available shop items from the icon database.
    /// </summary>
    public void RefreshShopItems()
    {
        availableItems.Clear();

        if (IconDatabase.Instance == null)
        {
            Debug.LogWarning("IconDatabase not available for shop items");
            return;
        }

        // Get all icons and create shop items from them
        var allIcons = IconDatabase.Instance.GetAllIcons();
        foreach (var icon in allIcons)
        {
            availableItems.Add(ShopItem.FromIconEntry(icon));
        }
    }

    /// <summary>
    /// Gets all available items in the shop.
    /// </summary>
    public List<ShopItem> GetAvailableItems()
    {
        return new List<ShopItem>(availableItems);
    }

    /// <summary>
    /// Gets shop items filtered by rarity.
    /// </summary>
    public List<ShopItem> GetItemsByRarity(IconRarity rarity)
    {
        return availableItems.FindAll(item => item.rarity == rarity);
    }

    /// <summary>
    /// Attempts to purchase an icon with coins.
    /// </summary>
    /// <param name="iconId">The icon ID to purchase.</param>
    /// <returns>True if purchase was successful, false otherwise.</returns>
    public bool PurchaseIcon(string iconId)
    {
        if (string.IsNullOrEmpty(iconId))
        {
            return false;
        }

        // Find the shop item
        ShopItem item = availableItems.Find(i => i.iconId == iconId);
        if (item == null)
        {
            Debug.LogWarning($"Icon {iconId} not found in shop");
            return false;
        }

        // Check if player can afford it
        if (CurrencyManager.Instance == null)
        {
            Debug.LogWarning("CurrencyManager not available");
            return false;
        }

        if (!CurrencyManager.Instance.CanAfford(item.price))
        {
            OnPurchaseFailed?.Invoke();
            return false;
        }

        // Spend coins
        if (!CurrencyManager.Instance.SpendCoins(item.price))
        {
            OnPurchaseFailed?.Invoke();
            return false;
        }

        // Add icon to inventory
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.AddIcon(iconId, 1);
        }

        OnIconPurchased?.Invoke(iconId);
        return true;
    }

    /// <summary>
    /// Attempts to purchase a pack with real money (IAP).
    /// Uses Unity IAP for actual purchases.
    /// </summary>
    /// <param name="pack">The pack to purchase.</param>
    /// <returns>True if purchase was initiated, false otherwise.</returns>
    public bool PurchasePack(IconPack pack)
    {
        if (pack == null)
        {
            return false;
        }

        // Use IAPManager for real IAP purchases
        if (IAPManager.Instance != null && IAPManager.Instance.IsInitialized)
        {
            IAPManager.Instance.PurchaseProduct(pack.packId);
            return true;
        }
        else
        {
            // Fallback to simulation if IAP is not available
            Debug.LogWarning("IAP not available, simulating purchase");
            return ProcessPackPurchase(pack);
        }
    }

    /// <summary>
    /// Called by IAPManager when an IAP purchase is successful.
    /// </summary>
    /// <param name="packId">The pack ID that was purchased.</param>
    public void ProcessIAPPurchase(string packId)
    {
        IconPack pack = FindPackById(packId);

        if (pack != null)
        {
            ProcessPackPurchase(pack);
        }
        else
        {
            Debug.LogWarning($"Pack {packId} not found for IAP processing");
        }
    }

    /// <summary>
    /// Simulates an IAP purchase for testing purposes.
    /// In production, this would be called after successful IAP verification.
    /// </summary>
    /// <param name="packId">The pack ID to process.</param>
    /// <returns>True if the pack was processed successfully.</returns>
    public bool SimulateIAPPurchase(string packId)
    {
        IconPack pack = FindPackById(packId);

        if (pack == null)
        {
            Debug.LogWarning($"Pack {packId} not found");
            return false;
        }

        return ProcessPackPurchase(pack);
    }

    /// <summary>
    /// Finds a pack by its ID.
    /// </summary>
    /// <param name="packId">The pack ID to find.</param>
    /// <returns>The pack if found, null otherwise.</returns>
    private IconPack FindPackById(string packId)
    {
        foreach (var pack in IconPacks.GetAllPacks())
        {
            if (pack.packId == packId)
            {
                return pack;
            }
        }
        return null;
    }

    /// <summary>
    /// Processes a pack purchase and distributes icons.
    /// </summary>
    private bool ProcessPackPurchase(IconPack pack)
    {
        // Generate pack contents
        List<IconEntry> packContents = pack.GeneratePackContents();

        if (packContents == null || packContents.Count == 0)
        {
            Debug.LogWarning("Failed to generate pack contents");
            return false;
        }

        // Add all icons to player's inventory
        if (PlayerInventory.Instance != null)
        {
            foreach (var icon in packContents)
            {
                PlayerInventory.Instance.AddIcon(icon.id, 1);
            }
        }

        OnPackPurchased?.Invoke(pack, packContents);
        return true;
    }

    /// <summary>
    /// Gets the price of an icon by ID.
    /// </summary>
    public int GetIconPrice(string iconId)
    {
        ShopItem item = availableItems.Find(i => i.iconId == iconId);
        return item?.price ?? 0;
    }

    /// <summary>
    /// Checks if the player can afford an icon.
    /// </summary>
    public bool CanAffordIcon(string iconId)
    {
        int price = GetIconPrice(iconId);
        return CurrencyManager.Instance != null && CurrencyManager.Instance.CanAfford(price);
    }

    /// <summary>
    /// Gets all available icon packs.
    /// </summary>
    public IconPack[] GetAvailablePacks()
    {
        return IconPacks.GetAllPacks();
    }
}
