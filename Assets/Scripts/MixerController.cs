using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Main controller for the Mixer (Mélangeur) screen.
/// Manages the 3x3 mixing grid, inventory display, and mix operations.
/// </summary>
public class MixerController : ScreenController
{
    [Header("Mixer Grid")]
    [SerializeField] private MixerSlot[] mixerSlots = new MixerSlot[9];
    
    [Header("Inventory Panel")]
    [SerializeField] private Transform inventoryContainer;
    [SerializeField] private GameObject inventoryIconPrefab;
    
    [Header("Multiplier Buttons")]
    [SerializeField] private Button multiplierX1Button;
    [SerializeField] private Button multiplierX5Button;
    [SerializeField] private Button multiplierX10Button;
    [SerializeField] private Button multiplierX100Button;
    
    [Header("Mix Button")]
    [SerializeField] private Button mixButton;
    [SerializeField] private TextMeshProUGUI mixButtonText;
    
    [Header("Result Animation")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI resultIconText;
    [SerializeField] private TextMeshProUGUI resultMessageText;
    
    [Header("Colors")]
    [SerializeField] private Color selectedMultiplierColor = new Color(0.2f, 0.6f, 1f);
    [SerializeField] private Color normalMultiplierColor = new Color(0.5f, 0.5f, 0.5f);
    
    private int currentMultiplier = 1;
    private List<InventoryIconDisplay> inventoryIcons = new List<InventoryIconDisplay>();

    /// <summary>
    /// Initializes references for runtime UI building.
    /// </summary>
    public void InitializeReferences(MixerSlot[] slots, Transform invContainer, GameObject invIconPrefab,
        Button x1Btn, Button x5Btn, Button x10Btn, Button x100Btn,
        Button mixBtn, TextMeshProUGUI mixBtnText,
        GameObject resultPnl, TextMeshProUGUI resultIcon, TextMeshProUGUI resultMsg,
        TextMeshProUGUI title)
    {
        mixerSlots = slots;
        inventoryContainer = invContainer;
        inventoryIconPrefab = invIconPrefab;
        multiplierX1Button = x1Btn;
        multiplierX5Button = x5Btn;
        multiplierX10Button = x10Btn;
        multiplierX100Button = x100Btn;
        mixButton = mixBtn;
        mixButtonText = mixBtnText;
        resultPanel = resultPnl;
        resultIconText = resultIcon;
        resultMessageText = resultMsg;
        titleText = title;
    }
    
    protected override void OnEnable()
    {
        base.OnEnable();
        RefreshInventoryDisplay();
        
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnInventoryChanged += RefreshInventoryDisplay;
        }
    }
    
