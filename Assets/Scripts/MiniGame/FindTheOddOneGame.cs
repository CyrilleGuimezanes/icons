using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Find the Odd One mini-game.
/// Player must find the one different icon among several identical ones.
/// </summary>
public class FindTheOddOneGame : MiniGameBase
{
    [Header("Game Settings")]
    [SerializeField] private int roundsToWin = 5;
    [SerializeField] private int gridSize = 9;
    [SerializeField] private float roundTimeLimit = 5f;

    [Header("Grid Settings")]
    [SerializeField] private Transform gridContainer;
    [SerializeField] private GameObject iconButtonPrefab;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI totalRoundsText;
    [SerializeField] private Image progressBar;

    [Header("Visual Settings")]
    [SerializeField] private Color correctColor = new Color(0.2f, 0.8f, 0.2f);
    [SerializeField] private Color wrongColor = new Color(0.9f, 0.3f, 0.3f);
    [SerializeField] private float feedbackDuration = 0.5f;

    private List<OddOneButton> buttons = new List<OddOneButton>();
    private int currentRound;
    private int oddIndex;
    private bool isProcessing;
    private float roundTimer;
    private Button[] optionButtons;
    private TextMeshProUGUI[] optionIcons;

    private void Awake()
    {
        base.Awake();
        gameName = "Trouve l'intrus !";
        gameDescription = $"Trouve l'icône différente dans chaque grille !";
        gameDuration = 45f;
    }

    /// <summary>
    /// Initializes references for runtime UI building.
    /// </summary>
    public void InitializeReferences(TextMeshProUGUI timer, TextMeshProUGUI score, TextMeshProUGUI instruction,
        GameObject panel, Button[] buttons, TextMeshProUGUI[] icons)
    {
        timerText = timer;
        scoreText = score;
        instructionText = instruction;
        gamePanel = panel;
        optionButtons = buttons;
        optionIcons = icons;
    }

    protected override void Update()
    {
        base.Update();

        // Track round timer
        if (isGameActive && !isProcessing)
        {
            roundTimer -= Time.deltaTime;
            if (roundTimer <= 0)
            {
                OnRoundTimeout();
            }
        }
    }

    protected override void OnGameStarted()
    {
        currentRound = 0;
        isProcessing = false;

        ClearGrid();
        UpdateUI();
        StartNextRound();
    }

    protected override void OnGameStopped()
    {
        StopAllCoroutines();
        ClearGrid();
    }

    private void ClearGrid()
    {
        foreach (var button in buttons)
        {
            if (button != null && button.gameObject != null)
            {
                Destroy(button.gameObject);
            }
        }
        buttons.Clear();

        if (gridContainer != null)
        {
            foreach (Transform child in gridContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void StartNextRound()
    {
        if (!isGameActive) return;

        ClearGrid();
        CreateRoundGrid();
        roundTimer = roundTimeLimit;
    }

    private void CreateRoundGrid()
    {
        if (gridContainer == null || iconButtonPrefab == null) return;

        // Get two different icons
        IconEntry mainIcon = null;
        IconEntry oddIcon = null;

        if (IconDatabase.Instance != null)
        {
            List<IconEntry> icons = IconDatabase.Instance.GetRandomIcons(2, false);
            if (icons.Count >= 2)
            {
                mainIcon = icons[0];
                oddIcon = icons[1];
            }
        }

        if (mainIcon == null || oddIcon == null)
        {
            mainIcon = new IconEntry("star", "Étoile", IconRarity.Common);
            oddIcon = new IconEntry("favorite", "Coeur", IconRarity.Common);
        }

        // Randomly choose which position will be odd
        oddIndex = Random.Range(0, gridSize);

        // Create buttons
        for (int i = 0; i < gridSize; i++)
        {
            GameObject buttonObj = Instantiate(iconButtonPrefab, gridContainer);
            OddOneButton button = buttonObj.GetComponent<OddOneButton>();

            if (button == null)
            {
                button = buttonObj.AddComponent<OddOneButton>();
            }

            bool isOdd = (i == oddIndex);
            IconEntry iconToUse = isOdd ? oddIcon : mainIcon;

            button.Setup(iconToUse.id, isOdd, i, this);
            buttons.Add(button);
        }
    }

    /// <summary>
    /// Called when a button is clicked.
    /// </summary>
    public void OnButtonClicked(OddOneButton button)
    {
        if (!isGameActive || isProcessing) return;

        isProcessing = true;

        if (button.IsOdd)
        {
            // Correct!
            StartCoroutine(OnCorrectSelection(button));
        }
        else
        {
            // Wrong!
            StartCoroutine(OnWrongSelection(button));
        }
    }

    private IEnumerator OnCorrectSelection(OddOneButton button)
    {
        // Show correct feedback
        button.ShowFeedback(correctColor);
        yield return new WaitForSeconds(feedbackDuration);

        currentRound++;
        UpdateUI();

        // Check win condition
        if (currentRound >= roundsToWin)
        {
            EndGame(true);
            yield break;
        }

        isProcessing = false;
        StartNextRound();
    }

    private IEnumerator OnWrongSelection(OddOneButton button)
    {
        // Show wrong feedback
        button.ShowFeedback(wrongColor);

        // Highlight the correct answer
        foreach (var btn in buttons)
        {
            if (btn.IsOdd)
            {
                btn.ShowFeedback(correctColor);
            }
        }

        yield return new WaitForSeconds(feedbackDuration * 2);

        // Lose the game
        EndGame(false);
    }

    private void OnRoundTimeout()
    {
        if (isProcessing) return;

        isProcessing = true;

        // Highlight the correct answer
        foreach (var button in buttons)
        {
            if (button.IsOdd)
            {
                button.ShowFeedback(correctColor);
            }
        }

        // Lose the game after showing the answer
        Invoke(nameof(LoseGame), feedbackDuration);
    }

    private void LoseGame()
    {
        EndGame(false);
    }

    private void UpdateUI()
    {
        if (roundText != null)
        {
            roundText.text = currentRound.ToString();
        }

        if (totalRoundsText != null)
        {
            totalRoundsText.text = $"/ {roundsToWin}";
        }

        if (progressBar != null)
        {
            progressBar.fillAmount = (float)currentRound / roundsToWin;
        }
    }

    protected override void OnTimeUp()
    {
        EndGame(currentRound >= roundsToWin);
    }
}

/// <summary>
/// Represents a button in the Find the Odd One game.
/// </summary>
public class OddOneButton : MonoBehaviour
{
    private string iconId;
    private bool isOdd;
    private int index;
    private FindTheOddOneGame game;
    private Button button;
    private TextMeshProUGUI iconText;
    private Image background;

    public bool IsOdd => isOdd;
    public int Index => index;

    public void Setup(string icon, bool odd, int idx, FindTheOddOneGame gameRef)
    {
        iconId = icon;
        isOdd = odd;
        index = idx;
        game = gameRef;

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

        background = GetComponent<Image>();
        if (background != null)
        {
            background.color = Color.white;
        }
    }

    private void OnClick()
    {
        if (game != null)
        {
            game.OnButtonClicked(this);
        }
    }

    public void ShowFeedback(Color color)
    {
        if (background != null)
        {
            background.color = color;
        }
    }
}
