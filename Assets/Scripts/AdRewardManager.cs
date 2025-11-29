using UnityEngine;
using UnityEngine.Advertisements;
using System;

/// <summary>
/// Manages rewarded video ads using Unity Ads.
/// Players can watch up to 5 videos per day to earn coins.
/// Each video watched rewards the player with 5 coins.
/// </summary>
public class AdRewardManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
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
    private string gameId;
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
    /// </summary>
    private void InitializeAds()
    {
#if UNITY_IOS
        gameId = iosGameId;
        rewardedAdUnitId = "Rewarded_iOS";
#elif UNITY_ANDROID
        gameId = androidGameId;
        rewardedAdUnitId = "Rewarded_Android";
#elif UNITY_EDITOR
        // For testing in editor, default to Android
        gameId = androidGameId;
#endif

        // Validate game ID configuration
        if (string.IsNullOrEmpty(gameId) || gameId.StartsWith("YOUR_"))
        {
            Debug.LogWarning("AdRewardManager: Unity Ads Game ID is not configured. Please set your Game ID in the inspector.");
            return;
        }

        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(gameId, testMode, this);
        }
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

        if (!isAdLoaded)
        {
            OnAdFailed?.Invoke("La publicité n'est pas encore prête. Veuillez réessayer.");
            LoadAd();
            return;
        }

        Advertisement.Show(rewardedAdUnitId, this);
    }

    /// <summary>
    /// Loads a rewarded ad.
    /// </summary>
    public void LoadAd()
    {
        if (isInitialized)
        {
            Advertisement.Load(rewardedAdUnitId, this);
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

    #region IUnityAdsInitializationListener

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
        isInitialized = true;
        LoadAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError($"Unity Ads initialization failed: {error} - {message}");
        OnAdFailed?.Invoke($"Erreur d'initialisation des publicités: {message}");
    }

    #endregion

    #region IUnityAdsLoadListener

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log($"Ad loaded: {placementId}");
        if (placementId == rewardedAdUnitId)
        {
            isAdLoaded = true;
            OnAdReady?.Invoke();
        }
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogError($"Failed to load ad {placementId}: {error} - {message}");
        isAdLoaded = false;
        OnAdFailed?.Invoke($"Impossible de charger la publicité: {message}");
    }

    #endregion

    #region IUnityAdsShowListener

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId == rewardedAdUnitId)
        {
            isAdLoaded = false;

            if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
            {
                // User watched the entire ad, grant reward
                GrantReward();
            }
            else if (showCompletionState == UnityAdsShowCompletionState.SKIPPED)
            {
                Debug.Log("Ad was skipped, no reward granted.");
            }

            // Load the next ad
            LoadAd();
        }
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogError($"Failed to show ad {placementId}: {error} - {message}");
        isAdLoaded = false;
        OnAdFailed?.Invoke($"Erreur lors de l'affichage de la publicité: {message}");
        LoadAd();
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log($"Ad started: {placementId}");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log($"Ad clicked: {placementId}");
    }

    #endregion

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
