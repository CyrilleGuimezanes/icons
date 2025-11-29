using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages hidden mini-games based on phone capabilities.
/// Hidden games are triggered by device states (battery, volume, orientation, etc.)
/// Each game can only be completed once, unlocking a special icon as reward.
/// </summary>
public class HiddenMiniGameManager : MonoBehaviour
{
    private const string SAVE_KEY = "HiddenMiniGames";

    /// <summary>
    /// Singleton instance for global access.
    /// </summary>
    public static HiddenMiniGameManager Instance { get; private set; }

    /// <summary>
    /// Event triggered when a hidden game is completed.
    /// </summary>
    public event Action<string, string> OnHiddenGameCompleted;

    [Header("Detection Settings")]
    [SerializeField] private float checkInterval = 1f;

    [Header("Battery Thresholds")]
    [SerializeField] private float lowBatteryThreshold = 0.2f;

    [Header("Volume Thresholds")]
    [SerializeField] private float silentVolumeThreshold = 0.1f;
    [SerializeField] private float loudVolumeThreshold = 0.9f;

    // Battery level constant
    private const float BATTERY_LEVEL_UNAVAILABLE = -1f;

    [SerializeField]
    private HiddenMiniGameData saveData = new HiddenMiniGameData();

    private float lastCheckTime;

    // Hidden game IDs
    public const string GAME_BATTERY_LOW = "hidden_battery_low";
    public const string GAME_BATTERY_CHARGING = "hidden_battery_charging";
    public const string GAME_SILENT_MODE = "hidden_silent_mode";
    public const string GAME_LOUD_MODE = "hidden_loud_mode";
    public const string GAME_PORTRAIT_MODE = "hidden_portrait_mode";
    public const string GAME_LANDSCAPE_MODE = "hidden_landscape_mode";
    public const string GAME_AIRPLANE_MODE = "hidden_airplane_mode";
    public const string GAME_STRONG_NETWORK = "hidden_strong_network";

    // Icon rewards for each hidden game
    private static readonly Dictionary<string, string> GameIconRewards = new Dictionary<string, string>
    {
        { GAME_BATTERY_LOW, "battery_0_bar" },
        { GAME_BATTERY_CHARGING, "battery_charging_full" },
        { GAME_SILENT_MODE, "volume_off" },
        { GAME_LOUD_MODE, "volume_up" },
        { GAME_PORTRAIT_MODE, "stay_current_portrait" },
        { GAME_LANDSCAPE_MODE, "stay_current_landscape" },
        { GAME_AIRPLANE_MODE, "airplanemode_active" },
        { GAME_STRONG_NETWORK, "signal_cellular_alt" }
    };

