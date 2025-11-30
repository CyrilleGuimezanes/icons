using UnityEngine;
using System;

/// <summary>
/// Manages rewarded video ads using Unity Ads.
/// Players can watch up to 5 videos per day to earn coins.
/// Each video watched rewards the player with 5 coins.
/// Note: Requires Unity Ads package (com.unity.ads) to be installed for full functionality.
/// When Unity Ads is not available, ads will show as "not available".
/// </summary>
public class AdRewardManager : MonoBehaviour
{
    private const string SAVE_KEY = "AdRewardData";
    private const int MAX_DAILY_ADS = 5;
    private const int COINS_PER_AD = 5;
    private const float RESET_HOURS = 24f;

    // Unity Ads configuration - Replace with your actual Game IDs
    [Header("Unity Ads Configuration")]
    [SerializeField] private string androidGameId = "YOUR_ANDROID_GAME_ID";
    [SerializeField] private string iosGameId = "YOUR_IOS_GAME_ID";
    [SerializeField] private string rewardedAdUnitId = "Rewarded_Android";
    [SerializeField] private bool testMode = true;

    /// <summary>
    /// Singleton instance for global access.
    /// </summary>
    public static AdRewardManager Instance { get; private set; }

    /// <summary>
    /// Event triggered when ads watched count changes.
    /// </summary>
    public event Action<int, int> OnAdsWatchedChanged; // (current, max)

    /// <summary>
    /// Event triggered when coins are rewarded from watching an ad.
    /// </summary>
    public event Action<int> OnAdRewardGranted;

    /// <summary>
    /// Event triggered when an ad fails to load or show.
    /// </summary>
    public event Action<string> OnAdFailed;

    /// <summary>
    /// Event triggered when the ad is ready to be shown.
    /// </summary>
    public event Action OnAdReady;

    private AdRewardData saveData;
    private bool isAdLoaded = false;
    private bool isInitialized = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
            InitializeAds();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        CheckDailyReset();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveData();
        }
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    /// <summary>
    /// Initializes Unity Ads.
    /// Note: Unity Ads package is not currently installed. Install com.unity.ads to enable ads.
    /// </summary>
    private void InitializeAds()
    {
        // Unity Ads package is not installed
        // To enable ads, add "com.unity.ads" to your Packages/manifest.json
        Debug.LogWarning("AdRewardManager: Unity Ads package is not installed. Ads will not be available.");
        isInitialized = false;
        isAdLoaded = false;
    }

    /// <summary>
    /// Gets the number of ads watched today.
    /// </summary>
    public int AdsWatchedToday => saveData.adsWatchedToday;

    /// <summary>
    /// Gets the maximum number of ads allowed per day.
    /// </summary>
    public int MaxDailyAds => MAX_DAILY_ADS;

    /// <summary>
    /// Gets the number of ads remaining today.
    /// </summary>
    public int AdsRemaining => MAX_DAILY_ADS - saveData.adsWatchedToday;

    /// <summary>
    /// Gets whether the player can watch more ads today.
    /// </summary>
    public bool CanWatchAd => saveData.adsWatchedToday < MAX_DAILY_ADS && isAdLoaded;

    /// <summary>
    /// Gets the coins rewarded per ad.
    /// </summary>
    public int CoinsPerAd => COINS_PER_AD;

    /// <summary>
    /// Gets whether an ad is currently loaded and ready to show.
    /// </summary>
    public bool IsAdReady => isAdLoaded;

    /// <summary>
    /// Shows a rewarded video ad if available.
    /// </summary>
    public void ShowRewardedAd()
    {
        if (saveData.adsWatchedToday >= MAX_DAILY_ADS)
        {
            OnAdFailed?.Invoke("Vous avez atteint la limite quotidienne de publicités.");
            return;
        }

        if (!isInitialized)
        {
            OnAdFailed?.Invoke("Les publicités ne sont pas disponibles. Le package Unity Ads n'est pas installé.");
            return;
        }

        if (!isAdLoaded)
        {
            OnAdFailed?.Invoke("La publicité n'est pas encore prête. Veuillez réessayer.");
            LoadAd();
            return;
        }

        // Unity Ads not available - show error
        OnAdFailed?.Invoke("Les publicités ne sont pas disponibles.");
    }

    /// <summary>
    /// Loads a rewarded ad.
    /// </summary>
    public void LoadAd()
    {
        // Unity Ads not available
        if (!isInitialized)
        {
            return;
        }
    }

    /// <summary>
    /// Gets the time remaining until the daily reset.
    /// </summary>
    public TimeSpan GetTimeUntilReset()
    {
        DateTime lastReset = DateTime.FromBinary(saveData.lastResetTimestamp);
        DateTime nextReset = lastReset.AddHours(RESET_HOURS);
        return nextReset - DateTime.Now;
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
            saveData.adsWatchedToday = 0;
            saveData.lastResetTimestamp = DateTime.Now.ToBinary();
            SaveData();
            OnAdsWatchedChanged?.Invoke(saveData.adsWatchedToday, MAX_DAILY_ADS);
        }
    }

    /// <summary>
    /// Grants the coin reward for watching an ad.
    /// </summary>
    private void GrantReward()
    {
        saveData.adsWatchedToday++;
        SaveData();

        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddCoins(COINS_PER_AD);
        }

        OnAdRewardGranted?.Invoke(COINS_PER_AD);
        OnAdsWatchedChanged?.Invoke(saveData.adsWatchedToday, MAX_DAILY_ADS);

        // Load next ad
        LoadAd();
    }

    /// <summary>
    /// Saves the ad reward data.
    /// </summary>
    private void SaveData()
    {
        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads the ad reward data.
    /// </summary>
    private void LoadData()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            if (!string.IsNullOrEmpty(json))
            {
                saveData = JsonUtility.FromJson<AdRewardData>(json);
                if (saveData == null)
                {
                    saveData = new AdRewardData();
                }
            }
            else
            {
                saveData = new AdRewardData();
            }
        }
        else
        {
            saveData = new AdRewardData();
        }
    }
}

/// <summary>
/// Serializable data for ad reward state persistence.
/// </summary>
[Serializable]
public class AdRewardData
{
    public int adsWatchedToday = 0;
    public long lastResetTimestamp;

    public AdRewardData()
    {
        adsWatchedToday = 0;
        lastResetTimestamp = DateTime.Now.ToBinary();
    }
}
