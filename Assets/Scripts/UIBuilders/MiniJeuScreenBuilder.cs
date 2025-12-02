using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Builds the Mini-Jeu (Mini-Game) screen UI at runtime.
/// </summary>
public class MiniJeuScreenBuilder : UIBuilderBase
{
    private MiniGameManager gameManager;

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

    // Mini-game builders
    private List<MiniGameBase> miniGames = new List<MiniGameBase>();

    public override void BuildScreen()
    {
        FindCanvas();

        // Create screen root
        screenRoot = CreateFullScreenPanel("MiniJeuScreen", transform, UIColors.Background);

        // Build UI sections
        CreateHeader();
        CreateMainMenuPanel();
        CreateGameContainer();
        CreateRewardPanel();
        CreateFailurePanel();

        // Build mini-games
        BuildMiniGames();

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

    private void BuildMiniGames()
    {
        // Build Tap The Icon game
        TapTheIconGameBuilder tapBuilder = gameContainer.AddComponent<TapTheIconGameBuilder>();
        tapBuilder.BuildGame(gameContainer.transform);
        if (tapBuilder.GetGame() != null)
        {
            miniGames.Add(tapBuilder.GetGame());
        }

        // Build Don't Tap The Bomb game
        DontTapTheBombGameBuilder bombBuilder = gameContainer.AddComponent<DontTapTheBombGameBuilder>();
        bombBuilder.BuildGame(gameContainer.transform);
        if (bombBuilder.GetGame() != null)
        {
            miniGames.Add(bombBuilder.GetGame());
        }

        // Build Memory Match game
        MemoryMatchGameBuilder memoryBuilder = gameContainer.AddComponent<MemoryMatchGameBuilder>();
        memoryBuilder.BuildGame(gameContainer.transform);
        if (memoryBuilder.GetGame() != null)
        {
            miniGames.Add(memoryBuilder.GetGame());
        }

        // Build Find The Odd One game
        FindTheOddOneGameBuilder oddBuilder = gameContainer.AddComponent<FindTheOddOneGameBuilder>();
        oddBuilder.BuildGame(gameContainer.transform);
        if (oddBuilder.GetGame() != null)
        {
            miniGames.Add(oddBuilder.GetGame());
        }

        // Build Quick Sort game
        QuickSortGameBuilder sortBuilder = gameContainer.AddComponent<QuickSortGameBuilder>();
        sortBuilder.BuildGame(gameContainer.transform);
        if (sortBuilder.GetGame() != null)
        {
            miniGames.Add(sortBuilder.GetGame());
        }

        // Build Icon Catcher game
        IconCatcherGameBuilder catcherBuilder = gameContainer.AddComponent<IconCatcherGameBuilder>();
        catcherBuilder.BuildGame(gameContainer.transform);
        if (catcherBuilder.GetGame() != null)
        {
            miniGames.Add(catcherBuilder.GetGame());
        }
    }

    private void SetupController()
    {
        // Add MiniGameManager component
        gameManager = screenRoot.AddComponent<MiniGameManager>();

        // Initialize references
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

    /// <summary>
    /// Gets the MiniGameManager component.
    /// </summary>
    public MiniGameManager GetController() => gameManager;
}