    private void Awake()
    {
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

    private void Update()
    {
        // Check device states periodically
        if (Time.time - lastCheckTime >= checkInterval)
        {
            lastCheckTime = Time.time;
            CheckAllHiddenGames();
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Save();
        }
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    /// <summary>
    /// Checks all hidden game conditions.
    /// </summary>
    private void CheckAllHiddenGames()
    {
        CheckBatteryLow();
        CheckBatteryCharging();
        CheckSilentMode();
        CheckLoudMode();
        CheckPortraitMode();
        CheckLandscapeMode();
        CheckAirplaneMode();
        CheckStrongNetwork();
    }

    /// <summary>
    /// Checks if battery is low (≤ 20%).
    /// </summary>
    private void CheckBatteryLow()
    {
        if (IsGameCompleted(GAME_BATTERY_LOW)) return;

        float batteryLevel = SystemInfo.batteryLevel;
        BatteryStatus batteryStatus = SystemInfo.batteryStatus;

        // Check battery level is available and below threshold
        if (batteryLevel > BATTERY_LEVEL_UNAVAILABLE && batteryLevel <= lowBatteryThreshold && 
            batteryStatus == BatteryStatus.Discharging)
        {
            CompleteHiddenGame(GAME_BATTERY_LOW);
        }
    }

    /// <summary>
    /// Checks if device is charging.
    /// </summary>
    private void CheckBatteryCharging()
    {
        if (IsGameCompleted(GAME_BATTERY_CHARGING)) return;

        BatteryStatus batteryStatus = SystemInfo.batteryStatus;

        if (batteryStatus == BatteryStatus.Charging || batteryStatus == BatteryStatus.Full)
        {
            CompleteHiddenGame(GAME_BATTERY_CHARGING);
        }
    }

    /// <summary>
    /// Checks if device is in silent mode (volume very low).
    /// Note: Uses AudioListener.volume as a proxy for game volume settings.
    /// This reflects the in-game audio level, not the actual device system volume.
    /// For accurate system volume detection, platform-specific plugins would be needed.
    /// </summary>
    private void CheckSilentMode()
    {
        if (IsGameCompleted(GAME_SILENT_MODE)) return;

        // Use AudioListener volume as a proxy for game audio settings
        float volume = AudioListener.volume;

        if (volume <= silentVolumeThreshold)
        {
            CompleteHiddenGame(GAME_SILENT_MODE);
        }
    }

    /// <summary>
    /// Checks if device volume is high.
    /// Note: Uses AudioListener.volume as a proxy for game volume settings.
    /// This reflects the in-game audio level, not the actual device system volume.
    /// </summary>
    private void CheckLoudMode()
    {
        if (IsGameCompleted(GAME_LOUD_MODE)) return;

        // Use AudioListener volume as a proxy for game audio settings
        float volume = AudioListener.volume;

        if (volume >= loudVolumeThreshold)
        {
            CompleteHiddenGame(GAME_LOUD_MODE);
        }
    }

    /// <summary>
    /// Checks if device is in portrait orientation.
    /// </summary>
    private void CheckPortraitMode()
    {
        if (IsGameCompleted(GAME_PORTRAIT_MODE)) return;

        DeviceOrientation orientation = Input.deviceOrientation;

        if (orientation == DeviceOrientation.Portrait || 
            orientation == DeviceOrientation.PortraitUpsideDown)
        {
            CompleteHiddenGame(GAME_PORTRAIT_MODE);
        }
    }

    /// <summary>
    /// Checks if device is in landscape orientation.
    /// </summary>
    private void CheckLandscapeMode()
    {
        if (IsGameCompleted(GAME_LANDSCAPE_MODE)) return;

        DeviceOrientation orientation = Input.deviceOrientation;

        if (orientation == DeviceOrientation.LandscapeLeft || 
            orientation == DeviceOrientation.LandscapeRight)
        {
            CompleteHiddenGame(GAME_LANDSCAPE_MODE);
        }
    }

    /// <summary>
    /// Checks if device is in airplane mode (no network).
    /// </summary>
    private void CheckAirplaneMode()
    {
        if (IsGameCompleted(GAME_AIRPLANE_MODE)) return;

        NetworkReachability reachability = Application.internetReachability;

        if (reachability == NetworkReachability.NotReachable)
        {
            CompleteHiddenGame(GAME_AIRPLANE_MODE);
        }
    }

    /// <summary>
    /// Checks if device has strong network signal.
    /// </summary>
    private void CheckStrongNetwork()
    {
        if (IsGameCompleted(GAME_STRONG_NETWORK)) return;

        NetworkReachability reachability = Application.internetReachability;

        // Reward for having WiFi connection (typically stronger signal)
        if (reachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            CompleteHiddenGame(GAME_STRONG_NETWORK);
        }
    }

    /// <summary>
    /// Completes a hidden game and awards the associated icon.
    /// </summary>
    private void CompleteHiddenGame(string gameId)
    {
        if (string.IsNullOrEmpty(gameId) || IsGameCompleted(gameId))
        {
            return;
        }

        // Mark as completed
        saveData.completedGameIds.Add(gameId);
        Save();

        // Get the reward icon
        string rewardIconId = GetRewardIconId(gameId);
        if (string.IsNullOrEmpty(rewardIconId))
        {
            return;
        }

        // Add icon to inventory
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.AddIcon(rewardIconId, 1);
        }

        // Unlock the icon
        if (UnlockedIconsManager.Instance != null)
        {
            UnlockedIconsManager.Instance.UnlockIcon(rewardIconId);
        }

        Debug.Log($"Hidden game completed: {gameId} - Unlocked icon: {rewardIconId}");
        OnHiddenGameCompleted?.Invoke(gameId, rewardIconId);
    }

    /// <summary>
    /// Checks if a hidden game has been completed.
    /// </summary>
    public bool IsGameCompleted(string gameId)
    {
        if (string.IsNullOrEmpty(gameId))
        {
            return false;
        }

        return saveData.completedGameIds.Contains(gameId);
    }

    /// <summary>
    /// Gets the reward icon ID for a hidden game.
    /// </summary>
    public string GetRewardIconId(string gameId)
    {
        if (GameIconRewards.TryGetValue(gameId, out string iconId))
        {
            return iconId;
        }
        return null;
    }

    /// <summary>
    /// Gets the number of completed hidden games.
    /// </summary>
    public int GetCompletedGameCount()
    {
        return saveData.completedGameIds.Count;
    }

    /// <summary>
    /// Gets the total number of hidden games.
    /// </summary>
    public int GetTotalGameCount()
    {
        return GameIconRewards.Count;
    }

    /// <summary>
    /// Gets all completed game IDs.
    /// </summary>
    public List<string> GetCompletedGameIds()
    {
        return new List<string>(saveData.completedGameIds);
    }

    /// <summary>
    /// Clears all hidden game completion data (for testing or reset).
    /// </summary>
    public void ClearCompletionData()
    {
        saveData.completedGameIds.Clear();
        Save();
    }

    /// <summary>
    /// Saves hidden game data to persistent storage.
    /// </summary>
    public void Save()
    {
        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Loads hidden game data from persistent storage.
    /// </summary>
    public void Load()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            if (!string.IsNullOrEmpty(json))
            {
                saveData = JsonUtility.FromJson<HiddenMiniGameData>(json);
                if (saveData == null)
                {
                    saveData = new HiddenMiniGameData();
                }
            }
        }
    }

