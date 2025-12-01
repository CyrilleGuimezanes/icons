using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controller for the Options/Settings screen.
/// Manages sound, music toggles, and game reset functionality.
/// </summary>
public class OptionsController : ScreenController
{
    [Header("Sound Settings")]
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Slider soundVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;

    [Header("Display Settings")]
    [SerializeField] private Toggle vibrationToggle;
    [SerializeField] private Toggle notificationsToggle;

    [Header("Reset Section")]
    [SerializeField] private Button resetProgressButton;
    [SerializeField] private Button resetCurrentSlotButton;

    [Header("Reset Confirmation")]
    [SerializeField] private GameObject resetConfirmationPanel;
    [SerializeField] private TextMeshProUGUI resetConfirmationText;
    [SerializeField] private Button confirmResetButton;
    [SerializeField] private Button cancelResetButton;

    [Header("Version Info")]
    [SerializeField] private TextMeshProUGUI versionText;

    [Header("References")]
    [SerializeField] private MenuManager menuManager;

    private const string SOUND_ENABLED_KEY = "SoundEnabled";
    private const string MUSIC_ENABLED_KEY = "MusicEnabled";
    private const string SOUND_VOLUME_KEY = "SoundVolume";
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string VIBRATION_ENABLED_KEY = "VibrationEnabled";
    private const string NOTIFICATIONS_ENABLED_KEY = "NotificationsEnabled";

    private bool isPendingFullReset = false;

    protected override void OnEnable()
    {
        base.OnEnable();
        LoadSettings();
        HideResetConfirmation();
    }

    private void Start()
    {
        SetupEventListeners();
        UpdateVersionDisplay();
        HideResetConfirmation();
    }

