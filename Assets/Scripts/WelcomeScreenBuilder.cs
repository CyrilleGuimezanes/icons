using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Dynamically creates the welcome screen UI at runtime.
/// This approach is used because the scene file structure is complex.
/// </summary>
public class WelcomeScreenBuilder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Canvas parentCanvas;
    [SerializeField] private MenuManager menuManager;

    private GameObject welcomeScreenRoot;
    private WelcomeScreenController welcomeController;

    private void Awake()
    {
        // Find MenuManager if not set
        if (menuManager == null)
        {
            menuManager = FindAnyObjectByType<MenuManager>();
        }
        
        BuildWelcomeScreen();
    }

    private void BuildWelcomeScreen()
    {
        if (parentCanvas == null)
        {
            parentCanvas = FindAnyObjectByType<Canvas>();
        }

        if (parentCanvas == null)
        {
            Debug.LogError("WelcomeScreenBuilder: No Canvas found!");
            return;
        }

        // Create welcome screen root
        welcomeScreenRoot = CreatePanel("WelcomeScreen", parentCanvas.transform);
        RectTransform rootRect = welcomeScreenRoot.GetComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        // Add background
        Image bg = welcomeScreenRoot.AddComponent<Image>();
        bg.color = new Color(0.96f, 0.96f, 0.96f, 1f);

        // Add WelcomeScreenController
        welcomeController = welcomeScreenRoot.AddComponent<WelcomeScreenController>();

        // Create title
        GameObject titleObj = CreateTextObject("Title", welcomeScreenRoot.transform, "Select Game", 36, FontStyles.Bold);
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.anchoredPosition = new Vector2(0, -80);
        titleRect.sizeDelta = new Vector2(-40, 60);

        // Create slots container
        GameObject slotsContainer = CreatePanel("SlotsContainer", welcomeScreenRoot.transform);
        RectTransform slotsRect = slotsContainer.GetComponent<RectTransform>();
        slotsRect.anchorMin = new Vector2(0, 0.3f);
        slotsRect.anchorMax = new Vector2(1, 0.85f);
        slotsRect.offsetMin = new Vector2(40, 0);
        slotsRect.offsetMax = new Vector2(-40, 0);

        // Add vertical layout
        VerticalLayoutGroup layout = slotsContainer.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 30;
        layout.padding = new RectOffset(0, 0, 20, 20);
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        // Create 3 slot UIs
        GameSlotUI[] slotUIs = new GameSlotUI[3];
        for (int i = 0; i < 3; i++)
        {
            slotUIs[i] = CreateSlotUI(slotsContainer.transform, i);
        }

        // Create delete confirmation panel
        GameObject deletePanel = CreateDeleteConfirmationPanel(welcomeScreenRoot.transform);

        // Set up controller references using reflection (since we can't use SerializeField at runtime)
        SetupControllerReferences(welcomeController, slotUIs, titleObj.GetComponent<TextMeshProUGUI>(), deletePanel);

        // Connect to menu manager
        if (menuManager != null)
        {
            ConnectToMenuManager();
        }

        // Hide initially (MenuManager will show it if needed)
        welcomeScreenRoot.SetActive(false);
    }

    private GameObject CreatePanel(string name, Transform parent)
    {
        GameObject panel = new GameObject(name, typeof(RectTransform));
        panel.transform.SetParent(parent, false);
        return panel;
    }

    private GameObject CreateTextObject(string name, Transform parent, string text, float fontSize, FontStyles style)
    {
        GameObject textObj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));
        textObj.transform.SetParent(parent, false);

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = style;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        return textObj;
    }

    private GameSlotUI CreateSlotUI(Transform parent, int slotIndex)
    {
        // Create slot container
        GameObject slotObj = new GameObject($"Slot{slotIndex}", typeof(RectTransform));
        slotObj.transform.SetParent(parent, false);

        RectTransform slotRect = slotObj.GetComponent<RectTransform>();
        slotRect.sizeDelta = new Vector2(0, 180);

        // Add background
        Image slotBg = slotObj.AddComponent<Image>();
        slotBg.color = new Color(1f, 1f, 1f, 1f);

        // Add GameSlotUI component
        GameSlotUI slotUI = slotObj.AddComponent<GameSlotUI>();

        // Create active content container
        GameObject activeContent = CreatePanel("ActiveContent", slotObj.transform);
        RectTransform activeRect = activeContent.GetComponent<RectTransform>();
        activeRect.anchorMin = Vector2.zero;
        activeRect.anchorMax = Vector2.one;
        activeRect.offsetMin = Vector2.zero;
        activeRect.offsetMax = Vector2.zero;

        // Slot name text
        GameObject nameObj = CreateTextObject("SlotName", activeContent.transform, $"Game {slotIndex + 1}", 24, FontStyles.Bold);
        RectTransform nameRect = nameObj.GetComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0.6f);
        nameRect.anchorMax = new Vector2(1, 0.9f);
        nameRect.offsetMin = new Vector2(20, 0);
        nameRect.offsetMax = new Vector2(-20, -10);

        // Stats text
        GameObject statsObj = CreateTextObject("Stats", activeContent.transform, "Icons: 0 | Time: < 1m", 16, FontStyles.Normal);
        RectTransform statsRect = statsObj.GetComponent<RectTransform>();
        statsRect.anchorMin = new Vector2(0, 0.4f);
        statsRect.anchorMax = new Vector2(1, 0.6f);
        statsRect.offsetMin = new Vector2(20, 0);
        statsRect.offsetMax = new Vector2(-20, 0);
        statsObj.GetComponent<TextMeshProUGUI>().color = new Color(0.5f, 0.5f, 0.5f, 1f);

        // Buttons container
        GameObject buttonsContainer = CreatePanel("Buttons", activeContent.transform);
        RectTransform buttonsRect = buttonsContainer.GetComponent<RectTransform>();
        buttonsRect.anchorMin = new Vector2(0, 0);
        buttonsRect.anchorMax = new Vector2(1, 0.35f);
        buttonsRect.offsetMin = new Vector2(20, 10);
        buttonsRect.offsetMax = new Vector2(-20, -10);

        HorizontalLayoutGroup buttonsLayout = buttonsContainer.AddComponent<HorizontalLayoutGroup>();
        buttonsLayout.spacing = 10;
        buttonsLayout.childAlignment = TextAnchor.MiddleCenter;
        buttonsLayout.childControlWidth = true;
        buttonsLayout.childControlHeight = true;
        buttonsLayout.childForceExpandWidth = true;
        buttonsLayout.childForceExpandHeight = true;

        // Play button
        Button playBtn = CreateButton("PlayButton", buttonsContainer.transform, "Play", new Color(0.2f, 0.6f, 1f, 1f));

        // Rename button
        Button renameBtn = CreateButton("RenameButton", buttonsContainer.transform, "Rename", new Color(0.6f, 0.6f, 0.6f, 1f));

        // Delete button
        Button deleteBtn = CreateButton("DeleteButton", buttonsContainer.transform, "Delete", new Color(0.9f, 0.3f, 0.3f, 1f));

        // Create empty content container
        GameObject emptyContent = CreatePanel("EmptyContent", slotObj.transform);
        RectTransform emptyRect = emptyContent.GetComponent<RectTransform>();
        emptyRect.anchorMin = Vector2.zero;
        emptyRect.anchorMax = Vector2.one;
        emptyRect.offsetMin = Vector2.zero;
        emptyRect.offsetMax = Vector2.zero;
        emptyContent.SetActive(false);

        // Empty slot text
        GameObject emptyTextObj = CreateTextObject("EmptyText", emptyContent.transform, "Empty Slot", 20, FontStyles.Italic);
        RectTransform emptyTextRect = emptyTextObj.GetComponent<RectTransform>();
        emptyTextRect.anchorMin = new Vector2(0, 0.5f);
        emptyTextRect.anchorMax = new Vector2(1, 0.8f);
        emptyTextRect.offsetMin = Vector2.zero;
        emptyTextRect.offsetMax = Vector2.zero;
        emptyTextObj.GetComponent<TextMeshProUGUI>().color = new Color(0.5f, 0.5f, 0.5f, 1f);

        // New game button for empty slot
        Button newGameBtn = CreateButton("NewGameButton", emptyContent.transform, "New Game", new Color(0.3f, 0.8f, 0.3f, 1f));
        RectTransform newGameRect = newGameBtn.GetComponent<RectTransform>();
        newGameRect.anchorMin = new Vector2(0.2f, 0.1f);
        newGameRect.anchorMax = new Vector2(0.8f, 0.45f);
        newGameRect.offsetMin = Vector2.zero;
        newGameRect.offsetMax = Vector2.zero;

        // Create name input field
        TMP_InputField nameInput = CreateInputField("NameInput", slotObj.transform);

        // Set up GameSlotUI references via reflection
        SetSlotUIReferences(slotUI, nameObj.GetComponent<TextMeshProUGUI>(), 
            statsObj.GetComponent<TextMeshProUGUI>(), activeContent, emptyContent,
            playBtn, renameBtn, deleteBtn, newGameBtn, nameInput);

        return slotUI;
    }

    private Button CreateButton(string name, Transform parent, string text, Color bgColor)
    {
        GameObject btnObj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));
        btnObj.transform.SetParent(parent, false);

        Image btnImage = btnObj.AddComponent<Image>();
        btnImage.color = bgColor;

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnImage;

        // Button text
        GameObject btnTextObj = CreateTextObject("Text", btnObj.transform, text, 14, FontStyles.Bold);
        RectTransform btnTextRect = btnTextObj.GetComponent<RectTransform>();
        btnTextRect.anchorMin = Vector2.zero;
        btnTextRect.anchorMax = Vector2.one;
        btnTextRect.offsetMin = Vector2.zero;
        btnTextRect.offsetMax = Vector2.zero;
        btnTextObj.GetComponent<TextMeshProUGUI>().color = Color.white;

        return btn;
    }

    private TMP_InputField CreateInputField(string name, Transform parent)
    {
        GameObject inputObj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));
        inputObj.transform.SetParent(parent, false);

        RectTransform inputRect = inputObj.GetComponent<RectTransform>();
        inputRect.anchorMin = new Vector2(0.1f, 0.3f);
        inputRect.anchorMax = new Vector2(0.9f, 0.7f);
        inputRect.offsetMin = Vector2.zero;
        inputRect.offsetMax = Vector2.zero;

        Image inputBg = inputObj.AddComponent<Image>();
        inputBg.color = Color.white;

        // Text area
        GameObject textArea = CreatePanel("TextArea", inputObj.transform);
        RectTransform textAreaRect = textArea.GetComponent<RectTransform>();
        textAreaRect.anchorMin = Vector2.zero;
        textAreaRect.anchorMax = Vector2.one;
        textAreaRect.offsetMin = new Vector2(10, 5);
        textAreaRect.offsetMax = new Vector2(-10, -5);
        RectMask2D mask = textArea.AddComponent<RectMask2D>();

        // Input text
        GameObject inputTextObj = CreateTextObject("Text", textArea.transform, "", 18, FontStyles.Normal);
        RectTransform inputTextRect = inputTextObj.GetComponent<RectTransform>();
        inputTextRect.anchorMin = Vector2.zero;
        inputTextRect.anchorMax = Vector2.one;
        inputTextRect.offsetMin = Vector2.zero;
        inputTextRect.offsetMax = Vector2.zero;
        TextMeshProUGUI inputText = inputTextObj.GetComponent<TextMeshProUGUI>();
        inputText.alignment = TextAlignmentOptions.Left;

        // Placeholder
        GameObject placeholderObj = CreateTextObject("Placeholder", textArea.transform, "Enter name...", 18, FontStyles.Italic);
        RectTransform placeholderRect = placeholderObj.GetComponent<RectTransform>();
        placeholderRect.anchorMin = Vector2.zero;
        placeholderRect.anchorMax = Vector2.one;
        placeholderRect.offsetMin = Vector2.zero;
        placeholderRect.offsetMax = Vector2.zero;
        TextMeshProUGUI placeholder = placeholderObj.GetComponent<TextMeshProUGUI>();
        placeholder.alignment = TextAlignmentOptions.Left;
        placeholder.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        TMP_InputField inputField = inputObj.AddComponent<TMP_InputField>();
        inputField.textViewport = textAreaRect;
        inputField.textComponent = inputText;
        inputField.placeholder = placeholder;
        inputField.characterLimit = 20;

        inputObj.SetActive(false);

        return inputField;
    }

    private GameObject CreateDeleteConfirmationPanel(Transform parent)
    {
        // Overlay
        GameObject overlay = CreatePanel("DeleteConfirmation", parent);
        RectTransform overlayRect = overlay.GetComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;

        Image overlayBg = overlay.AddComponent<Image>();
        overlayBg.color = new Color(0, 0, 0, 0.5f);

        // Dialog box
        GameObject dialog = CreatePanel("Dialog", overlay.transform);
        RectTransform dialogRect = dialog.GetComponent<RectTransform>();
        dialogRect.anchorMin = new Vector2(0.1f, 0.35f);
        dialogRect.anchorMax = new Vector2(0.9f, 0.65f);
        dialogRect.offsetMin = Vector2.zero;
        dialogRect.offsetMax = Vector2.zero;

        Image dialogBg = dialog.AddComponent<Image>();
        dialogBg.color = Color.white;

        // Confirmation text
        GameObject confirmText = CreateTextObject("ConfirmText", dialog.transform, 
            "Delete this game?\n\nThis cannot be undone.", 18, FontStyles.Normal);
        RectTransform confirmTextRect = confirmText.GetComponent<RectTransform>();
        confirmTextRect.anchorMin = new Vector2(0, 0.4f);
        confirmTextRect.anchorMax = new Vector2(1, 0.95f);
        confirmTextRect.offsetMin = new Vector2(20, 0);
        confirmTextRect.offsetMax = new Vector2(-20, -20);

        // Buttons container
        GameObject buttonsContainer = CreatePanel("Buttons", dialog.transform);
        RectTransform buttonsRect = buttonsContainer.GetComponent<RectTransform>();
        buttonsRect.anchorMin = new Vector2(0, 0);
        buttonsRect.anchorMax = new Vector2(1, 0.35f);
        buttonsRect.offsetMin = new Vector2(20, 20);
        buttonsRect.offsetMax = new Vector2(-20, 0);

        HorizontalLayoutGroup buttonsLayout = buttonsContainer.AddComponent<HorizontalLayoutGroup>();
        buttonsLayout.spacing = 20;
        buttonsLayout.childAlignment = TextAnchor.MiddleCenter;
        buttonsLayout.childControlWidth = true;
        buttonsLayout.childControlHeight = true;
        buttonsLayout.childForceExpandWidth = true;
        buttonsLayout.childForceExpandHeight = true;

        // Cancel button
        CreateButton("CancelButton", buttonsContainer.transform, "Cancel", new Color(0.6f, 0.6f, 0.6f, 1f));

        // Confirm button
        CreateButton("ConfirmButton", buttonsContainer.transform, "Delete", new Color(0.9f, 0.3f, 0.3f, 1f));

        overlay.SetActive(false);
        return overlay;
    }

    private void SetupControllerReferences(WelcomeScreenController controller, GameSlotUI[] slotUIs, 
        TextMeshProUGUI titleText, GameObject deletePanel)
    {
        // Use reflection to set private serialized fields
        var type = typeof(WelcomeScreenController);
        
        var titleField = type.GetField("titleText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        titleField?.SetValue(controller, titleText);

        var slotsField = type.GetField("slotUIs", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        slotsField?.SetValue(controller, slotUIs);

        var deletePanelField = type.GetField("deleteConfirmationPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        deletePanelField?.SetValue(controller, deletePanel);

        // Find confirmation text and buttons in delete panel
        var confirmText = deletePanel.transform.Find("Dialog/ConfirmText")?.GetComponent<TextMeshProUGUI>();
        var confirmTextField = type.GetField("deleteConfirmationText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        confirmTextField?.SetValue(controller, confirmText);

        var confirmBtn = deletePanel.transform.Find("Dialog/Buttons/ConfirmButton")?.GetComponent<Button>();
        var confirmBtnField = type.GetField("confirmDeleteButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        confirmBtnField?.SetValue(controller, confirmBtn);

        var cancelBtn = deletePanel.transform.Find("Dialog/Buttons/CancelButton")?.GetComponent<Button>();
        var cancelBtnField = type.GetField("cancelDeleteButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        cancelBtnField?.SetValue(controller, cancelBtn);

        var menuField = type.GetField("menuManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        menuField?.SetValue(controller, menuManager);
    }

    private void SetSlotUIReferences(GameSlotUI slotUI, TextMeshProUGUI nameText, TextMeshProUGUI statsText,
        GameObject activeContent, GameObject emptyContent, Button playBtn, Button renameBtn, 
        Button deleteBtn, Button newGameBtn, TMP_InputField nameInput)
    {
        var type = typeof(GameSlotUI);

        type.GetField("slotNameText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(slotUI, nameText);
        type.GetField("statsText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(slotUI, statsText);
        type.GetField("activeContent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(slotUI, activeContent);
        type.GetField("emptyContent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(slotUI, emptyContent);
        type.GetField("playButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(slotUI, playBtn);
        type.GetField("renameButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(slotUI, renameBtn);
        type.GetField("deleteButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(slotUI, deleteBtn);
        type.GetField("newGameButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(slotUI, newGameBtn);
        type.GetField("nameInputField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(slotUI, nameInput);
    }

    private void ConnectToMenuManager()
    {
        if (menuManager == null) return;

        var type = typeof(MenuManager);
        var welcomeField = type.GetField("welcomeScreen", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        welcomeField?.SetValue(menuManager, welcomeScreenRoot);
    }

    /// <summary>
    /// Gets the welcome screen root GameObject.
    /// </summary>
    public GameObject GetWelcomeScreen()
    {
        return welcomeScreenRoot;
    }
}
