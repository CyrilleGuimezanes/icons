#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

/// <summary>
/// Editor script that builds the Potager (Production) screen UI.
/// </summary>
public class EditorPotagerScreenBuilder : EditorUIBuilderBase
{
    // UI References
    private ProductionSlot[] productionSlots;
    private Transform inventoryContainer;
    private GameObject inventoryIconPrefab;
    private Button multiplierX1Button;
    private Button multiplierX5Button;
    private Button multiplierX10Button;
    private Button multiplierX100Button;
    private TextMeshProUGUI multiplierText;
    private GameObject selectionModal;
    private Transform selectionContainer;
    private GameObject productionSelectionItemPrefab;
    private Button closeSelectionButton;
    private Button plantTabButton;
    private Button industryTabButton;
    private TextMeshProUGUI plantTabText;
    private TextMeshProUGUI industryTabText;
    private TextMeshProUGUI titleText;

    [MenuItem("Icons/Build UI/Screens/Build Potager Screen")]
    public static void BuildPotagerScreen()
    {
        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No Canvas found in scene!");
            return;
        }

        Transform screensContainer = canvas.transform.Find("ScreensContainer");
        if (screensContainer == null)
        {
            Debug.LogError("ScreensContainer not found. Please build complete UI first.");
            return;
        }

        Transform existing = screensContainer.Find("PotagerScreen");
        if (existing != null)
        {
            Undo.DestroyObjectImmediate(existing.gameObject);
        }

