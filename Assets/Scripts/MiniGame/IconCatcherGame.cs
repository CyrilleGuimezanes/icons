using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Icon Catcher mini-game.
/// Player must catch falling icons by tapping them before they reach the bottom.
/// </summary>
public class IconCatcherGame : MiniGameBase
{
    [Header("Game Settings")]
    [SerializeField] private int targetCatches = 15;
    [SerializeField] private float spawnInterval = 0.6f;
    [SerializeField] private float fallSpeed = 200f;
    [SerializeField] private float speedIncrease = 10f;

    [Header("Spawn Area")]
    [SerializeField] private RectTransform spawnArea;
    [SerializeField] private float spawnPadding = 50f;
    [SerializeField] private float missedYThreshold = -500f;

    [Header("UI References")]
    [SerializeField] private Transform iconsContainer;
    [SerializeField] private GameObject fallingIconPrefab;
    [SerializeField] private TextMeshProUGUI catchCountText;
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private Image progressBar;

    [Header("Effects")]
    [SerializeField] private Color catchColor = new Color(0.2f, 0.8f, 0.2f);
    [SerializeField] private float catchScale = 1.5f;
    [SerializeField] private float catchFadeDuration = 0.3f;

    private List<FallingIcon> activeIcons = new List<FallingIcon>();
    private Coroutine spawnCoroutine;
    private int catchCount;
    private int missCount;
    private int maxMisses = 5;
    private RectTransform catcherRect;
    private RectTransform iconSpawnArea;
    private TextMeshProUGUI caughtCountText;
    private GameObject iconPrefab;

    private void Awake()
    {
        base.Awake();
        gameName = "Attrape les icônes !";
        gameDescription = $"Attrape {targetCatches} icônes avant qu'elles tombent !";
        gameDuration = 20f;
    }

    /// <summary>
    /// Initializes references for runtime UI building.
    /// </summary>
    public void InitializeReferences(TextMeshProUGUI timer, TextMeshProUGUI score, TextMeshProUGUI instruction,
        GameObject panel, RectTransform catcher, RectTransform spawn, TextMeshProUGUI caughtCount,
        GameObject iconPrefabTemplate)
    {
        timerText = timer;
        scoreText = score;
        instructionText = instruction;
        gamePanel = panel;
        catcherRect = catcher;
        iconSpawnArea = spawn;
        caughtCountText = caughtCount;
        iconPrefab = iconPrefabTemplate;
    }

    protected override void OnGameStarted()
    {
        catchCount = 0;
        missCount = 0;

        ClearIcons();
        UpdateUI();

        // Start spawning icons
        spawnCoroutine = StartCoroutine(SpawnIconsRoutine());
    }

