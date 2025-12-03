#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Editor script that builds the Boutique (Shop) screen UI.
/// </summary>
public class EditorBoutiqueScreenBuilder : EditorUIBuilderBase
{
    // UI References
    private TextMeshProUGUI coinBalanceText;
    private TextMeshProUGUI nextCoinTimerText;
    private Button iconsTabButton;
    private Button packsTabButton;
    private GameObject iconsPanel;
    private Transform iconsContainer;
    private GameObject shopIconPrefab;
    private GameObject packsPanel;
    private Transform packsContainer;
    private GameObject shopPackPrefab;
    private GameObject confirmationPanel;
    private TextMeshProUGUI confirmationText;
    private TextMeshProUGUI confirmationPriceText;
    private Button confirmButton;
    private Button cancelButton;
    private GameObject resultPanel;
    private TextMeshProUGUI resultTitleText;
    private TextMeshProUGUI resultMessageText;
    private Button resultCloseButton;
    private GameObject packOpeningPanel;
    private Transform packContentsContainer;
    private GameObject packIconDisplayPrefab;
    private Button closePackButton;
    private TMP_Dropdown rarityFilterDropdown;
    private TextMeshProUGUI titleText;

    [MenuItem("Icons/Build UI/Screens/Build Boutique Screen")]
    public static void BuildBoutiqueScreen()
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

        Transform existing = screensContainer.Find("BoutiqueScreen");
        if (existing != null)
        {
            Undo.DestroyObjectImmediate(existing.gameObject);
        }

