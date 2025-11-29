using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System;

/// <summary>
/// Base class for all mini-games.
/// Provides common functionality like timing, scoring, and game state management.
/// </summary>
public abstract class MiniGameBase : MonoBehaviour
{
    [Header("Game Info")]
    [SerializeField] protected string gameName;
    [SerializeField] protected string gameDescription;
    [SerializeField] protected float gameDuration = 10f;

    [Header("UI References")]
    [SerializeField] protected TextMeshProUGUI timerText;
    [SerializeField] protected TextMeshProUGUI scoreText;
    [SerializeField] protected TextMeshProUGUI instructionText;
    [SerializeField] protected GameObject gamePanel;

    [Header("Events")]
    public UnityEvent<bool> OnGameComplete;
    public UnityEvent OnGameStart;

    protected float currentTime;
    protected int currentScore;
    protected bool isGameActive;
    protected MiniGameManager gameManager;

    /// <summary>
    /// The name of this mini-game.
    /// </summary>
    public string GameName => gameName;

    /// <summary>
    /// The description of this mini-game.
    /// </summary>
    public string GameDescription => gameDescription;

    /// <summary>
    /// Whether the game is currently active.
    /// </summary>
    public bool IsGameActive => isGameActive;

    /// <summary>
    /// The current score.
    /// </summary>
    public int CurrentScore => currentScore;

    protected virtual void Awake()
    {
        gameManager = GetComponentInParent<MiniGameManager>();
    }

    protected virtual void Update()
    {
        if (isGameActive)
        {
            UpdateTimer();
        }
    }

    /// <summary>
    /// Initializes and starts the mini-game.
    /// </summary>
    public virtual void StartGame()
    {
        isGameActive = true;
        currentTime = gameDuration;
        currentScore = 0;

        UpdateUI();
        ShowInstructions();
        OnGameStart?.Invoke();

        if (gamePanel != null)
        {
            gamePanel.SetActive(true);
        }

        OnGameStarted();
    }

    /// <summary>
    /// Stops the mini-game and returns to the main screen.
    /// </summary>
    public virtual void StopGame()
    {
        isGameActive = false;

        if (gamePanel != null)
        {
            gamePanel.SetActive(false);
        }

        OnGameStopped();
    }

    /// <summary>
    /// Called when the game timer runs out or game ends.
    /// </summary>
    protected virtual void EndGame(bool success)
    {
        isGameActive = false;
        OnGameComplete?.Invoke(success);

        if (gameManager != null)
        {
            gameManager.OnMiniGameComplete(success);
        }
        else
        {
            Debug.LogWarning($"MiniGameBase: gameManager is null for {gameName}. Game may not have been started from MiniGameManager.");
        }
    }

    /// <summary>
    /// Updates the game timer.
    /// </summary>
    protected virtual void UpdateTimer()
    {
        currentTime -= Time.deltaTime;

        if (timerText != null)
        {
            timerText.text = Mathf.Max(0, currentTime).ToString("F1") + "s";
        }

        if (currentTime <= 0)
        {
            OnTimeUp();
        }
    }

    /// <summary>
    /// Called when time runs out. Override to define behavior.
    /// </summary>
    protected virtual void OnTimeUp()
    {
        EndGame(false);
    }

    /// <summary>
    /// Updates the score display.
    /// </summary>
    protected virtual void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }

        if (timerText != null)
        {
            timerText.text = currentTime.ToString("F1") + "s";
        }
    }

    /// <summary>
    /// Shows the game instructions.
    /// </summary>
    protected virtual void ShowInstructions()
    {
        if (instructionText != null)
        {
            instructionText.text = gameDescription;
        }
    }

    /// <summary>
    /// Adds points to the score.
    /// </summary>
    protected virtual void AddScore(int points)
    {
        currentScore += points;
        UpdateUI();
    }

    /// <summary>
    /// Called when the game starts. Override in subclasses.
    /// </summary>
    protected abstract void OnGameStarted();

    /// <summary>
    /// Called when the game stops. Override in subclasses.
    /// </summary>
    protected abstract void OnGameStopped();

    /// <summary>
    /// Resets the game to initial state.
    /// </summary>
    public virtual void ResetGame()
    {
        currentTime = gameDuration;
        currentScore = 0;
        isGameActive = false;
        UpdateUI();
    }
}
