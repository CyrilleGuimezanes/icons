using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// UI screen that displays all hidden mini-games and their completion status.
/// Shows hints about how to complete each hidden challenge.
/// </summary>
public class HiddenGamesScreen : ScreenController
{
    [Header("UI References")]
    [SerializeField] private Transform contentContainer;
    [SerializeField] private GameObject hiddenGameItemPrefab;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI headerText;

    [Header("Colors")]
    [SerializeField] private Color completedColor = new Color(0.2f, 0.8f, 0.2f);
    [SerializeField] private Color lockedColor = new Color(0.5f, 0.5f, 0.5f);
    [SerializeField] private Color hintColor = new Color(1f, 0.8f, 0.2f);

    private List<GameObject> spawnedItems = new List<GameObject>();

    protected override void OnEnable()
    {
        base.OnEnable();
        RefreshDisplay();

        // Subscribe to hidden game completion events
        if (HiddenMiniGameManager.Instance != null)
        {
            HiddenMiniGameManager.Instance.OnHiddenGameCompleted += OnHiddenGameCompleted;
        }
    }

    private void OnDisable()
    {
        if (HiddenMiniGameManager.Instance != null)
        {
            HiddenMiniGameManager.Instance.OnHiddenGameCompleted -= OnHiddenGameCompleted;
        }
    }

    private void OnHiddenGameCompleted(string gameId, string rewardIconId)
    {
        RefreshDisplay();
    }

    /// <summary>
    /// Refreshes the display of all hidden games.
    /// </summary>
    public void RefreshDisplay()
    {
        // Clear existing items
        foreach (var item in spawnedItems)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
        spawnedItems.Clear();

        if (HiddenMiniGameManager.Instance == null)
        {
            Debug.LogWarning("HiddenMiniGameManager not found!");
            return;
        }

        // Update progress text
        int completed = HiddenMiniGameManager.Instance.GetCompletedGameCount();
        int total = HiddenMiniGameManager.Instance.GetTotalGameCount();

        if (progressText != null)
        {
            progressText.text = $"Défis Secrets: {completed} / {total}";
        }

        if (headerText != null)
        {
            if (completed == total)
            {
                headerText.text = "🎉 Tous les défis secrets complétés!";
            }
            else
            {
                headerText.text = "🔍 Découvre les défis cachés!";
            }
        }

        // Create items for each hidden game
        var hiddenGames = HiddenMiniGameManager.Instance.GetAllHiddenGames();

        foreach (var game in hiddenGames)
        {
            CreateHiddenGameItem(game);
        }
    }

    /// <summary>
    /// Creates a UI item for a hidden game.
    /// </summary>
    private void CreateHiddenGameItem(HiddenGameInfo gameInfo)
    {
        if (contentContainer == null) return;

        GameObject item;

        // Use prefab if available, otherwise create a simple item
        if (hiddenGameItemPrefab != null)
        {
            item = Instantiate(hiddenGameItemPrefab, contentContainer);
        }
        else
        {
            item = CreateDefaultItem();
        }

        if (item == null) return;

        spawnedItems.Add(item);

        // Configure the item
        ConfigureHiddenGameItem(item, gameInfo);
    }

    /// <summary>
    /// Creates a default item when no prefab is provided.
    /// </summary>
    private GameObject CreateDefaultItem()
    {
        if (contentContainer == null) return null;

        // Create container
        GameObject item = new GameObject("HiddenGameItem");
        item.transform.SetParent(contentContainer, false);

        RectTransform rectTransform = item.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(0, 100);

        // Add layout element
        LayoutElement layoutElement = item.AddComponent<LayoutElement>();
        layoutElement.minHeight = 100;
        layoutElement.preferredHeight = 100;
        layoutElement.flexibleWidth = 1;

        // Add background
        Image background = item.AddComponent<Image>();
        background.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

        // Add horizontal layout
        HorizontalLayoutGroup layout = item.AddComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(10, 10, 10, 10);
        layout.spacing = 10;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = false;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = true;

        // Create icon text
        GameObject iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(item.transform, false);
        TextMeshProUGUI iconText = iconObj.AddComponent<TextMeshProUGUI>();
        iconText.fontSize = 36;
        iconText.alignment = TextAlignmentOptions.Center;
        LayoutElement iconLayout = iconObj.AddComponent<LayoutElement>();
        iconLayout.minWidth = 60;
        iconLayout.preferredWidth = 60;

        // Create info container
        GameObject infoObj = new GameObject("Info");
        infoObj.transform.SetParent(item.transform, false);
        VerticalLayoutGroup infoLayout = infoObj.AddComponent<VerticalLayoutGroup>();
        infoLayout.spacing = 5;
        infoLayout.childControlWidth = true;
        infoLayout.childControlHeight = true;
        infoLayout.childForceExpandWidth = true;
        infoLayout.childForceExpandHeight = false;
        LayoutElement infoLayoutElement = infoObj.AddComponent<LayoutElement>();
        infoLayoutElement.flexibleWidth = 1;

        // Create title text
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(infoObj.transform, false);
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.fontSize = 20;
        titleText.fontStyle = FontStyles.Bold;

        // Create description text
        GameObject descObj = new GameObject("Description");
        descObj.transform.SetParent(infoObj.transform, false);
        TextMeshProUGUI descText = descObj.AddComponent<TextMeshProUGUI>();
        descText.fontSize = 14;

        // Create status indicator
        GameObject statusObj = new GameObject("Status");
        statusObj.transform.SetParent(item.transform, false);
        TextMeshProUGUI statusText = statusObj.AddComponent<TextMeshProUGUI>();
        statusText.fontSize = 24;
        statusText.alignment = TextAlignmentOptions.Center;
        LayoutElement statusLayout = statusObj.AddComponent<LayoutElement>();
        statusLayout.minWidth = 50;
        statusLayout.preferredWidth = 50;

        return item;
    }

    /// <summary>
    /// Configures a hidden game item with the game info.
    /// </summary>
    private void ConfigureHiddenGameItem(GameObject item, HiddenGameInfo gameInfo)
    {
        if (item == null) return;

        // Find child components
        TextMeshProUGUI iconText = item.transform.Find("Icon")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI titleText = item.transform.Find("Info/Title")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI descText = item.transform.Find("Info/Description")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI statusText = item.transform.Find("Status")?.GetComponent<TextMeshProUGUI>();
        Image background = item.GetComponent<Image>();

        // Set icon
        if (iconText != null)
        {
            if (gameInfo.isCompleted)
            {
                iconText.text = gameInfo.rewardIconId;
                iconText.color = completedColor;
            }
            else
            {
                iconText.text = "?";
                iconText.color = lockedColor;
            }
        }

        // Set title
        if (titleText != null)
        {
            titleText.text = gameInfo.displayName;
            titleText.color = gameInfo.isCompleted ? completedColor : Color.white;
        }

        // Set description
        if (descText != null)
        {
            if (gameInfo.isCompleted)
            {
                descText.text = "✓ Complété!";
                descText.color = completedColor;
            }
            else
            {
                descText.text = gameInfo.description;
                descText.color = hintColor;
            }
        }

        // Set status
        if (statusText != null)
        {
            if (gameInfo.isCompleted)
            {
                statusText.text = "✓";
                statusText.color = completedColor;
            }
            else
            {
                statusText.text = "🔒";
                statusText.color = lockedColor;
            }
        }

        // Set background color
        if (background != null)
        {
            if (gameInfo.isCompleted)
            {
                background.color = new Color(0.1f, 0.2f, 0.1f, 0.9f);
            }
            else
            {
                background.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
            }
        }
    }
}
