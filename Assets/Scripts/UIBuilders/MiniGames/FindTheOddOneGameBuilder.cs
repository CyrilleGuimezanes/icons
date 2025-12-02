using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Builds the "Find the Odd One" mini-game UI at runtime.
/// </summary>
public class FindTheOddOneGameBuilder : MiniGameBuilderBase
{
    private List<Button> optionButtons = new List<Button>();
    private List<TextMeshProUGUI> optionIcons = new List<TextMeshProUGUI>();
    private const int OPTION_COUNT = 9; // 3x3 grid

    public override void BuildGame(Transform parent)
    {
        gameRoot = CreateFullScreenPanel("FindTheOddOneGame", parent);
        gameRoot.SetActive(false);

        CreateGameHeader(gameRoot.transform, "🔍 Trouve l'intrus !");
        CreateInstructionPanel(gameRoot.transform, "Trouve l'icône différente des autres !");
        CreateGameArea(gameRoot.transform);
        BuildOddOneGameUI();
        SetupController();
    }

    private void BuildOddOneGameUI()
    {
        // Round counter
        TextMeshProUGUI roundText = CreateText("Round", gamePanel.transform, "Manche 1/5", FontSizes.Body);
        roundText.color = UIColors.Primary;
        RectTransform roundRect = roundText.GetComponent<RectTransform>();
        roundRect.anchorMin = new Vector2(0, 0.88f);
        roundRect.anchorMax = new Vector2(1, 0.98f);
        roundRect.offsetMin = Vector2.zero;
        roundRect.offsetMax = Vector2.zero;

        // Create 3x3 grid
        GameObject gridContainer = CreatePanelWithBackground("GridContainer", gamePanel.transform, UIColors.White);
        RectTransform gridRect = gridContainer.GetComponent<RectTransform>();
        gridRect.anchorMin = new Vector2(0.1f, 0.2f);
        gridRect.anchorMax = new Vector2(0.9f, 0.85f);
        gridRect.offsetMin = Vector2.zero;
        gridRect.offsetMax = Vector2.zero;

        GridLayoutGroup gridLayout = CreateGridLayout(gridContainer, new Vector2(80, 80), new Vector2(12, 12));
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 3;
        gridLayout.childAlignment = TextAnchor.MiddleCenter;
        gridLayout.padding = new RectOffset(15, 15, 15, 15);

        // Create option cells
        for (int i = 0; i < OPTION_COUNT; i++)
        {
            CreateOptionCell(gridContainer.transform, i);
        }

        // Feedback text
        TextMeshProUGUI feedbackText = CreateText("Feedback", gamePanel.transform, "", FontSizes.Body);
        RectTransform feedbackRect = feedbackText.GetComponent<RectTransform>();
        feedbackRect.anchorMin = new Vector2(0, 0.02f);
        feedbackRect.anchorMax = new Vector2(1, 0.15f);
        feedbackRect.offsetMin = Vector2.zero;
        feedbackRect.offsetMax = Vector2.zero;
    }

    private void CreateOptionCell(Transform parent, int index)
    {
        GameObject cellObj = new GameObject($"Option_{index}", typeof(RectTransform), typeof(CanvasRenderer));
        cellObj.transform.SetParent(parent, false);

        Image cellBg = cellObj.AddComponent<Image>();
        cellBg.color = new Color(0.95f, 0.95f, 0.95f, 1f);

        Button cellButton = cellObj.AddComponent<Button>();
        cellButton.targetGraphic = cellBg;
        optionButtons.Add(cellButton);

        // Icon
        TextMeshProUGUI iconText = CreateText("Icon", cellObj.transform, "⭐", 40f);
        RectTransform iconRect = iconText.GetComponent<RectTransform>();
        SetFullStretch(iconRect);
        optionIcons.Add(iconText);
    }

    private void SetupController()
    {
        miniGame = gameRoot.AddComponent<FindTheOddOneGame>();

        ((FindTheOddOneGame)miniGame).InitializeReferences(
            timerText,
            scoreText,
            instructionText,
            gamePanel,
            optionButtons.ToArray(),
            optionIcons.ToArray()
        );
    }
}
