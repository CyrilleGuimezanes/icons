using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Main orchestrator for building the entire UI at runtime.
/// Creates and manages all screens and navigation.
/// </summary>
public class MainUIBuilder : MonoBehaviour
{
    [Header("Canvas Reference")]
    [SerializeField] private Canvas mainCanvas;

    [Header("Screen Builders")]
    private MelangeurScreenBuilder melangeurBuilder;
    private MiniJeuScreenBuilder miniJeuBuilder;
    private PotagerScreenBuilder potagerBuilder;
    private BoutiqueScreenBuilder boutiqueBuilder;
    private CollectionScreenBuilder collectionBuilder;
    private OptionsScreenBuilder optionsBuilder;
    private BottomNavigationBuilder navigationBuilder;

    [Header("Generated Screens")]
    private GameObject screensContainer;
    private GameObject melangeurScreen;
    private GameObject miniJeuScreen;
    private GameObject potagerScreen;
    private GameObject boutiqueScreen;
    private GameObject collectionScreen;
    private GameObject optionsScreen;
    private GameObject bottomNavigation;

    private MenuManager menuManager;

    private void Awake()
    {
        if (mainCanvas == null)
        {
            mainCanvas = FindAnyObjectByType<Canvas>();
        }

        if (mainCanvas == null)
        {
            Debug.LogError("MainUIBuilder: No Canvas found in scene!");
            return;
        }

        BuildUI();
    }

    /// <summary>
    /// Builds the complete UI hierarchy.
    /// </summary>
    public void BuildUI()
    {
        // Create the screens container
        CreateScreensContainer();

        // Create all screens using their builders
        CreateAllScreens();

        // Create bottom navigation
        CreateBottomNavigation();

        // Setup MenuManager
        SetupMenuManager();

        // Connect everything
        ConnectNavigation();

        Debug.Log("MainUIBuilder: UI built successfully!");
    }

    private void CreateScreensContainer()
    {
        // Create screens container
        screensContainer = new GameObject("ScreensContainer", typeof(RectTransform), typeof(CanvasRenderer));
        screensContainer.transform.SetParent(mainCanvas.transform, false);

        RectTransform containerRect = screensContainer.GetComponent<RectTransform>();
        containerRect.anchorMin = Vector2.zero;
        containerRect.anchorMax = Vector2.one;
        containerRect.offsetMin = new Vector2(0, 80); // Leave room for bottom navigation
        containerRect.offsetMax = Vector2.zero;

        // Add Image for potential background
        Image bg = screensContainer.AddComponent<Image>();
        bg.color = new Color(0.96f, 0.96f, 0.96f, 1f);
    }

    private void CreateAllScreens()
    {
        // Create and build Mélangeur screen
        melangeurBuilder = screensContainer.AddComponent<MelangeurScreenBuilder>();
        melangeurBuilder.BuildScreen();
        melangeurScreen = melangeurBuilder.GetScreenRoot();

        // Create and build Mini-Jeu screen
        miniJeuBuilder = screensContainer.AddComponent<MiniJeuScreenBuilder>();
        miniJeuBuilder.BuildScreen();
        miniJeuScreen = miniJeuBuilder.GetScreenRoot();

        // Create and build Potager screen
        potagerBuilder = screensContainer.AddComponent<PotagerScreenBuilder>();
        potagerBuilder.BuildScreen();
        potagerScreen = potagerBuilder.GetScreenRoot();

        // Create and build Boutique screen
        boutiqueBuilder = screensContainer.AddComponent<BoutiqueScreenBuilder>();
        boutiqueBuilder.BuildScreen();
        boutiqueScreen = boutiqueBuilder.GetScreenRoot();

        // Create and build Collection screen
        collectionBuilder = screensContainer.AddComponent<CollectionScreenBuilder>();
        collectionBuilder.BuildScreen();
        collectionScreen = collectionBuilder.GetScreenRoot();

        // Create and build Options screen
        optionsBuilder = screensContainer.AddComponent<OptionsScreenBuilder>();
        optionsBuilder.BuildScreen();
        optionsScreen = optionsBuilder.GetScreenRoot();

        // Hide all screens initially except Mélangeur
        melangeurScreen.SetActive(true);
        miniJeuScreen.SetActive(false);
        potagerScreen.SetActive(false);
        boutiqueScreen.SetActive(false);
        collectionScreen.SetActive(false);
        optionsScreen.SetActive(false);
    }

    private void CreateBottomNavigation()
    {
        navigationBuilder = mainCanvas.gameObject.AddComponent<BottomNavigationBuilder>();
        navigationBuilder.BuildScreen();
        bottomNavigation = navigationBuilder.GetScreenRoot();
    }

    private void SetupMenuManager()
    {
        // Create MenuManager
        menuManager = screensContainer.AddComponent<MenuManager>();

        // Initialize the MenuManager with screen references using public method
        menuManager.InitializeScreens(
            null, // welcomeScreen - will be created by WelcomeScreenBuilder if needed
            bottomNavigation,
            screensContainer,
            melangeurScreen,
            miniJeuScreen,
            potagerScreen,
            boutiqueScreen,
            collectionScreen,
            optionsScreen
        );
    }

    private void ConnectNavigation()
    {
        // Get the BottomNavigation component from the navigation builder
        BottomNavigation nav = bottomNavigation.GetComponent<BottomNavigation>();
        if (nav != null)
        {
            nav.InitializeWithMenuManager(menuManager);
        }
    }

    /// <summary>
    /// Gets the MenuManager instance.
    /// </summary>
    public MenuManager GetMenuManager() => menuManager;

    /// <summary>
    /// Gets a specific screen by type.
    /// </summary>
    public GameObject GetScreen(MenuScreen screenType)
    {
        return screenType switch
        {
            MenuScreen.Melangeur => melangeurScreen,
            MenuScreen.MiniJeu => miniJeuScreen,
            MenuScreen.Potager => potagerScreen,
            MenuScreen.Boutique => boutiqueScreen,
            MenuScreen.Collection => collectionScreen,
            MenuScreen.Options => optionsScreen,
            _ => null
        };
    }
}
