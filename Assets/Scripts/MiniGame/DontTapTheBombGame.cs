using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// "Don't Tap the Bomb" mini-game.
/// Player must tap icons but avoid tapping bombs.
/// Tap 15 icons successfully to win.
/// </summary>
public class DontTapTheBombGame : MiniGameBase
{
    [Header("Game Settings")]
    [SerializeField] private int targetScore = 15;
    [SerializeField] private float spawnInterval = 0.8f;
    [SerializeField] private float itemLifetime = 2f;
    [SerializeField] private float bombChance = 0.25f;

    [Header("Grid Settings")]
    [SerializeField] private Transform gridContainer;
    [SerializeField] private GameObject iconButtonPrefab;
    [SerializeField] private int gridColumns = 3;
    [SerializeField] private int gridRows = 3;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private Image progressBar;

    [Header("Bomb Settings")]
    [SerializeField] private string bombIcon = "dangerous";
    [SerializeField] private Color bombColor = Color.red;
    [SerializeField] private Color safeColor = Color.white;

    private List<GridItem> activeItems = new List<GridItem>();
    private List<int> availablePositions;
    private Coroutine spawnCoroutine;
    private bool gameOver;

    private void Awake()
    {
        base.Awake();
        gameName = "Évite les bombes !";
        gameDescription = $"Tape sur les icônes, évite les bombes ! ({targetScore} à trouver)";
        gameDuration = 15f;
    }

    protected override void OnGameStarted()
    {
        gameOver = false;
        currentScore = 0;

        // Initialize available positions
        availablePositions = new List<int>();
        for (int i = 0; i < gridColumns * gridRows; i++)
        {
            availablePositions.Add(i);
        }

        ClearGrid();
        UpdateGameUI();

        // Start spawning items
        spawnCoroutine = StartCoroutine(SpawnItemsRoutine());
    }

    protected override void OnGameStopped()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        StopAllCoroutines();
        ClearGrid();
    }

    private void ClearGrid()
    {
        foreach (var item in activeItems)
        {
            if (item != null && item.gameObject != null)
            {
                Destroy(item.gameObject);
            }
        }
        activeItems.Clear();

        // Clear any remaining children
        if (gridContainer != null)
        {
            foreach (Transform child in gridContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private IEnumerator SpawnItemsRoutine()
    {
        while (isGameActive && !gameOver)
        {
            SpawnItem();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnItem()
    {
        if (gridContainer == null || iconButtonPrefab == null) return;
        if (activeItems.Count >= gridColumns * gridRows) return;

        // Find an available position
        if (availablePositions.Count == 0) return;

        int posIndex = Random.Range(0, availablePositions.Count);
        int position = availablePositions[posIndex];
        availablePositions.RemoveAt(posIndex);

        // Create the item
        GameObject itemObj = Instantiate(iconButtonPrefab, gridContainer);
        GridItem item = itemObj.GetComponent<GridItem>();

        if (item == null)
        {
            item = itemObj.AddComponent<GridItem>();
        }

        // Decide if this is a bomb
        bool isBomb = Random.value < bombChance;

        // Get icon to display
        string iconId;
        Color itemColor;

        if (isBomb)
        {
            iconId = bombIcon;
            itemColor = bombColor;
        }
        else if (IconDatabase.Instance != null)
        {
            IconEntry randomIcon = IconDatabase.Instance.GetRandomIcon();
            iconId = randomIcon != null ? randomIcon.id : "star";
            itemColor = safeColor;
        }
        else
        {
            iconId = "star";
            itemColor = safeColor;
        }

        // Setup the item
        item.Setup(iconId, isBomb, position, itemColor, this);
        activeItems.Add(item);

        // Start lifetime countdown
        StartCoroutine(ItemLifetimeRoutine(item, position));
    }

    private IEnumerator ItemLifetimeRoutine(GridItem item, int position)
    {
        yield return new WaitForSeconds(itemLifetime);

        if (item != null && activeItems.Contains(item))
        {
            RemoveItem(item, position);
        }
    }

    /// <summary>
    /// Called when an item is tapped.
    /// </summary>
    public void OnItemTapped(GridItem item)
    {
        if (!isGameActive || gameOver) return;

        if (item.IsBomb)
        {
            // Game over - tapped a bomb!
            gameOver = true;
            EndGame(false);
        }
        else
        {
            // Good tap!
            currentScore++;
            UpdateGameUI();

            // Remove the item
            RemoveItem(item, item.Position);

            // Check win condition
            if (currentScore >= targetScore)
            {
                EndGame(true);
            }
        }
    }

    private void RemoveItem(GridItem item, int position)
    {
        if (item != null)
        {
            activeItems.Remove(item);
            availablePositions.Add(position);

            if (item.gameObject != null)
            {
                Destroy(item.gameObject);
            }
        }
    }

    private void UpdateGameUI()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }

        if (targetText != null)
        {
            targetText.text = $"/ {targetScore}";
        }

        if (progressBar != null)
        {
            progressBar.fillAmount = (float)currentScore / targetScore;
        }
    }

    protected override void OnTimeUp()
    {
        // Only succeed if we reached the target
        EndGame(currentScore >= targetScore);
    }
}

/// <summary>
/// Represents a tappable item in the grid.
/// </summary>
public class GridItem : MonoBehaviour
{
    private bool isBomb;
    private int position;
    private DontTapTheBombGame game;
    private Button button;
    private TextMeshProUGUI iconText;
    private Image background;

    public bool IsBomb => isBomb;
    public int Position => position;

    public void Setup(string iconId, bool bomb, int pos, Color color, DontTapTheBombGame gameRef)
    {
        isBomb = bomb;
        position = pos;
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
            background.color = color;
        }
    }

    private void OnClick()
    {
        if (game != null)
        {
            game.OnItemTapped(this);
        }
    }
}
