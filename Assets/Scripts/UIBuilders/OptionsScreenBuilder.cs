using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Builds the Options (Settings) screen UI at runtime.
/// </summary>
public class OptionsScreenBuilder : UIBuilderBase
{
    private OptionsController optionsController;

    // UI References
    private Toggle soundToggle;
    private Toggle musicToggle;
    private Slider soundVolumeSlider;
    private Slider musicVolumeSlider;
    private Toggle vibrationToggle;
    private Toggle notificationsToggle;
    private Button resetProgressButton;
    private Button resetCurrentSlotButton;
    private GameObject resetConfirmationPanel;
    private TextMeshProUGUI resetConfirmationText;
    private Button confirmResetButton;
    private Button cancelResetButton;
    private TextMeshProUGUI versionText;
    private TextMeshProUGUI titleText;

    public override void BuildScreen()
    {
        FindCanvas();

        screenRoot = CreateFullScreenPanel("OptionsScreen", transform, UIColors.Background);

        CreateHeader();
        CreateSoundSettings();
        CreateDisplaySettings();
        CreateResetSection();
        CreateVersionInfo();
        CreateResetConfirmationPanel();

        SetupController();
    }

    private void CreateHeader()
    {
        GameObject header = CreatePanel("Header", screenRoot.transform);
        RectTransform headerRect = header.GetComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0, 0.92f);
        headerRect.anchorMax = new Vector2(1, 1);
        headerRect.offsetMin = Vector2.zero;
        headerRect.offsetMax = Vector2.zero;