        EditorPotagerScreenBuilder builder = new EditorPotagerScreenBuilder();
        builder.BuildScreen(screensContainer);
        
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("Potager screen built successfully!");
    }

    public override void BuildScreen(Transform parent)
    {
        FindCanvas();

        // Create screen root
        screenRoot = CreateFullScreenPanel("PotagerScreen", parent, UIColors.Background);

        // Build UI sections
        CreateHeader();
        CreateTabButtons();
        CreateProductionSlots();
        CreateMultiplierButtons();
        CreateInventoryPanel();
        CreateSelectionModal();

        // Setup controller
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

        titleText = CreateTitle("Title", header.transform, "🌱 Potager / Industrie");
        RectTransform titleRect = titleText.GetComponent<RectTransform>();
        SetFullStretch(titleRect);
        titleText.alignment = TextAlignmentOptions.Center;
    }

    private void CreateTabButtons()
    {
        GameObject tabContainer = CreatePanel("TabContainer", screenRoot.transform);
        RectTransform tabRect = tabContainer.GetComponent<RectTransform>();
        tabRect.anchorMin = new Vector2(0.1f, 0.85f);
        tabRect.anchorMax = new Vector2(0.9f, 0.91f);
        tabRect.offsetMin = Vector2.zero;
        tabRect.offsetMax = Vector2.zero;

        HorizontalLayoutGroup layout = CreateHorizontalLayout(tabContainer, 10);

        // Plant tab
        plantTabButton = CreateButton("PlantTab", tabContainer.transform, "🌱 Potager", UIColors.Primary);
        plantTabText = plantTabButton.GetComponentInChildren<TextMeshProUGUI>();

        // Industry tab
        industryTabButton = CreateButton("IndustryTab", tabContainer.transform, "🏭 Industrie", UIColors.Secondary);
        industryTabText = industryTabButton.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void CreateProductionSlots()
    {
        GameObject slotsContainer = CreatePanel("SlotsContainer", screenRoot.transform);
        RectTransform slotsRect = slotsContainer.GetComponent<RectTransform>();
        slotsRect.anchorMin = new Vector2(0.05f, 0.45f);
        slotsRect.anchorMax = new Vector2(0.95f, 0.84f);
        slotsRect.offsetMin = Vector2.zero;
        slotsRect.offsetMax = Vector2.zero;

        // Grid layout for 5 slots (2 columns)
        GridLayoutGroup gridLayout = CreateGridLayout(slotsContainer, new Vector2(150, 140), new Vector2(15, 15));
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 2;
        gridLayout.childAlignment = TextAnchor.UpperCenter;
        gridLayout.padding = new RectOffset(10, 10, 10, 10);

        // Create 5 production slots
        productionSlots = new ProductionSlot[5];
        for (int i = 0; i < 5; i++)
        {
            productionSlots[i] = CreateProductionSlot(slotsContainer.transform, i);
        }
    }

    private ProductionSlot CreateProductionSlot(Transform parent, int index)
    {
        GameObject slotObj = new GameObject($"ProductionSlot_{index}", typeof(RectTransform), typeof(CanvasRenderer));
        slotObj.transform.SetParent(parent, false);
        Undo.RegisterCreatedObjectUndo(slotObj, $"Create ProductionSlot_{index}");

        // Background
        Image slotBg = slotObj.AddComponent<Image>();
        slotBg.color = UIColors.White;

        // Content container
        GameObject content = CreatePanel("Content", slotObj.transform);
        RectTransform contentRect = content.GetComponent<RectTransform>();
        SetFullStretch(contentRect, new Vector2(10, 10), new Vector2(-10, -10));

        CreateVerticalLayout(content, 5);

        // Icon display
        TextMeshProUGUI iconText = CreateText("Icon", content.transform, "➕", 32f);
        AddLayoutElement(iconText.gameObject, preferredHeight: 50);

        // Name text
        TextMeshProUGUI nameText = CreateText("Name", content.transform, "Sélectionner", FontSizes.Small);
        AddLayoutElement(nameText.gameObject, preferredHeight: 20);
        nameText.color = UIColors.TextLight;

        // Progress bar background
        GameObject progressBg = CreatePanelWithBackground("ProgressBg", content.transform, UIColors.Secondary);
        AddLayoutElement(progressBg, preferredHeight: 15);

        // Progress bar fill
        GameObject progressFill = CreatePanelWithBackground("ProgressFill", progressBg.transform, UIColors.Success);
        RectTransform fillRect = progressFill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = new Vector2(0, 1);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        Image progressImage = progressFill.GetComponent<Image>();
        progressImage.type = Image.Type.Filled;
        progressImage.fillMethod = Image.FillMethod.Horizontal;

        // Time remaining text
        TextMeshProUGUI timeText = CreateText("Time", content.transform, "", FontSizes.Tiny);
        AddLayoutElement(timeText.gameObject, preferredHeight: 15);
        timeText.color = UIColors.TextLight;

        // Button for slot interaction
        Button slotButton = slotObj.AddComponent<Button>();
        slotButton.targetGraphic = slotBg;

        // Add ProductionSlot component
        ProductionSlot slot = slotObj.AddComponent<ProductionSlot>();
        slot.InitializeReferences(iconText, nameText, progressImage, timeText);

        return slot;
    }

    private void CreateMultiplierButtons()
    {
        GameObject multiplierContainer = CreatePanel("MultiplierContainer", screenRoot.transform);
        RectTransform containerRect = multiplierContainer.GetComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.05f, 0.38f);
        containerRect.anchorMax = new Vector2(0.95f, 0.44f);
        containerRect.offsetMin = Vector2.zero;
        containerRect.offsetMax = Vector2.zero;

        HorizontalLayoutGroup layout = CreateHorizontalLayout(multiplierContainer, 5);

        // Multiplier label
        multiplierText = CreateText("MultiplierLabel", multiplierContainer.transform, "x1", FontSizes.Body);
        AddLayoutElement(multiplierText.gameObject, preferredWidth: 50);

        // Multiplier buttons
        multiplierX1Button = CreateButton("x1Button", multiplierContainer.transform, "x1", UIColors.Primary);
        multiplierX5Button = CreateButton("x5Button", multiplierContainer.transform, "x5", UIColors.Secondary);
        multiplierX10Button = CreateButton("x10Button", multiplierContainer.transform, "x10", UIColors.Secondary);
        multiplierX100Button = CreateButton("x100Button", multiplierContainer.transform, "x100", UIColors.Secondary);
    }

    private void CreateInventoryPanel()
    {
        // Inventory header
        GameObject inventoryHeader = CreatePanel("InventoryHeader", screenRoot.transform);
        RectTransform headerRect = inventoryHeader.GetComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0, 0.32f);
        headerRect.anchorMax = new Vector2(1, 0.38f);
        headerRect.offsetMin = new Vector2(20, 0);
        headerRect.offsetMax = new Vector2(-20, 0);

        TextMeshProUGUI inventoryTitle = CreateSubtitle("InventoryTitle", inventoryHeader.transform, "Ressources disponibles");
        RectTransform titleRect = inventoryTitle.GetComponent<RectTransform>();
        SetFullStretch(titleRect);
        inventoryTitle.alignment = TextAlignmentOptions.Left;
        inventoryTitle.fontSize = FontSizes.Body;

        // Inventory scroll view
        ScrollRect scrollView = CreateScrollView("InventoryScrollView", screenRoot.transform, true, false);
        RectTransform scrollRect = scrollView.GetComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0, 0.02f);
        scrollRect.anchorMax = new Vector2(1, 0.32f);
        scrollRect.offsetMin = new Vector2(10, 0);
        scrollRect.offsetMax = new Vector2(-10, 0);

        inventoryContainer = scrollView.content;
        RectTransform contentRect = inventoryContainer.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 0);
        contentRect.anchorMax = new Vector2(0, 1);
        contentRect.pivot = new Vector2(0, 0.5f);

        HorizontalLayoutGroup contentLayout = inventoryContainer.gameObject.AddComponent<HorizontalLayoutGroup>();
        contentLayout.spacing = 10;
        contentLayout.padding = new RectOffset(10, 10, 10, 10);
        contentLayout.childAlignment = TextAnchor.MiddleLeft;
        contentLayout.childControlWidth = false;
        contentLayout.childControlHeight = true;
        contentLayout.childForceExpandWidth = false;
        contentLayout.childForceExpandHeight = true;

        ContentSizeFitter sizeFitter = inventoryContainer.gameObject.AddComponent<ContentSizeFitter>();
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

        // Create inventory icon prefab
        CreateInventoryIconPrefab();
    }

    private void CreateInventoryIconPrefab()
    {
        inventoryIconPrefab = new GameObject("ProductionIconTemplate", typeof(RectTransform));
        inventoryIconPrefab.transform.SetParent(screenRoot.transform, false);
        inventoryIconPrefab.SetActive(false);
        Undo.RegisterCreatedObjectUndo(inventoryIconPrefab, "Create ProductionIconTemplate");

        RectTransform iconRect = inventoryIconPrefab.GetComponent<RectTransform>();
        iconRect.sizeDelta = new Vector2(70, 70);

        Image iconBg = inventoryIconPrefab.AddComponent<Image>();
        iconBg.color = UIColors.White;

        Button iconBtn = inventoryIconPrefab.AddComponent<Button>();
        iconBtn.targetGraphic = iconBg;

        // Icon text
        GameObject iconTextObj = new GameObject("IconText", typeof(RectTransform), typeof(CanvasRenderer));
        iconTextObj.transform.SetParent(inventoryIconPrefab.transform, false);
        TextMeshProUGUI iconText = iconTextObj.AddComponent<TextMeshProUGUI>();
        iconText.fontSize = 28f;
        iconText.alignment = TextAlignmentOptions.Center;
        iconText.color = UIColors.Text;
        RectTransform iconTextRect = iconTextObj.GetComponent<RectTransform>();
        iconTextRect.anchorMin = new Vector2(0, 0.25f);
        iconTextRect.anchorMax = Vector2.one;
        iconTextRect.offsetMin = Vector2.zero;
        iconTextRect.offsetMax = Vector2.zero;

        // Quantity text
        GameObject qtyTextObj = new GameObject("QuantityText", typeof(RectTransform), typeof(CanvasRenderer));
        qtyTextObj.transform.SetParent(inventoryIconPrefab.transform, false);
        TextMeshProUGUI qtyText = qtyTextObj.AddComponent<TextMeshProUGUI>();
        qtyText.fontSize = 10f;
        qtyText.alignment = TextAlignmentOptions.Center;
        qtyText.color = UIColors.TextLight;
        RectTransform qtyRect = qtyTextObj.GetComponent<RectTransform>();
        qtyRect.anchorMin = Vector2.zero;
        qtyRect.anchorMax = new Vector2(1, 0.25f);
        qtyRect.offsetMin = Vector2.zero;
        qtyRect.offsetMax = Vector2.zero;

        // Add component
        ProductionIconDisplay display = inventoryIconPrefab.AddComponent<ProductionIconDisplay>();
        display.InitializeReferences(iconText, qtyText);
    }

    private void CreateSelectionModal()
    {
        selectionModal = CreateModalOverlay("SelectionModal", screenRoot.transform);
        selectionModal.SetActive(false);

        // Dialog
        GameObject dialog = CreateDialogBox("Dialog", selectionModal.transform, new Vector2(500, 600));

        // Header
        TextMeshProUGUI modalTitle = CreateTitle("ModalTitle", dialog.transform, "Sélectionner une production");
        modalTitle.fontSize = FontSizes.Subtitle;
        RectTransform titleRect = modalTitle.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.88f);
        titleRect.anchorMax = new Vector2(1, 0.98f);
        titleRect.offsetMin = new Vector2(20, 0);
        titleRect.offsetMax = new Vector2(-20, 0);

        // Close button
        closeSelectionButton = CreateButton("CloseButton", dialog.transform, "✕", UIColors.Danger);
        RectTransform closeBtnRect = closeSelectionButton.GetComponent<RectTransform>();
        closeBtnRect.anchorMin = new Vector2(0.85f, 0.88f);
        closeBtnRect.anchorMax = new Vector2(0.98f, 0.98f);
        closeBtnRect.offsetMin = Vector2.zero;
        closeBtnRect.offsetMax = Vector2.zero;

        // Selection scroll view
        ScrollRect scrollView = CreateScrollView("SelectionScrollView", dialog.transform);
        RectTransform scrollRect = scrollView.GetComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0.05f, 0.05f);
        scrollRect.anchorMax = new Vector2(0.95f, 0.85f);
        scrollRect.offsetMin = Vector2.zero;
        scrollRect.offsetMax = Vector2.zero;

        selectionContainer = scrollView.content;
        VerticalLayoutGroup contentLayout = CreateVerticalLayout(selectionContainer.gameObject, 10, new RectOffset(10, 10, 10, 10));

        ContentSizeFitter sizeFitter = selectionContainer.gameObject.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // Create selection item prefab
        CreateSelectionItemPrefab();
    }

    private void CreateSelectionItemPrefab()
    {
        productionSelectionItemPrefab = new GameObject("ProductionSelectionItem", typeof(RectTransform));
        productionSelectionItemPrefab.transform.SetParent(screenRoot.transform, false);
        productionSelectionItemPrefab.SetActive(false);
        Undo.RegisterCreatedObjectUndo(productionSelectionItemPrefab, "Create ProductionSelectionItem");

        RectTransform itemRect = productionSelectionItemPrefab.GetComponent<RectTransform>();
        itemRect.sizeDelta = new Vector2(0, 60);

        Image itemBg = productionSelectionItemPrefab.AddComponent<Image>();
        itemBg.color = new Color(0.95f, 0.95f, 0.95f, 1f);

        Button itemBtn = productionSelectionItemPrefab.AddComponent<Button>();
        itemBtn.targetGraphic = itemBg;

        // Item content
        GameObject content = CreatePanel("Content", productionSelectionItemPrefab.transform);
        RectTransform contentRect = content.GetComponent<RectTransform>();
        SetFullStretch(contentRect, new Vector2(15, 10), new Vector2(-15, -10));

        HorizontalLayoutGroup layout = content.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 15;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = false;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = true;

        // Icon
        GameObject iconObj = new GameObject("Icon", typeof(RectTransform), typeof(CanvasRenderer));
        iconObj.transform.SetParent(content.transform, false);
        TextMeshProUGUI iconText = iconObj.AddComponent<TextMeshProUGUI>();
        iconText.fontSize = 28f;
        iconText.alignment = TextAlignmentOptions.Center;
        LayoutElement iconLayout = iconObj.AddComponent<LayoutElement>();
        iconLayout.preferredWidth = 40;

        // Name and time
        GameObject textContainer = CreatePanel("TextContainer", content.transform);
        VerticalLayoutGroup textLayout = CreateVerticalLayout(textContainer, 2);
        textLayout.childControlHeight = false;
        LayoutElement textLayoutElement = textContainer.AddComponent<LayoutElement>();
        textLayoutElement.flexibleWidth = 1;

        GameObject nameObj = new GameObject("Name", typeof(RectTransform), typeof(CanvasRenderer));
        nameObj.transform.SetParent(textContainer.transform, false);
        TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
        nameText.fontSize = FontSizes.Body;
        nameText.alignment = TextAlignmentOptions.Left;
        nameText.color = UIColors.Text;
        LayoutElement nameLayout = nameObj.AddComponent<LayoutElement>();
        nameLayout.preferredHeight = 25;

        GameObject timeObj = new GameObject("Time", typeof(RectTransform), typeof(CanvasRenderer));
        timeObj.transform.SetParent(textContainer.transform, false);
        TextMeshProUGUI timeText = timeObj.AddComponent<TextMeshProUGUI>();
        timeText.fontSize = FontSizes.Small;
        timeText.alignment = TextAlignmentOptions.Left;
        timeText.color = UIColors.TextLight;
        LayoutElement timeLayout = timeObj.AddComponent<LayoutElement>();
        timeLayout.preferredHeight = 20;

        // Layout element for whole item
        LayoutElement itemLayout = productionSelectionItemPrefab.AddComponent<LayoutElement>();
        itemLayout.preferredHeight = 60;
    }

    private void SetupController()
    {
        ProductionController controller = screenRoot.GetComponent<ProductionController>();
        if (controller == null)
        {
            controller = screenRoot.AddComponent<ProductionController>();
        }

        controller.InitializeReferences(
            productionSlots,
            inventoryContainer,
            inventoryIconPrefab,
            multiplierX1Button,
            multiplierX5Button,
            multiplierX10Button,
            multiplierX100Button,
            multiplierText,
            selectionModal,
            selectionContainer,
            productionSelectionItemPrefab,
            closeSelectionButton,
            plantTabButton,
            industryTabButton,
            plantTabText,
            industryTabText,
            titleText
        );
    }
}
#endif