    protected override void OnGameStopped()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        StopAllCoroutines();
        ClearIcons();
    }

    private void ClearIcons()
    {
        foreach (var icon in activeIcons)
        {
            if (icon != null && icon.gameObject != null)
            {
                Destroy(icon.gameObject);
            }
        }
        activeIcons.Clear();

        if (iconsContainer != null)
        {
            foreach (Transform child in iconsContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private IEnumerator SpawnIconsRoutine()
    {
        while (isGameActive)
        {
            SpawnIcon();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnIcon()
    {
        if (iconsContainer == null || fallingIconPrefab == null) return;

        // Create the falling icon
        GameObject iconObj = Instantiate(fallingIconPrefab, iconsContainer);
        FallingIcon icon = iconObj.GetComponent<FallingIcon>();

        if (icon == null)
        {
            icon = iconObj.AddComponent<FallingIcon>();
        }

        // Get random position at the top
        RectTransform rectTransform = iconObj.GetComponent<RectTransform>();
        if (rectTransform != null && spawnArea != null)
        {
            float minX = spawnArea.rect.xMin + spawnPadding;
            float maxX = spawnArea.rect.xMax - spawnPadding;
            float spawnX = Random.Range(minX, maxX);
            float spawnY = spawnArea.rect.yMax;

            rectTransform.anchoredPosition = new Vector2(spawnX, spawnY);
        }

        // Get random icon
        string iconId = "star";
        if (IconDatabase.Instance != null)
        {
            IconEntry entry = IconDatabase.Instance.GetRandomIcon();
            if (entry != null)
            {
                iconId = entry.id;
            }
        }

        // Calculate current fall speed (increases over time)
        float currentSpeed = fallSpeed + (speedIncrease * catchCount);

        // Setup the icon with configurable missed threshold
        icon.Setup(iconId, currentSpeed, this, missedYThreshold);
        activeIcons.Add(icon);
    }

    /// <summary>
    /// Called when an icon is caught (tapped).
    /// </summary>
    public void OnIconCaught(FallingIcon icon)
    {
        if (!isGameActive) return;

        catchCount++;
        UpdateUI();

        // Remove and animate the catch
        StartCoroutine(AnimateCatch(icon));

        // Check win condition
        if (catchCount >= targetCatches)
        {
            EndGame(true);
        }
    }

    /// <summary>
    /// Called when an icon is missed (falls off screen).
    /// </summary>
    public void OnIconMissed(FallingIcon icon)
    {
        if (!isGameActive) return;

        missCount++;

        // Remove the icon
        RemoveIcon(icon);

        // Check lose condition
        if (missCount >= maxMisses)
        {
            EndGame(false);
        }
    }

    private IEnumerator AnimateCatch(FallingIcon icon)
    {
        if (icon == null || icon.gameObject == null) yield break;

        RectTransform rectTransform = icon.GetComponent<RectTransform>();
        Image background = icon.GetComponent<Image>();
        CanvasGroup canvasGroup = icon.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = icon.gameObject.AddComponent<CanvasGroup>();
        }

        // Stop falling
        icon.StopFalling();

        // Animate scale up and fade out
        float elapsed = 0f;
        Vector3 startScale = rectTransform.localScale;
        Vector3 endScale = startScale * catchScale;

        if (background != null)
        {
            background.color = catchColor;
        }

        while (elapsed < catchFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / catchFadeDuration;

            rectTransform.localScale = Vector3.Lerp(startScale, endScale, t);
            canvasGroup.alpha = 1f - t;

            yield return null;
        }

        RemoveIcon(icon);
    }

    private void RemoveIcon(FallingIcon icon)
    {
        if (icon != null)
        {
            activeIcons.Remove(icon);
            if (icon.gameObject != null)
            {
                Destroy(icon.gameObject);
            }
        }
    }

    private void UpdateUI()
    {
        if (catchCountText != null)
        {
            catchCountText.text = catchCount.ToString();
        }

        if (targetText != null)
        {
            targetText.text = $"/ {targetCatches}";
        }

        if (progressBar != null)
        {
            progressBar.fillAmount = (float)catchCount / targetCatches;
        }
    }

    protected override void OnTimeUp()
    {
        EndGame(catchCount >= targetCatches);
    }
}

/// <summary>
/// Represents a falling icon in the catcher game.
/// </summary>
public class FallingIcon : MonoBehaviour
{
    private string iconId;
    private float fallSpeed;
    private float missedYThreshold;
    private IconCatcherGame game;
    private RectTransform rectTransform;
    private bool isFalling = true;
    private Button button;
    private TextMeshProUGUI iconText;

    public void Setup(string icon, float speed, IconCatcherGame gameRef, float yThreshold = -500f)
    {
        iconId = icon;
        fallSpeed = speed;
        game = gameRef;
        missedYThreshold = yThreshold;
        rectTransform = GetComponent<RectTransform>();

        // Setup UI components
        button = GetComponent<Button>();
        if (button == null)
        {
            button = gameObject.AddComponent<Button>();
        }
        button.onClick.AddListener(OnClick);

        iconText = GetComponentInChildren<TextMeshProUGUI>();
        if (iconText != null)
        {
            iconText.text = iconId;
        }
    }

    private void Update()
    {
        if (!isFalling || rectTransform == null) return;

        // Move down
        Vector2 position = rectTransform.anchoredPosition;
        position.y -= fallSpeed * Time.deltaTime;
        rectTransform.anchoredPosition = position;

        // Check if off screen (missed)
        if (position.y < missedYThreshold)
        {
            if (game != null)
            {
                game.OnIconMissed(this);
            }
        }
    }

    private void OnClick()
    {
        if (isFalling && game != null)
        {
            game.OnIconCaught(this);
        }
    }

    public void StopFalling()
    {
        isFalling = false;
    }
}