    private void SetupEventListeners()
    {
        // Sound settings
        if (soundToggle != null)
        {
            soundToggle.onValueChanged.AddListener(OnSoundToggleChanged);
        }

        if (musicToggle != null)
        {
            musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
        }

        if (soundVolumeSlider != null)
        {
            soundVolumeSlider.onValueChanged.AddListener(OnSoundVolumeChanged);
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        // Display settings
        if (vibrationToggle != null)
        {
            vibrationToggle.onValueChanged.AddListener(OnVibrationToggleChanged);
        }

        if (notificationsToggle != null)
        {
            notificationsToggle.onValueChanged.AddListener(OnNotificationsToggleChanged);
        }

        // Reset buttons
        if (resetProgressButton != null)
        {
            resetProgressButton.onClick.AddListener(OnResetProgressClicked);
        }

        if (resetCurrentSlotButton != null)
        {
            resetCurrentSlotButton.onClick.AddListener(OnResetCurrentSlotClicked);
        }

        // Confirmation buttons
        if (confirmResetButton != null)
        {
            confirmResetButton.onClick.AddListener(ConfirmReset);
        }

        if (cancelResetButton != null)
        {
            cancelResetButton.onClick.AddListener(CancelReset);
        }
    }

    /// <summary>
    /// Loads all settings from PlayerPrefs.
    /// </summary>
    public void LoadSettings()
    {
        // Sound settings
        if (soundToggle != null)
        {
            soundToggle.isOn = PlayerPrefs.GetInt(SOUND_ENABLED_KEY, 1) == 1;
        }

        if (musicToggle != null)
        {
            musicToggle.isOn = PlayerPrefs.GetInt(MUSIC_ENABLED_KEY, 1) == 1;
        }

        if (soundVolumeSlider != null)
        {
            soundVolumeSlider.value = PlayerPrefs.GetFloat(SOUND_VOLUME_KEY, 1f);
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
        }

        // Display settings
        if (vibrationToggle != null)
        {
            vibrationToggle.isOn = PlayerPrefs.GetInt(VIBRATION_ENABLED_KEY, 1) == 1;
        }

        if (notificationsToggle != null)
        {
            notificationsToggle.isOn = PlayerPrefs.GetInt(NOTIFICATIONS_ENABLED_KEY, 1) == 1;
        }
    }

    /// <summary>
    /// Saves a boolean setting to PlayerPrefs.
    /// </summary>
    private void SaveBoolSetting(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Saves a float setting to PlayerPrefs.
    /// </summary>
    private void SaveFloatSetting(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }

    // Sound toggle handler
    private void OnSoundToggleChanged(bool isOn)
    {
        SaveBoolSetting(SOUND_ENABLED_KEY, isOn);
        ApplySoundSettings();
    }

    // Music toggle handler
    private void OnMusicToggleChanged(bool isOn)
    {
        SaveBoolSetting(MUSIC_ENABLED_KEY, isOn);
        ApplyMusicSettings();
    }

    // Sound volume handler
    private void OnSoundVolumeChanged(float value)
    {
        SaveFloatSetting(SOUND_VOLUME_KEY, value);
        ApplySoundSettings();
    }

    // Music volume handler
    private void OnMusicVolumeChanged(float value)
    {
        SaveFloatSetting(MUSIC_VOLUME_KEY, value);
        ApplyMusicSettings();
    }

    // Vibration toggle handler
    private void OnVibrationToggleChanged(bool isOn)
    {
        SaveBoolSetting(VIBRATION_ENABLED_KEY, isOn);
    }

    // Notifications toggle handler
    private void OnNotificationsToggleChanged(bool isOn)
    {
        SaveBoolSetting(NOTIFICATIONS_ENABLED_KEY, isOn);
    }

    /// <summary>
    /// Applies sound settings to the audio system.
    /// </summary>
    private void ApplySoundSettings()
    {
        bool soundEnabled = PlayerPrefs.GetInt(SOUND_ENABLED_KEY, 1) == 1;
        float soundVolume = PlayerPrefs.GetFloat(SOUND_VOLUME_KEY, 1f);

        if (!soundEnabled)
        {
            soundVolume = 0f;
        }

        // Apply to AudioListener for global volume control
        AudioListener.volume = soundVolume;
    }

    /// <summary>
    /// Applies music settings to the audio system.
    /// Note: Music volume should be applied to dedicated music AudioSource(s).
    /// The musicVolume value is stored and can be retrieved via IsMusicEnabled() and GetMusicVolume().
    /// </summary>
    private void ApplyMusicSettings()
    {
        // Music volume is stored in PlayerPrefs and can be applied by music managers
        // This method triggers the save - actual application depends on the music system
    }

    /// <summary>
    /// Called when reset progress button is clicked.
    /// </summary>
    private void OnResetProgressClicked()
    {
        isPendingFullReset = true;
        ShowResetConfirmation("Reset ALL Progress?", "This will delete ALL saved games. This action cannot be undone.");
    }

    /// <summary>
    /// Called when reset current slot button is clicked.
    /// </summary>
    private void OnResetCurrentSlotClicked()
    {
        isPendingFullReset = false;
        string slotName = "current game";
        if (GameSlotsManager.Instance != null && GameSlotsManager.Instance.HasActiveSlot)
        {
            var slot = GameSlotsManager.Instance.CurrentSlot;
            if (slot != null)
            {
                slotName = slot.slotName;
            }
        }
        ShowResetConfirmation($"Reset \"{slotName}\"?", "This will delete this saved game. This action cannot be undone.");
    }

    /// <summary>
    /// Shows the reset confirmation dialog.
    /// </summary>
    private void ShowResetConfirmation(string title, string message)
    {
        if (resetConfirmationPanel != null)
        {
            resetConfirmationPanel.SetActive(true);
        }

        if (resetConfirmationText != null)
        {
            resetConfirmationText.text = $"{title}\n\n{message}";
        }
    }

    /// <summary>
    /// Hides the reset confirmation dialog.
    /// </summary>
    private void HideResetConfirmation()
    {
        if (resetConfirmationPanel != null)
        {
            resetConfirmationPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Confirms the reset operation.
    /// </summary>
    public void ConfirmReset()
    {
        if (isPendingFullReset)
        {
            ResetAllProgress();
        }
        else
        {
            ResetCurrentSlot();
        }

        HideResetConfirmation();
    }

    /// <summary>
    /// Cancels the reset operation.
    /// </summary>
    public void CancelReset()
    {
        HideResetConfirmation();
    }

    /// <summary>
    /// Resets all game progress.
    /// </summary>
    private void ResetAllProgress()
    {
        // Clear all PlayerPrefs except settings
        bool soundEnabled = PlayerPrefs.GetInt(SOUND_ENABLED_KEY, 1) == 1;
        bool musicEnabled = PlayerPrefs.GetInt(MUSIC_ENABLED_KEY, 1) == 1;
        float soundVolume = PlayerPrefs.GetFloat(SOUND_VOLUME_KEY, 1f);
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
        bool vibrationEnabled = PlayerPrefs.GetInt(VIBRATION_ENABLED_KEY, 1) == 1;
        bool notificationsEnabled = PlayerPrefs.GetInt(NOTIFICATIONS_ENABLED_KEY, 1) == 1;

        PlayerPrefs.DeleteAll();

        // Restore settings
        SaveBoolSetting(SOUND_ENABLED_KEY, soundEnabled);
        SaveBoolSetting(MUSIC_ENABLED_KEY, musicEnabled);
        SaveFloatSetting(SOUND_VOLUME_KEY, soundVolume);
        SaveFloatSetting(MUSIC_VOLUME_KEY, musicVolume);
        SaveBoolSetting(VIBRATION_ENABLED_KEY, vibrationEnabled);
        SaveBoolSetting(NOTIFICATIONS_ENABLED_KEY, notificationsEnabled);

        // Delete all game slots
        if (GameSlotsManager.Instance != null)
        {
            for (int i = 0; i < 3; i++)
            {
                GameSlotsManager.Instance.DeleteSlot(i);
            }
        }

        // Show welcome screen
        if (menuManager != null)
        {
            menuManager.ShowWelcomeScreen();
        }
    }

    /// <summary>
    /// Resets the current game slot.
    /// </summary>
    private void ResetCurrentSlot()
    {
        if (GameSlotsManager.Instance != null && GameSlotsManager.Instance.HasActiveSlot)
        {
            int currentSlotIndex = GameSlotsManager.Instance.CurrentSlotIndex;
            GameSlotsManager.Instance.DeleteSlot(currentSlotIndex);

            // Show welcome screen
            if (menuManager != null)
            {
                menuManager.ShowWelcomeScreen();
            }
        }
    }

    /// <summary>
    /// Updates the version display text.
    /// </summary>
    private void UpdateVersionDisplay()
    {
        if (versionText != null)
        {
            versionText.text = $"Version {Application.version}";
        }
    }

    /// <summary>
    /// Gets whether sound is enabled.
    /// </summary>
    public static bool IsSoundEnabled()
    {
        return PlayerPrefs.GetInt(SOUND_ENABLED_KEY, 1) == 1;
    }

    /// <summary>
    /// Gets whether music is enabled.
    /// </summary>
    public static bool IsMusicEnabled()
    {
        return PlayerPrefs.GetInt(MUSIC_ENABLED_KEY, 1) == 1;
    }

    /// <summary>
    /// Gets whether vibration is enabled.
    /// </summary>
    public static bool IsVibrationEnabled()
    {
        return PlayerPrefs.GetInt(VIBRATION_ENABLED_KEY, 1) == 1;
    }

    /// <summary>
    /// Gets whether notifications are enabled.
    /// </summary>
    public static bool AreNotificationsEnabled()
    {
        return PlayerPrefs.GetInt(NOTIFICATIONS_ENABLED_KEY, 1) == 1;
    }
}
