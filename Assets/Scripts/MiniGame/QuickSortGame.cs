using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Quick Sort mini-game.
/// Player must sort icons by rarity into the correct bins.
/// </summary>
public class QuickSortGame : MiniGameBase
{
    [Header("Game Settings")]
    [SerializeField] private int targetSorts = 12;
    [SerializeField] private float iconSpawnInterval = 1.5f;

    [Header("UI References")]
    [SerializeField] private RectTransform currentIconContainer;
    [SerializeField] private TextMeshProUGUI currentIconText;
    [SerializeField] private TextMeshProUGUI currentRarityText;
    [SerializeField] private Image currentIconBackground;

    [Header("Rarity Bins")]
    [SerializeField] private Button commonBin;
    [SerializeField] private Button uncommonBin;
    [SerializeField] private Button rareBin;
    [SerializeField] private Button legendaryBin;

    [Header("Bin Colors")]
    [SerializeField] private Color commonColor = new Color(0.2f, 0.8f, 0.2f);
    [SerializeField] private Color uncommonColor = new Color(0.2f, 0.6f, 1f);
    [SerializeField] private Color rareColor = new Color(0.6f, 0.2f, 0.8f);
    [SerializeField] private Color legendaryColor = new Color(1f, 0.6f, 0.2f);

    [Header("Score Display")]
    [SerializeField] private TextMeshProUGUI sortCountText;
    [SerializeField] private TextMeshProUGUI targetSortsText;
    [SerializeField] private TextMeshProUGUI streakText;
    [SerializeField] private Image progressBar;

    [Header("Feedback")]
    [SerializeField] private float correctFeedbackDuration = 0.3f;
    [SerializeField] private float wrongPenaltyTime = 2f;

    private IconEntry currentIcon;
    private int correctSorts;
    private int streak;
    private int wrongSorts;
    private int maxWrongSorts = 3;
    private bool isWaiting;
    private Button leftCategoryButton;
    private Button rightCategoryButton;
    private TextMeshProUGUI leftCategoryText;
    private TextMeshProUGUI rightCategoryText;

    private void Awake()
    {
        base.Awake();
        gameName = "Trie les icônes !";
        gameDescription = $"Trie {targetSorts} icônes par rareté !";
        gameDuration = 30f;
    }

    /// <summary>
    /// Initializes references for runtime UI building.
    /// </summary>
    public void InitializeReferences(TextMeshProUGUI timer, TextMeshProUGUI score, TextMeshProUGUI instruction,
        GameObject panel, TextMeshProUGUI currentIcon, Button leftBtn, Button rightBtn,
        TextMeshProUGUI leftText, TextMeshProUGUI rightText)
    {
        timerText = timer;
        scoreText = score;
        instructionText = instruction;
        gamePanel = panel;
        currentIconText = currentIcon;
        leftCategoryButton = leftBtn;
        rightCategoryButton = rightBtn;
        leftCategoryText = leftText;
        rightCategoryText = rightText;
    }

    private void Start()
    {
        SetupBinButtons();
        SetupBinColors();
    }

    private void SetupBinButtons()
    {
        if (commonBin != null)
            commonBin.onClick.AddListener(() => OnBinSelected(IconRarity.Common));
        if (uncommonBin != null)
            uncommonBin.onClick.AddListener(() => OnBinSelected(IconRarity.Uncommon));
        if (rareBin != null)
            rareBin.onClick.AddListener(() => OnBinSelected(IconRarity.Rare));
        if (legendaryBin != null)
            legendaryBin.onClick.AddListener(() => OnBinSelected(IconRarity.Legendary));
    }

    private void SetupBinColors()
    {
        SetBinColor(commonBin, commonColor);
        SetBinColor(uncommonBin, uncommonColor);
        SetBinColor(rareBin, rareColor);
        SetBinColor(legendaryBin, legendaryColor);
    }

    private void SetBinColor(Button bin, Color color)
    {
        if (bin == null) return;

        Image binImage = bin.GetComponent<Image>();
        if (binImage != null)
        {
            binImage.color = color;
        }
    }

    protected override void OnGameStarted()
    {
        correctSorts = 0;
        wrongSorts = 0;
        streak = 0;
        isWaiting = false;

        UpdateUI();
        SpawnNextIcon();
    }

