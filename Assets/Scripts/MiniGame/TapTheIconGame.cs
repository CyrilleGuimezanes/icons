using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// "Tap the Icon" mini-game.
/// Player must tap the target icon 10 times within 3 seconds.
/// </summary>
public class TapTheIconGame : MiniGameBase
{
    [Header("Tap Game Settings")]
    [SerializeField] private int requiredTaps = 10;
    [SerializeField] private float timeLimit = 3f;

    [Header("UI References")]
    [SerializeField] private Button tapButton;
    [SerializeField] private TextMeshProUGUI tapIconText;
    [SerializeField] private TextMeshProUGUI tapCountText;
    [SerializeField] private TextMeshProUGUI targetCountText;
    [SerializeField] private Image progressBar;

    [Header("Animation")]
    [SerializeField] private float tapScaleMultiplier = 1.2f;
    [SerializeField] private float tapAnimationDuration = 0.1f;

    private int tapCount;
    private IconEntry targetIcon;
    private Coroutine scaleAnimation;

    private void Awake()
    {
        base.Awake();
        gameName = "Tape l'icône !";
        gameDescription = $"Tape sur l'icône {requiredTaps} fois en {timeLimit} secondes !";
        gameDuration = timeLimit;
    }

    /// <summary>
    /// Initializes references for runtime UI building.
    /// </summary>
    public void InitializeReferences(TextMeshProUGUI timer, TextMeshProUGUI score, TextMeshProUGUI instruction,
        GameObject panel, Button tapBtn, TextMeshProUGUI tapIcon, TextMeshProUGUI tapCount,
        TextMeshProUGUI targetCount, Image progress)
    {
        timerText = timer;
        scoreText = score;
        instructionText = instruction;
        gamePanel = panel;
        tapButton = tapBtn;
        tapIconText = tapIcon;
        tapCountText = tapCount;
        targetCountText = targetCount;
        progressBar = progress;
    }

    private void Start()
    {
        if (tapButton != null)
        {
            tapButton.onClick.AddListener(OnTap);
        }
    }

    protected override void OnGameStarted()
    {
        tapCount = 0;

        // Get a random icon to display
        if (IconDatabase.Instance != null)
        {
            targetIcon = IconDatabase.Instance.GetRandomIcon();
            if (tapIconText != null && targetIcon != null)
            {
                tapIconText.text = targetIcon.id;
            }
        }

        UpdateTapUI();
    }

    protected override void OnGameStopped()
    {
        if (scaleAnimation != null)
        {
            StopCoroutine(scaleAnimation);
            scaleAnimation = null;
        }

        // Reset button scale
        if (tapButton != null)
        {
            tapButton.transform.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// Called when the tap button is clicked.
    /// </summary>
    private void OnTap()
    {
        if (!isGameActive) return;

        tapCount++;
        UpdateTapUI();

        // Animate the tap
        if (scaleAnimation != null)
        {
            StopCoroutine(scaleAnimation);
        }
        scaleAnimation = StartCoroutine(AnimateTap());

        // Check if goal reached
        if (tapCount >= requiredTaps)
        {
            EndGame(true);
        }
    }

    private void UpdateTapUI()
    {
        if (tapCountText != null)
        {
            tapCountText.text = tapCount.ToString();
        }

        if (targetCountText != null)
        {
            targetCountText.text = $"/ {requiredTaps}";
        }

        if (progressBar != null)
        {
            progressBar.fillAmount = (float)tapCount / requiredTaps;
        }
    }

    private IEnumerator AnimateTap()
    {
        if (tapButton == null) yield break;

        Transform buttonTransform = tapButton.transform;
        Vector3 originalScale = Vector3.one;
        Vector3 targetScale = Vector3.one * tapScaleMultiplier;

        // Scale up
        float elapsed = 0f;
        while (elapsed < tapAnimationDuration / 2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (tapAnimationDuration / 2f);
            buttonTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        // Scale down
        elapsed = 0f;
        while (elapsed < tapAnimationDuration / 2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (tapAnimationDuration / 2f);
            buttonTransform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        buttonTransform.localScale = originalScale;
        scaleAnimation = null;
    }

    protected override void OnTimeUp()
    {
        // Only fail if we haven't reached the goal
        EndGame(tapCount >= requiredTaps);
    }
}