        titleText = CreateTitle("Title", header.transform, "⚙️ Options");
        RectTransform titleRect = titleText.GetComponent<RectTransform>();
        SetFullStretch(titleRect);
        titleText.alignment = TextAlignmentOptions.Center;
    }

    private void CreateSoundSettings()
    {
        // Sound section
        GameObject soundSection = CreatePanelWithBackground("SoundSection", screenRoot.transform, UIColors.White);
        RectTransform soundRect = soundSection.GetComponent<RectTransform>();
        soundRect.anchorMin = new Vector2(0.05f, 0.62f);
        soundRect.anchorMax = new Vector2(0.95f, 0.90f);
        soundRect.offsetMin = Vector2.zero;
        soundRect.offsetMax = Vector2.zero;

        // Section title
        TextMeshProUGUI soundTitle = CreateSubtitle("SoundTitle", soundSection.transform, "🔊 Audio");
        RectTransform titleRect = soundTitle.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.82f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = new Vector2(15, 0);
        titleRect.offsetMax = new Vector2(-15, -5);
        soundTitle.alignment = TextAlignmentOptions.Left;
        soundTitle.fontSize = FontSizes.Body;

        // Sound toggle row
        GameObject soundToggleRow = CreateSettingRow("SoundToggleRow", soundSection.transform, 0.58f, 0.80f);
        TextMeshProUGUI soundLabel = CreateText("SoundLabel", soundToggleRow.transform, "Effets sonores", FontSizes.Body);
        soundLabel.alignment = TextAlignmentOptions.Left;
        RectTransform soundLabelRect = soundLabel.GetComponent<RectTransform>();
        soundLabelRect.anchorMin = Vector2.zero;
        soundLabelRect.anchorMax = new Vector2(0.6f, 1);
        soundLabelRect.offsetMin = new Vector2(15, 0);
        soundLabelRect.offsetMax = Vector2.zero;

        soundToggle = CreateSimpleToggle("SoundToggle", soundToggleRow.transform);
        RectTransform soundToggleRect = soundToggle.GetComponent<RectTransform>();
        soundToggleRect.anchorMin = new Vector2(0.75f, 0.2f);
        soundToggleRect.anchorMax = new Vector2(0.95f, 0.8f);
        soundToggleRect.offsetMin = Vector2.zero;
        soundToggleRect.offsetMax = Vector2.zero;

        // Sound volume row
        GameObject soundVolumeRow = CreateSettingRow("SoundVolumeRow", soundSection.transform, 0.36f, 0.58f);
        TextMeshProUGUI volumeLabel = CreateText("VolumeLabel", soundVolumeRow.transform, "Volume sons", FontSizes.Small);
        volumeLabel.alignment = TextAlignmentOptions.Left;
        RectTransform volumeLabelRect = volumeLabel.GetComponent<RectTransform>();
        volumeLabelRect.anchorMin = Vector2.zero;
        volumeLabelRect.anchorMax = new Vector2(0.35f, 1);
        volumeLabelRect.offsetMin = new Vector2(15, 0);
        volumeLabelRect.offsetMax = Vector2.zero;

        soundVolumeSlider = CreateSimpleSlider("SoundVolumeSlider", soundVolumeRow.transform);
        RectTransform soundSliderRect = soundVolumeSlider.GetComponent<RectTransform>();
        soundSliderRect.anchorMin = new Vector2(0.38f, 0.3f);
        soundSliderRect.anchorMax = new Vector2(0.95f, 0.7f);
        soundSliderRect.offsetMin = Vector2.zero;
        soundSliderRect.offsetMax = Vector2.zero;

        // Music toggle row
        GameObject musicToggleRow = CreateSettingRow("MusicToggleRow", soundSection.transform, 0.14f, 0.36f);
        TextMeshProUGUI musicLabel = CreateText("MusicLabel", musicToggleRow.transform, "Musique", FontSizes.Body);
        musicLabel.alignment = TextAlignmentOptions.Left;
        RectTransform musicLabelRect = musicLabel.GetComponent<RectTransform>();
        musicLabelRect.anchorMin = Vector2.zero;
        musicLabelRect.anchorMax = new Vector2(0.6f, 1);
        musicLabelRect.offsetMin = new Vector2(15, 0);
        musicLabelRect.offsetMax = Vector2.zero;

        musicToggle = CreateSimpleToggle("MusicToggle", musicToggleRow.transform);
        RectTransform musicToggleRect = musicToggle.GetComponent<RectTransform>();
        musicToggleRect.anchorMin = new Vector2(0.75f, 0.2f);
        musicToggleRect.anchorMax = new Vector2(0.95f, 0.8f);
        musicToggleRect.offsetMin = Vector2.zero;
        musicToggleRect.offsetMax = Vector2.zero;

        // Music volume (hidden for now, uses same as sound)
        musicVolumeSlider = soundVolumeSlider; // Share slider for simplicity
    }

    private void CreateDisplaySettings()
    {
        GameObject displaySection = CreatePanelWithBackground("DisplaySection", screenRoot.transform, UIColors.White);
        RectTransform displayRect = displaySection.GetComponent<RectTransform>();
        displayRect.anchorMin = new Vector2(0.05f, 0.42f);
        displayRect.anchorMax = new Vector2(0.95f, 0.60f);
        displayRect.offsetMin = Vector2.zero;
        displayRect.offsetMax = Vector2.zero;

        // Section title
        TextMeshProUGUI displayTitle = CreateSubtitle("DisplayTitle", displaySection.transform, "📱 Affichage");
        RectTransform titleRect = displayTitle.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.72f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = new Vector2(15, 0);
        titleRect.offsetMax = new Vector2(-15, -5);
        displayTitle.alignment = TextAlignmentOptions.Left;
        displayTitle.fontSize = FontSizes.Body;

        // Vibration toggle row
        GameObject vibrationRow = CreateSettingRow("VibrationRow", displaySection.transform, 0.38f, 0.70f);
        TextMeshProUGUI vibrationLabel = CreateText("VibrationLabel", vibrationRow.transform, "Vibrations", FontSizes.Body);
        vibrationLabel.alignment = TextAlignmentOptions.Left;
        RectTransform vibrationLabelRect = vibrationLabel.GetComponent<RectTransform>();
        vibrationLabelRect.anchorMin = Vector2.zero;
        vibrationLabelRect.anchorMax = new Vector2(0.6f, 1);
        vibrationLabelRect.offsetMin = new Vector2(15, 0);
        vibrationLabelRect.offsetMax = Vector2.zero;

        vibrationToggle = CreateSimpleToggle("VibrationToggle", vibrationRow.transform);
        RectTransform vibrationToggleRect = vibrationToggle.GetComponent<RectTransform>();
        vibrationToggleRect.anchorMin = new Vector2(0.75f, 0.2f);
        vibrationToggleRect.anchorMax = new Vector2(0.95f, 0.8f);
        vibrationToggleRect.offsetMin = Vector2.zero;
        vibrationToggleRect.offsetMax = Vector2.zero;

        // Notifications toggle row
        GameObject notificationsRow = CreateSettingRow("NotificationsRow", displaySection.transform, 0.05f, 0.38f);
        TextMeshProUGUI notificationsLabel = CreateText("NotificationsLabel", notificationsRow.transform, "Notifications", FontSizes.Body);
        notificationsLabel.alignment = TextAlignmentOptions.Left;
        RectTransform notificationsLabelRect = notificationsLabel.GetComponent<RectTransform>();
        notificationsLabelRect.anchorMin = Vector2.zero;
        notificationsLabelRect.anchorMax = new Vector2(0.6f, 1);
        notificationsLabelRect.offsetMin = new Vector2(15, 0);
        notificationsLabelRect.offsetMax = Vector2.zero;

        notificationsToggle = CreateSimpleToggle("NotificationsToggle", notificationsRow.transform);
        RectTransform notificationsToggleRect = notificationsToggle.GetComponent<RectTransform>();
        notificationsToggleRect.anchorMin = new Vector2(0.75f, 0.2f);
        notificationsToggleRect.anchorMax = new Vector2(0.95f, 0.8f);
        notificationsToggleRect.offsetMin = Vector2.zero;
        notificationsToggleRect.offsetMax = Vector2.zero;
    }

    private void CreateResetSection()
    {
        GameObject resetSection = CreatePanelWithBackground("ResetSection", screenRoot.transform, UIColors.White);
        RectTransform resetRect = resetSection.GetComponent<RectTransform>();
        resetRect.anchorMin = new Vector2(0.05f, 0.18f);
        resetRect.anchorMax = new Vector2(0.95f, 0.40f);
        resetRect.offsetMin = Vector2.zero;
        resetRect.offsetMax = Vector2.zero;

        // Section title
        TextMeshProUGUI resetTitle = CreateSubtitle("ResetTitle", resetSection.transform, "⚠️ Réinitialisation");
        RectTransform titleRect = resetTitle.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.72f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = new Vector2(15, 0);
        titleRect.offsetMax = new Vector2(-15, -5);
        resetTitle.alignment = TextAlignmentOptions.Left;
        resetTitle.fontSize = FontSizes.Body;

        // Reset current slot button
        resetCurrentSlotButton = CreateButton("ResetCurrentSlot", resetSection.transform, "Réinitialiser la partie", UIColors.Warning);
        RectTransform currentSlotRect = resetCurrentSlotButton.GetComponent<RectTransform>();
        currentSlotRect.anchorMin = new Vector2(0.05f, 0.38f);
        currentSlotRect.anchorMax = new Vector2(0.95f, 0.65f);
        currentSlotRect.offsetMin = Vector2.zero;
        currentSlotRect.offsetMax = Vector2.zero;

        // Reset all progress button
        resetProgressButton = CreateButton("ResetAllProgress", resetSection.transform, "Supprimer toutes les données", UIColors.Danger);
        RectTransform allProgressRect = resetProgressButton.GetComponent<RectTransform>();
        allProgressRect.anchorMin = new Vector2(0.05f, 0.05f);
        allProgressRect.anchorMax = new Vector2(0.95f, 0.32f);
        allProgressRect.offsetMin = Vector2.zero;
        allProgressRect.offsetMax = Vector2.zero;
    }

    private void CreateVersionInfo()
    {
        versionText = CreateText("VersionText", screenRoot.transform, "Version 1.0.0", FontSizes.Small);
        versionText.color = UIColors.TextLight;
        RectTransform versionRect = versionText.GetComponent<RectTransform>();
        versionRect.anchorMin = new Vector2(0, 0.02f);
        versionRect.anchorMax = new Vector2(1, 0.08f);
        versionRect.offsetMin = Vector2.zero;
        versionRect.offsetMax = Vector2.zero;
    }

    private void CreateResetConfirmationPanel()
    {
        resetConfirmationPanel = CreateModalOverlay("ResetConfirmation", screenRoot.transform);
        resetConfirmationPanel.SetActive(false);

        GameObject dialog = CreateDialogBox("Dialog", resetConfirmationPanel.transform, new Vector2(450, 280));

        // Warning icon
        TextMeshProUGUI warningIcon = CreateText("WarningIcon", dialog.transform, "⚠️", 48f);
        RectTransform iconRect = warningIcon.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0, 0.7f);
        iconRect.anchorMax = new Vector2(1, 0.95f);
        iconRect.offsetMin = Vector2.zero;
        iconRect.offsetMax = Vector2.zero;

        // Confirmation text
        resetConfirmationText = CreateText("ConfirmText", dialog.transform, "Êtes-vous sûr ?", FontSizes.Body);
        RectTransform textRect = resetConfirmationText.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 0.35f);
        textRect.anchorMax = new Vector2(1, 0.7f);
        textRect.offsetMin = new Vector2(20, 0);
        textRect.offsetMax = new Vector2(-20, 0);

        // Buttons
        GameObject buttonContainer = CreatePanel("ButtonContainer", dialog.transform);
        RectTransform btnContainerRect = buttonContainer.GetComponent<RectTransform>();
        btnContainerRect.anchorMin = new Vector2(0.1f, 0.08f);
        btnContainerRect.anchorMax = new Vector2(0.9f, 0.30f);
        btnContainerRect.offsetMin = Vector2.zero;
        btnContainerRect.offsetMax = Vector2.zero;

        HorizontalLayoutGroup btnLayout = CreateHorizontalLayout(buttonContainer, 20);

        cancelResetButton = CreateButton("CancelButton", buttonContainer.transform, "Annuler", UIColors.Secondary);
        confirmResetButton = CreateButton("ConfirmButton", buttonContainer.transform, "Confirmer", UIColors.Danger);
    }

    private GameObject CreateSettingRow(string name, Transform parent, float yMin, float yMax)
    {
        GameObject row = CreatePanel(name, parent);
        RectTransform rowRect = row.GetComponent<RectTransform>();
        rowRect.anchorMin = new Vector2(0, yMin);
        rowRect.anchorMax = new Vector2(1, yMax);
        rowRect.offsetMin = Vector2.zero;
        rowRect.offsetMax = Vector2.zero;
        return row;
    }

    private Toggle CreateSimpleToggle(string name, Transform parent)
    {
        GameObject toggleObj = new GameObject(name, typeof(RectTransform));
        toggleObj.transform.SetParent(parent, false);

        // Background
        Image bgImage = toggleObj.AddComponent<Image>();
        bgImage.color = UIColors.Secondary;

        // Checkmark
        GameObject checkmark = CreatePanelWithBackground("Checkmark", toggleObj.transform, UIColors.Primary);
        RectTransform checkRect = checkmark.GetComponent<RectTransform>();
        checkRect.anchorMin = new Vector2(0.1f, 0.1f);
        checkRect.anchorMax = new Vector2(0.9f, 0.9f);
        checkRect.offsetMin = Vector2.zero;
        checkRect.offsetMax = Vector2.zero;

        Toggle toggle = toggleObj.AddComponent<Toggle>();
        toggle.targetGraphic = bgImage;
        toggle.graphic = checkmark.GetComponent<Image>();
        toggle.isOn = true;

        return toggle;
    }

    private Slider CreateSimpleSlider(string name, Transform parent)
    {
        GameObject sliderObj = new GameObject(name, typeof(RectTransform));
        sliderObj.transform.SetParent(parent, false);

        // Background
        GameObject background = CreatePanelWithBackground("Background", sliderObj.transform, UIColors.Secondary);
        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 0.4f);
        bgRect.anchorMax = new Vector2(1, 0.6f);
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // Fill area
        GameObject fillArea = CreatePanel("FillArea", sliderObj.transform);
        RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
        fillAreaRect.anchorMin = new Vector2(0, 0.4f);
        fillAreaRect.anchorMax = new Vector2(1, 0.6f);
        fillAreaRect.offsetMin = new Vector2(5, 0);
        fillAreaRect.offsetMax = new Vector2(-5, 0);

        // Fill
        GameObject fill = CreatePanelWithBackground("Fill", fillArea.transform, UIColors.Primary);
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = new Vector2(0, 1);
        fillRect.sizeDelta = new Vector2(10, 0);

        // Handle area
        GameObject handleArea = CreatePanel("HandleArea", sliderObj.transform);
        RectTransform handleAreaRect = handleArea.GetComponent<RectTransform>();
        handleAreaRect.anchorMin = new Vector2(0, 0);
        handleAreaRect.anchorMax = new Vector2(1, 1);
        handleAreaRect.offsetMin = new Vector2(10, 0);
        handleAreaRect.offsetMax = new Vector2(-10, 0);

        // Handle
        GameObject handle = CreatePanelWithBackground("Handle", handleArea.transform, UIColors.White);
        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(20, 0);

        Slider slider = sliderObj.AddComponent<Slider>();
        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;
        slider.targetGraphic = handle.GetComponent<Image>();

        return slider;
    }

    private void SetupController()
    {
        optionsController = screenRoot.AddComponent<OptionsController>();

        optionsController.InitializeReferences(
            soundToggle,
            musicToggle,
            soundVolumeSlider,
            musicVolumeSlider,
            vibrationToggle,
            notificationsToggle,
            resetProgressButton,
            resetCurrentSlotButton,
            resetConfirmationPanel,
            resetConfirmationText,
            confirmResetButton,
            cancelResetButton,
            versionText,
            titleText
        );
    }

    public OptionsController GetController() => optionsController;
}
