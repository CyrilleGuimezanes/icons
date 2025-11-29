using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// UI component for displaying a shop pack item.
/// Shows the pack icon, name, contents count, and price.
/// </summary>
public class ShopPackDisplay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI iconText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Button purchaseButton;

    [Header("Colors")]
    [SerializeField] private Color smallPackColor = new Color(0.4f, 0.7f, 0.4f);
    [SerializeField] private Color bigPackColor = new Color(0.4f, 0.4f, 0.9f);
    [SerializeField] private Color hugePackColor = new Color(0.9f, 0.6f, 0.2f);

    private IconPack pack;
    private Action<IconPack> onClickCallback;

    /// <summary>
    /// Sets up the display with an icon pack.
    /// </summary>
    /// <param name="iconPack">The icon pack to display.</param>
    /// <param name="onClick">Callback when the pack is clicked.</param>
    public void Setup(IconPack iconPack, Action<IconPack> onClick)
    {
        pack = iconPack;
        onClickCallback = onClick;

        UpdateDisplay();

        if (purchaseButton != null)
        {
            purchaseButton.onClick.RemoveAllListeners();
            purchaseButton.onClick.AddListener(OnButtonClicked);
        }
    }

    /// <summary>
    /// Updates the visual display based on the current pack.
    /// </summary>
    public void UpdateDisplay()
    {
        if (pack == null)
        {
            return;
        }

        // Set icon (Material Icons ligature)
        if (iconText != null)
        {
            iconText.text = pack.packIconId;
        }

        // Set name
        if (nameText != null)
        {
            nameText.text = pack.displayName;
        }

        // Set description
        if (descriptionText != null)
        {
            descriptionText.text = pack.description;
        }

        // Set price (use localized price from IAP if available)
        if (priceText != null)
        {
            if (IAPManager.Instance != null && IAPManager.Instance.IsInitialized)
            {
                priceText.text = IAPManager.Instance.GetLocalizedPrice(pack.packId);
            }
            else
            {
                priceText.text = pack.realMoneyPrice;
            }
        }

        // Set background color based on pack type
        if (backgroundImage != null)
        {
            backgroundImage.color = GetPackColor(pack.packId);
        }
    }

    /// <summary>
    /// Gets the color for a specific pack type.
    /// </summary>
    private Color GetPackColor(string packId)
    {
        return packId switch
        {
            "pack_small" => smallPackColor,
            "pack_big" => bigPackColor,
            "pack_huge" => hugePackColor,
            _ => Color.white
        };
    }

    /// <summary>
    /// Called when the purchase button is clicked.
    /// </summary>
    private void OnButtonClicked()
    {
        onClickCallback?.Invoke(pack);
    }
}
