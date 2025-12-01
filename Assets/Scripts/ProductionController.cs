using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Main controller for the Production (Potager/Industrie) screen.
/// Manages production slots, selection modal, and production operations.
/// </summary>
public class ProductionController : ScreenController
{
    [Header("Production Slots")]
    [SerializeField] private ProductionSlot[] productionSlots = new ProductionSlot[5];

    [Header("Inventory Panel")]
    [SerializeField] private Transform inventoryContainer;
    [SerializeField] private GameObject inventoryIconPrefab;

    [Header("Multiplier Buttons")]
    [SerializeField] private Button multiplierX1Button;
    [SerializeField] private Button multiplierX5Button;
    [SerializeField] private Button multiplierX10Button;
    [SerializeField] private Button multiplierX100Button;
    [SerializeField] private TextMeshProUGUI multiplierText;

    [Header("Selection Modal")]
    [SerializeField] private GameObject selectionModal;
    [SerializeField] private Transform selectionContainer;
    [SerializeField] private GameObject productionSelectionItemPrefab;
    [SerializeField] private Button closeSelectionButton;

    [Header("Tab Buttons")]
    [SerializeField] private Button plantTabButton;
    [SerializeField] private Button industryTabButton;
    [SerializeField] private TextMeshProUGUI plantTabText;
    [SerializeField] private TextMeshProUGUI industryTabText;

    [Header("Colors")]
    [SerializeField] private Color selectedTabColor = new Color(0.2f, 0.6f, 1f);
    [SerializeField] private Color normalTabColor = new Color(0.5f, 0.5f, 0.5f);
    [SerializeField] private Color selectedMultiplierColor = new Color(0.2f, 0.6f, 1f);
    [SerializeField] private Color normalMultiplierColor = new Color(0.5f, 0.5f, 0.5f);

    private int currentMultiplier = 1;
    private int selectedSlotIndex = -1;
    private ProductionType currentTabType = ProductionType.Plant;
    private List<ProductionIconDisplay> inventoryIcons = new List<ProductionIconDisplay>();
    private List<GameObject> selectionItems = new List<GameObject>();

    /// <summary>
    /// Initializes references for runtime UI building.
    /// </summary>
    public void InitializeReferences(ProductionSlot[] slots, Transform invContainer, GameObject invIconPrefab,
        Button x1Btn, Button x5Btn, Button x10Btn, Button x100Btn, TextMeshProUGUI multiplierTxt,
        GameObject selModal, Transform selContainer, GameObject selItemPrefab, Button closeSelBtn,
        Button plantTab, Button industryTab, TextMeshProUGUI plantTabTxt, TextMeshProUGUI industryTabTxt,
        TextMeshProUGUI title)
    {
        productionSlots = slots;
        inventoryContainer = invContainer;
        inventoryIconPrefab = invIconPrefab;
        multiplierX1Button = x1Btn;
        multiplierX5Button = x5Btn;
        multiplierX10Button = x10Btn;
        multiplierX100Button = x100Btn;
        multiplierText = multiplierTxt;
        selectionModal = selModal;
        selectionContainer = selContainer;
        productionSelectionItemPrefab = selItemPrefab;
        closeSelectionButton = closeSelBtn;
        plantTabButton = plantTab;
        industryTabButton = industryTab;
        plantTabText = plantTabTxt;
        industryTabText = industryTabTxt;
        titleText = title;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        RefreshInventoryDisplay();
        RefreshProductionSlots();

        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnInventoryChanged += RefreshInventoryDisplay;
        }

