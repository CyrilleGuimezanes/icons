using UnityEngine;
using System;

/// <summary>
/// Manages the player's in-game currency (coins).
/// Provides methods to add, spend, and track coin balance.
/// Implements passive income system: 1 coin per hour.
/// Persists currency data across game sessions using PlayerPrefs.
/// </summary>
public class CurrencyManager : MonoBehaviour
{
    private const string SAVE_KEY_BASE = "CurrencyData";
    private const string COIN_ICON_ID = "paid";

    /// <summary>
    /// Coins earned per hour passively.
    /// </summary>
    public const int COINS_PER_HOUR = 1;

    /// <summary>
    /// Singleton instance for global access.
    /// </summary>
    public static CurrencyManager Instance { get; private set; }

    /// <summary>
    /// Gets the current save key with slot suffix.
    /// </summary>
    private string SaveKey
    {
        get
        {
            if (GameSlotsManager.Instance != null)
            {
                return SAVE_KEY_BASE + GameSlotsManager.Instance.GetCurrentSlotSuffix();
            }
            return SAVE_KEY_BASE;
        }
    }

    /// <summary>
    /// Event triggered when the coin balance changes.
    /// </summary>
    public event Action<int> OnCoinsChanged;

    /// <summary>
    /// Event triggered when the coin balance changes (alias for OnCoinsChanged).
    /// </summary>
    public event Action<int> OnBalanceChanged;

    /// <summary>
    /// Event triggered when passive income is collected.
    /// </summary>
    public event Action<int> OnPassiveIncomeCollected;

    [SerializeField]
    private CurrencyData currencyData = new CurrencyData();

    /// <summary>
    /// The icon ID used for coins.
    /// </summary>
    public static string CoinIconId => COIN_ICON_ID;

    /// <summary>
    /// Gets the current coin balance.
    /// </summary>
    public int Coins => currencyData.coins;

    /// <summary>
    /// Current coin balance (alias for Coins).
    /// </summary>
    public int Balance => currencyData.coins;

    private void Awake()
    {
        // Singleton pattern with persistence across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
            CollectPassiveIncome();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Save();
        }
        else
        {
            CollectPassiveIncome();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            Save();
        }
        else
        {
            CollectPassiveIncome();
        }
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    /// <summary>
    /// Adds coins to the player's balance.
    /// </summary>
    /// <param name="amount">The amount of coins to add (must be positive).</param>
    public void AddCoins(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        currencyData.coins += amount;
        Save();
        OnCoinsChanged?.Invoke(currencyData.coins);
        OnBalanceChanged?.Invoke(currencyData.coins);
    }

    /// <summary>
    /// Removes coins from the player's balance.
    /// </summary>
    /// <param name="amount">The amount of coins to remove (must be positive).</param>
    /// <returns>True if the removal was successful, false if insufficient coins.</returns>
    public bool RemoveCoins(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (currencyData.coins < amount)
        {
            return false;
        }

        currencyData.coins -= amount;
        Save();
        OnCoinsChanged?.Invoke(currencyData.coins);
        OnBalanceChanged?.Invoke(currencyData.coins);
        return true;
    }

    /// <summary>
    /// Attempts to spend coins from the player's balance.
    /// Alias for RemoveCoins for compatibility.
    /// </summary>
    /// <param name="amount">The amount of coins to spend (must be positive).</param>
    /// <returns>True if the transaction was successful, false if insufficient funds.</returns>
    public bool SpendCoins(int amount)
    {
        return RemoveCoins(amount);
    }

    /// <summary>
    /// Checks if the player has at least the specified amount of coins.
    /// </summary>
    /// <param name="amount">The required amount of coins.</param>
    /// <returns>True if the player has enough coins.</returns>
    public bool HasCoins(int amount)
    {
        return currencyData.coins >= amount;
    }

    /// <summary>
    /// Checks if the player can afford a certain amount.
    /// Alias for HasCoins for compatibility.
    /// </summary>
    /// <param name="amount">The amount to check.</param>
    /// <returns>True if the player has enough coins.</returns>
    public bool CanAfford(int amount)
    {
        return HasCoins(amount);
    }

    /// <summary>
    /// Collects passive income earned since last collection.
    /// Called on app resume and startup.
    /// </summary>
    public void CollectPassiveIncome()
    {
        DateTime lastCollection = DateTime.FromBinary(currencyData.lastPassiveIncomeTimestamp);
        TimeSpan timeSinceCollection = DateTime.UtcNow - lastCollection;

        // Calculate hours elapsed (only full hours count)
        int hoursElapsed = (int)timeSinceCollection.TotalHours;

        if (hoursElapsed > 0)
        {
            int coinsEarned = hoursElapsed * COINS_PER_HOUR;
            currencyData.coins += coinsEarned;
            currencyData.lastPassiveIncomeTimestamp = DateTime.UtcNow.ToBinary();
            Save();
            OnCoinsChanged?.Invoke(currencyData.coins);
            OnBalanceChanged?.Invoke(currencyData.coins);
            OnPassiveIncomeCollected?.Invoke(coinsEarned);
        }
    }

    /// <summary>
    /// Gets the time until the next passive coin is earned.
    /// </summary>
    /// <returns>TimeSpan until next coin.</returns>
    public TimeSpan GetTimeUntilNextCoin()
    {
        DateTime lastCollection = DateTime.FromBinary(currencyData.lastPassiveIncomeTimestamp);
        DateTime nextCoin = lastCollection.AddHours(1);
        TimeSpan remaining = nextCoin - DateTime.UtcNow;
        return remaining.TotalSeconds > 0 ? remaining : TimeSpan.Zero;
    }

    /// <summary>
    /// Saves the currency data to persistent storage.
    /// </summary>
    public void Save()
    {
        string json = JsonUtility.ToJson(currencyData);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads the currency data from persistent storage.
    /// </summary>
    public void Load()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            string json = PlayerPrefs.GetString(SaveKey);
            if (!string.IsNullOrEmpty(json))
            {
                currencyData = JsonUtility.FromJson<CurrencyData>(json);
                if (currencyData == null)
                {
                    currencyData = new CurrencyData();
                }
            }
        }
        else
        {
            currencyData = new CurrencyData();
        }
        OnCoinsChanged?.Invoke(currencyData.coins);
        OnBalanceChanged?.Invoke(currencyData.coins);
    }

    /// <summary>
    /// Resets currency data (for testing purposes).
    /// </summary>
    public void ResetCurrency()
    {
        currencyData = new CurrencyData();
        Save();
        OnCoinsChanged?.Invoke(currencyData.coins);
        OnBalanceChanged?.Invoke(currencyData.coins);
    }
}

/// <summary>
/// Serializable container for currency data.
/// Used for JSON serialization of currency state.
/// </summary>
[Serializable]
public class CurrencyData
{
    /// <summary>
    /// Current coin balance.
    /// </summary>
    public int coins = 0;

    /// <summary>
    /// Timestamp of last passive income collection.
    /// </summary>
    public long lastPassiveIncomeTimestamp;

    public CurrencyData()
    {
        coins = 0;
        lastPassiveIncomeTimestamp = DateTime.UtcNow.ToBinary();
    }
}
