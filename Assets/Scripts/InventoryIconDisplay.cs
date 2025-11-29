using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Displays an icon from the player's inventory with quantity.
/// Supports drag and drop to mixer slots.
/// </summary>
public class InventoryIconDisplay : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("UI References")]
    [SerializeField] private Image iconBackground;
    [SerializeField] private TextMeshProUGUI iconText;
    [SerializeField] private TextMeshProUGUI quantityText;
    
    [Header("Visual Settings")]
    [SerializeField] private Color normalColor = new Color(1f, 1f, 1f);
    [SerializeField] private Color dragColor = new Color(0.8f, 0.8f, 0.8f, 0.8f);
    
    private string iconId;
    private int quantity;
    private MixerController mixerController;
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Transform originalParent;
    private bool isDragging;
    
    /// <summary>
    /// The icon ID this display represents.
    /// </summary>
    public string IconId => iconId;
    
    /// <summary>
    /// The quantity of this icon owned.
    /// </summary>
    public int Quantity => quantity;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        canvas = GetComponentInParent<Canvas>();
    }
    
    /// <summary>
    /// Sets up the icon display with the given data.
    /// </summary>
    /// <param name="id">The icon ID to display</param>
    /// <param name="qty">The quantity owned</param>
    /// <param name="controller">Reference to the mixer controller</param>
    public void Setup(string id, int qty, MixerController controller)
    {
        iconId = id;
        quantity = qty;
        mixerController = controller;
        
        UpdateVisuals();
    }
    
    /// <summary>
    /// Updates the quantity displayed.
    /// </summary>
    /// <param name="newQuantity">The new quantity</param>
    public void UpdateQuantity(int newQuantity)
    {
        quantity = newQuantity;
        UpdateVisuals();
    }
    
    private void UpdateVisuals()
    {
        if (iconText != null)
        {
            iconText.text = iconId;
        }
        
        if (quantityText != null)
        {
            quantityText.text = quantity.ToString();
            quantityText.gameObject.SetActive(quantity > 0);
        }
        
        if (iconBackground != null)
        {
            iconBackground.color = normalColor;
        }
    }
    
    /// <summary>
    /// Called when the player starts dragging this icon.
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Check if we have enough quantity for the current multiplier
        if (mixerController != null && quantity < mixerController.CurrentMultiplier)
        {
            eventData.pointerDrag = null;
            return;
        }
        
        isDragging = true;
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        
        // Move to front
        transform.SetAsLastSibling();
        
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0.8f;
        }
        
        if (iconBackground != null)
        {
            iconBackground.color = dragColor;
        }
    }
    
    /// <summary>
    /// Called while the player is dragging this icon.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (canvas != null && rectTransform != null)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }
    
    /// <summary>
    /// Called when the player stops dragging this icon.
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;
        }
        
        if (iconBackground != null)
        {
            iconBackground.color = normalColor;
        }
        
        // Return to original position if not dropped on a valid target
        rectTransform.anchoredPosition = originalPosition;
    }
    
    /// <summary>
    /// Called when this icon is successfully placed in a mixer slot.
    /// </summary>
    public void OnPlacedInMixer()
    {
        // Return to original position in inventory
        rectTransform.anchoredPosition = originalPosition;
        
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;
        }
        
        if (iconBackground != null)
        {
            iconBackground.color = normalColor;
        }
        
        // Refresh the inventory display
        if (mixerController != null)
        {
            mixerController.RefreshInventoryDisplay();
        }
    }
    
    /// <summary>
    /// Called when the icon is clicked (tap to add to mixer).
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // Ignore if we were dragging
        if (isDragging) return;
        
        // Try to add to first available mixer slot
        if (mixerController != null)
        {
            mixerController.AddIconToMixer(iconId);
        }
    }
}
