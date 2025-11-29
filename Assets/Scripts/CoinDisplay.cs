using UnityEngine;
using TMPro;

/// <summary>
/// Simple coin display component that shows the current coin balance.
/// Can be placed anywhere in the UI (typically in the header).
/// </summary>
public class CoinDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private string prefix = "";
    [SerializeField] private string suffix = "";

    private void OnEnable()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCoinsChanged += UpdateDisplay;
            UpdateDisplay(CurrencyManager.Instance.Coins);
        }
    }

    private void OnDisable()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCoinsChanged -= UpdateDisplay;
        }
    }

    private void Start()
    {
        UpdateDisplay(CurrencyManager.Instance?.Coins ?? 0);
    }

    /// <summary>
    /// Updates the coin display text.
    /// </summary>
    private void UpdateDisplay(int coins)
    {
        if (coinText != null)
        {
            coinText.text = $"{prefix}{coins}{suffix}";
        }
    }
}
