using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// UI controller for the rewarded video ads feature.
/// Displays ad button, remaining ads count, and coin balance.
/// Can be integrated into the Boutique screen or any other screen.
/// </summary>
public class AdRewardUI : MonoBehaviour
{
    [Header("Coin Display")]
    [SerializeField] private TextMeshProUGUI coinBalanceText;

    [Header("Watch Ad Button")]
    [SerializeField] private Button watchAdButton;
    [SerializeField] private TextMeshProUGUI watchAdButtonText;

    [Header("Ads Remaining Display")]
    [SerializeField] private TextMeshProUGUI adsRemainingText;
    [SerializeField] private TextMeshProUGUI nextResetText;

    [Header("Reward Info")]
    [SerializeField] private TextMeshProUGUI rewardInfoText;

    [Header("Feedback Panel")]
    [SerializeField] private GameObject feedbackPanel;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private float feedbackDisplayDuration = 3f;

    private void OnEnable()
    {
        // Subscribe to events
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCoinsChanged += UpdateCoinDisplay;
        }

        if (AdRewardManager.Instance != null)
        {
            AdRewardManager.Instance.OnAdsWatchedChanged += UpdateAdsDisplay;
            AdRewardManager.Instance.OnAdRewardGranted += OnRewardGranted;
            AdRewardManager.Instance.OnAdFailed += OnAdFailed;
            AdRewardManager.Instance.OnAdReady += OnAdReady;
        }

        // Initial UI update
        UpdateAllUI();
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCoinsChanged -= UpdateCoinDisplay;
        }

        if (AdRewardManager.Instance != null)
        {
            AdRewardManager.Instance.OnAdsWatchedChanged -= UpdateAdsDisplay;
            AdRewardManager.Instance.OnAdRewardGranted -= OnRewardGranted;
            AdRewardManager.Instance.OnAdFailed -= OnAdFailed;
            AdRewardManager.Instance.OnAdReady -= OnAdReady;
        }
    }

    private void Start()
    {
        SetupButton();
        HideFeedback();
        UpdateAllUI();
    }

    private void Update()
    {
        // Update the reset timer display
        UpdateNextResetDisplay();
    }

    private void SetupButton()
    {
        if (watchAdButton != null)
        {
            watchAdButton.onClick.AddListener(OnWatchAdClicked);
        }
    }

    /// <summary>
    /// Updates all UI elements to reflect current state.
    /// </summary>
    private void UpdateAllUI()
    {
        UpdateCoinDisplay(CurrencyManager.Instance?.Coins ?? 0);
        
        if (AdRewardManager.Instance != null)
        {
            UpdateAdsDisplay(AdRewardManager.Instance.AdsWatchedToday, AdRewardManager.Instance.MaxDailyAds);
        }
        else
        {
            UpdateAdsDisplay(0, 5);
        }

        UpdateRewardInfo();
        UpdateButtonState();
    }

    /// <summary>
    /// Updates the coin balance display.
    /// </summary>
    private void UpdateCoinDisplay(int coins)
    {
        if (coinBalanceText != null)
        {
            coinBalanceText.text = coins.ToString();
        }
    }

    /// <summary>
    /// Updates the ads remaining display.
    /// </summary>
    private void UpdateAdsDisplay(int watched, int max)
    {
        int remaining = max - watched;

        if (adsRemainingText != null)
        {
            adsRemainingText.text = $"{remaining} / {max}";
        }

        UpdateButtonState();
    }

    /// <summary>
    /// Updates the next reset time display.
    /// </summary>
    private void UpdateNextResetDisplay()
    {
        if (nextResetText != null && AdRewardManager.Instance != null)
        {
            TimeSpan timeUntilReset = AdRewardManager.Instance.GetTimeUntilReset();
            if (timeUntilReset.TotalSeconds > 0)
            {
                nextResetText.text = $"Reset: {timeUntilReset.Hours:D2}:{timeUntilReset.Minutes:D2}:{timeUntilReset.Seconds:D2}";
            }
            else
            {
                nextResetText.text = "Reset maintenant !";
            }
        }
    }

    /// <summary>
    /// Updates the reward info text.
    /// </summary>
    private void UpdateRewardInfo()
    {
        if (rewardInfoText != null && AdRewardManager.Instance != null)
        {
            rewardInfoText.text = $"+{AdRewardManager.Instance.CoinsPerAd} pièces par vidéo";
        }
    }

    /// <summary>
    /// Updates the watch ad button state.
    /// </summary>
    private void UpdateButtonState()
    {
        if (watchAdButton == null) return;

        bool canWatch = AdRewardManager.Instance?.CanWatchAd ?? false;
        bool hasAdsRemaining = (AdRewardManager.Instance?.AdsRemaining ?? 0) > 0;

        watchAdButton.interactable = canWatch;

        if (watchAdButtonText != null)
        {
            if (!hasAdsRemaining)
            {
                watchAdButtonText.text = "Reviens demain !";
            }
            else if (!canWatch)
            {
                watchAdButtonText.text = "Chargement...";
            }
            else
            {
                watchAdButtonText.text = "Regarder une pub";
            }
        }
    }

    /// <summary>
    /// Called when the watch ad button is clicked.
    /// </summary>
    private void OnWatchAdClicked()
    {
        if (AdRewardManager.Instance != null)
        {
            AdRewardManager.Instance.ShowRewardedAd();
        }
    }

    /// <summary>
    /// Called when a reward is granted.
    /// </summary>
    private void OnRewardGranted(int coins)
    {
        ShowFeedback($"+{coins} pièces gagnées !", Color.green);
        UpdateButtonState();
    }

    /// <summary>
    /// Called when an ad fails.
    /// </summary>
    private void OnAdFailed(string message)
    {
        ShowFeedback(message, Color.red);
        UpdateButtonState();
    }

    /// <summary>
    /// Called when an ad is ready.
    /// </summary>
    private void OnAdReady()
    {
        UpdateButtonState();
    }

    /// <summary>
    /// Shows a feedback message.
    /// </summary>
    private void ShowFeedback(string message, Color color)
    {
        if (feedbackPanel != null)
        {
            feedbackPanel.SetActive(true);
        }

        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.color = color;
        }

        CancelInvoke(nameof(HideFeedback));
        Invoke(nameof(HideFeedback), feedbackDisplayDuration);
    }

    /// <summary>
    /// Hides the feedback panel.
    /// </summary>
    private void HideFeedback()
    {
        if (feedbackPanel != null)
        {
            feedbackPanel.SetActive(false);
        }
    }
}
