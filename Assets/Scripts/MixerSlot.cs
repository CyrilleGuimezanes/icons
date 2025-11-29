using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Represents a single slot in the 3x3 mixer grid.
/// Supports drag and drop of icons from inventory.
/// </summary>
public class MixerSlot : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    [Header("UI References")]
    [SerializeField] private Image slotBackground;
    [SerializeField] private TextMeshProUGUI iconText;
    [SerializeField] private TextMeshProUGUI quantityText;
    
    [Header("Visual Settings")]
    [SerializeField] private Color emptyColor = new Color(0.9f, 0.9f, 0.9f);
    [SerializeField] private Color filledColor = new Color(1f, 1f, 1f);
    [SerializeField] private Color highlightColor = new Color(0.8f, 0.95f, 1f);
    
    private MixerController mixerController;
    private string currentIconId;
    
    /// <summary>
    /// The current icon ID in this slot, or null/empty if slot is empty.
    /// </summary>
    public string CurrentIconId => currentIconId;
    
    private void Awake()
    {
        // Find the mixer controller in parent hierarchy
        mixerController = GetComponentInParent<MixerController>();
    }
    
    private void Start()
    {
        ClearSlot();
    }
    
    /// <summary>
    /// Sets the icon displayed in this slot.
    /// </summary>
    /// <param name="iconId">The icon ID to display</param>
    public void SetIcon(string iconId)
    {
        currentIconId = iconId;
        UpdateVisuals();
    }
    
    /// <summary>
    /// Clears the slot, removing any icon.
    /// </summary>
    public void ClearSlot()
    {
        currentIconId = null;
        UpdateVisuals();
    }
    
    private void UpdateVisuals()
    {
        bool hasIcon = !string.IsNullOrEmpty(currentIconId);
        
        if (slotBackground != null)
        {
            slotBackground.color = hasIcon ? filledColor : emptyColor;
        }
        
        if (iconText != null)
        {
            iconText.text = hasIcon ? currentIconId : "";
            iconText.gameObject.SetActive(hasIcon);
        }
        
        if (quantityText != null)
        {
            // Show quantity based on multiplier - cache the multiplier value
            int multiplier = mixerController != null ? mixerController.CurrentMultiplier : 1;
            if (hasIcon && multiplier > 1)
            {
                quantityText.text = $"x{multiplier}";
                quantityText.gameObject.SetActive(true);
            }
            else
            {
                quantityText.gameObject.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// Called when a draggable item is dropped on this slot.
    /// </summary>
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            var draggedIcon = eventData.pointerDrag.GetComponent<InventoryIconDisplay>();
            
            if (draggedIcon != null && string.IsNullOrEmpty(currentIconId))
            {
                // Accept the drop - place the icon in this slot
                SetIcon(draggedIcon.IconId);
                draggedIcon.OnPlacedInMixer();
            }
        }
    }
    
    /// <summary>
    /// Called when the slot is clicked.
    /// If the slot has an icon, returns it to inventory.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(currentIconId))
        {
            string iconToReturn = currentIconId;
            ClearSlot();
            
            if (mixerController != null)
            {
                mixerController.ReturnIconToInventory(iconToReturn);
            }
        }
    }
    
    /// <summary>
    /// Sets the highlight state of this slot (for drag hover feedback).
    /// </summary>
    /// <param name="highlight">Whether to highlight the slot</param>
    public void SetHighlight(bool highlight)
    {
        if (slotBackground != null && string.IsNullOrEmpty(currentIconId))
        {
            slotBackground.color = highlight ? highlightColor : emptyColor;
        }
    }
}
