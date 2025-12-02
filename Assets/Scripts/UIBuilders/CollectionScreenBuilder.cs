using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Builds the Collection screen UI at runtime.
/// </summary>
public class CollectionScreenBuilder : UIBuilderBase
{
    private CollectionController collectionController;

    // UI References
    private Transform gridContainer;
    private GameObject collectionIconPrefab;
    private TextMeshProUGUI totalUnlockedText;
    private TextMeshProUGUI progressText;
    private Button allFilterButton;
    private Button commonFilterButton;
    private Button uncommonFilterButton;
    private Button rareFilterButton;
    private Button legendaryFilterButton;
    private TextMeshProUGUI titleText;

    public override void BuildScreen()
    {
        FindCanvas();

        screenRoot = CreateFullScreenPanel("CollectionScreen", transform, UIColors.Background);

        CreateHeader();
        CreateStatistics();
        CreateFilterButtons();
        CreateCollectionGrid();

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

        titleText = CreateTitle("Title", header.transform, "📚 Collection");
        RectTransform titleRect = titleText.GetComponent<RectTransform>();
        SetFullStretch(titleRect);
        titleText.alignment = TextAlignmentOptions.Center;
    }

    private void CreateStatistics()
    {
        GameObject statsContainer = CreatePanelWithBackground("Statistics", screenRoot.transform, UIColors.White);
        RectTransform statsRect = statsContainer.GetComponent<RectTransform>();
        statsRect.anchorMin = new Vector2(0.05f, 0.83f);
        statsRect.anchorMax = new Vector2(0.95f, 0.91f);
        statsRect.offsetMin = Vector2.zero;
        statsRect.offsetMax = Vector2.zero;

        HorizontalLayoutGroup layout = CreateHorizontalLayout(statsContainer, 20, new RectOffset(20, 20, 10, 10));

        // Total unlocked
        GameObject unlockedContainer = CreatePanel("UnlockedContainer", statsContainer.transform);
        VerticalLayoutGroup unlockedLayout = CreateVerticalLayout(unlockedContainer, 2);
        unlockedLayout.childAlignment = TextAnchor.MiddleCenter;
        AddLayoutElement(unlockedContainer, flexibleWidth: 1);

        TextMeshProUGUI unlockedLabel = CreateText("UnlockedLabel", unlockedContainer.transform, "Débloquées", FontSizes.Small);
        unlockedLabel.color = UIColors.TextLight;
        AddLayoutElement(unlockedLabel.gameObject, preferredHeight: 18);

        totalUnlockedText = CreateText("UnlockedCount", unlockedContainer.transform, "0 / 100", FontSizes.Body, FontStyles.Bold);
        AddLayoutElement(totalUnlockedText.gameObject, preferredHeight: 25);

        // Progress
        GameObject progressContainer = CreatePanel("ProgressContainer", statsContainer.transform);
        VerticalLayoutGroup progressLayout = CreateVerticalLayout(progressContainer, 2);
        progressLayout.childAlignment = TextAnchor.MiddleCenter;
        AddLayoutElement(progressContainer, flexibleWidth: 1);

        TextMeshProUGUI progressLabel = CreateText("ProgressLabel", progressContainer.transform, "Progression", FontSizes.Small);
        progressLabel.color = UIColors.TextLight;
        AddLayoutElement(progressLabel.gameObject, preferredHeight: 18);

        progressText = CreateText("ProgressPercent", progressContainer.transform, "0%", FontSizes.Body, FontStyles.Bold);
        progressText.color = UIColors.Primary;
        AddLayoutElement(progressText.gameObject, preferredHeight: 25);
    }

    private void CreateFilterButtons()
    {
        GameObject filterContainer = CreatePanel("FilterContainer", screenRoot.transform);
        RectTransform filterRect = filterContainer.GetComponent<RectTransform>();
        filterRect.anchorMin = new Vector2(0.02f, 0.75f);
        filterRect.anchorMax = new Vector2(0.98f, 0.82f);
        filterRect.offsetMin = Vector2.zero;
        filterRect.offsetMax = Vector2.zero;

        HorizontalLayoutGroup layout = CreateHorizontalLayout(filterContainer, 5);

        // All filter
        allFilterButton = CreateFilterButton("AllFilter", filterContainer.transform, "Tous", UIColors.Primary);

        // Common filter
        commonFilterButton = CreateFilterButton("CommonFilter", filterContainer.transform, "Commun", UIColors.Common);

        // Uncommon filter
        uncommonFilterButton = CreateFilterButton("UncommonFilter", filterContainer.transform, "Peu commun", UIColors.Uncommon);

        // Rare filter
        rareFilterButton = CreateFilterButton("RareFilter", filterContainer.transform, "Rare", UIColors.Rare);

        // Legendary filter
        legendaryFilterButton = CreateFilterButton("LegendaryFilter", filterContainer.transform, "Légendaire", UIColors.Legendary);
    }

