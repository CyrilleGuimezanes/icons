using UnityEngine;
using TMPro;
using System;

/// <summary>
/// UI component that displays the player's coin balance.
/// This component should be placed on a persistent UI element
/// that is always visible on screen.
/// </summary>
public class CoinDisplay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI coinIconText;
    [SerializeField] private TextMeshProUGUI coinAmountText;
    [SerializeField] private TextMeshProUGUI nextCoinTimerText;

    [Header("Settings")]
    [SerializeField] private bool showNextCoinTimer = true;
    [SerializeField] private string coinIconId = "paid";

    private void Start()
    {
        // Set the coin icon (Material Icons ligature)
        if (coinIconText != null)
        {
            coinIconText.text = coinIconId;
        }

        UpdateDisplay();
    }

    private void OnEnable()
    {
        // Subscribe to balance changes
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnBalanceChanged += OnBalanceChanged;
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from balance changes
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnBalanceChanged -= OnBalanceChanged;
        }
    }

    private void Update()
    {
        // Update the timer display
        if (showNextCoinTimer && nextCoinTimerText != null)
        {
            UpdateTimerDisplay();
        }
    }

    /// <summary>
    /// Updates the coin amount display.
    /// </summary>
    public void UpdateDisplay()
    {
        if (coinAmountText != null && CurrencyManager.Instance != null)
        {
            coinAmountText.text = CurrencyManager.Instance.Balance.ToString();
        }
    }

    /// <summary>
    /// Updates the next coin timer display.
    /// </summary>
    private void UpdateTimerDisplay()
    {
        if (CurrencyManager.Instance == null)
        {
            return;
        }

        TimeSpan remaining = CurrencyManager.Instance.GetTimeUntilNextCoin();
        if (remaining.TotalSeconds > 0)
        {
            nextCoinTimerText.text = $"{remaining.Minutes:D2}:{remaining.Seconds:D2}";
        }
        else
        {
            nextCoinTimerText.text = "00:00";
        }
    }

    /// <summary>
    /// Called when the coin balance changes.
    /// </summary>
    private void OnBalanceChanged(int newBalance)
    {
        if (coinAmountText != null)
        {
            coinAmountText.text = newBalance.ToString();
        }
    }
}