        EditorBoutiqueScreenBuilder builder = new EditorBoutiqueScreenBuilder();
        builder.BuildScreen(screensContainer);
        
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("Boutique screen built successfully!");
    }

    public override void BuildScreen(Transform parent)
    {
        FindCanvas();

        screenRoot = CreateFullScreenPanel("BoutiqueScreen", parent, UIColors.Background);

        CreateHeader();
        CreateCoinDisplay();
        CreateTabButtons();
        CreateIconsPanel();
        CreatePacksPanel();
        CreateConfirmationPanel();
        CreateResultPanel();
        CreatePackOpeningPanel();

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

        titleText = CreateTitle("Title", header.transform, "🛒 Boutique");
        RectTransform titleRect = titleText.GetComponent<RectTransform>();
        SetFullStretch(titleRect);
        titleText.alignment = TextAlignmentOptions.Center;
    }

    private void CreateCoinDisplay()
    {
        GameObject coinContainer = CreatePanelWithBackground("CoinDisplay", screenRoot.transform, UIColors.White);
        RectTransform coinRect = coinContainer.GetComponent<RectTransform>();
        coinRect.anchorMin = new Vector2(0.05f, 0.85f);
        coinRect.anchorMax = new Vector2(0.95f, 0.91f);
        coinRect.offsetMin = Vector2.zero;
        coinRect.offsetMax = Vector2.zero;

        HorizontalLayoutGroup layout = CreateHorizontalLayout(coinContainer, 15, new RectOffset(15, 15, 5, 5));

        // Coin icon and balance
        TextMeshProUGUI coinIcon = CreateText("CoinIcon", coinContainer.transform, "🪙", 24f);
        AddLayoutElement(coinIcon.gameObject, preferredWidth: 35);

        coinBalanceText = CreateText("CoinBalance", coinContainer.transform, "0", FontSizes.Body, FontStyles.Bold);
        coinBalanceText.alignment = TextAlignmentOptions.Left;
        AddLayoutElement(coinBalanceText.gameObject, flexibleWidth: 1);

        // Next coin timer
        nextCoinTimerText = CreateText("NextCoinTimer", coinContainer.transform, "Prochaine: 59:59", FontSizes.Small);
        nextCoinTimerText.color = UIColors.TextLight;
        nextCoinTimerText.alignment = TextAlignmentOptions.Right;
        AddLayoutElement(nextCoinTimerText.gameObject, preferredWidth: 150);
    }

    private void CreateTabButtons()
    {
        GameObject tabContainer = CreatePanel("TabContainer", screenRoot.transform);
        RectTransform tabRect = tabContainer.GetComponent<RectTransform>();
        tabRect.anchorMin = new Vector2(0.05f, 0.78f);
        tabRect.anchorMax = new Vector2(0.95f, 0.84f);
        tabRect.offsetMin = Vector2.zero;
        tabRect.offsetMax = Vector2.zero;

        HorizontalLayoutGroup layout = CreateHorizontalLayout(tabContainer, 10);

        iconsTabButton = CreateButton("IconsTab", tabContainer.transform, "🎯 Icônes", UIColors.Primary);
        packsTabButton = CreateButton("PacksTab", tabContainer.transform, "📦 Packs", UIColors.Secondary);

        // Rarity filter dropdown
        CreateRarityFilter(tabContainer.transform);
    }

    private void CreateRarityFilter(Transform parent)
    {
        GameObject dropdownObj = new GameObject("RarityFilter", typeof(RectTransform));
        dropdownObj.transform.SetParent(parent, false);
        Undo.RegisterCreatedObjectUndo(dropdownObj, "Create RarityFilter");

        AddLayoutElement(dropdownObj, preferredWidth: 120);

        Image dropdownBg = dropdownObj.AddComponent<Image>();
        dropdownBg.color = UIColors.White;

        rarityFilterDropdown = dropdownObj.AddComponent<TMP_Dropdown>();

        // Label
        GameObject labelObj = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer));
        labelObj.transform.SetParent(dropdownObj.transform, false);
        TextMeshProUGUI label = labelObj.AddComponent<TextMeshProUGUI>();
        label.text = "Tous";
        label.fontSize = FontSizes.Small;
        label.alignment = TextAlignmentOptions.Left;
        label.color = UIColors.Text;
        RectTransform labelRect = labelObj.GetComponent<RectTransform>();
        SetFullStretch(labelRect, new Vector2(10, 5), new Vector2(-25, -5));

        // Arrow
        GameObject arrowObj = new GameObject("Arrow", typeof(RectTransform), typeof(CanvasRenderer));
        arrowObj.transform.SetParent(dropdownObj.transform, false);
        TextMeshProUGUI arrow = arrowObj.AddComponent<TextMeshProUGUI>();
        arrow.text = "▼";
        arrow.fontSize = 10f;
        arrow.alignment = TextAlignmentOptions.Center;
        RectTransform arrowRect = arrowObj.GetComponent<RectTransform>();
        arrowRect.anchorMin = new Vector2(1, 0);
        arrowRect.anchorMax = Vector2.one;
        arrowRect.sizeDelta = new Vector2(25, 0);
        arrowRect.anchoredPosition = new Vector2(-12.5f, 0);

        // Template (hidden)
        GameObject template = CreatePanelWithBackground("Template", dropdownObj.transform, UIColors.White);
        RectTransform templateRect = template.GetComponent<RectTransform>();
        templateRect.anchorMin = new Vector2(0, 0);
        templateRect.anchorMax = new Vector2(1, 0);
        templateRect.pivot = new Vector2(0.5f, 1);
        templateRect.sizeDelta = new Vector2(0, 150);

        ScrollRect scrollRect = template.AddComponent<ScrollRect>();

        GameObject viewport = CreatePanel("Viewport", template.transform);
        RectTransform viewportRect = viewport.GetComponent<RectTransform>();
        SetFullStretch(viewportRect);
        viewport.AddComponent<Mask>().showMaskGraphic = false;
        viewport.AddComponent<Image>().color = UIColors.White;

        GameObject content = CreatePanel("Content", viewport.transform);
        RectTransform contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = Vector2.one;
        contentRect.pivot = new Vector2(0.5f, 1);

        scrollRect.content = contentRect;
        scrollRect.viewport = viewportRect;

        // Item template
        GameObject item = CreatePanelWithBackground("Item", content.transform, UIColors.White);
        RectTransform itemRect = item.GetComponent<RectTransform>();
        itemRect.sizeDelta = new Vector2(0, 30);

        Toggle itemToggle = item.AddComponent<Toggle>();

        GameObject itemLabel = new GameObject("Item Label", typeof(RectTransform), typeof(CanvasRenderer));
        itemLabel.transform.SetParent(item.transform, false);
        TextMeshProUGUI itemLabelText = itemLabel.AddComponent<TextMeshProUGUI>();
        itemLabelText.fontSize = FontSizes.Small;
        itemLabelText.alignment = TextAlignmentOptions.Left;
        itemLabelText.color = UIColors.Text;
        RectTransform itemLabelRect = itemLabel.GetComponent<RectTransform>();
        SetFullStretch(itemLabelRect, new Vector2(10, 2), new Vector2(-10, -2));

        rarityFilterDropdown.template = templateRect;
        rarityFilterDropdown.captionText = label;
        rarityFilterDropdown.itemText = itemLabelText;

        // Add options
        rarityFilterDropdown.options.Clear();
        rarityFilterDropdown.options.Add(new TMP_Dropdown.OptionData("Tous"));
        rarityFilterDropdown.options.Add(new TMP_Dropdown.OptionData("Commun"));
        rarityFilterDropdown.options.Add(new TMP_Dropdown.OptionData("Peu commun"));
        rarityFilterDropdown.options.Add(new TMP_Dropdown.OptionData("Rare"));
        rarityFilterDropdown.options.Add(new TMP_Dropdown.OptionData("Légendaire"));

        template.SetActive(false);
    }

    private void CreateIconsPanel()
    {
        iconsPanel = CreatePanel("IconsPanel", screenRoot.transform);
        RectTransform panelRect = iconsPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 0.02f);
        panelRect.anchorMax = new Vector2(1, 0.77f);
        panelRect.offsetMin = new Vector2(10, 0);
        panelRect.offsetMax = new Vector2(-10, 0);

        ScrollRect scrollView = CreateScrollView("IconsScrollView", iconsPanel.transform);
        RectTransform scrollRect = scrollView.GetComponent<RectTransform>();
        SetFullStretch(scrollRect);

        iconsContainer = scrollView.content;
        GridLayoutGroup gridLayout = CreateGridLayout(iconsContainer.gameObject, new Vector2(100, 130), new Vector2(10, 10));
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 3;
        gridLayout.padding = new RectOffset(10, 10, 10, 10);

        ContentSizeFitter sizeFitter = iconsContainer.gameObject.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        CreateShopIconPrefab();
    }

    private void CreateShopIconPrefab()
    {
        shopIconPrefab = new GameObject("ShopIconTemplate", typeof(RectTransform));
        shopIconPrefab.transform.SetParent(screenRoot.transform, false);
        shopIconPrefab.SetActive(false);
        Undo.RegisterCreatedObjectUndo(shopIconPrefab, "Create ShopIconTemplate");

        Image iconBg = shopIconPrefab.AddComponent<Image>();
        iconBg.color = UIColors.White;

        Button iconBtn = shopIconPrefab.AddComponent<Button>();
        iconBtn.targetGraphic = iconBg;

        // Icon
        GameObject iconObj = new GameObject("Icon", typeof(RectTransform), typeof(CanvasRenderer));
        iconObj.transform.SetParent(shopIconPrefab.transform, false);
        TextMeshProUGUI iconText = iconObj.AddComponent<TextMeshProUGUI>();
        iconText.fontSize = 36f;
        iconText.alignment = TextAlignmentOptions.Center;
        RectTransform iconRect = iconObj.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0, 0.4f);
        iconRect.anchorMax = Vector2.one;
        iconRect.offsetMin = Vector2.zero;
        iconRect.offsetMax = Vector2.zero;

        // Name
        GameObject nameObj = new GameObject("Name", typeof(RectTransform), typeof(CanvasRenderer));
        nameObj.transform.SetParent(shopIconPrefab.transform, false);
        TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
        nameText.fontSize = FontSizes.Tiny;
        nameText.alignment = TextAlignmentOptions.Center;
        nameText.color = UIColors.Text;
        RectTransform nameRect = nameObj.GetComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0.2f);
        nameRect.anchorMax = new Vector2(1, 0.4f);
        nameRect.offsetMin = new Vector2(5, 0);
        nameRect.offsetMax = new Vector2(-5, 0);

        // Price
        GameObject priceObj = new GameObject("Price", typeof(RectTransform), typeof(CanvasRenderer));
        priceObj.transform.SetParent(shopIconPrefab.transform, false);
        TextMeshProUGUI priceText = priceObj.AddComponent<TextMeshProUGUI>();
        priceText.fontSize = FontSizes.Small;
        priceText.alignment = TextAlignmentOptions.Center;
        priceText.color = UIColors.Primary;
        RectTransform priceRect = priceObj.GetComponent<RectTransform>();
        priceRect.anchorMin = Vector2.zero;
        priceRect.anchorMax = new Vector2(1, 0.2f);
        priceRect.offsetMin = Vector2.zero;
        priceRect.offsetMax = Vector2.zero;

        // Add ShopIconDisplay component
        ShopIconDisplay display = shopIconPrefab.AddComponent<ShopIconDisplay>();
        display.InitializeReferences(iconText, nameText, priceText);
    }

    private void CreatePacksPanel()
    {
        packsPanel = CreatePanel("PacksPanel", screenRoot.transform);
        RectTransform panelRect = packsPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 0.02f);
        panelRect.anchorMax = new Vector2(1, 0.77f);
        panelRect.offsetMin = new Vector2(10, 0);
        panelRect.offsetMax = new Vector2(-10, 0);
        packsPanel.SetActive(false);

        ScrollRect scrollView = CreateScrollView("PacksScrollView", packsPanel.transform);
        RectTransform scrollRect = scrollView.GetComponent<RectTransform>();
        SetFullStretch(scrollRect);

        packsContainer = scrollView.content;
        VerticalLayoutGroup layout = CreateVerticalLayout(packsContainer.gameObject, 15, new RectOffset(10, 10, 10, 10));

        ContentSizeFitter sizeFitter = packsContainer.gameObject.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        CreateShopPackPrefab();
    }

    private void CreateShopPackPrefab()
    {
        shopPackPrefab = new GameObject("ShopPackTemplate", typeof(RectTransform));
        shopPackPrefab.transform.SetParent(screenRoot.transform, false);
        shopPackPrefab.SetActive(false);
        Undo.RegisterCreatedObjectUndo(shopPackPrefab, "Create ShopPackTemplate");

        RectTransform packRect = shopPackPrefab.GetComponent<RectTransform>();
        packRect.sizeDelta = new Vector2(0, 120);

        Image packBg = shopPackPrefab.AddComponent<Image>();
        packBg.color = UIColors.White;

        Button packBtn = shopPackPrefab.AddComponent<Button>();
        packBtn.targetGraphic = packBg;

        // Content
        GameObject content = CreatePanel("Content", shopPackPrefab.transform);
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
        iconText.text = "📦";
        iconText.fontSize = 48f;
        iconText.alignment = TextAlignmentOptions.Center;
        LayoutElement iconLayout = iconObj.AddComponent<LayoutElement>();
        iconLayout.preferredWidth = 60;

        // Info container
        GameObject infoContainer = CreatePanel("Info", content.transform);
        VerticalLayoutGroup infoLayout = CreateVerticalLayout(infoContainer, 3);
        infoLayout.childControlHeight = false;
        LayoutElement infoLayoutElement = infoContainer.AddComponent<LayoutElement>();
        infoLayoutElement.flexibleWidth = 1;

        // Name
        GameObject nameObj = new GameObject("Name", typeof(RectTransform), typeof(CanvasRenderer));
        nameObj.transform.SetParent(infoContainer.transform, false);
        TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
        nameText.fontSize = FontSizes.Body;
        nameText.fontStyle = FontStyles.Bold;
        nameText.alignment = TextAlignmentOptions.Left;
        LayoutElement nameLayout = nameObj.AddComponent<LayoutElement>();
        nameLayout.preferredHeight = 25;

        // Description
        GameObject descObj = new GameObject("Description", typeof(RectTransform), typeof(CanvasRenderer));
        descObj.transform.SetParent(infoContainer.transform, false);
        TextMeshProUGUI descText = descObj.AddComponent<TextMeshProUGUI>();
        descText.fontSize = FontSizes.Small;
        descText.color = UIColors.TextLight;
        descText.alignment = TextAlignmentOptions.Left;
        LayoutElement descLayout = descObj.AddComponent<LayoutElement>();
        descLayout.preferredHeight = 40;

        // Price
        GameObject priceObj = new GameObject("Price", typeof(RectTransform), typeof(CanvasRenderer));
        priceObj.transform.SetParent(infoContainer.transform, false);
        TextMeshProUGUI priceText = priceObj.AddComponent<TextMeshProUGUI>();
        priceText.fontSize = FontSizes.Body;
        priceText.color = UIColors.Success;
        priceText.alignment = TextAlignmentOptions.Left;
        LayoutElement priceLayout = priceObj.AddComponent<LayoutElement>();
        priceLayout.preferredHeight = 20;

        // Layout element for whole pack
        LayoutElement packLayout = shopPackPrefab.AddComponent<LayoutElement>();
        packLayout.preferredHeight = 120;

        // Add ShopPackDisplay component
        ShopPackDisplay display = shopPackPrefab.AddComponent<ShopPackDisplay>();
        display.InitializeReferences(iconText, nameText, descText, priceText);
    }

    private void CreateConfirmationPanel()
    {
        confirmationPanel = CreateModalOverlay("ConfirmationPanel", screenRoot.transform);
        confirmationPanel.SetActive(false);

        GameObject dialog = CreateDialogBox("Dialog", confirmationPanel.transform, new Vector2(450, 280));

        confirmationText = CreateText("ConfirmText", dialog.transform, "Acheter cet article ?", FontSizes.Body);
        RectTransform textRect = confirmationText.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 0.55f);
        textRect.anchorMax = new Vector2(1, 0.9f);
        textRect.offsetMin = new Vector2(20, 0);
        textRect.offsetMax = new Vector2(-20, 0);

        confirmationPriceText = CreateText("PriceText", dialog.transform, "100 pièces", FontSizes.Subtitle, FontStyles.Bold);
        confirmationPriceText.color = UIColors.Primary;
        RectTransform priceRect = confirmationPriceText.GetComponent<RectTransform>();
        priceRect.anchorMin = new Vector2(0, 0.38f);
        priceRect.anchorMax = new Vector2(1, 0.55f);
        priceRect.offsetMin = new Vector2(20, 0);
        priceRect.offsetMax = new Vector2(-20, 0);

        // Buttons
        GameObject buttonContainer = CreatePanel("ButtonContainer", dialog.transform);
        RectTransform btnContainerRect = buttonContainer.GetComponent<RectTransform>();
        btnContainerRect.anchorMin = new Vector2(0.1f, 0.08f);
        btnContainerRect.anchorMax = new Vector2(0.9f, 0.32f);
        btnContainerRect.offsetMin = Vector2.zero;
        btnContainerRect.offsetMax = Vector2.zero;

        HorizontalLayoutGroup btnLayout = CreateHorizontalLayout(buttonContainer, 20);

        cancelButton = CreateButton("CancelButton", buttonContainer.transform, "Annuler", UIColors.Secondary);
        confirmButton = CreateButton("ConfirmButton", buttonContainer.transform, "Acheter", UIColors.Success);
    }

    private void CreateResultPanel()
    {
        resultPanel = CreateModalOverlay("ResultPanel", screenRoot.transform);
        resultPanel.SetActive(false);

        GameObject dialog = CreateDialogBox("Dialog", resultPanel.transform, new Vector2(400, 250));

        resultTitleText = CreateTitle("ResultTitle", dialog.transform, "Succès !");
        RectTransform titleRect = resultTitleText.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.65f);
        titleRect.anchorMax = new Vector2(1, 0.9f);
        titleRect.offsetMin = new Vector2(20, 0);
        titleRect.offsetMax = new Vector2(-20, 0);

        resultMessageText = CreateText("ResultMessage", dialog.transform, "", FontSizes.Body);
        RectTransform msgRect = resultMessageText.GetComponent<RectTransform>();
        msgRect.anchorMin = new Vector2(0, 0.35f);
        msgRect.anchorMax = new Vector2(1, 0.65f);
        msgRect.offsetMin = new Vector2(20, 0);
        msgRect.offsetMax = new Vector2(-20, 0);

        resultCloseButton = CreateButton("CloseButton", dialog.transform, "OK", UIColors.Primary);
        RectTransform closeRect = resultCloseButton.GetComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(0.25f, 0.08f);
        closeRect.anchorMax = new Vector2(0.75f, 0.28f);
        closeRect.offsetMin = Vector2.zero;
        closeRect.offsetMax = Vector2.zero;
    }

    private void CreatePackOpeningPanel()
    {
        packOpeningPanel = CreateModalOverlay("PackOpeningPanel", screenRoot.transform);
        packOpeningPanel.SetActive(false);

        GameObject dialog = CreateDialogBox("Dialog", packOpeningPanel.transform, new Vector2(550, 500));

        TextMeshProUGUI packTitle = CreateTitle("PackTitle", dialog.transform, "🎁 Pack ouvert !");
        RectTransform titleRect = packTitle.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.85f);
        titleRect.anchorMax = new Vector2(1, 0.95f);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;

        // Contents scroll view
        ScrollRect scrollView = CreateScrollView("ContentsScrollView", dialog.transform);
        RectTransform scrollRect = scrollView.GetComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0.05f, 0.18f);
        scrollRect.anchorMax = new Vector2(0.95f, 0.82f);
        scrollRect.offsetMin = Vector2.zero;
        scrollRect.offsetMax = Vector2.zero;

        packContentsContainer = scrollView.content;
        GridLayoutGroup gridLayout = CreateGridLayout(packContentsContainer.gameObject, new Vector2(80, 100), new Vector2(10, 10));
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 4;
        gridLayout.padding = new RectOffset(10, 10, 10, 10);

        ContentSizeFitter sizeFitter = packContentsContainer.gameObject.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        CreatePackIconDisplayPrefab();

        closePackButton = CreateButton("ClosePackButton", dialog.transform, "Fermer", UIColors.Primary);
        RectTransform closeRect = closePackButton.GetComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(0.25f, 0.03f);
        closeRect.anchorMax = new Vector2(0.75f, 0.14f);
        closeRect.offsetMin = Vector2.zero;
        closeRect.offsetMax = Vector2.zero;
    }

    private void CreatePackIconDisplayPrefab()
    {
        packIconDisplayPrefab = new GameObject("PackIconDisplay", typeof(RectTransform));
        packIconDisplayPrefab.transform.SetParent(screenRoot.transform, false);
        packIconDisplayPrefab.SetActive(false);
        Undo.RegisterCreatedObjectUndo(packIconDisplayPrefab, "Create PackIconDisplay");

        Image iconBg = packIconDisplayPrefab.AddComponent<Image>();
        iconBg.color = UIColors.White;

        // Icon
        GameObject iconObj = new GameObject("Icon", typeof(RectTransform), typeof(CanvasRenderer));
        iconObj.transform.SetParent(packIconDisplayPrefab.transform, false);
        TextMeshProUGUI iconText = iconObj.AddComponent<TextMeshProUGUI>();
        iconText.fontSize = 32f;
        iconText.alignment = TextAlignmentOptions.Center;
        RectTransform iconRect = iconObj.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0, 0.3f);
        iconRect.anchorMax = Vector2.one;
        iconRect.offsetMin = Vector2.zero;
        iconRect.offsetMax = Vector2.zero;

        // Rarity indicator
        GameObject rarityObj = new GameObject("Rarity", typeof(RectTransform), typeof(CanvasRenderer));
        rarityObj.transform.SetParent(packIconDisplayPrefab.transform, false);
        TextMeshProUGUI rarityText = rarityObj.AddComponent<TextMeshProUGUI>();
        rarityText.fontSize = FontSizes.Tiny;
        rarityText.alignment = TextAlignmentOptions.Center;
        RectTransform rarityRect = rarityObj.GetComponent<RectTransform>();
        rarityRect.anchorMin = Vector2.zero;
        rarityRect.anchorMax = new Vector2(1, 0.3f);
        rarityRect.offsetMin = Vector2.zero;
        rarityRect.offsetMax = Vector2.zero;
    }

    private void SetupController()
    {
        ShopController controller = screenRoot.GetComponent<ShopController>();
        if (controller == null)
        {
            controller = screenRoot.AddComponent<ShopController>();
        }

        controller.InitializeReferences(
            coinBalanceText,
            nextCoinTimerText,
            iconsTabButton,
            packsTabButton,
            iconsPanel,
            iconsContainer,
            shopIconPrefab,
            packsPanel,
            packsContainer,
            shopPackPrefab,
            confirmationPanel,
            confirmationText,
            confirmationPriceText,
            confirmButton,
            cancelButton,
            resultPanel,
            resultTitleText,
            resultMessageText,
            resultCloseButton,
            packOpeningPanel,
            packContentsContainer,
            packIconDisplayPrefab,
            closePackButton,
            rarityFilterDropdown,
            titleText
        );
    }
}
#endif