    private Button CreateFilterButton(string name, Transform parent, string text, Color color)
    {
        Button btn = CreateButton(name, parent, text, color);
        TextMeshProUGUI btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (btnText != null)
        {
            btnText.fontSize = FontSizes.Tiny;
        }
        return btn;
    }

    private void CreateCollectionGrid()
    {
        // Scroll view for collection
        ScrollRect scrollView = CreateScrollView("CollectionScrollView", screenRoot.transform);
        RectTransform scrollRect = scrollView.GetComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0, 0.02f);
        scrollRect.anchorMax = new Vector2(1, 0.74f);
        scrollRect.offsetMin = new Vector2(10, 0);
        scrollRect.offsetMax = new Vector2(-10, 0);

        gridContainer = scrollView.content;
        GridLayoutGroup gridLayout = CreateGridLayout(gridContainer.gameObject, new Vector2(90, 110), new Vector2(10, 10));
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 3;
        gridLayout.padding = new RectOffset(10, 10, 10, 10);
        gridLayout.childAlignment = TextAnchor.UpperCenter;

        ContentSizeFitter sizeFitter = gridContainer.gameObject.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        CreateCollectionIconPrefab();
    }

    private void CreateCollectionIconPrefab()
    {
        collectionIconPrefab = new GameObject("CollectionIconTemplate", typeof(RectTransform));
        collectionIconPrefab.SetActive(false);

        Image iconBg = collectionIconPrefab.AddComponent<Image>();
        iconBg.color = UIColors.White;

        // Content
        GameObject content = CreatePanel("Content", collectionIconPrefab.transform);
        RectTransform contentRect = content.GetComponent<RectTransform>();
        SetFullStretch(contentRect);

        // Icon
        GameObject iconObj = new GameObject("Icon", typeof(RectTransform), typeof(CanvasRenderer));
        iconObj.transform.SetParent(content.transform, false);
        TextMeshProUGUI iconText = iconObj.AddComponent<TextMeshProUGUI>();
        iconText.fontSize = 36f;
        iconText.alignment = TextAlignmentOptions.Center;
        RectTransform iconRect = iconObj.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0, 0.35f);
        iconRect.anchorMax = Vector2.one;
        iconRect.offsetMin = Vector2.zero;
        iconRect.offsetMax = Vector2.zero;

        // Name
        GameObject nameObj = new GameObject("Name", typeof(RectTransform), typeof(CanvasRenderer));
        nameObj.transform.SetParent(content.transform, false);
        TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
        nameText.fontSize = FontSizes.Tiny;
        nameText.alignment = TextAlignmentOptions.Center;
        nameText.color = UIColors.Text;
        RectTransform nameRect = nameObj.GetComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0.12f);
        nameRect.anchorMax = new Vector2(1, 0.35f);
        nameRect.offsetMin = new Vector2(5, 0);
        nameRect.offsetMax = new Vector2(-5, 0);

        // Rarity indicator (bar at bottom)
        GameObject rarityBar = CreatePanelWithBackground("RarityBar", content.transform, UIColors.Common);
        RectTransform rarityRect = rarityBar.GetComponent<RectTransform>();
        rarityRect.anchorMin = Vector2.zero;
        rarityRect.anchorMax = new Vector2(1, 0.08f);
        rarityRect.offsetMin = Vector2.zero;
        rarityRect.offsetMax = Vector2.zero;
    }

    private void SetupController()
    {
        collectionController = screenRoot.AddComponent<CollectionController>();

        collectionController.InitializeReferences(
            gridContainer,
            collectionIconPrefab,
            totalUnlockedText,
            progressText,
            allFilterButton,
            commonFilterButton,
            uncommonFilterButton,
            rareFilterButton,
            legendaryFilterButton,
            titleText
        );
    }

    public CollectionController GetController() => collectionController;
}
