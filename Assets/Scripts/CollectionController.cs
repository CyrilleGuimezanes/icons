using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Controller for the Collection screen.
/// Displays all unlocked icons in a grid format with filtering options.
/// </summary>
public class CollectionController : ScreenController
{
    [Header("Collection Grid")]
    [SerializeField] private Transform gridContainer;
    [SerializeField] private GameObject collectionIconPrefab;

    [Header("Statistics")]
    [SerializeField] private TextMeshProUGUI totalUnlockedText;
    [SerializeField] private TextMeshProUGUI progressText;

    [Header("Filter Buttons")]
    [SerializeField] private Button allFilterButton;
    [SerializeField] private Button commonFilterButton;
    [SerializeField] private Button uncommonFilterButton;
    [SerializeField] private Button rareFilterButton;
    [SerializeField] private Button legendaryFilterButton;

    [Header("Colors")]
    [SerializeField] private Color selectedFilterColor = new Color(0.2f, 0.6f, 1f);
    [SerializeField] private Color normalFilterColor = new Color(0.5f, 0.5f, 0.5f);
    [SerializeField] private Color commonColor = new Color(0.4f, 0.8f, 0.4f);
    [SerializeField] private Color uncommonColor = new Color(0.3f, 0.5f, 0.9f);
    [SerializeField] private Color rareColor = new Color(0.6f, 0.3f, 0.8f);
    [SerializeField] private Color legendaryColor = new Color(1f, 0.6f, 0.2f);

    private IconRarity? currentFilter = null;
    private List<GameObject> displayedIcons = new List<GameObject>();

    protected override void OnEnable()
    {
        base.OnEnable();
        RefreshCollection();

        if (UnlockedIconsManager.Instance != null)
        {
            UnlockedIconsManager.Instance.OnIconUnlocked += OnIconUnlocked;
        }
    }

    private void OnDisable()
    {
        if (UnlockedIconsManager.Instance != null)
        {
            UnlockedIconsManager.Instance.OnIconUnlocked -= OnIconUnlocked;
        }
    }

    private void Start()
    {
        SetupFilterButtons();
        SelectFilter(null); // Show all by default
    }

    private void SetupFilterButtons()
    {
        if (allFilterButton != null)
            allFilterButton.onClick.AddListener(() => SelectFilter(null));
        if (commonFilterButton != null)
            commonFilterButton.onClick.AddListener(() => SelectFilter(IconRarity.Common));
        if (uncommonFilterButton != null)
            uncommonFilterButton.onClick.AddListener(() => SelectFilter(IconRarity.Uncommon));
        if (rareFilterButton != null)
            rareFilterButton.onClick.AddListener(() => SelectFilter(IconRarity.Rare));
        if (legendaryFilterButton != null)
            legendaryFilterButton.onClick.AddListener(() => SelectFilter(IconRarity.Legendary));
    }

    /// <summary>
    /// Selects a filter to apply to the collection display.
    /// </summary>
    /// <param name="rarity">The rarity to filter by, or null for all icons.</param>
    public void SelectFilter(IconRarity? rarity)
    {
        currentFilter = rarity;
        UpdateFilterButtonVisuals();
        RefreshCollection();
    }

    private void UpdateFilterButtonVisuals()
    {
        UpdateButtonColor(allFilterButton, currentFilter == null);
        UpdateButtonColor(commonFilterButton, currentFilter == IconRarity.Common);
        UpdateButtonColor(uncommonFilterButton, currentFilter == IconRarity.Uncommon);
        UpdateButtonColor(rareFilterButton, currentFilter == IconRarity.Rare);
        UpdateButtonColor(legendaryFilterButton, currentFilter == IconRarity.Legendary);
    }

    private void UpdateButtonColor(Button button, bool isSelected)
    {
        if (button == null) return;

        var buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.color = isSelected ? selectedFilterColor : normalFilterColor;
        }

        var buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = isSelected ? selectedFilterColor : Color.white;
        }
    }

    /// <summary>
    /// Refreshes the collection display based on current filter.
    /// </summary>
    public void RefreshCollection()
    {
        ClearDisplayedIcons();

        if (UnlockedIconsManager.Instance == null || IconDatabase.Instance == null)
        {
            UpdateStatistics();
            return;
        }

        List<string> unlockedIds = UnlockedIconsManager.Instance.GetAllUnlockedIconIds();

        foreach (string iconId in unlockedIds)
        {
            IconEntry icon = IconDatabase.Instance.GetIconById(iconId);
            if (icon == null) continue;

            // Apply filter
            if (currentFilter.HasValue && icon.rarity != currentFilter.Value)
            {
                continue;
            }

            CreateIconDisplay(icon);
        }

        UpdateStatistics();
    }

    private void ClearDisplayedIcons()
    {
        foreach (var iconObj in displayedIcons)
        {
            if (iconObj != null)
            {
                Destroy(iconObj);
            }
        }
        displayedIcons.Clear();
    }

    private void CreateIconDisplay(IconEntry icon)
    {
        if (gridContainer == null || collectionIconPrefab == null) return;

        GameObject iconObj = Instantiate(collectionIconPrefab, gridContainer);
        displayedIcons.Add(iconObj);

        // Setup icon display
        TextMeshProUGUI iconText = iconObj.GetComponentInChildren<TextMeshProUGUI>();
        if (iconText != null)
        {
            iconText.text = icon.id;
        }

        // Set rarity color
        Image iconImage = iconObj.GetComponent<Image>();
        if (iconImage != null)
        {
            iconImage.color = GetRarityColor(icon.rarity);
        }
    }

    private Color GetRarityColor(IconRarity rarity)
    {
        return rarity switch
        {
            IconRarity.Common => commonColor,
            IconRarity.Uncommon => uncommonColor,
            IconRarity.Rare => rareColor,
            IconRarity.Legendary => legendaryColor,
            _ => Color.white
        };
    }

    private void UpdateStatistics()
    {
        if (UnlockedIconsManager.Instance == null) return;

        int unlockedCount = UnlockedIconsManager.Instance.GetUnlockedIconCount();
        int totalCount = IconDatabase.Instance != null ? IconDatabase.Instance.TotalIconCount : 0;

        if (totalUnlockedText != null)
        {
            totalUnlockedText.text = $"{unlockedCount} / {totalCount}";
        }

        if (progressText != null && totalCount > 0)
        {
            float percentage = (float)unlockedCount / totalCount * 100f;
            progressText.text = $"{percentage:F1}% Complete";
        }
    }

    private void OnIconUnlocked(string iconId)
    {
        RefreshCollection();
    }

    /// <summary>
    /// Gets the color associated with a rarity level.
    /// </summary>
    /// <param name="rarity">The rarity level.</param>
    /// <returns>The color for that rarity.</returns>
    public Color GetIconRarityColor(IconRarity rarity)
    {
        return GetRarityColor(rarity);
    }
}
