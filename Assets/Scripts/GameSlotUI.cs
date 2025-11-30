using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI component for displaying a single game slot.
/// Shows slot name, stats, and action buttons.
/// </summary>
public class GameSlotUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI slotNameText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private GameObject activeContent;
    [SerializeField] private GameObject emptyContent;
    [SerializeField] private Button playButton;
    [SerializeField] private Button renameButton;
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private TMP_InputField nameInputField;

    private int slotIndex;
    private WelcomeScreenController welcomeScreen;

    /// <summary>
    /// Initializes the slot UI with the welcome screen reference.
    /// </summary>
    /// <param name="controller">The welcome screen controller.</param>
    /// <param name="index">The slot index.</param>
    public void Initialize(WelcomeScreenController controller, int index)
    {
        welcomeScreen = controller;
        slotIndex = index;

        // Set up button listeners
        if (playButton != null)
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(OnPlayClicked);
        }

        if (renameButton != null)
        {
            renameButton.onClick.RemoveAllListeners();
            renameButton.onClick.AddListener(OnRenameClicked);
        }

        if (deleteButton != null)
        {
            deleteButton.onClick.RemoveAllListeners();
            deleteButton.onClick.AddListener(OnDeleteClicked);
        }

        if (newGameButton != null)
        {
            newGameButton.onClick.RemoveAllListeners();
            newGameButton.onClick.AddListener(OnNewGameClicked);
        }

        if (nameInputField != null)
        {
            nameInputField.onEndEdit.RemoveAllListeners();
            nameInputField.onEndEdit.AddListener(OnNameInputSubmit);
            nameInputField.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Updates the slot display with the given data.
    /// </summary>
    /// <param name="slotData">The slot data to display.</param>
    public void UpdateDisplay(GameSlotData slotData)
    {
        bool isActive = slotData != null && slotData.isActive;

        if (activeContent != null)
        {
            activeContent.SetActive(isActive);
        }

        if (emptyContent != null)
        {
            emptyContent.SetActive(!isActive);
        }

        if (isActive)
        {
            if (slotNameText != null)
            {
                slotNameText.text = slotData.slotName;
            }

            if (statsText != null)
            {
                statsText.text = $"Icons: {slotData.unlockedIconsCount} | Time: {slotData.GetFormattedPlayTime()}";
            }
        }

        // Hide name input when updating
        if (nameInputField != null)
        {
            nameInputField.gameObject.SetActive(false);
        }
    }

    private void OnPlayClicked()
    {
        welcomeScreen?.SelectSlot(slotIndex);
    }

    private void OnRenameClicked()
    {
        if (nameInputField != null)
        {
            var slotData = GameSlotsManager.Instance?.GetSlot(slotIndex);
            nameInputField.text = slotData?.slotName ?? "";
            nameInputField.gameObject.SetActive(true);
            nameInputField.Select();
            nameInputField.ActivateInputField();
        }
    }

    private void OnDeleteClicked()
    {
        welcomeScreen?.ShowDeleteConfirmation(slotIndex);
    }

    private void OnNewGameClicked()
    {
        if (nameInputField != null)
        {
            nameInputField.text = $"Game {slotIndex + 1}";
            nameInputField.gameObject.SetActive(true);
            nameInputField.Select();
            nameInputField.ActivateInputField();
        }
    }

    private void OnNameInputSubmit(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            newName = $"Game {slotIndex + 1}";
        }

        var slotData = GameSlotsManager.Instance?.GetSlot(slotIndex);
        
        if (slotData != null && slotData.isActive)
        {
            // Renaming existing slot
            welcomeScreen?.RenameSlot(slotIndex, newName);
        }
        else
        {
            // Creating new slot
            welcomeScreen?.CreateNewSlot(slotIndex, newName);
        }

        if (nameInputField != null)
        {
            nameInputField.gameObject.SetActive(false);
        }
    }
}
