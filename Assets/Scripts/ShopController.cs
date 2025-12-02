using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

/// <summary>
/// Controller for the Shop/Boutique screen.
/// Handles UI display for purchasing icons and icon packs.
/// </summary>
public class ShopController : ScreenController
{
    [Header("Currency Display")]
    [SerializeField] private TextMeshProUGUI coinBalanceText;
    [SerializeField] private TextMeshProUGUI nextCoinTimerText;

    [Header("Tab Buttons")]
    [SerializeField] private Button iconsTabButton;
    [SerializeField] private Button packsTabButton;

    [Header("Icons Panel")]
    [SerializeField] private GameObject iconsPanel;
    [SerializeField] private Transform iconsContainer;
    [SerializeField] private GameObject shopIconPrefab;

    [Header("Packs Panel")]
    [SerializeField] private GameObject packsPanel;
    [SerializeField] private Transform packsContainer;
    [SerializeField] private GameObject shopPackPrefab;

    [Header("Purchase Confirmation")]
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private TextMeshProUGUI confirmationText;
    [SerializeField] private TextMeshProUGUI confirmationPriceText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    [Header("Purchase Result")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI resultTitleText;
    [SerializeField] private TextMeshProUGUI resultMessageText;
    [SerializeField] private Button resultCloseButton;

    [Header("Pack Opening")]
    [SerializeField] private GameObject packOpeningPanel;
    [SerializeField] private Transform packContentsContainer;
    [SerializeField] private GameObject packIconDisplayPrefab;
    [SerializeField] private Button closePackButton;

    [Header("Rarity Filter")]
    [SerializeField] private TMP_Dropdown rarityFilterDropdown;

    private string pendingPurchaseIconId;
    private IconPack pendingPurchasePack;
    private bool isShowingIcons = true;

    /// <summary>
    /// Initializes references for runtime UI building.
    /// </summary>
    public void InitializeReferences(TextMeshProUGUI coinBalance, TextMeshProUGUI nextCoinTimer,
        Button iconsTab, Button packsTab,
        GameObject iconsPnl, Transform iconsCont, GameObject iconPrefab,
        GameObject packsPnl, Transform packsCont, GameObject packPrefab,
        GameObject confirmPnl, TextMeshProUGUI confirmTxt, TextMeshProUGUI confirmPrice,
        Button confirmBtn, Button cancelBtn,
        GameObject resultPnl, TextMeshProUGUI resultTitle, TextMeshProUGUI resultMsg, Button resultClose,
        GameObject packOpenPnl, Transform packContentsCont, GameObject packIconPrefab, Button closePack,
        TMP_Dropdown rarityFilter, TextMeshProUGUI title)
    {
        coinBalanceText = coinBalance;
        nextCoinTimerText = nextCoinTimer;
        iconsTabButton = iconsTab;
        packsTabButton = packsTab;
        iconsPanel = iconsPnl;
        iconsContainer = iconsCont;
        shopIconPrefab = iconPrefab;
        packsPanel = packsPnl;
        packsContainer = packsCont;
        shopPackPrefab = packPrefab;
        confirmationPanel = confirmPnl;
        confirmationText = confirmTxt;
        confirmationPriceText = confirmPrice;
        confirmButton = confirmBtn;
        cancelButton = cancelBtn;
        resultPanel = resultPnl;
        resultTitleText = resultTitle;
        resultMessageText = resultMsg;
        resultCloseButton = resultClose;
        packOpeningPanel = packOpenPnl;
        packContentsContainer = packContentsCont;
        packIconDisplayPrefab = packIconPrefab;
        closePackButton = closePack;
        rarityFilterDropdown = rarityFilter;
        titleText = title;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SetupEventListeners();
        RefreshUI();
        ShowIconsTab();
    }

    private void OnDisable()
    {
        RemoveEventListeners();
    }

    private void Update()
    {
        UpdateNextCoinTimer();
    }

    private void SetupEventListeners()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnBalanceChanged += OnBalanceChanged;
        }

        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.OnIconPurchased += OnIconPurchased;
            ShopManager.Instance.OnPackPurchased += OnPackPurchased;
            ShopManager.Instance.OnPurchaseFailed += OnPurchaseFailed;
        }

        // Tab buttons
        if (iconsTabButton != null)
        {
            iconsTabButton.onClick.AddListener(ShowIconsTab);
        }

        if (packsTabButton != null)
        {
            packsTabButton.onClick.AddListener(ShowPacksTab);
        }

        // Confirmation buttons
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(ConfirmPurchase);
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(CancelPurchase);
        }

        // Result close button
        if (resultCloseButton != null)
        {
            resultCloseButton.onClick.AddListener(CloseResultPanel);
        }

        // Pack close button
        if (closePackButton != null)
        {
            closePackButton.onClick.AddListener(ClosePackOpeningPanel);
        }

        // Rarity filter
        if (rarityFilterDropdown != null)
        {
            rarityFilterDropdown.onValueChanged.AddListener(OnRarityFilterChanged);
        }
    }

    private void RemoveEventListeners()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnBalanceChanged -= OnBalanceChanged;
        }

        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.OnIconPurchased -= OnIconPurchased;
            ShopManager.Instance.OnPackPurchased -= OnPackPurchased;
            ShopManager.Instance.OnPurchaseFailed -= OnPurchaseFailed;
        }
    }

    /// <summary>
    /// Refreshes the entire UI.
    /// </summary>
    public void RefreshUI()
    {
        UpdateCoinBalance();
        UpdateNextCoinTimer();
        HideAllPopups();

        if (isShowingIcons)
        {
            PopulateIconsPanel();
        }
        else
        {
            PopulatePacksPanel();
        }
    }

    /// <summary>
    /// Updates the coin balance display.
    /// </summary>
    private void UpdateCoinBalance()
    {
        if (coinBalanceText != null && CurrencyManager.Instance != null)
        {
            coinBalanceText.text = CurrencyManager.Instance.Balance.ToString();
        }
    }

    /// <summary>
    /// Updates the next coin timer display.
    /// </summary>
    private void UpdateNextCoinTimer()
    {
        if (nextCoinTimerText != null && CurrencyManager.Instance != null)
        {
            TimeSpan remaining = CurrencyManager.Instance.GetTimeUntilNextCoin();
            if (remaining.TotalSeconds > 0)
            {
                nextCoinTimerText.text = $"Prochaine pièce: {remaining.Minutes:D2}:{remaining.Seconds:D2}";
            }
            else
            {
                nextCoinTimerText.text = "Pièce disponible !";
            }
        }
    }

    /// <summary>
    /// Shows the icons purchasing tab.
    /// </summary>
    public void ShowIconsTab()
    {
        isShowingIcons = true;

        if (iconsPanel != null) iconsPanel.SetActive(true);
        if (packsPanel != null) packsPanel.SetActive(false);

        PopulateIconsPanel();
    }

    /// <summary>
    /// Shows the packs purchasing tab.
    /// </summary>
    public void ShowPacksTab()
    {
        isShowingIcons = false;

        if (iconsPanel != null) iconsPanel.SetActive(false);
        if (packsPanel != null) packsPanel.SetActive(true);

        PopulatePacksPanel();
    }

    /// <summary>
    /// Populates the icons panel with purchasable icons.
    /// </summary>
    private void PopulateIconsPanel()
    {
        if (iconsContainer == null || shopIconPrefab == null)
        {
            return;
        }

        // Clear existing items
        foreach (Transform child in iconsContainer)
        {
            Destroy(child.gameObject);
        }

        if (ShopManager.Instance == null)
        {
            return;
        }

        // Get items filtered by rarity if filter is set
        List<ShopItem> items;
        if (rarityFilterDropdown != null && rarityFilterDropdown.value > 0)
        {
            IconRarity selectedRarity = (IconRarity)(rarityFilterDropdown.value - 1);
            items = ShopManager.Instance.GetItemsByRarity(selectedRarity);
        }
        else
        {
            items = ShopManager.Instance.GetAvailableItems();
        }

        foreach (var item in items)
        {
            CreateShopIconItem(item);
        }
    }

    /// <summary>
    /// Creates a shop icon item in the UI.
    /// </summary>
    private void CreateShopIconItem(ShopItem item)
    {
        GameObject iconObj = Instantiate(shopIconPrefab, iconsContainer);
        
        // Set up the icon display
        ShopIconDisplay display = iconObj.GetComponent<ShopIconDisplay>();
        if (display != null)
        {
            display.Setup(item, OnIconClicked);
        }
    }

    /// <summary>
    /// Populates the packs panel with purchasable packs.
    /// </summary>
    private void PopulatePacksPanel()
    {
        if (packsContainer == null || shopPackPrefab == null)
        {
            return;
        }

        // Clear existing items
        foreach (Transform child in packsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var pack in IconPacks.GetAllPacks())
        {
            CreateShopPackItem(pack);
        }
    }

    /// <summary>
    /// Creates a shop pack item in the UI.
    /// </summary>
    private void CreateShopPackItem(IconPack pack)
    {
        GameObject packObj = Instantiate(shopPackPrefab, packsContainer);
        
        // Set up the pack display
        ShopPackDisplay display = packObj.GetComponent<ShopPackDisplay>();
        if (display != null)
        {
            display.Setup(pack, OnPackClicked);
        }
    }

    /// <summary>
    /// Called when an icon is clicked in the shop.
    /// </summary>
    private void OnIconClicked(string iconId)
    {
        pendingPurchaseIconId = iconId;
        pendingPurchasePack = null;
        ShowIconPurchaseConfirmation(iconId);
    }

    /// <summary>
    /// Called when a pack is clicked in the shop.
    /// </summary>
    private void OnPackClicked(IconPack pack)
    {
        pendingPurchasePack = pack;
        pendingPurchaseIconId = null;
        ShowPackPurchaseConfirmation(pack);
    }

    /// <summary>
    /// Shows the purchase confirmation panel for an icon.
    /// </summary>
    private void ShowIconPurchaseConfirmation(string iconId)
    {
        if (confirmationPanel == null || ShopManager.Instance == null)
        {
            return;
        }

        var items = ShopManager.Instance.GetAvailableItems();
        var item = items.Find(i => i.iconId == iconId);
        if (item == null) return;

        confirmationPanel.SetActive(true);

        if (confirmationText != null)
        {
            confirmationText.text = $"Acheter {item.displayName} ?";
        }

        if (confirmationPriceText != null)
        {
            confirmationPriceText.text = $"{item.price} pièces";
        }
    }

    /// <summary>
    /// Shows the purchase confirmation panel for a pack.
    /// </summary>
    private void ShowPackPurchaseConfirmation(IconPack pack)
    {
        if (confirmationPanel == null)
        {
            return;
        }

        confirmationPanel.SetActive(true);

        if (confirmationText != null)
        {
            confirmationText.text = $"Acheter {pack.displayName} ?\n{pack.description}";
        }

        if (confirmationPriceText != null)
        {
            confirmationPriceText.text = pack.realMoneyPrice;
        }
    }

    /// <summary>
    /// Confirms the pending purchase.
    /// </summary>
    private void ConfirmPurchase()
    {
        HideConfirmationPanel();

        if (!string.IsNullOrEmpty(pendingPurchaseIconId))
        {
            ShopManager.Instance?.PurchaseIcon(pendingPurchaseIconId);
        }
        else if (pendingPurchasePack != null)
        {
            ShopManager.Instance?.PurchasePack(pendingPurchasePack);
        }

        pendingPurchaseIconId = null;
        pendingPurchasePack = null;
    }

    /// <summary>
    /// Cancels the pending purchase.
    /// </summary>
    private void CancelPurchase()
    {
        HideConfirmationPanel();
        pendingPurchaseIconId = null;
        pendingPurchasePack = null;
    }

    /// <summary>
    /// Hides the confirmation panel.
    /// </summary>
    private void HideConfirmationPanel()
    {
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Hides all popup panels.
    /// </summary>
    private void HideAllPopups()
    {
        HideConfirmationPanel();
        CloseResultPanel();
        ClosePackOpeningPanel();
    }

    /// <summary>
    /// Shows the result panel with a message.
    /// </summary>
    private void ShowResultPanel(string title, string message)
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }

        if (resultTitleText != null)
        {
            resultTitleText.text = title;
        }

        if (resultMessageText != null)
        {
            resultMessageText.text = message;
        }
    }

    /// <summary>
    /// Closes the result panel.
    /// </summary>
    private void CloseResultPanel()
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Shows the pack opening panel with contents.
    /// </summary>
    private void ShowPackOpeningPanel(List<IconEntry> contents)
    {
        if (packOpeningPanel == null || packContentsContainer == null)
        {
            return;
        }

        // Clear existing contents
        foreach (Transform child in packContentsContainer)
        {
            Destroy(child.gameObject);
        }

        packOpeningPanel.SetActive(true);

        // Display each icon from the pack
        if (packIconDisplayPrefab != null)
        {
            foreach (var icon in contents)
            {
                GameObject iconObj = Instantiate(packIconDisplayPrefab, packContentsContainer);
                
                // Set up display (simplified)
                TextMeshProUGUI iconText = iconObj.GetComponentInChildren<TextMeshProUGUI>();
                if (iconText != null)
                {
                    iconText.text = icon.id;
                }
            }
        }
    }

    /// <summary>
    /// Closes the pack opening panel.
    /// </summary>
    private void ClosePackOpeningPanel()
    {
        if (packOpeningPanel != null)
        {
            packOpeningPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Called when rarity filter changes.
    /// </summary>
    private void OnRarityFilterChanged(int value)
    {
        PopulateIconsPanel();
    }

    // Event handlers

    private void OnBalanceChanged(int newBalance)
    {
        UpdateCoinBalance();
        PopulateIconsPanel(); // Refresh to update affordability states
    }

    private void OnIconPurchased(string iconId)
    {
        if (IconDatabase.Instance != null)
        {
            var icon = IconDatabase.Instance.GetIconById(iconId);
            if (icon != null)
            {
                ShowResultPanel("Achat réussi !", $"Vous avez acheté: {icon.displayName}");
            }
        }
        RefreshUI();
    }

    private void OnPackPurchased(IconPack pack, List<IconEntry> contents)
    {
        ShowPackOpeningPanel(contents);
        RefreshUI();
    }

    private void OnPurchaseFailed()
    {
        ShowResultPanel("Achat échoué", "Pas assez de pièces !");
    }
}