    private void OnDisable()
    {
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnInventoryChanged -= RefreshInventoryDisplay;
        }
    }
    
    private void Start()
    {
        SetupMultiplierButtons();
        SetupMixButton();
        SelectMultiplier(1);
        HideResultPanel();
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
    
    private void SetupMixButton()
    {
        if (mixButton != null)
        {
            mixButton.onClick.AddListener(OnMixButtonClicked);
        }
    }
    
    /// <summary>
    /// Sets the current multiplier for mix operations.
    /// </summary>
    /// <param name="multiplier">The multiplier value (1, 5, 10, or 100)</param>
    public void SelectMultiplier(int multiplier)
    {
        currentMultiplier = multiplier;
        UpdateMultiplierButtonVisuals();
        
        if (mixButtonText != null)
        {
            mixButtonText.text = multiplier > 1 ? $"Fusionner x{multiplier}" : "Fusionner";
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
                InventoryIconDisplay iconDisplay = iconObj.GetComponent<InventoryIconDisplay>();
                
                if (iconDisplay != null)
                {
                    iconDisplay.Setup(item.iconId, item.quantity, this);
                    inventoryIcons.Add(iconDisplay);
                }
            }
        }
    }
    
    /// <summary>
    /// Called when the mix button is clicked.
    /// Attempts to perform the mix operation.
    /// </summary>
    public void OnMixButtonClicked()
    {
        List<string> slotContents = GetMixerSlotContents();
        
        if (slotContents.Count == 0)
        {
            ShowFailureResult("Ajoutez des icônes !");
            return;
        }
        
        // Try to find a matching recipe
        string result = TryMix(slotContents);
        
        if (!string.IsNullOrEmpty(result))
        {
            // Successful mix - first verify we can add the result, then consume ingredients
            if (PlayerInventory.Instance != null)
            {
                // Add result first to ensure the operation can succeed
                PlayerInventory.Instance.AddIcon(result, currentMultiplier);
                // Then consume the ingredients
                ConsumeIngredientsFromSlots();
                ShowSuccessResult(result);
            }
            else
            {
                ShowFailureResult("Erreur d'inventaire !");
            }
        }
        else
        {
            // Failed mix - show humorous failure
            ShowFailureResult("Combinaison inconnue !");
        }
        
        ClearMixerSlots();
        RefreshInventoryDisplay();
    }
    
    private List<string> GetMixerSlotContents()
    {
        List<string> contents = new List<string>();
        
        foreach (var slot in mixerSlots)
        {
            if (slot != null && !string.IsNullOrEmpty(slot.CurrentIconId))
            {
                contents.Add(slot.CurrentIconId);
            }
        }
        
        return contents;
    }
    
    /// <summary>
    /// Tries to find a matching recipe for the given ingredients.
    /// This is a placeholder - actual recipes will be implemented later.
    /// </summary>
    /// <param name="ingredients">List of icon IDs in the mixer</param>
    /// <returns>The result icon ID if a recipe matches, null otherwise</returns>
    private string TryMix(List<string> ingredients)
    {
        // Placeholder: Recipe system will be implemented later
        // For now, return null (no valid combinations)
        return null;
    }
    
    private void ConsumeIngredientsFromSlots()
    {
        foreach (var slot in mixerSlots)
        {
            if (slot != null && !string.IsNullOrEmpty(slot.CurrentIconId))
            {
                PlayerInventory.Instance?.RemoveIcon(slot.CurrentIconId, currentMultiplier);
            }
        }
    }
    
    private void ClearMixerSlots()
    {
        foreach (var slot in mixerSlots)
        {
            if (slot != null)
            {
                slot.ClearSlot();
            }
        }
    }
    
    private void ShowSuccessResult(string iconId)
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
            
            if (resultIconText != null)
            {
                resultIconText.text = iconId;
            }
            
            if (resultMessageText != null)
            {
                resultMessageText.text = currentMultiplier > 1 
                    ? $"Succès ! x{currentMultiplier}" 
                    : "Succès !";
            }
            
            // Auto-hide after delay
            Invoke(nameof(HideResultPanel), 2f);
        }
    }
    
    private void ShowFailureResult(string message)
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
            
            if (resultIconText != null)
            {
                resultIconText.text = "❌";
            }
            
            if (resultMessageText != null)
            {
                resultMessageText.text = message;
            }
            
            // Auto-hide after delay
            Invoke(nameof(HideResultPanel), 2f);
        }
    }
    
    private void HideResultPanel()
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// Adds an icon from inventory to the first available mixer slot.
    /// </summary>
    /// <param name="iconId">The icon ID to add</param>
    /// <returns>True if the icon was added successfully</returns>
    public bool AddIconToMixer(string iconId)
    {
        // Check if we have enough in inventory
        if (PlayerInventory.Instance == null || 
            !PlayerInventory.Instance.HasIcon(iconId, currentMultiplier))
        {
            return false;
        }
        
        // Find first empty slot
        foreach (var slot in mixerSlots)
        {
            if (slot != null && string.IsNullOrEmpty(slot.CurrentIconId))
            {
                slot.SetIcon(iconId);
                return true;
            }
        }
        
        return false; // No empty slots
    }
    
    /// <summary>
    /// Returns an icon from a mixer slot back to the inventory display.
    /// </summary>
    /// <param name="iconId">The icon ID to return</param>
    public void ReturnIconToInventory(string iconId)
    {
        // The icon is already in inventory, just refresh the display
        RefreshInventoryDisplay();
    }
    
    /// <summary>
    /// Gets the current multiplier value.
    /// </summary>
    public int CurrentMultiplier => currentMultiplier;
}