    /// <summary>
    /// Gets all hidden game definitions with their completion status.
    /// </summary>
    public List<HiddenGameInfo> GetAllHiddenGames()
    {
        var games = new List<HiddenGameInfo>
        {
            new HiddenGameInfo(GAME_BATTERY_LOW, "Batterie Faible", 
                "Jouez avec une batterie à 20% ou moins", 
                GetRewardIconId(GAME_BATTERY_LOW), IsGameCompleted(GAME_BATTERY_LOW)),
            new HiddenGameInfo(GAME_BATTERY_CHARGING, "En Charge", 
                "Jouez pendant que votre téléphone charge", 
                GetRewardIconId(GAME_BATTERY_CHARGING), IsGameCompleted(GAME_BATTERY_CHARGING)),
            new HiddenGameInfo(GAME_SILENT_MODE, "Mode Silencieux", 
                "Jouez avec le volume au minimum", 
                GetRewardIconId(GAME_SILENT_MODE), IsGameCompleted(GAME_SILENT_MODE)),
            new HiddenGameInfo(GAME_LOUD_MODE, "Volume Maximum", 
                "Jouez avec le volume au maximum", 
                GetRewardIconId(GAME_LOUD_MODE), IsGameCompleted(GAME_LOUD_MODE)),
            new HiddenGameInfo(GAME_PORTRAIT_MODE, "Mode Portrait", 
                "Jouez en tenant le téléphone verticalement", 
                GetRewardIconId(GAME_PORTRAIT_MODE), IsGameCompleted(GAME_PORTRAIT_MODE)),
            new HiddenGameInfo(GAME_LANDSCAPE_MODE, "Mode Paysage", 
                "Jouez en tenant le téléphone horizontalement", 
                GetRewardIconId(GAME_LANDSCAPE_MODE), IsGameCompleted(GAME_LANDSCAPE_MODE)),
            new HiddenGameInfo(GAME_AIRPLANE_MODE, "Mode Avion", 
                "Jouez sans connexion internet", 
                GetRewardIconId(GAME_AIRPLANE_MODE), IsGameCompleted(GAME_AIRPLANE_MODE)),
            new HiddenGameInfo(GAME_STRONG_NETWORK, "Connexion WiFi", 
                "Jouez connecté au WiFi", 
                GetRewardIconId(GAME_STRONG_NETWORK), IsGameCompleted(GAME_STRONG_NETWORK))
        };
        return games;
    }
}

/// <summary>
/// Serializable data for hidden mini-game completion tracking.
/// </summary>
[Serializable]
public class HiddenMiniGameData
{
    public List<string> completedGameIds = new List<string>();
}

/// <summary>
/// Information about a hidden game.
/// </summary>
[Serializable]
public class HiddenGameInfo
{
    public string gameId;
    public string displayName;
    public string description;
    public string rewardIconId;
    public bool isCompleted;

    public HiddenGameInfo(string gameId, string displayName, string description, string rewardIconId, bool isCompleted)
    {
        this.gameId = gameId;
        this.displayName = displayName;
        this.description = description;
        this.rewardIconId = rewardIconId;
        this.isCompleted = isCompleted;
    }
}
