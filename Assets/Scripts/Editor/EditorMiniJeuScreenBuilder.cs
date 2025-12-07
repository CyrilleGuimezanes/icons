#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Editor script that builds the Mini-Jeu (Mini-Game) screen UI.
/// </summary>
public class EditorMiniJeuScreenBuilder : EditorUIBuilderBase
{
    // UI References
    private GameObject mainMenuPanel;
    private TextMeshProUGUI playsRemainingText;
    private TextMeshProUGUI nextResetText;
    private Button playButton;
    private TextMeshProUGUI playButtonText;
    private GameObject gameContainer;
    private GameObject rewardPanel;
    private TextMeshProUGUI rewardIconText;
    private TextMeshProUGUI rewardNameText;
    private TextMeshProUGUI rewardRarityText;
    private Image rewardBackground;
    private Button claimRewardButton;
    private GameObject failurePanel;
    private TextMeshProUGUI failureMessageText;
    private Button tryAgainButton;
    private TextMeshProUGUI titleText;

    [MenuItem("Icons/Build UI/Screens/Build MiniJeu Screen")]
    public static void BuildMiniJeuScreen()
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

        Transform existing = screensContainer.Find("MiniJeuScreen");
        if (existing != null)
        {
            Undo.DestroyObjectImmediate(existing.gameObject);
        }

