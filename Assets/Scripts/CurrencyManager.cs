using UnityEngine;
using System;

/// <summary>
/// Manages the player's coin currency.
/// Provides methods to add, remove, and query coins.
/// Persists currency data across game sessions using PlayerPrefs.
/// </summary>
public class CurrencyManager : MonoBehaviour
{
    private const string SAVE_KEY = "PlayerCurrency";

    /// <summary>
    /// Singleton instance for global access.
    /// </summary>
    public static CurrencyManager Instance { get; private set; }

    /// <summary>
    /// Event triggered when the coin balance changes.
    /// </summary>
    public event Action<int> OnCoinsChanged;

    [SerializeField]
    private CurrencyData currencyData = new CurrencyData();

    private void Awake()
    {
        // Singleton pattern with persistence across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
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
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            Save();
        }
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    /// <summary>
    /// Gets the current coin balance.
    /// </summary>
    public int Coins => currencyData.coins;

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
        return true;
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
    /// Saves the currency data to persistent storage.
    /// </summary>
    public void Save()
    {
        string json = JsonUtility.ToJson(currencyData);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads the currency data from persistent storage.
    /// </summary>
    public void Load()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            if (!string.IsNullOrEmpty(json))
            {
                currencyData = JsonUtility.FromJson<CurrencyData>(json);
                if (currencyData == null)
                {
                    currencyData = new CurrencyData();
                }
            }
        }
    }
}

/// <summary>
/// Serializable container for currency data.
/// Used for JSON serialization of the currency.
/// </summary>
[Serializable]
public class CurrencyData
{
    public int coins = 0;
}
