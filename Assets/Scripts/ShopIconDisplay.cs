using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// UI component for displaying a shop icon item.
/// Shows the icon, name, price, and rarity.
/// </summary>
public class ShopIconDisplay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI iconText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI rarityText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private Image rarityBorder;

    [Header("Colors")]
    [SerializeField] private Color affordableColor = Color.white;
    [SerializeField] private Color unaffordableColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    private ShopItem item;
    private Action<string> onClickCallback;

    /// <summary>
    /// Initializes UI references for runtime building.
    /// </summary>
    public void InitializeReferences(TextMeshProUGUI icon, TextMeshProUGUI name, TextMeshProUGUI price)
    {
        iconText = icon;
        nameText = name;
        priceText = price;
    }

    /// <summary>
    /// Sets up the display with a shop item.
    /// </summary>
    /// <param name="shopItem">The shop item to display.</param>
    /// <param name="onClick">Callback when the item is clicked.</param>
    public void Setup(ShopItem shopItem, Action<string> onClick)
    {
        item = shopItem;
        onClickCallback = onClick;

        UpdateDisplay();

        if (purchaseButton != null)
        {
            purchaseButton.onClick.RemoveAllListeners();
            purchaseButton.onClick.AddListener(OnButtonClicked);
        }
    }

    /// <summary>
    /// Updates the visual display based on the current item.
    /// </summary>
    public void UpdateDisplay()
    {
        if (item == null)
        {
            return;
        }

        // Set icon (Material Icons ligature)
        if (iconText != null)
        {
            iconText.text = item.iconId;
        }

        // Set name
        if (nameText != null)
        {
            nameText.text = item.displayName;
        }

        // Set price
        if (priceText != null)
        {
            priceText.text = $"{item.price}";
        }

        // Set rarity
        if (rarityText != null)
        {
            rarityText.text = GetRarityText(item.rarity);
            rarityText.color = GetRarityColor(item.rarity);
        }

        // Set rarity border color
        if (rarityBorder != null)
        {
            rarityBorder.color = GetRarityColor(item.rarity);
        }

        // Update affordability visual
        UpdateAffordabilityVisual();
    }

    /// <summary>
    /// Updates the visual state based on whether the player can afford this item.
    /// </summary>
    private void UpdateAffordabilityVisual()
    {
        bool canAfford = CurrencyManager.Instance != null && CurrencyManager.Instance.CanAfford(item.price);

        if (backgroundImage != null)
        {
            backgroundImage.color = canAfford ? affordableColor : unaffordableColor;
        }

        if (purchaseButton != null)
        {
            purchaseButton.interactable = canAfford;
        }
    }

    /// <summary>
    /// Called when the purchase button is clicked.
    /// </summary>
    private void OnButtonClicked()
    {
        onClickCallback?.Invoke(item.iconId);
    }

    /// <summary>
    /// Gets the display text for a rarity level.
    /// </summary>
    private string GetRarityText(IconRarity rarity)
    {
        return rarity switch
        {
            IconRarity.Common => "Commun",
            IconRarity.Uncommon => "Peu commun",
            IconRarity.Rare => "Rare",
            IconRarity.Legendary => "Légendaire",
            _ => "Inconnu"
        };
    }

    /// <summary>
    /// Gets the color for a rarity level.
    /// </summary>
    private Color GetRarityColor(IconRarity rarity)
    {
        return rarity switch
        {
            IconRarity.Common => new Color(0.2f, 0.8f, 0.2f),     // Green
            IconRarity.Uncommon => new Color(0.2f, 0.6f, 1f),    // Blue
            IconRarity.Rare => new Color(0.6f, 0.2f, 0.8f),      // Purple
            IconRarity.Legendary => new Color(1f, 0.6f, 0.2f),   // Orange
            _ => Color.white
        };
    }
}