        EditorMiniJeuScreenBuilder builder = new EditorMiniJeuScreenBuilder();
        builder.BuildScreen(screensContainer);
        
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("MiniJeu screen built successfully!");
    }

    public override void BuildScreen(Transform parent)
    {
        FindCanvas();

        // Create screen root
        screenRoot = CreateFullScreenPanel("MiniJeuScreen", parent, UIColors.Background);

        // Build UI sections
        CreateHeader();
        CreateMainMenuPanel();
        CreateGameContainer();
        CreateRewardPanel();
        CreateFailurePanel();

        // Setup controller
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

        titleText = CreateTitle("Title", header.transform, "🎲 Mini-Jeu");
        RectTransform titleRect = titleText.GetComponent<RectTransform>();
        SetFullStretch(titleRect);
        titleText.alignment = TextAlignmentOptions.Center;
    }

    private void CreateMainMenuPanel()
    {
        mainMenuPanel = CreateFullScreenPanel("MainMenuPanel", screenRoot.transform);

        // Play count display
        GameObject playCountContainer = CreatePanel("PlayCountContainer", mainMenuPanel.transform);
        RectTransform playCountRect = playCountContainer.GetComponent<RectTransform>();
        playCountRect.anchorMin = new Vector2(0.1f, 0.65f);
        playCountRect.anchorMax = new Vector2(0.9f, 0.85f);
        playCountRect.offsetMin = Vector2.zero;
        playCountRect.offsetMax = Vector2.zero;

        // Create vertical layout for play count info
        CreateVerticalLayout(playCountContainer, 10);

        // Label
        TextMeshProUGUI playLabel = CreateText("PlayLabel", playCountContainer.transform, "Parties restantes", FontSizes.Body);
        AddLayoutElement(playLabel.gameObject, preferredHeight: 30);

        // Remaining count
        playsRemainingText = CreateText("PlaysRemaining", playCountContainer.transform, "5 / 5", FontSizes.Title, FontStyles.Bold);
        playsRemainingText.color = UIColors.Primary;
        AddLayoutElement(playsRemainingText.gameObject, preferredHeight: 50);

        // Next reset timer
        nextResetText = CreateText("NextReset", playCountContainer.transform, "Reset dans: 23:59:59", FontSizes.Small);
        nextResetText.color = UIColors.TextLight;
        AddLayoutElement(nextResetText.gameObject, preferredHeight: 25);

        // Play button
        GameObject playButtonContainer = CreatePanel("PlayButtonContainer", mainMenuPanel.transform);
        RectTransform btnContainerRect = playButtonContainer.GetComponent<RectTransform>();
        btnContainerRect.anchorMin = new Vector2(0.2f, 0.45f);
        btnContainerRect.anchorMax = new Vector2(0.8f, 0.58f);
        btnContainerRect.offsetMin = Vector2.zero;
        btnContainerRect.offsetMax = Vector2.zero;

        playButton = CreateButton("PlayButton", playButtonContainer.transform, "Jouer !", UIColors.Success);
        RectTransform playBtnRect = playButton.GetComponent<RectTransform>();
        SetFullStretch(playBtnRect);
        playButtonText = playButton.GetComponentInChildren<TextMeshProUGUI>();
        playButtonText.fontSize = FontSizes.Subtitle;

        // Instructions
        TextMeshProUGUI instructions = CreateText("Instructions", mainMenuPanel.transform, 
            "Gagne des icônes en jouant aux mini-jeux !\nChaque victoire te rapporte une icône aléatoire.", FontSizes.Body);
        RectTransform instrRect = instructions.GetComponent<RectTransform>();
        instrRect.anchorMin = new Vector2(0.1f, 0.25f);
        instrRect.anchorMax = new Vector2(0.9f, 0.42f);
        instrRect.offsetMin = Vector2.zero;
        instrRect.offsetMax = Vector2.zero;
        instructions.color = UIColors.TextLight;
    }

    private void CreateGameContainer()
    {
        gameContainer = CreateFullScreenPanel("GameContainer", screenRoot.transform);
        gameContainer.SetActive(false);
    }

    private void CreateRewardPanel()
    {
        rewardPanel = CreateModalOverlay("RewardPanel", screenRoot.transform);
        rewardPanel.SetActive(false);

        // Dialog
        GameObject dialog = CreateDialogBox("Dialog", rewardPanel.transform, new Vector2(500, 450));

        // Background image for rarity effect
        rewardBackground = dialog.GetComponent<Image>();

        // Title
        TextMeshProUGUI rewardTitle = CreateTitle("RewardTitle", dialog.transform, "🎉 Bravo !");
        RectTransform titleRect = rewardTitle.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.8f);
        titleRect.anchorMax = new Vector2(1, 0.95f);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;

        // Reward icon
        rewardIconText = CreateText("RewardIcon", dialog.transform, "", 80f);
        RectTransform iconRect = rewardIconText.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0, 0.45f);
        iconRect.anchorMax = new Vector2(1, 0.8f);
        iconRect.offsetMin = Vector2.zero;
        iconRect.offsetMax = Vector2.zero;

        // Reward name
        rewardNameText = CreateText("RewardName", dialog.transform, "", FontSizes.Subtitle, FontStyles.Bold);
        RectTransform nameRect = rewardNameText.GetComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0.32f);
        nameRect.anchorMax = new Vector2(1, 0.45f);
        nameRect.offsetMin = new Vector2(20, 0);
        nameRect.offsetMax = new Vector2(-20, 0);

        // Reward rarity
        rewardRarityText = CreateText("RewardRarity", dialog.transform, "", FontSizes.Body);
        RectTransform rarityRect = rewardRarityText.GetComponent<RectTransform>();
        rarityRect.anchorMin = new Vector2(0, 0.22f);
        rarityRect.anchorMax = new Vector2(1, 0.32f);
        rarityRect.offsetMin = new Vector2(20, 0);
        rarityRect.offsetMax = new Vector2(-20, 0);

        // Claim button
        claimRewardButton = CreateButton("ClaimButton", dialog.transform, "Récupérer", UIColors.Success);
        RectTransform claimRect = claimRewardButton.GetComponent<RectTransform>();
        claimRect.anchorMin = new Vector2(0.2f, 0.05f);
        claimRect.anchorMax = new Vector2(0.8f, 0.18f);
        claimRect.offsetMin = Vector2.zero;
        claimRect.offsetMax = Vector2.zero;
    }

    private void CreateFailurePanel()
    {
        failurePanel = CreateModalOverlay("FailurePanel", screenRoot.transform);
        failurePanel.SetActive(false);

        // Dialog
        GameObject dialog = CreateDialogBox("Dialog", failurePanel.transform, new Vector2(450, 350));

        // Failure icon
        TextMeshProUGUI failIcon = CreateText("FailIcon", dialog.transform, "😢", 64f);
        RectTransform iconRect = failIcon.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0, 0.55f);
        iconRect.anchorMax = new Vector2(1, 0.9f);
        iconRect.offsetMin = Vector2.zero;
        iconRect.offsetMax = Vector2.zero;

        // Failure message
        failureMessageText = CreateText("FailMessage", dialog.transform, "Dommage !", FontSizes.Subtitle, FontStyles.Bold);
        RectTransform msgRect = failureMessageText.GetComponent<RectTransform>();
        msgRect.anchorMin = new Vector2(0, 0.35f);
        msgRect.anchorMax = new Vector2(1, 0.55f);
        msgRect.offsetMin = new Vector2(20, 0);
        msgRect.offsetMax = new Vector2(-20, 0);

        // Try again button
        tryAgainButton = CreateButton("TryAgainButton", dialog.transform, "Retour", UIColors.Secondary);
        RectTransform tryRect = tryAgainButton.GetComponent<RectTransform>();
        tryRect.anchorMin = new Vector2(0.25f, 0.08f);
        tryRect.anchorMax = new Vector2(0.75f, 0.25f);
        tryRect.offsetMin = Vector2.zero;
        tryRect.offsetMax = Vector2.zero;
    }

    private void SetupController()
    {
        // Add MiniGameManager component
        MiniGameManager gameManager = screenRoot.GetComponent<MiniGameManager>();
        if (gameManager == null)
        {
            gameManager = screenRoot.AddComponent<MiniGameManager>();
        }

        // Initialize references (mini-games list will be empty - they need to be built separately)
        List<MiniGameBase> miniGames = new List<MiniGameBase>();

        gameManager.InitializeReferences(
            miniGames,
            mainMenuPanel,
            playsRemainingText,
            nextResetText,
            playButton,
            playButtonText,
            gameContainer,
            rewardPanel,
            rewardIconText,
            rewardNameText,
            rewardRarityText,
            rewardBackground,
            claimRewardButton,
            failurePanel,
            failureMessageText,
            tryAgainButton,
            titleText
        );
    }
}
#endif
