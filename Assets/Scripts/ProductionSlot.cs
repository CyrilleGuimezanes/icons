using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Represents a single production slot in the production screen.
/// Handles displaying production state, progress, and user interactions.
/// </summary>
public class ProductionSlot : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    [Header("UI References")]
    [SerializeField] private Image slotBackground;
    [SerializeField] private TextMeshProUGUI iconText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Image progressBar;
    [SerializeField] private GameObject placeholderContainer;
    [SerializeField] private TextMeshProUGUI placeholderText;
    [SerializeField] private Button cancelButton;

    [Header("Visual Settings")]
    [SerializeField] private Color emptyColor = new Color(0.9f, 0.9f, 0.9f);
    [SerializeField] private Color readyColor = new Color(0.8f, 0.95f, 0.8f);
    [SerializeField] private Color inProgressColor = new Color(0.8f, 0.9f, 1f);
    [SerializeField] private Color completeColor = new Color(0.6f, 1f, 0.6f);

    private ProductionController productionController;
    private int slotIndex;
    private string selectedProductionId;
    private bool hasPlaceholder;
    private ActiveProduction activeProduction;

    /// <summary>
    /// The slot index (0-4).
    /// </summary>
    public int SlotIndex => slotIndex;

    /// <summary>
    /// Whether this slot has an active production.
    /// </summary>
    public bool HasActiveProduction => activeProduction != null;

    /// <summary>
    /// Whether this slot has a selected production placeholder.
    /// </summary>
    public bool HasPlaceholder => hasPlaceholder;

    /// <summary>
    /// The selected production ID (for placeholder state).
    /// </summary>
    public string SelectedProductionId => selectedProductionId;

    private void Awake()
    {
        productionController = GetComponentInParent<ProductionController>();
    }

    /// <summary>
    /// Initializes UI references for runtime building.
    /// </summary>
    public void InitializeReferences(TextMeshProUGUI icon, TextMeshProUGUI status, Image progress, TextMeshProUGUI timeText, Button slotButton)
    {
        iconText = icon;
        statusText = status;
        progressBar = progress;
        // Note: slotButton is used for interaction but stored elsewhere
    }

    private void Start()
    {
        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(OnCancelClicked);
        }

        ClearSlot();
    }

    // Timer for throttled UI updates
    private float lastUIUpdateTime;
    private const float UI_UPDATE_INTERVAL = 0.25f; // Update 4 times per second

    private void Update()
    {
        if (activeProduction != null)
        {
            // Throttle UI updates to reduce overhead
            if (Time.time - lastUIUpdateTime >= UI_UPDATE_INTERVAL)
            {
                lastUIUpdateTime = Time.time;
                UpdateProgress();
            }
        }
    }

    /// <summary>
    /// Initializes the slot with its index.
    /// </summary>
    /// <param name="index">The slot index (0-4)</param>
    public void Initialize(int index)
    {
        slotIndex = index;
        ClearSlot();
    }

    /// <summary>
    /// Clears the slot to its empty state.
    /// </summary>
    public void ClearSlot()
    {
        activeProduction = null;
        selectedProductionId = null;
        hasPlaceholder = false;

        UpdateVisuals();
    }

    /// <summary>
    /// Sets a production placeholder (recipe selected but not started).
    /// </summary>
    /// <param name="productionId">The production recipe ID</param>
    public void SetPlaceholder(string productionId)
    {
        if (string.IsNullOrEmpty(productionId))
        {
            ClearSlot();
            return;
        }

        var production = ProductionManager.Instance?.GetProduction(productionId);
        if (production == null)
        {
            return;
        }

        selectedProductionId = productionId;
        hasPlaceholder = true;
        activeProduction = null;

        UpdateVisuals();
    }

    /// <summary>
    /// Sets an active production for this slot.
    /// </summary>
    /// <param name="production">The active production</param>
    public void SetActiveProduction(ActiveProduction production)
    {
        activeProduction = production;
        hasPlaceholder = false;

        if (production != null)
        {
            selectedProductionId = production.productionId;
        }

        UpdateVisuals();
    }

    /// <summary>
    /// Updates the visual state of the slot.
    /// </summary>
    private void UpdateVisuals()
    {
        bool isEmpty = !hasPlaceholder && activeProduction == null;
        bool isPlaceholder = hasPlaceholder && activeProduction == null;
        bool isInProgress = activeProduction != null && !activeProduction.IsComplete();
        bool isComplete = activeProduction != null && activeProduction.IsComplete();

        // Background color
        if (slotBackground != null)
        {
            if (isEmpty)
            {
                slotBackground.color = emptyColor;
            }
            else if (isPlaceholder)
            {
                slotBackground.color = readyColor;
            }
            else if (isComplete)
            {
                slotBackground.color = completeColor;
            }
            else
            {
                slotBackground.color = inProgressColor;
            }
        }

        // Icon text
        if (iconText != null)
        {
            if (isEmpty)
            {
                iconText.text = "+";
                iconText.gameObject.SetActive(true);
            }
            else
            {
                var production = ProductionManager.Instance?.GetProduction(selectedProductionId);
                iconText.text = production?.result ?? "?";
                iconText.gameObject.SetActive(true);
            }
        }

        // Status text
        if (statusText != null)
        {
            if (isEmpty)
            {
                statusText.text = "Appuyez pour sélectionner";
                statusText.gameObject.SetActive(true);
            }
            else if (isPlaceholder)
            {
                var production = ProductionManager.Instance?.GetProduction(selectedProductionId);
                if (production != null)
                {
                    // Show required ingredients
                    string ingredientsList = string.Join(", ", production.ingredients);
                    statusText.text = $"Besoin: {ingredientsList}";
                }
                else
                {
                    statusText.text = "Déposez les ingrédients";
                }
                statusText.gameObject.SetActive(true);
            }
            else if (isInProgress)
            {
                statusText.text = FormatRemainingTime(activeProduction.GetRemainingTime());
                statusText.gameObject.SetActive(true);
            }
            else if (isComplete)
            {
                statusText.text = "Terminé !";
                statusText.gameObject.SetActive(true);
            }
        }

        // Progress bar
        if (progressBar != null)
        {
            if (isInProgress)
            {
                progressBar.gameObject.SetActive(true);
                progressBar.fillAmount = activeProduction.GetProgress();
            }
            else
            {
                progressBar.gameObject.SetActive(false);
            }
        }

        // Placeholder container
        if (placeholderContainer != null)
        {
            if (isPlaceholder)
            {
                placeholderContainer.SetActive(true);
                if (placeholderText != null)
                {
                    var production = ProductionManager.Instance?.GetProduction(selectedProductionId);
                    placeholderText.text = production?.displayName ?? selectedProductionId;
                }
            }
            else
            {
                placeholderContainer.SetActive(false);
            }
        }

        // Cancel button
        if (cancelButton != null)
        {
            cancelButton.gameObject.SetActive(isPlaceholder || isInProgress);
        }
    }

    /// <summary>
    /// Updates the progress display.
    /// </summary>
    private void UpdateProgress()
    {
        if (progressBar != null && activeProduction != null)
        {
            progressBar.fillAmount = activeProduction.GetProgress();
        }

        if (statusText != null && activeProduction != null)
        {
            if (activeProduction.IsComplete())
            {
                statusText.text = "Terminé !";
            }
            else
            {
                statusText.text = FormatRemainingTime(activeProduction.GetRemainingTime());
            }
        }

        // Update colors when production completes
        if (activeProduction != null && activeProduction.IsComplete())
        {
            if (slotBackground != null)
            {
                slotBackground.color = completeColor;
            }
        }
    }

    /// <summary>
    /// Formats remaining time as a readable string.
    /// </summary>
    private string FormatRemainingTime(float seconds)
    {
        if (seconds <= 0)
        {
            return "0s";
        }

        int minutes = Mathf.FloorToInt(seconds / 60);
        int secs = Mathf.FloorToInt(seconds % 60);

        if (minutes > 0)
        {
            return $"{minutes}m {secs}s";
        }
        else
        {
            return $"{secs}s";
        }
    }

    /// <summary>
    /// Called when an item is dropped on this slot.
    /// </summary>
    public void OnDrop(PointerEventData eventData)
    {
        if (activeProduction != null)
        {
            return; // Can't drop on active production
        }

        if (!hasPlaceholder)
        {
            return; // Need a placeholder (selected production) first
        }

        var production = ProductionManager.Instance?.GetProduction(selectedProductionId);
        if (production == null)
        {
            return;
        }

        // Check if the dropped item is one of the required ingredients
        var draggedIcon = eventData.pointerDrag?.GetComponent<ProductionIconDisplay>();
        if (draggedIcon == null)
        {
            return;
        }

        // Check if this ingredient is needed
        if (!production.ingredients.Contains(draggedIcon.IconId))
        {
            return;
        }

        // Try to start production through the controller
        if (productionController != null)
        {
            productionController.TryStartProduction(slotIndex);
        }
    }

    /// <summary>
    /// Called when the slot is clicked.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (activeProduction != null)
        {
            // If production is complete, it should be collected by ProductionManager
            // The ProductionManager handles adding items to inventory
            // We don't manually clear here - let RefreshProductionSlots handle sync
            return;
        }

        if (hasPlaceholder)
        {
            // Show option to start or cancel
            return;
        }

        // Empty slot - show production selection
        if (productionController != null)
        {
            productionController.ShowProductionSelection(slotIndex);
        }
    }

    /// <summary>
    /// Called when the cancel button is clicked.
    /// </summary>
    private void OnCancelClicked()
    {
        if (activeProduction != null)
        {
            // Cancel active production
            if (productionController != null)
            {
                productionController.CancelProduction(slotIndex);
            }
        }
        else if (hasPlaceholder)
        {
            // Clear placeholder
            ClearSlot();
        }
    }
}
