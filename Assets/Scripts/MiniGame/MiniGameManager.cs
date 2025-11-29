using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Main manager for the Mini-Game screen.
/// Handles daily play limits, random game selection, and rewards.
/// </summary>
public class MiniGameManager : ScreenController
{
    private const string SAVE_KEY = "MiniGameData";
    private const int MAX_DAILY_PLAYS = 5;
    private const float RESET_HOURS = 24f;

    [Header("Game Selection")]
    [SerializeField] private List<MiniGameBase> availableGames = new List<MiniGameBase>();

    [Header("Main Menu UI")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private TextMeshProUGUI playsRemainingText;
    [SerializeField] private TextMeshProUGUI nextResetText;
    [SerializeField] private Button playButton;
    [SerializeField] private TextMeshProUGUI playButtonText;

    [Header("Game Container")]
    [SerializeField] private GameObject gameContainer;

    [Header("Reward Panel")]
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private TextMeshProUGUI rewardIconText;
    [SerializeField] private TextMeshProUGUI rewardNameText;
    [SerializeField] private TextMeshProUGUI rewardRarityText;
    [SerializeField] private Image rewardBackground;
    [SerializeField] private Button claimRewardButton;

    [Header("Failure Panel")]
    [SerializeField] private GameObject failurePanel;
    [SerializeField] private TextMeshProUGUI failureMessageText;
    [SerializeField] private Button tryAgainButton;

    [Header("Animation Settings")]
    [SerializeField] private float rewardDisplayDuration = 3f;

    private MiniGameData saveData;
    private MiniGameBase currentGame;
    private IconEntry pendingReward;

    protected override void OnEnable()
    {
        base.OnEnable();
        LoadData();
        CheckDailyReset();
        UpdateUI();
    }

    private void Start()
    {
        SetupButtons();
        HideAllPanels();
        ShowMainMenu();
    }

    private void Update()
    {
        // Update the reset timer display
        if (mainMenuPanel != null && mainMenuPanel.activeSelf)
        {
            UpdateNextResetDisplay();
        }
    }

    private void SetupButtons()
    {
        if (playButton != null)
        {
            playButton.onClick.AddListener(OnPlayButtonClicked);
        }

        if (claimRewardButton != null)
        {
            claimRewardButton.onClick.AddListener(OnClaimRewardClicked);
        }

        if (tryAgainButton != null)
        {
            tryAgainButton.onClick.AddListener(OnTryAgainClicked);
        }
    }

    private void HideAllPanels()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (rewardPanel != null) rewardPanel.SetActive(false);
        if (failurePanel != null) failurePanel.SetActive(false);

        foreach (var game in availableGames)
        {
            if (game != null && game.gameObject != null)
            {
                game.gameObject.SetActive(false);
            }
        }
    }

    private void ShowMainMenu()
    {
        HideAllPanels();
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
        UpdateUI();
    }

    /// <summary>
    /// Updates the UI to reflect current state.
    /// </summary>
    private void UpdateUI()
    {
        int playsRemaining = MAX_DAILY_PLAYS - saveData.playsToday;

        if (playsRemainingText != null)
        {
            playsRemainingText.text = $"{playsRemaining} / {MAX_DAILY_PLAYS}";
        }

        if (playButton != null)
        {
            playButton.interactable = playsRemaining > 0;
        }

        if (playButtonText != null)
        {
            playButtonText.text = playsRemaining > 0 ? "Jouer !" : "Reviens demain !";
        }

        UpdateNextResetDisplay();
    }

    private void UpdateNextResetDisplay()
    {
        if (nextResetText != null)
        {
            TimeSpan timeUntilReset = GetTimeUntilReset();
            if (timeUntilReset.TotalSeconds > 0)
            {
                nextResetText.text = $"Reset dans: {timeUntilReset.Hours:D2}:{timeUntilReset.Minutes:D2}:{timeUntilReset.Seconds:D2}";
            }
            else
            {
                nextResetText.text = "Reset maintenant !";
                CheckDailyReset();
                UpdateUI();
            }
        }
    }

    /// <summary>
    /// Called when the play button is clicked.
    /// Starts a random mini-game.
    /// </summary>
    private void OnPlayButtonClicked()
    {
        if (saveData.playsToday >= MAX_DAILY_PLAYS)
        {
            return;
        }

        StartRandomGame();
    }

    /// <summary>
    /// Starts a randomly selected mini-game.
    /// </summary>
    private void StartRandomGame()
    {
        if (availableGames == null || availableGames.Count == 0)
        {
            Debug.LogWarning("No mini-games available!");
            return;
        }

        // Select a random game
        int randomIndex = UnityEngine.Random.Range(0, availableGames.Count);
        currentGame = availableGames[randomIndex];

        if (currentGame == null)
        {
            Debug.LogWarning("Selected mini-game is null!");
            return;
        }

        // Hide main menu and show game
        HideAllPanels();
        currentGame.gameObject.SetActive(true);
        currentGame.StartGame();

        // Increment plays
        saveData.playsToday++;
        SaveData();
    }

    /// <summary>
    /// Called by mini-games when they complete.
    /// </summary>
    public void OnMiniGameComplete(bool success)
    {
        if (currentGame != null)
        {
            currentGame.StopGame();
            currentGame.gameObject.SetActive(false);
        }

        if (success)
        {
            GiveReward();
        }
        else
        {
            ShowFailure();
        }
    }

