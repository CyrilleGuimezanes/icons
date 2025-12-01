using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Base class for mini-game UI builders.
/// Provides common UI elements for all mini-games.
/// </summary>
public abstract class MiniGameBuilderBase : UIBuilderBase
{
    protected GameObject gameRoot;
    protected MiniGameBase miniGame;

    // Common UI elements
    protected TextMeshProUGUI timerText;
    protected TextMeshProUGUI scoreText;
    protected TextMeshProUGUI instructionText;
    protected GameObject gamePanel;

    /// <summary>
    /// Builds the mini-game UI within the specified parent.
    /// </summary>
    public abstract void BuildGame(Transform parent);

    /// <summary>
    /// Gets the mini-game component.
    /// </summary>
    public MiniGameBase GetGame() => miniGame;

    /// <summary>
    /// Gets the game root GameObject.
    /// </summary>
    public override GameObject GetScreenRoot() => gameRoot;

    /// <summary>
    /// Creates the common game header with timer and score.
    /// </summary>
    protected GameObject CreateGameHeader(Transform parent, string gameName)
    {
        GameObject header = CreatePanel("Header", parent);
        RectTransform headerRect = header.GetComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0, 0.88f);
        headerRect.anchorMax = new Vector2(1, 1);
        headerRect.offsetMin = Vector2.zero;
        headerRect.offsetMax = Vector2.zero;

        // Game name
        TextMeshProUGUI nameText = CreateText("GameName", header.transform, gameName, FontSizes.Subtitle, FontStyles.Bold);
        RectTransform nameRect = nameText.GetComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0);
        nameRect.anchorMax = new Vector2(1, 0.6f);
        nameRect.offsetMin = Vector2.zero;
        nameRect.offsetMax = Vector2.zero;

        // Timer and score container
        GameObject statsContainer = CreatePanel("Stats", header.transform);
        RectTransform statsRect = statsContainer.GetComponent<RectTransform>();
        statsRect.anchorMin = new Vector2(0, 0.6f);
        statsRect.anchorMax = new Vector2(1, 1);
        statsRect.offsetMin = new Vector2(20, 0);
        statsRect.offsetMax = new Vector2(-20, 0);

        HorizontalLayoutGroup layout = CreateHorizontalLayout(statsContainer, 30);

        // Timer
        GameObject timerContainer = CreatePanel("TimerContainer", statsContainer.transform);
        HorizontalLayoutGroup timerLayout = CreateHorizontalLayout(timerContainer, 5);
        timerLayout.childForceExpandWidth = false;
        AddLayoutElement(timerContainer, flexibleWidth: 1);

        TextMeshProUGUI timerIcon = CreateText("TimerIcon", timerContainer.transform, "⏱️", FontSizes.Body);
        AddLayoutElement(timerIcon.gameObject, preferredWidth: 25);

        timerText = CreateText("TimerValue", timerContainer.transform, "10.0s", FontSizes.Body, FontStyles.Bold);
        timerText.color = UIColors.Primary;
        AddLayoutElement(timerText.gameObject, preferredWidth: 60);

        // Score
        GameObject scoreContainer = CreatePanel("ScoreContainer", statsContainer.transform);
        HorizontalLayoutGroup scoreLayout = CreateHorizontalLayout(scoreContainer, 5);
        scoreLayout.childForceExpandWidth = false;
        AddLayoutElement(scoreContainer, flexibleWidth: 1);

        TextMeshProUGUI scoreIcon = CreateText("ScoreIcon", scoreContainer.transform, "⭐", FontSizes.Body);
        AddLayoutElement(scoreIcon.gameObject, preferredWidth: 25);

        scoreText = CreateText("ScoreValue", scoreContainer.transform, "0", FontSizes.Body, FontStyles.Bold);
        scoreText.color = UIColors.Warning;
        AddLayoutElement(scoreText.gameObject, preferredWidth: 40);

        return header;
    }

    /// <summary>
    /// Creates the instruction panel.
    /// </summary>
    protected GameObject CreateInstructionPanel(Transform parent, string instruction)
    {
        GameObject instructionPanel = CreatePanelWithBackground("InstructionPanel", parent, UIColors.White);
        RectTransform panelRect = instructionPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.05f, 0.78f);
        panelRect.anchorMax = new Vector2(0.95f, 0.87f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        instructionText = CreateText("Instruction", instructionPanel.transform, instruction, FontSizes.Body);
        RectTransform textRect = instructionText.GetComponent<RectTransform>();
        SetFullStretch(textRect, new Vector2(10, 5), new Vector2(-10, -5));

        return instructionPanel;
    }

    /// <summary>
    /// Creates the main game area panel.
    /// </summary>
    protected GameObject CreateGameArea(Transform parent)
    {
        gamePanel = CreatePanel("GameArea", parent);
        RectTransform areaRect = gamePanel.GetComponent<RectTransform>();
        areaRect.anchorMin = new Vector2(0.02f, 0.02f);
        areaRect.anchorMax = new Vector2(0.98f, 0.77f);
        areaRect.offsetMin = Vector2.zero;
        areaRect.offsetMax = Vector2.zero;

        return gamePanel;
    }

    public override void BuildScreen()
    {
        // Not used directly - use BuildGame instead
    }
}
