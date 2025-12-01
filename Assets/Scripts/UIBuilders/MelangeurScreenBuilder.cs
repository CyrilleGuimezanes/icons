using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Builds the Mélangeur (Mixer) screen UI at runtime.
/// </summary>
public class MelangeurScreenBuilder : UIBuilderBase
{
    private MixerController mixerController;
    private MixerSlot[] mixerSlots;
    private Transform inventoryContainer;
    private GameObject inventoryIconPrefab;

    // UI References for controller
    private Button multiplierX1Button;
    private Button multiplierX5Button;
    private Button multiplierX10Button;
    private Button multiplierX100Button;
    private Button mixButton;
    private TextMeshProUGUI mixButtonText;
    private GameObject resultPanel;
    private TextMeshProUGUI resultIconText;
    private TextMeshProUGUI resultMessageText;
    private TextMeshProUGUI titleText;

    public override void BuildScreen()
    {
        FindCanvas();

        // Create screen root
        screenRoot = CreateFullScreenPanel("MelangeurScreen", transform, UIColors.Background);

        // Build UI sections
        CreateHeader();
        CreateMixerGrid();
        CreateMultiplierButtons();
        CreateMixButton();
        CreateInventoryPanel();
        CreateResultPanel();

        // Add and configure controller
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

        titleText = CreateTitle("Title", header.transform, "🧪 Mélangeur");
        RectTransform titleRect = titleText.GetComponent<RectTransform>();
        SetFullStretch(titleRect);
        titleText.alignment = TextAlignmentOptions.Center;
    }

    private void CreateMixerGrid()
    {
        // Create grid container
        GameObject gridContainer = CreatePanelWithBackground("MixerGrid", screenRoot.transform, UIColors.White);
        RectTransform gridRect = gridContainer.GetComponent<RectTransform>();
        gridRect.anchorMin = new Vector2(0.1f, 0.55f);
        gridRect.anchorMax = new Vector2(0.9f, 0.9f);
        gridRect.offsetMin = Vector2.zero;
        gridRect.offsetMax = Vector2.zero;

        // Add grid layout
        GridLayoutGroup gridLayout = CreateGridLayout(gridContainer, new Vector2(90, 90), new Vector2(10, 10));
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 3;
        gridLayout.childAlignment = TextAnchor.MiddleCenter;
        gridLayout.padding = new RectOffset(20, 20, 20, 20);

        // Create 9 mixer slots
        mixerSlots = new MixerSlot[9];
        for (int i = 0; i < 9; i++)
        {
            mixerSlots[i] = CreateMixerSlot(gridContainer.transform, i);
        }
    }

    private MixerSlot CreateMixerSlot(Transform parent, int index)
    {
        GameObject slotObj = new GameObject($"MixerSlot_{index}", typeof(RectTransform), typeof(CanvasRenderer));
        slotObj.transform.SetParent(parent, false);

        // Background
        Image slotBg = slotObj.AddComponent<Image>();
        slotBg.color = new Color(0.95f, 0.95f, 0.95f, 1f);

        // Icon text
        TextMeshProUGUI iconText = CreateText("Icon", slotObj.transform, "", 36f);
        RectTransform iconRect = iconText.GetComponent<RectTransform>();
        SetFullStretch(iconRect);

        // Add button for interaction
        Button slotButton = slotObj.AddComponent<Button>();
        slotButton.targetGraphic = slotBg;

        // Add MixerSlot component
        MixerSlot slot = slotObj.AddComponent<MixerSlot>();
        slot.InitializeReferences(iconText, slotBg);

        return slot;
    }

    private void CreateMultiplierButtons()
    {
        GameObject multiplierContainer = CreatePanel("MultiplierContainer", screenRoot.transform);
        RectTransform containerRect = multiplierContainer.GetComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.1f, 0.48f);
        containerRect.anchorMax = new Vector2(0.9f, 0.54f);
        containerRect.offsetMin = Vector2.zero;
        containerRect.offsetMax = Vector2.zero;

        // Add horizontal layout
        HorizontalLayoutGroup layout = CreateHorizontalLayout(multiplierContainer, 10);

