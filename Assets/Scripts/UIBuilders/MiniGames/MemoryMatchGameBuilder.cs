using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Builds the "Memory Match" mini-game UI at runtime.
/// </summary>
public class MemoryMatchGameBuilder : MiniGameBuilderBase
{
    private List<Button> cardButtons = new List<Button>();
    private List<TextMeshProUGUI> cardIcons = new List<TextMeshProUGUI>();
    private List<Image> cardBackgrounds = new List<Image>();
    private const int GRID_SIZE = 12; // 4x3 grid for 6 pairs

    public override void BuildGame(Transform parent)
    {
        gameRoot = CreateFullScreenPanel("MemoryMatchGame", parent);
        gameRoot.SetActive(false);

        CreateGameHeader(gameRoot.transform, "🧠 Memory Match");
        CreateInstructionPanel(gameRoot.transform, "Trouve toutes les paires d'icônes !");
        CreateGameArea(gameRoot.transform);
        BuildMemoryGameUI();
        SetupController();
    }

    private void BuildMemoryGameUI()
    {
        // Create 4x3 grid
        GameObject gridContainer = CreatePanelWithBackground("GridContainer", gamePanel.transform, UIColors.White);
        RectTransform gridRect = gridContainer.GetComponent<RectTransform>();
        gridRect.anchorMin = new Vector2(0.05f, 0.15f);
        gridRect.anchorMax = new Vector2(0.95f, 0.95f);
        gridRect.offsetMin = Vector2.zero;
        gridRect.offsetMax = Vector2.zero;

        GridLayoutGroup gridLayout = CreateGridLayout(gridContainer, new Vector2(70, 85), new Vector2(8, 8));
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 4;
        gridLayout.childAlignment = TextAnchor.MiddleCenter;
        gridLayout.padding = new RectOffset(10, 10, 10, 10);

        // Create cards
        for (int i = 0; i < GRID_SIZE; i++)
        {
            CreateCard(gridContainer.transform, i);
        }

        // Pairs found text
        TextMeshProUGUI pairsText = CreateText("PairsFound", gamePanel.transform, "Paires: 0/6", FontSizes.Body);
        pairsText.color = UIColors.Primary;
        RectTransform pairsRect = pairsText.GetComponent<RectTransform>();
        pairsRect.anchorMin = new Vector2(0, 0.02f);
        pairsRect.anchorMax = new Vector2(1, 0.12f);
        pairsRect.offsetMin = Vector2.zero;
        pairsRect.offsetMax = Vector2.zero;
    }

    private void CreateCard(Transform parent, int index)
    {
        GameObject cardObj = new GameObject($"Card_{index}", typeof(RectTransform), typeof(CanvasRenderer));
        cardObj.transform.SetParent(parent, false);

        Image cardBg = cardObj.AddComponent<Image>();
        cardBg.color = UIColors.Primary; // Face down color
        cardBackgrounds.Add(cardBg);

        Button cardButton = cardObj.AddComponent<Button>();
        cardButton.targetGraphic = cardBg;
        cardButtons.Add(cardButton);

        // Icon (hidden when face down)
        TextMeshProUGUI iconText = CreateText("Icon", cardObj.transform, "❓", 32f);
        RectTransform iconRect = iconText.GetComponent<RectTransform>();
        SetFullStretch(iconRect);
        iconText.color = Color.white;
        cardIcons.Add(iconText);
    }

    private void SetupController()
    {
        miniGame = gameRoot.AddComponent<MemoryMatchGame>();

        ((MemoryMatchGame)miniGame).InitializeReferences(
            timerText,
            scoreText,
            instructionText,
            gamePanel,
            cardButtons.ToArray(),
            cardIcons.ToArray(),
            cardBackgrounds.ToArray()
        );
    }
}
