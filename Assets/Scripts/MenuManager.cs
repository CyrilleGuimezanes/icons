using UnityEngine;

/// <summary>
/// Manages the main menu navigation and screen switching.
/// Controls which screen is currently visible based on user interaction.
/// </summary>
public class MenuManager : MonoBehaviour
{
    [Header("Welcome Screen")]
    [SerializeField] private GameObject welcomeScreen;
    [SerializeField] private GameObject bottomNavigation;
    [SerializeField] private GameObject screensContainer;

    [Header("Screens")]
    [SerializeField] private GameObject melangeurScreen;
    [SerializeField] private GameObject miniJeuScreen;
    [SerializeField] private GameObject potagerScreen;
    [SerializeField] private GameObject boutiqueScreen;
    [SerializeField] private GameObject collectionScreen;
    [SerializeField] private GameObject optionsScreen;

    private GameObject currentScreen;
    private GameObject[] allScreens;
    private bool isShowingWelcomeScreen;

    private void Awake()
    {
        allScreens = new GameObject[]
        {
            melangeurScreen,
            miniJeuScreen,
            potagerScreen,
            boutiqueScreen,
            collectionScreen,
            optionsScreen
        };
    }

    private void Start()
    {
        // Check if we need to show the welcome screen
        if (GameSlotsManager.Instance == null || !GameSlotsManager.Instance.HasActiveSlot)
        {
            ShowWelcomeScreen();
        }
        else
        {
            HideWelcomeScreen();
        }
    }

    /// <summary>
    /// Shows the welcome screen for slot selection.
    /// </summary>
    public void ShowWelcomeScreen()
    {
        isShowingWelcomeScreen = true;

        // Hide game screens
        foreach (var s in allScreens)
        {
            if (s != null)
            {
                s.SetActive(false);
            }
        }

        // Hide bottom navigation
        if (bottomNavigation != null)
        {
            bottomNavigation.SetActive(false);
        }

        // Hide screens container (optional, for cleaner UI)
        if (screensContainer != null)
        {
            screensContainer.SetActive(false);
        }

        // Show welcome screen
        if (welcomeScreen != null)
        {
            welcomeScreen.SetActive(true);
        }
    }

    /// <summary>
    /// Hides the welcome screen and shows the game.
    /// </summary>
    public void HideWelcomeScreen()
    {
        isShowingWelcomeScreen = false;

        // Hide welcome screen
        if (welcomeScreen != null)
        {
            welcomeScreen.SetActive(false);
        }

        // Show screens container
        if (screensContainer != null)
        {
            screensContainer.SetActive(true);
        }

        // Show bottom navigation
        if (bottomNavigation != null)
        {
            bottomNavigation.SetActive(true);
        }

        // Show the default screen
        ShowScreen(MenuScreen.Melangeur);
    }

    /// <summary>
    /// Gets whether the welcome screen is currently showing.
    /// </summary>
    public bool IsShowingWelcomeScreen => isShowingWelcomeScreen;

    /// <summary>
    /// Sets the welcome screen reference (for runtime creation).
    /// </summary>
    /// <param name="screen">The welcome screen GameObject.</param>
    public void SetWelcomeScreen(GameObject screen)
    {
        welcomeScreen = screen;
    }

    /// <summary>
    /// Initializes all screen references (for runtime UI building).
    /// </summary>
    public void InitializeScreens(GameObject welcome, GameObject bottomNav, GameObject screens,
        GameObject melangeur, GameObject miniJeu, GameObject potager,
        GameObject boutique, GameObject collection, GameObject options)
    {
        welcomeScreen = welcome;
        bottomNavigation = bottomNav;
        screensContainer = screens;
        melangeurScreen = melangeur;
        miniJeuScreen = miniJeu;
        potagerScreen = potager;
        boutiqueScreen = boutique;
        collectionScreen = collection;
        optionsScreen = options;

        allScreens = new GameObject[]
        {
            melangeurScreen,
            miniJeuScreen,
            potagerScreen,
            boutiqueScreen,
            collectionScreen,
            optionsScreen
        };
    }

    /// <summary>
    /// Shows the specified screen and hides all others.
    /// </summary>
    /// <param name="screen">The screen to display</param>
    public void ShowScreen(MenuScreen screen)
    {
        if (isShowingWelcomeScreen) return;

        // Hide all screens first
        foreach (var s in allScreens)
        {
            if (s != null)
            {
                s.SetActive(false);
            }
        }

        // Show the selected screen
        currentScreen = GetScreenObject(screen);
        if (currentScreen != null)
        {
            currentScreen.SetActive(true);
        }
    }

    /// <summary>
    /// Shows the screen by index (used by UI buttons).
    /// </summary>
    /// <param name="index">Screen index (0-5)</param>
    public void ShowScreenByIndex(int index)
    {
        if (index >= 0 && index < allScreens.Length)
        {
            ShowScreen((MenuScreen)index);
        }
    }

    private GameObject GetScreenObject(MenuScreen screen)
    {
        return screen switch
        {
            MenuScreen.Melangeur => melangeurScreen,
            MenuScreen.MiniJeu => miniJeuScreen,
            MenuScreen.Potager => potagerScreen,
            MenuScreen.Boutique => boutiqueScreen,
            MenuScreen.Collection => collectionScreen,
            MenuScreen.Options => optionsScreen,
            _ => melangeurScreen
        };
    }
}

/// <summary>
/// Enumeration of all menu screens in the game.
/// </summary>
public enum MenuScreen
{
    Melangeur = 0,
    MiniJeu = 1,
    Potager = 2,
    Boutique = 3,
    Collection = 4,
    Options = 5
}