        // Create multiplier buttons
        multiplierX1Button = CreateButton("x1Button", multiplierContainer.transform, "x1", UIColors.Secondary);
        multiplierX5Button = CreateButton("x5Button", multiplierContainer.transform, "x5", UIColors.Secondary);
        multiplierX10Button = CreateButton("x10Button", multiplierContainer.transform, "x10", UIColors.Secondary);
        multiplierX100Button = CreateButton("x100Button", multiplierContainer.transform, "x100", UIColors.Secondary);
    }

    private void CreateMixButton()
    {
        GameObject mixButtonContainer = CreatePanel("MixButtonContainer", screenRoot.transform);
        RectTransform containerRect = mixButtonContainer.GetComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.2f, 0.40f);
        containerRect.anchorMax = new Vector2(0.8f, 0.47f);
        containerRect.offsetMin = Vector2.zero;
        containerRect.offsetMax = Vector2.zero;

        mixButton = CreateButton("MixButton", mixButtonContainer.transform, "Fusionner", UIColors.Primary);
        RectTransform btnRect = mixButton.GetComponent<RectTransform>();
        SetFullStretch(btnRect);

        mixButtonText = mixButton.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void CreateInventoryPanel()
    {
        // Inventory section header
        GameObject inventoryHeader = CreatePanel("InventoryHeader", screenRoot.transform);
        RectTransform headerRect = inventoryHeader.GetComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0, 0.34f);
        headerRect.anchorMax = new Vector2(1, 0.40f);
        headerRect.offsetMin = new Vector2(20, 0);
        headerRect.offsetMax = new Vector2(-20, 0);

        TextMeshProUGUI inventoryTitle = CreateSubtitle("InventoryTitle", inventoryHeader.transform, "Inventaire");
        RectTransform titleRect = inventoryTitle.GetComponent<RectTransform>();
        SetFullStretch(titleRect);
        inventoryTitle.alignment = TextAlignmentOptions.Left;

        // Inventory scroll view
        ScrollRect scrollView = CreateScrollView("InventoryScrollView", screenRoot.transform, true, false);
        RectTransform scrollRect = scrollView.GetComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0, 0.02f);
        scrollRect.anchorMax = new Vector2(1, 0.34f);
        scrollRect.offsetMin = new Vector2(10, 0);
        scrollRect.offsetMax = new Vector2(-10, 0);

        // Configure content for horizontal scrolling
        inventoryContainer = scrollView.content;
        RectTransform contentRect = inventoryContainer.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 0);
        contentRect.anchorMax = new Vector2(0, 1);
        contentRect.pivot = new Vector2(0, 0.5f);

        // Add horizontal layout to content
        HorizontalLayoutGroup contentLayout = inventoryContainer.gameObject.AddComponent<HorizontalLayoutGroup>();
        contentLayout.spacing = 10;
        contentLayout.padding = new RectOffset(10, 10, 10, 10);
        contentLayout.childAlignment = TextAnchor.MiddleLeft;
        contentLayout.childControlWidth = false;
        contentLayout.childControlHeight = true;
        contentLayout.childForceExpandWidth = false;
        contentLayout.childForceExpandHeight = true;

        // Add content size fitter for dynamic width
        ContentSizeFitter sizeFitter = inventoryContainer.gameObject.AddComponent<ContentSizeFitter>();
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;

        // Create inventory icon prefab template (will be used for instantiation)
        CreateInventoryIconPrefab();
    }

    private void CreateInventoryIconPrefab()
    {
        // Create a template for inventory icons
        inventoryIconPrefab = new GameObject("InventoryIconTemplate", typeof(RectTransform));
        inventoryIconPrefab.SetActive(false);

        RectTransform iconRect = inventoryIconPrefab.GetComponent<RectTransform>();
        iconRect.sizeDelta = new Vector2(80, 80);

        // Background
        Image iconBg = inventoryIconPrefab.AddComponent<Image>();
        iconBg.color = UIColors.White;

        // Button
        Button iconBtn = inventoryIconPrefab.AddComponent<Button>();
        iconBtn.targetGraphic = iconBg;

        // Icon text
        GameObject iconTextObj = new GameObject("IconText", typeof(RectTransform), typeof(CanvasRenderer));
        iconTextObj.transform.SetParent(inventoryIconPrefab.transform, false);
        TextMeshProUGUI iconText = iconTextObj.AddComponent<TextMeshProUGUI>();
        iconText.fontSize = 32f;
        iconText.alignment = TextAlignmentOptions.Center;
        iconText.color = UIColors.Text;
        RectTransform iconTextRect = iconTextObj.GetComponent<RectTransform>();
        iconTextRect.anchorMin = new Vector2(0, 0.3f);
        iconTextRect.anchorMax = Vector2.one;
        iconTextRect.offsetMin = Vector2.zero;
        iconTextRect.offsetMax = Vector2.zero;

        // Quantity text
        GameObject qtyTextObj = new GameObject("QuantityText", typeof(RectTransform), typeof(CanvasRenderer));
        qtyTextObj.transform.SetParent(inventoryIconPrefab.transform, false);
        TextMeshProUGUI qtyText = qtyTextObj.AddComponent<TextMeshProUGUI>();
        qtyText.fontSize = 12f;
        qtyText.alignment = TextAlignmentOptions.Center;
        qtyText.color = UIColors.TextLight;
        RectTransform qtyRect = qtyTextObj.GetComponent<RectTransform>();
        qtyRect.anchorMin = Vector2.zero;
        qtyRect.anchorMax = new Vector2(1, 0.3f);
        qtyRect.offsetMin = Vector2.zero;
        qtyRect.offsetMax = Vector2.zero;

        // Add InventoryIconDisplay component
        InventoryIconDisplay display = inventoryIconPrefab.AddComponent<InventoryIconDisplay>();
        display.InitializeReferences(iconText, qtyText);
    }

    private void CreateResultPanel()
    {
        // Create result panel overlay
        resultPanel = CreateModalOverlay("ResultPanel", screenRoot.transform);
        resultPanel.SetActive(false);

        // Create dialog
        GameObject dialog = CreateDialogBox("Dialog", resultPanel.transform, new Vector2(400, 300));

        // Result icon
        resultIconText = CreateText("ResultIcon", dialog.transform, "", 64f);
        RectTransform iconRect = resultIconText.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0, 0.5f);
        iconRect.anchorMax = new Vector2(1, 0.9f);
        iconRect.offsetMin = Vector2.zero;
        iconRect.offsetMax = Vector2.zero;

        // Result message
        resultMessageText = CreateText("ResultMessage", dialog.transform, "", FontSizes.Subtitle);
        RectTransform msgRect = resultMessageText.GetComponent<RectTransform>();
        msgRect.anchorMin = new Vector2(0, 0.2f);
        msgRect.anchorMax = new Vector2(1, 0.5f);
        msgRect.offsetMin = new Vector2(20, 0);
        msgRect.offsetMax = new Vector2(-20, 0);

        // Close button
        Button closeBtn = CreateButton("CloseButton", dialog.transform, "OK", UIColors.Primary);
        RectTransform closeBtnRect = closeBtn.GetComponent<RectTransform>();
        closeBtnRect.anchorMin = new Vector2(0.3f, 0.05f);
        closeBtnRect.anchorMax = new Vector2(0.7f, 0.18f);
        closeBtnRect.offsetMin = Vector2.zero;
        closeBtnRect.offsetMax = Vector2.zero;

        closeBtn.onClick.AddListener(() => resultPanel.SetActive(false));
    }

    private void SetupController()
    {
        // Add MixerController (alias MelangeurController)
        mixerController = screenRoot.AddComponent<MelangeurController>();

        // Initialize controller references
        mixerController.InitializeReferences(
            mixerSlots,
            inventoryContainer,
            inventoryIconPrefab,
            multiplierX1Button,
            multiplierX5Button,
            multiplierX10Button,
            multiplierX100Button,
            mixButton,
            mixButtonText,
            resultPanel,
            resultIconText,
            resultMessageText,
            titleText
        );
    }

    /// <summary>
    /// Gets the MixerController component.
    /// </summary>
    public MixerController GetController() => mixerController;
}
