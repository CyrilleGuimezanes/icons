using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Builds the "Don't Tap the Bomb" mini-game UI at runtime.
/// </summary>
public class DontTapTheBombGameBuilder : MiniGameBuilderBase
{
    private List<Button> gridButtons = new List<Button>();
    private List<TextMeshProUGUI> gridIcons = new List<TextMeshProUGUI>();
    private const int GRID_SIZE = 9; // 3x3 grid

    public override void BuildGame(Transform parent)
    {
        gameRoot = CreateFullScreenPanel("DontTapTheBombGame", parent);
        gameRoot.SetActive(false);

        CreateGameHeader(gameRoot.transform, "💣 Évite les bombes !");
        CreateInstructionPanel(gameRoot.transform, "Tape les icônes sauf les bombes ! 💣");
        CreateGameArea(gameRoot.transform);
        BuildBombGameUI();
        SetupController();
    }

    private void BuildBombGameUI()
    {
        // Create 3x3 grid
        GameObject gridContainer = CreatePanelWithBackground("GridContainer", gamePanel.transform, UIColors.White);
        RectTransform gridRect = gridContainer.GetComponent<RectTransform>();
        gridRect.anchorMin = new Vector2(0.1f, 0.2f);
        gridRect.anchorMax = new Vector2(0.9f, 0.9f);
        gridRect.offsetMin = Vector2.zero;
        gridRect.offsetMax = Vector2.zero;

        GridLayoutGroup gridLayout = CreateGridLayout(gridContainer, new Vector2(85, 85), new Vector2(10, 10));
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 3;
        gridLayout.childAlignment = TextAnchor.MiddleCenter;
        gridLayout.padding = new RectOffset(15, 15, 15, 15);

        // Create grid cells
        for (int i = 0; i < GRID_SIZE; i++)
        {
            CreateGridCell(gridContainer.transform, i);
        }

        // Score feedback text
        TextMeshProUGUI feedbackText = CreateText("Feedback", gamePanel.transform, "", FontSizes.Body);
        feedbackText.color = UIColors.Success;
        RectTransform feedbackRect = feedbackText.GetComponent<RectTransform>();
        feedbackRect.anchorMin = new Vector2(0, 0.02f);
        feedbackRect.anchorMax = new Vector2(1, 0.15f);
        feedbackRect.offsetMin = Vector2.zero;
        feedbackRect.offsetMax = Vector2.zero;
    }

    private void CreateGridCell(Transform parent, int index)
    {
        GameObject cellObj = new GameObject($"Cell_{index}", typeof(RectTransform), typeof(CanvasRenderer));
        cellObj.transform.SetParent(parent, false);

        Image cellBg = cellObj.AddComponent<Image>();
        cellBg.color = new Color(0.95f, 0.95f, 0.95f, 1f);

        Button cellButton = cellObj.AddComponent<Button>();
        cellButton.targetGraphic = cellBg;
        gridButtons.Add(cellButton);

        // Icon
        TextMeshProUGUI iconText = CreateText("Icon", cellObj.transform, "⭐", 36f);
        RectTransform iconRect = iconText.GetComponent<RectTransform>();
        SetFullStretch(iconRect);
        gridIcons.Add(iconText);
    }

    private void SetupController()
    {
        miniGame = gameRoot.AddComponent<DontTapTheBombGame>();

        ((DontTapTheBombGame)miniGame).InitializeReferences(
            timerText,
            scoreText,
            instructionText,
            gamePanel,
            gridButtons.ToArray(),
            gridIcons.ToArray()
        );
    }
}
