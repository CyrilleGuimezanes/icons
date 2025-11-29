using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Memory Match mini-game.
/// Player must find matching pairs of icons within the time limit.
/// </summary>
public class MemoryMatchGame : MiniGameBase
{
    [Header("Game Settings")]
    [SerializeField] private int pairsCount = 6;

    [Header("Grid Settings")]
    [SerializeField] private Transform gridContainer;
    [SerializeField] private GameObject cardPrefab;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI pairsFoundText;
    [SerializeField] private TextMeshProUGUI totalPairsText;
    [SerializeField] private Image progressBar;

    [Header("Card Settings")]
    [SerializeField] private Color cardBackColor = new Color(0.3f, 0.3f, 0.3f);
    [SerializeField] private Color cardFrontColor = Color.white;
    [SerializeField] private string cardBackIcon = "question_mark";
    [SerializeField] private float flipDuration = 0.2f;
    [SerializeField] private float matchDelay = 0.5f;
    [SerializeField] private float mismatchDelay = 1f;

    private List<MemoryCard> cards = new List<MemoryCard>();
    private MemoryCard firstFlipped;
    private MemoryCard secondFlipped;
    private int pairsFound;
    private bool isProcessing;

    private void Awake()
    {
        base.Awake();
        gameName = "Memory Match !";
        gameDescription = $"Trouve les {pairsCount} paires d'icônes !";
        gameDuration = 30f;
    }

    protected override void OnGameStarted()
    {
        pairsFound = 0;
        isProcessing = false;
        firstFlipped = null;
        secondFlipped = null;

        CreateCards();
        UpdateUI();
    }

    protected override void OnGameStopped()
    {
        ClearCards();
    }

    private void ClearCards()
    {
        foreach (var card in cards)
        {
            if (card != null && card.gameObject != null)
            {
                Destroy(card.gameObject);
            }
        }
        cards.Clear();

        if (gridContainer != null)
        {
            foreach (Transform child in gridContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void CreateCards()
    {
        ClearCards();

        if (gridContainer == null || cardPrefab == null) return;

        // Get icons for pairs
        List<IconEntry> icons;
        if (IconDatabase.Instance != null)
        {
            icons = IconDatabase.Instance.GetRandomIcons(pairsCount, false);
        }
        else
        {
            icons = new List<IconEntry>();
            for (int i = 0; i < pairsCount; i++)
            {
                icons.Add(new IconEntry($"icon_{i}", $"Icon {i}", IconRarity.Common));
            }
        }

        // Create pairs
        List<string> cardIcons = new List<string>();
        foreach (var icon in icons)
        {
            cardIcons.Add(icon.id);
            cardIcons.Add(icon.id); // Add twice for pair
        }

        // Shuffle cards
        ShuffleList(cardIcons);

        // Create card objects
        for (int i = 0; i < cardIcons.Count; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, gridContainer);
            MemoryCard card = cardObj.GetComponent<MemoryCard>();

            if (card == null)
            {
                card = cardObj.AddComponent<MemoryCard>();
            }

            card.Setup(cardIcons[i], cardBackIcon, cardFrontColor, cardBackColor, this);
            cards.Add(card);
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    /// <summary>
    /// Called when a card is flipped.
    /// </summary>
    public void OnCardFlipped(MemoryCard card)
    {
        if (!isGameActive || isProcessing) return;
        if (card.IsMatched || card.IsFlipped) return;

        // Flip the card
        card.Flip(true);

        if (firstFlipped == null)
        {
            // First card of the pair
            firstFlipped = card;
        }
        else if (secondFlipped == null && card != firstFlipped)
        {
            // Second card of the pair
            secondFlipped = card;

            // Check for match
            isProcessing = true;
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(matchDelay);

        if (firstFlipped.IconId == secondFlipped.IconId)
        {
            // Match found!
            firstFlipped.SetMatched(true);
            secondFlipped.SetMatched(true);
            pairsFound++;
            UpdateUI();

            // Check win condition
            if (pairsFound >= pairsCount)
            {
                EndGame(true);
                yield break;
            }
        }
        else
        {
            // No match - flip back after delay
            yield return new WaitForSeconds(mismatchDelay - matchDelay);

            firstFlipped.Flip(false);
            secondFlipped.Flip(false);
        }

        firstFlipped = null;
        secondFlipped = null;
        isProcessing = false;
    }

    private void UpdateUI()
    {
        if (pairsFoundText != null)
        {
            pairsFoundText.text = pairsFound.ToString();
        }

        if (totalPairsText != null)
        {
            totalPairsText.text = $"/ {pairsCount}";
        }

        if (progressBar != null)
        {
            progressBar.fillAmount = (float)pairsFound / pairsCount;
        }
    }

    protected override void OnTimeUp()
    {
        // Only succeed if all pairs found
        EndGame(pairsFound >= pairsCount);
    }
}

/// <summary>
/// Represents a card in the memory match game.
/// </summary>
public class MemoryCard : MonoBehaviour
{
    private string iconId;
    private string backIcon;
    private Color frontColor;
    private Color backColor;
    private MemoryMatchGame game;

    private bool isFlipped;
    private bool isMatched;
    private Button button;
    private TextMeshProUGUI iconText;
    private Image background;

    public string IconId => iconId;
    public bool IsFlipped => isFlipped;
    public bool IsMatched => isMatched;

    public void Setup(string icon, string back, Color front, Color backCol, MemoryMatchGame gameRef)
    {
        iconId = icon;
        backIcon = back;
        frontColor = front;
        backColor = backCol;
        game = gameRef;
        isFlipped = false;
        isMatched = false;

        // Setup UI components
        button = GetComponent<Button>();
        if (button == null)
        {
            button = gameObject.AddComponent<Button>();
        }
        button.onClick.AddListener(OnClick);

        iconText = GetComponentInChildren<TextMeshProUGUI>();
        background = GetComponent<Image>();

        // Start face down
        ShowBack();
    }

    private void OnClick()
    {
        if (!isMatched && !isFlipped && game != null)
        {
            game.OnCardFlipped(this);
        }
    }

    public void Flip(bool showFront)
    {
        isFlipped = showFront;

        if (showFront)
        {
            ShowFront();
        }
        else
        {
            ShowBack();
        }
    }

    private void ShowFront()
    {
        if (iconText != null)
        {
            iconText.text = iconId;
        }
        if (background != null)
        {
            background.color = frontColor;
        }
    }

    private void ShowBack()
    {
        if (iconText != null)
        {
            iconText.text = backIcon;
        }
        if (background != null)
        {
            background.color = backColor;
        }
    }

    public void SetMatched(bool matched)
    {
        isMatched = matched;
        if (matched)
        {
            // Visual feedback for matched cards
            if (background != null)
            {
                Color matchedColor = frontColor;
                matchedColor.a = 0.7f;
                background.color = matchedColor;
            }

            if (button != null)
            {
                button.interactable = false;
            }
        }
    }
}
