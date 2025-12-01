using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Builds the "Quick Sort" mini-game UI at runtime.
/// </summary>
public class QuickSortGameBuilder : MiniGameBuilderBase
{
    private List<Button> iconButtons = new List<Button>();
    private List<TextMeshProUGUI> iconTexts = new List<TextMeshProUGUI>();
    private Button leftCategoryButton;
    private Button rightCategoryButton;
    private TextMeshProUGUI leftCategoryText;
    private TextMeshProUGUI rightCategoryText;
    private TextMeshProUGUI currentIconText;

    public override void BuildGame(Transform parent)
    {
        gameRoot = CreateFullScreenPanel("QuickSortGame", parent);
        gameRoot.SetActive(false);

        CreateGameHeader(gameRoot.transform, "⚡ Tri rapide !");
        CreateInstructionPanel(gameRoot.transform, "Trie les icônes dans la bonne catégorie !");
        CreateGameArea(gameRoot.transform);
        BuildSortGameUI();
        SetupController();
    }

    private void BuildSortGameUI()
    {
        // Current icon to sort (large, centered)
        GameObject currentIconContainer = CreatePanelWithBackground("CurrentIcon", gamePanel.transform, UIColors.White);
        RectTransform iconContainerRect = currentIconContainer.GetComponent<RectTransform>();
        iconContainerRect.anchorMin = new Vector2(0.3f, 0.5f);
        iconContainerRect.anchorMax = new Vector2(0.7f, 0.85f);
        iconContainerRect.offsetMin = Vector2.zero;
        iconContainerRect.offsetMax = Vector2.zero;

        currentIconText = CreateText("Icon", currentIconContainer.transform, "🎯", 64f);
        RectTransform iconRect = currentIconText.GetComponent<RectTransform>();
        SetFullStretch(iconRect);

        // Category buttons at bottom
        GameObject categoriesContainer = CreatePanel("Categories", gamePanel.transform);
        RectTransform categoriesRect = categoriesContainer.GetComponent<RectTransform>();
        categoriesRect.anchorMin = new Vector2(0.05f, 0.1f);
        categoriesRect.anchorMax = new Vector2(0.95f, 0.4f);
        categoriesRect.offsetMin = Vector2.zero;
        categoriesRect.offsetMax = Vector2.zero;

        HorizontalLayoutGroup layout = CreateHorizontalLayout(categoriesContainer, 20);

        // Left category
        leftCategoryButton = CreateCategoryButton("LeftCategory", categoriesContainer.transform, "🍎 Fruits", UIColors.Success);
        leftCategoryText = leftCategoryButton.GetComponentInChildren<TextMeshProUGUI>();

        // Right category
        rightCategoryButton = CreateCategoryButton("RightCategory", categoriesContainer.transform, "🥕 Légumes", UIColors.Primary);
        rightCategoryText = rightCategoryButton.GetComponentInChildren<TextMeshProUGUI>();

        // Progress indicator
        TextMeshProUGUI progressText = CreateText("Progress", gamePanel.transform, "Trié: 0/10", FontSizes.Body);
        progressText.color = UIColors.Primary;
        RectTransform progressRect = progressText.GetComponent<RectTransform>();
        progressRect.anchorMin = new Vector2(0, 0.02f);
        progressRect.anchorMax = new Vector2(1, 0.08f);
        progressRect.offsetMin = Vector2.zero;
        progressRect.offsetMax = Vector2.zero;
    }

    private Button CreateCategoryButton(string name, Transform parent, string label, Color color)
    {
        GameObject btnObj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));
        btnObj.transform.SetParent(parent, false);

        Image btnBg = btnObj.AddComponent<Image>();
        btnBg.color = color;

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnBg;

        // Vertical layout for icon + text
        VerticalLayoutGroup layout = CreateVerticalLayout(btnObj, 5, new RectOffset(10, 10, 15, 15));
        layout.childAlignment = TextAnchor.MiddleCenter;

        TextMeshProUGUI labelText = CreateText("Label", btnObj.transform, label, FontSizes.Body, FontStyles.Bold);
        labelText.color = Color.white;
        AddLayoutElement(labelText.gameObject, preferredHeight: 30);

        return btn;
    }

    private void SetupController()
    {
        miniGame = gameRoot.AddComponent<QuickSortGame>();

        ((QuickSortGame)miniGame).InitializeReferences(
            timerText,
            scoreText,
            instructionText,
            gamePanel,
            currentIconText,
            leftCategoryButton,
            rightCategoryButton,
            leftCategoryText,
            rightCategoryText
        );
    }
}
