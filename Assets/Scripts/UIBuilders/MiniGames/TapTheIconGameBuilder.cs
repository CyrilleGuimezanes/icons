using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Builds the "Tap the Icon" mini-game UI at runtime.
/// </summary>
public class TapTheIconGameBuilder : MiniGameBuilderBase
{
    private Button tapButton;
    private TextMeshProUGUI tapIconText;
    private TextMeshProUGUI tapCountText;
    private TextMeshProUGUI targetCountText;
    private Image progressBar;

    public override void BuildGame(Transform parent)
    {
        // Create game root
        gameRoot = CreateFullScreenPanel("TapTheIconGame", parent);
        gameRoot.SetActive(false);

        // Create header
        CreateGameHeader(gameRoot.transform, "🎯 Tape l'icône !");

        // Create instruction
        CreateInstructionPanel(gameRoot.transform, "Tape sur l'icône 10 fois en 3 secondes !");

        // Create game area
        CreateGameArea(gameRoot.transform);

        // Build game-specific UI
        BuildTapGameUI();

        // Setup controller
        SetupController();
    }

    private void BuildTapGameUI()
    {
        // Main tap button (large, centered)
        GameObject tapButtonObj = new GameObject("TapButton", typeof(RectTransform), typeof(CanvasRenderer));
        tapButtonObj.transform.SetParent(gamePanel.transform, false);

        RectTransform btnRect = tapButtonObj.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.2f, 0.35f);
        btnRect.anchorMax = new Vector2(0.8f, 0.75f);
        btnRect.offsetMin = Vector2.zero;
        btnRect.offsetMax = Vector2.zero;

        Image btnBg = tapButtonObj.AddComponent<Image>();
        btnBg.color = UIColors.Primary;

        tapButton = tapButtonObj.AddComponent<Button>();
        tapButton.targetGraphic = btnBg;

        // Icon text on button
        tapIconText = CreateText("TapIcon", tapButtonObj.transform, "🎯", 72f);
        RectTransform iconRect = tapIconText.GetComponent<RectTransform>();
        SetFullStretch(iconRect);
        tapIconText.color = Color.white;

        // Progress section
        GameObject progressSection = CreatePanel("ProgressSection", gamePanel.transform);
        RectTransform progressRect = progressSection.GetComponent<RectTransform>();
        progressRect.anchorMin = new Vector2(0.1f, 0.12f);
        progressRect.anchorMax = new Vector2(0.9f, 0.30f);
        progressRect.offsetMin = Vector2.zero;
        progressRect.offsetMax = Vector2.zero;

        // Tap count display
        GameObject countContainer = CreatePanel("CountContainer", progressSection.transform);
        RectTransform countRect = countContainer.GetComponent<RectTransform>();
        countRect.anchorMin = new Vector2(0, 0.5f);
        countRect.anchorMax = new Vector2(1, 1);
        countRect.offsetMin = Vector2.zero;
        countRect.offsetMax = Vector2.zero;

        HorizontalLayoutGroup countLayout = CreateHorizontalLayout(countContainer, 10);
        countLayout.childAlignment = TextAnchor.MiddleCenter;

        tapCountText = CreateText("TapCount", countContainer.transform, "0", FontSizes.Title, FontStyles.Bold);
        tapCountText.color = UIColors.Primary;
        AddLayoutElement(tapCountText.gameObject, preferredWidth: 60);

        targetCountText = CreateText("TargetCount", countContainer.transform, "/ 10", FontSizes.Subtitle);
        targetCountText.color = UIColors.TextLight;
        AddLayoutElement(targetCountText.gameObject, preferredWidth: 50);

        // Progress bar background
        GameObject progressBarBg = CreatePanelWithBackground("ProgressBarBg", progressSection.transform, UIColors.Secondary);
        RectTransform bgRect = progressBarBg.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 0);
        bgRect.anchorMax = new Vector2(1, 0.4f);
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // Progress bar fill
        GameObject progressFill = CreatePanelWithBackground("ProgressFill", progressBarBg.transform, UIColors.Success);
        RectTransform fillRect = progressFill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = new Vector2(0, 1);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        progressBar = progressFill.GetComponent<Image>();
        progressBar.type = Image.Type.Filled;
        progressBar.fillMethod = Image.FillMethod.Horizontal;
    }

    private void SetupController()
    {
        miniGame = gameRoot.AddComponent<TapTheIconGame>();

        ((TapTheIconGame)miniGame).InitializeReferences(
            timerText,
            scoreText,
            instructionText,
            gamePanel,
            tapButton,
            tapIconText,
            tapCountText,
            targetCountText,
            progressBar
        );
    }
}