        if (ProductionManager.Instance != null)
        {
            ProductionManager.Instance.OnProductionsChanged += RefreshProductionSlots;
        }
    }

    private void OnDisable()
    {
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnInventoryChanged -= RefreshInventoryDisplay;
        }

        if (ProductionManager.Instance != null)
        {
            ProductionManager.Instance.OnProductionsChanged -= RefreshProductionSlots;
        }
    }

    private void Start()
    {
        InitializeSlots();
        SetupMultiplierButtons();
        SetupTabButtons();
        SetupSelectionModal();
        SelectMultiplier(1);
        HideSelectionModal();
    }

    /// <summary>
    /// Initializes production slots with their indices.
    /// </summary>
    private void InitializeSlots()
    {
        for (int i = 0; i < productionSlots.Length; i++)
        {
            if (productionSlots[i] != null)
            {
                productionSlots[i].Initialize(i);
            }
        }
    }

    private void SetupMultiplierButtons()
    {
        if (multiplierX1Button != null)
            multiplierX1Button.onClick.AddListener(() => SelectMultiplier(1));
        if (multiplierX5Button != null)
            multiplierX5Button.onClick.AddListener(() => SelectMultiplier(5));
        if (multiplierX10Button != null)
            multiplierX10Button.onClick.AddListener(() => SelectMultiplier(10));
        if (multiplierX100Button != null)
            multiplierX100Button.onClick.AddListener(() => SelectMultiplier(100));
    }

    private void SetupTabButtons()
    {
        if (plantTabButton != null)
            plantTabButton.onClick.AddListener(() => SelectTab(ProductionType.Plant));
        if (industryTabButton != null)
            industryTabButton.onClick.AddListener(() => SelectTab(ProductionType.ManufacturedGood));
    }

    private void SetupSelectionModal()
    {
        if (closeSelectionButton != null)
        {
            closeSelectionButton.onClick.AddListener(HideSelectionModal);
        }
    }

    /// <summary>
    /// Sets the current multiplier for production operations.
    /// </summary>
    /// <param name="multiplier">The multiplier value (1, 5, 10, or 100)</param>
    public void SelectMultiplier(int multiplier)
    {
        currentMultiplier = multiplier;
        UpdateMultiplierButtonVisuals();

        if (multiplierText != null)
        {
            multiplierText.text = multiplier > 1 ? $"x{multiplier}" : "x1";
        }
    }

    private void UpdateMultiplierButtonVisuals()
    {
        UpdateButtonColor(multiplierX1Button, currentMultiplier == 1);
        UpdateButtonColor(multiplierX5Button, currentMultiplier == 5);
        UpdateButtonColor(multiplierX10Button, currentMultiplier == 10);
        UpdateButtonColor(multiplierX100Button, currentMultiplier == 100);
    }

    private void UpdateButtonColor(Button button, bool isSelected)
    {
        if (button == null) return;

        var buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.color = isSelected ? selectedMultiplierColor : normalMultiplierColor;
        }

        var buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = isSelected ? selectedMultiplierColor : Color.white;
        }
    }

    /// <summary>
    /// Selects a production type tab (Plant or Industry).
    /// </summary>
    /// <param name="type">The production type to show</param>
    public void SelectTab(ProductionType type)
    {
        currentTabType = type;
        UpdateTabVisuals();
        
        if (selectionModal != null && selectionModal.activeSelf)
        {
            RefreshSelectionList();
        }
    }

    private void UpdateTabVisuals()
    {
        if (plantTabText != null)
        {
            plantTabText.color = currentTabType == ProductionType.Plant ? selectedTabColor : normalTabColor;
        }

        if (industryTabText != null)
        {
            industryTabText.color = currentTabType == ProductionType.ManufacturedGood ? selectedTabColor : normalTabColor;
        }
    }

    /// <summary>
    /// Refreshes the inventory display to show current player items.
    /// </summary>
    public void RefreshInventoryDisplay()
    {
        if (inventoryContainer == null || inventoryIconPrefab == null) return;

        // Clear existing icons
        foreach (var icon in inventoryIcons)
        {
            if (icon != null && icon.gameObject != null)
            {
                Destroy(icon.gameObject);
            }
        }
        inventoryIcons.Clear();

        // Get all items from player inventory
        if (PlayerInventory.Instance == null) return;

        List<InventoryItem> items = PlayerInventory.Instance.GetAllItems();

        foreach (var item in items)
        {
            if (item.quantity > 0)
            {
                GameObject iconObj = Instantiate(inventoryIconPrefab, inventoryContainer);
                ProductionIconDisplay iconDisplay = iconObj.GetComponent<ProductionIconDisplay>();

                if (iconDisplay != null)
                {
                    iconDisplay.Setup(item.iconId, item.quantity, this);
                    inventoryIcons.Add(iconDisplay);
                }
            }
        }
    }

    /// <summary>
    /// Refreshes all production slots to match current state.
    /// </summary>
    public void RefreshProductionSlots()
    {
        if (ProductionManager.Instance == null) return;

        foreach (var slot in productionSlots)
        {
            if (slot == null) continue;

            var activeProduction = ProductionManager.Instance.GetActiveProduction(slot.SlotIndex);
            if (activeProduction != null)
            {
                slot.SetActiveProduction(activeProduction);
            }
            else if (!slot.HasPlaceholder)
            {
                slot.ClearSlot();
            }
        }
    }

    /// <summary>
    /// Shows the production selection modal for a specific slot.
    /// </summary>
    /// <param name="slotIndex">The slot index to select production for</param>
    public void ShowProductionSelection(int slotIndex)
    {
        selectedSlotIndex = slotIndex;

        if (selectionModal != null)
        {
            selectionModal.SetActive(true);
            RefreshSelectionList();
        }
    }

    /// <summary>
    /// Hides the production selection modal.
    /// </summary>
    public void HideSelectionModal()
    {
        selectedSlotIndex = -1;

        if (selectionModal != null)
        {
            selectionModal.SetActive(false);
        }
    }

    /// <summary>
    /// Refreshes the production selection list based on current tab.
    /// </summary>
    private void RefreshSelectionList()
    {
        if (selectionContainer == null || productionSelectionItemPrefab == null) return;

        // Clear existing items
        foreach (var item in selectionItems)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
        selectionItems.Clear();

        // Get discovered productions for current tab
        if (ProductionManager.Instance == null) return;

        List<ProductionData> productions = ProductionManager.Instance.GetDiscoveredProductionsByType(currentTabType);

        foreach (var production in productions)
        {
            GameObject itemObj = Instantiate(productionSelectionItemPrefab, selectionContainer);
            selectionItems.Add(itemObj);

            // Setup the selection item
            var itemButton = itemObj.GetComponent<Button>();
            if (itemButton != null)
            {
                string productionId = production.id;
                itemButton.onClick.AddListener(() => OnProductionSelected(productionId));
            }

            // Set display text
            var nameText = itemObj.GetComponentInChildren<TextMeshProUGUI>();
            if (nameText != null)
            {
                nameText.text = $"{production.displayName} ({production.baseProductionTime}s)";
            }
        }

        if (productions.Count == 0)
        {
            // Show "no productions available" message
            GameObject emptyMessage = new GameObject("EmptyMessage");
            emptyMessage.transform.SetParent(selectionContainer, false);
            var text = emptyMessage.AddComponent<TextMeshProUGUI>();
            text.text = "Aucune production disponible";
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 16;
            selectionItems.Add(emptyMessage);
        }
    }

    /// <summary>
    /// Called when a production is selected from the modal.
    /// </summary>
    /// <param name="productionId">The selected production ID</param>
    private void OnProductionSelected(string productionId)
    {
        // Validate slot index before any operations
        if (selectedSlotIndex < 0 || selectedSlotIndex >= productionSlots.Length)
        {
            selectedSlotIndex = -1;
            HideSelectionModal();
            return;
        }

        var slot = productionSlots[selectedSlotIndex];
        if (slot == null)
        {
            selectedSlotIndex = -1;
            HideSelectionModal();
            return;
        }

        slot.SetPlaceholder(productionId);
        HideSelectionModal();
    }

    /// <summary>
    /// Tries to start production in the specified slot.
    /// </summary>
    /// <param name="slotIndex">The slot index</param>
    /// <returns>True if production started successfully</returns>
    public bool TryStartProduction(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= productionSlots.Length)
        {
            return false;
        }

        var slot = productionSlots[slotIndex];
        if (slot == null || !slot.HasPlaceholder)
        {
            return false;
        }

        if (ProductionManager.Instance == null)
        {
            return false;
        }

        bool success = ProductionManager.Instance.StartProduction(
            slotIndex,
            slot.SelectedProductionId,
            currentMultiplier
        );

        if (success)
        {
            RefreshProductionSlots();
            RefreshInventoryDisplay();
        }

        return success;
    }

    /// <summary>
    /// Cancels a production in the specified slot.
    /// </summary>
    /// <param name="slotIndex">The slot index</param>
    public void CancelProduction(int slotIndex)
    {
        if (ProductionManager.Instance != null)
        {
            ProductionManager.Instance.CancelProduction(slotIndex);
        }

        if (slotIndex >= 0 && slotIndex < productionSlots.Length)
        {
            productionSlots[slotIndex]?.ClearSlot();
        }

        RefreshInventoryDisplay();
    }

    /// <summary>
    /// Gets the current multiplier value.
    /// </summary>
    public int CurrentMultiplier => currentMultiplier;
}
