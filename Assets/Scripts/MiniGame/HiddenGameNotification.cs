using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Displays a notification popup when a hidden mini-game is completed.
/// Shows the unlocked icon and animates the reveal.
/// </summary>
public class HiddenGameNotification : MonoBehaviour
{
    /// <summary>
    /// Singleton instance for global access.
    /// </summary>
    public static HiddenGameNotification Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI iconText;
    [SerializeField] private TextMeshProUGUI iconNameText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Button dismissButton;

    [Header("Animation Settings")]
    [SerializeField] private float displayDuration = 5f;
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.3f;

    [Header("Colors")]
    [SerializeField] private Color notificationColor = new Color(1f, 0.8f, 0.2f, 0.95f);

    private CanvasGroup canvasGroup;
    private Coroutine displayCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        canvasGroup = GetComponentInChildren<CanvasGroup>();
        if (canvasGroup == null && notificationPanel != null)
        {
            canvasGroup = notificationPanel.AddComponent<CanvasGroup>();
        }

        if (dismissButton != null)
        {
            dismissButton.onClick.AddListener(DismissNotification);
        }

        HideNotification();
    }

    private void Start()
    {
        // Subscribe to hidden game completion events
        if (HiddenMiniGameManager.Instance != null)
        {
            HiddenMiniGameManager.Instance.OnHiddenGameCompleted += OnHiddenGameCompleted;
        }
    }

    private void OnDestroy()
    {
        if (HiddenMiniGameManager.Instance != null)
        {
            HiddenMiniGameManager.Instance.OnHiddenGameCompleted -= OnHiddenGameCompleted;
        }
    }

    /// <summary>
    /// Called when a hidden game is completed.
    /// </summary>
    private void OnHiddenGameCompleted(string gameId, string rewardIconId)
    {
        // Get game info for display
        var gameInfo = GetGameDisplayInfo(gameId);
        ShowNotification(gameInfo.title, gameInfo.description, rewardIconId, gameInfo.iconName);
    }

    /// <summary>
    /// Shows the notification with the specified content.
    /// </summary>
    public void ShowNotification(string title, string description, string iconId, string iconName)
    {
        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
        }

        displayCoroutine = StartCoroutine(DisplayNotificationCoroutine(title, description, iconId, iconName));
    }

    private IEnumerator DisplayNotificationCoroutine(string title, string description, string iconId, string iconName)
    {
        // Set content
        if (titleText != null) titleText.text = title;
        if (descriptionText != null) descriptionText.text = description;
        if (iconText != null) iconText.text = iconId;
        if (iconNameText != null) iconNameText.text = iconName;
        if (backgroundImage != null) backgroundImage.color = notificationColor;

        // Show panel
        if (notificationPanel != null) notificationPanel.SetActive(true);

        // Fade in
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
                yield return null;
            }
            canvasGroup.alpha = 1f;
        }

        // Wait for display duration
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        if (canvasGroup != null)
        {
            float elapsed = 0f;
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
                yield return null;
            }
            canvasGroup.alpha = 0f;
        }

        HideNotification();
        displayCoroutine = null;
    }

    /// <summary>
    /// Dismisses the notification immediately.
    /// </summary>
    public void DismissNotification()
    {
        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
            displayCoroutine = null;
        }

        HideNotification();
    }

    private void HideNotification()
    {
        if (notificationPanel != null)
        {
            notificationPanel.SetActive(false);
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }

    /// <summary>
    /// Gets display information for a hidden game.
    /// </summary>
    private (string title, string description, string iconName) GetGameDisplayInfo(string gameId)
    {
        return gameId switch
        {
            HiddenMiniGameManager.GAME_BATTERY_LOW => 
                ("🔋 Défi Batterie Faible!", "Tu as joué avec seulement 20% de batterie!", "Batterie Faible"),
            HiddenMiniGameManager.GAME_BATTERY_CHARGING => 
                ("🔌 Défi En Charge!", "Tu as joué pendant que ton téléphone chargeait!", "Batterie Pleine"),
            HiddenMiniGameManager.GAME_SILENT_MODE => 
                ("🔇 Défi Mode Silencieux!", "Tu as joué sans le son!", "Volume Off"),
            HiddenMiniGameManager.GAME_LOUD_MODE => 
                ("🔊 Défi Volume Maximum!", "Tu as joué à fond!", "Volume Max"),
            HiddenMiniGameManager.GAME_PORTRAIT_MODE => 
                ("📱 Défi Mode Portrait!", "Tu as tenu ton téléphone verticalement!", "Mode Portrait"),
            HiddenMiniGameManager.GAME_LANDSCAPE_MODE => 
                ("🖥️ Défi Mode Paysage!", "Tu as tourné ton téléphone!", "Mode Paysage"),
            HiddenMiniGameManager.GAME_AIRPLANE_MODE => 
                ("✈️ Défi Mode Avion!", "Tu as joué hors ligne!", "Mode Avion"),
            HiddenMiniGameManager.GAME_STRONG_NETWORK => 
                ("📶 Défi WiFi!", "Tu as une bonne connexion!", "Signal Fort"),
            _ => ("🎮 Défi Secret!", "Tu as découvert un secret!", "Secret")
        };
    }
}
