using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controller for the welcome screen that allows players to manage game slots.
/// Supports up to 3 game slots with create, rename, play, and delete functionality.
/// </summary>
public class WelcomeScreenController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private GameSlotUI[] slotUIs;
    
    [Header("Delete Confirmation")]
    [SerializeField] private GameObject deleteConfirmationPanel;
    [SerializeField] private TextMeshProUGUI deleteConfirmationText;
    [SerializeField] private Button confirmDeleteButton;
    [SerializeField] private Button cancelDeleteButton;

    [Header("Events")]
    [SerializeField] private MenuManager menuManager;

    private int pendingDeleteSlotIndex = -1;

    /// <summary>
    /// Initializes the controller with UI references (for runtime creation).
    /// </summary>
    public void InitializeReferences(TextMeshProUGUI title, GameSlotUI[] slots, 
        GameObject deletePanel, TextMeshProUGUI deleteText, 
        Button confirmBtn, Button cancelBtn, MenuManager menu)
    {
        titleText = title;
        slotUIs = slots;
        deleteConfirmationPanel = deletePanel;
        deleteConfirmationText = deleteText;
        confirmDeleteButton = confirmBtn;
        cancelDeleteButton = cancelBtn;
        menuManager = menu;
    }

    private void OnEnable()
    {
        RefreshSlots();
        
        if (GameSlotsManager.Instance != null)
        {
            GameSlotsManager.Instance.OnSlotsUpdated += RefreshSlots;
        }
    }

    private void OnDisable()
    {
        if (GameSlotsManager.Instance != null)
        {
            GameSlotsManager.Instance.OnSlotsUpdated -= RefreshSlots;
        }
    }

    private void Start()
    {
        // Initialize slot UIs
        for (int i = 0; i < slotUIs.Length; i++)
        {
            if (slotUIs[i] != null)
            {
                slotUIs[i].Initialize(this, i);
            }
        }

        // Set up delete confirmation buttons
        if (confirmDeleteButton != null)
        {
            confirmDeleteButton.onClick.RemoveAllListeners();
            confirmDeleteButton.onClick.AddListener(ConfirmDelete);
        }

        if (cancelDeleteButton != null)
        {
            cancelDeleteButton.onClick.RemoveAllListeners();
            cancelDeleteButton.onClick.AddListener(CancelDelete);
        }

        // Hide delete confirmation panel initially
        if (deleteConfirmationPanel != null)
        {
            deleteConfirmationPanel.SetActive(false);
        }

        // Set title
        if (titleText != null)
        {
            titleText.text = "Select Game";
        }

        RefreshSlots();
    }

    /// <summary>
    /// Refreshes the display of all slots.
    /// </summary>
    public void RefreshSlots()
    {
        if (GameSlotsManager.Instance == null) return;

        var slots = GameSlotsManager.Instance.GetAllSlots();
        
        for (int i = 0; i < slotUIs.Length && i < slots.Count; i++)
        {
            if (slotUIs[i] != null)
            {
                slotUIs[i].UpdateDisplay(slots[i]);
            }
        }
    }

    /// <summary>
    /// Creates a new game in the specified slot.
    /// </summary>
    /// <param name="slotIndex">The slot index.</param>
    /// <param name="slotName">The name for the new game.</param>
    public void CreateNewSlot(int slotIndex, string slotName)
    {
        if (GameSlotsManager.Instance == null) return;

        if (GameSlotsManager.Instance.CreateNewSlot(slotIndex, slotName))
        {
            RefreshSlots();
        }
    }

    /// <summary>
    /// Renames a slot.
    /// </summary>
    /// <param name="slotIndex">The slot index.</param>
    /// <param name="newName">The new name.</param>
    public void RenameSlot(int slotIndex, string newName)
    {
        if (GameSlotsManager.Instance == null) return;

        if (GameSlotsManager.Instance.RenameSlot(slotIndex, newName))
        {
            RefreshSlots();
        }
    }

    /// <summary>
    /// Selects a slot and starts the game.
    /// </summary>
    /// <param name="slotIndex">The slot index to select.</param>
    public void SelectSlot(int slotIndex)
    {
        if (GameSlotsManager.Instance == null) return;

        if (GameSlotsManager.Instance.SelectSlot(slotIndex))
        {
            // Reload game data for the selected slot
            ReloadGameData();
            
            // Hide welcome screen and show game
            if (menuManager != null)
            {
                menuManager.HideWelcomeScreen();
            }
        }
    }

    /// <summary>
    /// Shows the delete confirmation dialog.
    /// </summary>
    /// <param name="slotIndex">The slot index to potentially delete.</param>
    public void ShowDeleteConfirmation(int slotIndex)
    {
        pendingDeleteSlotIndex = slotIndex;
        
        if (deleteConfirmationPanel != null)
        {
            deleteConfirmationPanel.SetActive(true);
        }

        if (deleteConfirmationText != null)
        {
            var slot = GameSlotsManager.Instance?.GetSlot(slotIndex);
            string slotName = slot?.slotName ?? $"Game {slotIndex + 1}";
            deleteConfirmationText.text = $"Delete \"{slotName}\"?\n\nThis cannot be undone.";
        }
    }

    /// <summary>
    /// Confirms the pending delete operation.
    /// </summary>
    public void ConfirmDelete()
    {
        if (pendingDeleteSlotIndex >= 0 && GameSlotsManager.Instance != null)
        {
            GameSlotsManager.Instance.DeleteSlot(pendingDeleteSlotIndex);
            RefreshSlots();
        }

        CancelDelete();
    }

    /// <summary>
    /// Cancels the delete operation and hides the confirmation panel.
    /// </summary>
    public void CancelDelete()
    {
        pendingDeleteSlotIndex = -1;
        
        if (deleteConfirmationPanel != null)
        {
            deleteConfirmationPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Reloads game data for the current slot.
    /// </summary>
    private void ReloadGameData()
    {
        // Reload all managers' data for the new slot
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.Load();
        }

        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.Load();
        }

        if (UnlockedIconsManager.Instance != null)
        {
            UnlockedIconsManager.Instance.Load();
        }
    }
}