    protected override void OnGameStopped()
    {
        StopAllCoroutines();
    }

    private void SpawnNextIcon()
    {
        if (!isGameActive) return;

        if (IconDatabase.Instance != null)
        {
            currentIcon = IconDatabase.Instance.GetRandomIcon();
            DisplayCurrentIcon();
        }
    }

    private void DisplayCurrentIcon()
    {
        if (currentIcon == null) return;

        if (currentIconText != null)
        {
            currentIconText.text = currentIcon.id;
        }

        if (currentRarityText != null)
        {
            // Show a hint - the icon name, not the rarity
            currentRarityText.text = currentIcon.displayName;
        }

        if (currentIconBackground != null)
        {
            // Set background to a neutral color (player must guess rarity)
            currentIconBackground.color = Color.white;
        }

        // Animate the icon appearing
        if (currentIconContainer != null)
        {
            StartCoroutine(AnimateIconAppear());
        }
    }

    private IEnumerator AnimateIconAppear()
    {
        if (currentIconContainer == null) yield break;

        currentIconContainer.localScale = Vector3.zero;

        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            currentIconContainer.localScale = Vector3.one * Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        currentIconContainer.localScale = Vector3.one;
    }

    private void OnBinSelected(IconRarity selectedRarity)
    {
        if (!isGameActive || isWaiting || currentIcon == null) return;

        if (currentIcon.rarity == selectedRarity)
        {
            // Correct!
            OnCorrectSort();
        }
        else
        {
            // Wrong!
            OnWrongSort();
        }
    }

    private void OnCorrectSort()
    {
        correctSorts++;
        streak++;

        // Show correct feedback
        StartCoroutine(ShowFeedback(true));

        UpdateUI();

        // Check win condition
        if (correctSorts >= targetSorts)
        {
            EndGame(true);
            return;
        }

        // Spawn next icon
        isWaiting = true;
        Invoke(nameof(SpawnNextIconDelayed), correctFeedbackDuration);
    }

    private void OnWrongSort()
    {
        wrongSorts++;
        streak = 0;

        // Time penalty - ensure time doesn't go negative
        currentTime = Mathf.Max(0f, currentTime - wrongPenaltyTime);

        // Show wrong feedback
        StartCoroutine(ShowFeedback(false));

        UpdateUI();

        // Check lose condition
        if (wrongSorts >= maxWrongSorts)
        {
            EndGame(false);
            return;
        }

        // Spawn next icon
        isWaiting = true;
        Invoke(nameof(SpawnNextIconDelayed), correctFeedbackDuration);
    }

    private void SpawnNextIconDelayed()
    {
        isWaiting = false;
        SpawnNextIcon();
    }

    private IEnumerator ShowFeedback(bool correct)
    {
        if (currentIconBackground == null) yield break;

        Color feedbackColor = correct ? Color.green : Color.red;
        Color originalColor = Color.white;

        currentIconBackground.color = feedbackColor;

        yield return new WaitForSeconds(correctFeedbackDuration);

        if (currentIconBackground != null)
        {
            currentIconBackground.color = originalColor;
        }
    }

    private void UpdateUI()
    {
        if (sortCountText != null)
        {
            sortCountText.text = correctSorts.ToString();
        }

        if (targetSortsText != null)
        {
            targetSortsText.text = $"/ {targetSorts}";
        }

        if (streakText != null)
        {
            streakText.text = streak > 1 ? $"Série: {streak}!" : "";
        }

        if (progressBar != null)
        {
            progressBar.fillAmount = (float)correctSorts / targetSorts;
        }
    }

    protected override void OnTimeUp()
    {
        EndGame(correctSorts >= targetSorts);
    }

    /// <summary>
    /// Gets the French name for a rarity level.
    /// </summary>
    private string GetRarityName(IconRarity rarity)
    {
        return rarity switch
        {
            IconRarity.Common => "Commun",
            IconRarity.Uncommon => "Peu commun",
            IconRarity.Rare => "Rare",
            IconRarity.Legendary => "Légendaire",
            _ => "Inconnu"
        };
    }
}