    /// <summary>
    /// Gives a reward icon to the player.
    /// </summary>
    private void GiveReward()
    {
        if (IconDatabase.Instance == null)
        {
            Debug.LogWarning("IconDatabase not found!");
            ShowMainMenu();
            return;
        }

        // Get a random icon based on rarity
        pendingReward = IconDatabase.Instance.GetRandomIcon();

        if (pendingReward == null)
        {
            Debug.LogWarning("Failed to get random icon!");
            ShowMainMenu();
            return;
        }

        // Show reward panel
        ShowRewardPanel(pendingReward);
    }

    /// <summary>
    /// Shows the reward panel with the won icon.
    /// </summary>
    private void ShowRewardPanel(IconEntry icon)
    {
        HideAllPanels();

        if (rewardPanel != null)
        {
            rewardPanel.SetActive(true);
        }

        if (rewardIconText != null)
        {
            rewardIconText.text = icon.id;
        }

        if (rewardNameText != null)
        {
            rewardNameText.text = icon.displayName;
        }

        if (rewardRarityText != null)
        {
            rewardRarityText.text = GetRarityText(icon.rarity);
            rewardRarityText.color = icon.GetRarityColor();
        }

        if (rewardBackground != null)
        {
            // Set a subtle background color based on rarity
            Color bgColor = icon.GetRarityColor();
            bgColor.a = 0.2f;
            rewardBackground.color = bgColor;
        }
    }

    /// <summary>
    /// Called when the claim reward button is clicked.
    /// </summary>
    private void OnClaimRewardClicked()
    {
        if (pendingReward != null && PlayerInventory.Instance != null)
        {
            // Add the icon to inventory
            PlayerInventory.Instance.AddIcon(pendingReward.id, 1);

            // Check if this is a new unlock
            bool isNewUnlock = false;
            if (UnlockedIconsManager.Instance != null)
            {
                isNewUnlock = UnlockedIconsManager.Instance.UnlockIcon(pendingReward.id);
            }

            pendingReward = null;
        }

        ShowMainMenu();
    }

    /// <summary>
    /// Shows the failure panel.
    /// </summary>
    private void ShowFailure()
    {
        HideAllPanels();

        if (failurePanel != null)
        {
            failurePanel.SetActive(true);
        }

        if (failureMessageText != null)
        {
            string[] failureMessages = new string[]
            {
                "Dommage ! Essaie encore !",
                "Pas cette fois...",
                "Presque ! Réessaie !",
                "Oups ! Encore un essai ?",
                "Raté ! Tu peux le faire !"
            };
            failureMessageText.text = failureMessages[UnityEngine.Random.Range(0, failureMessages.Length)];
        }
    }

    /// <summary>
    /// Called when the try again button is clicked.
    /// </summary>
    private void OnTryAgainClicked()
    {
        ShowMainMenu();
    }

    /// <summary>
    /// Gets the display text for a rarity level.
    /// </summary>
    private string GetRarityText(IconRarity rarity)
    {
        return rarity switch
        {
            IconRarity.Common => "Commun",
            IconRarity.Uncommon => "Peu commun",
            IconRarity.Rare => "Rare",
            IconRarity.Legendary => "Légendaire",
            _ => "Inconnu"
        };
    }

    /// <summary>
    /// Checks if a daily reset is needed and performs it.
    /// </summary>
    private void CheckDailyReset()
    {
        DateTime lastReset = DateTime.FromBinary(saveData.lastResetTimestamp);
        TimeSpan timeSinceReset = DateTime.Now - lastReset;

        if (timeSinceReset.TotalHours >= RESET_HOURS)
        {
            // Reset daily plays
            saveData.playsToday = 0;
            saveData.lastResetTimestamp = DateTime.Now.ToBinary();
            SaveData();
        }
    }

    /// <summary>
    /// Gets the time remaining until the next reset.
    /// </summary>
    private TimeSpan GetTimeUntilReset()
    {
        DateTime lastReset = DateTime.FromBinary(saveData.lastResetTimestamp);
        DateTime nextReset = lastReset.AddHours(RESET_HOURS);
        return nextReset - DateTime.Now;
    }

    /// <summary>
    /// Saves the mini-game data.
    /// </summary>
    private void SaveData()
    {
        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads the mini-game data.
    /// </summary>
    private void LoadData()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            if (!string.IsNullOrEmpty(json))
            {
                saveData = JsonUtility.FromJson<MiniGameData>(json);
                if (saveData == null)
                {
                    saveData = new MiniGameData();
                }
            }
            else
            {
                saveData = new MiniGameData();
            }
        }
        else
        {
            saveData = new MiniGameData();
        }
    }

    /// <summary>
    /// Gets the number of plays remaining today.
    /// </summary>
    public int PlaysRemaining => MAX_DAILY_PLAYS - saveData.playsToday;

    /// <summary>
    /// Gets whether any plays are remaining today.
    /// </summary>
    public bool CanPlay => saveData.playsToday < MAX_DAILY_PLAYS;
}

/// <summary>
/// Serializable data for mini-game state persistence.
/// </summary>
[Serializable]
public class MiniGameData
{
    public int playsToday = 0;
    public long lastResetTimestamp;

    public MiniGameData()
    {
        playsToday = 0;
        lastResetTimestamp = DateTime.Now.ToBinary();
    }
}
