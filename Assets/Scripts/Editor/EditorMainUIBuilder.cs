#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Editor script that generates the complete UI and saves the scene.
/// This script runs in the Unity Editor and creates all UI elements when triggered.
/// </summary>
public class EditorMainUIBuilder : EditorUIBuilderBase
{
    private Canvas mainCanvas;
    
    // Generated UI elements
    private GameObject screensContainer;
    private GameObject melangeurScreen;
    private GameObject miniJeuScreen;
    private GameObject potagerScreen;
    private GameObject boutiqueScreen;
    private GameObject collectionScreen;
    private GameObject optionsScreen;
    private GameObject bottomNavigation;

    [MenuItem("Icons/Build UI/Build Complete UI")]
    public static void BuildCompleteUI()
    {
        EditorMainUIBuilder builder = new EditorMainUIBuilder();
        builder.BuildUI();
    }

    [MenuItem("Icons/Build UI/Build and Save Scene")]
    public static void BuildAndSaveScene()
    {
        EditorMainUIBuilder builder = new EditorMainUIBuilder();
        builder.BuildUI();
        
        // Mark scene as dirty and save
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        
        Debug.Log("UI built and scene saved successfully!");
    }

    public override void BuildScreen(Transform parent)
    {
        BuildUI();
    }

    /// <summary>
    /// Builds the complete UI hierarchy.
    /// </summary>
    public void BuildUI()
    {
        FindCanvas();
        
        if (mainCanvas == null)
        {
            mainCanvas = Object.FindAnyObjectByType<Canvas>();
        }

        if (mainCanvas == null)
        {
            Debug.LogError("EditorMainUIBuilder: No Canvas found in scene!");
            return;
        }

        // Clear existing generated UI
        ClearExistingUI();

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

        Debug.Log("EditorMainUIBuilder: UI built successfully!");
    }

    private void ClearExistingUI()
    {
        // Find and destroy existing ScreensContainer if it exists
        Transform existingContainer = mainCanvas.transform.Find("ScreensContainer");
        if (existingContainer != null)
        {
            Undo.DestroyObjectImmediate(existingContainer.gameObject);
        }

        // Find and destroy existing BottomNavigation if it exists
        Transform existingNav = mainCanvas.transform.Find("BottomNavigation");
        if (existingNav != null)
        {
            Undo.DestroyObjectImmediate(existingNav.gameObject);
        }
    }

    private void CreateScreensContainer()
    {
        // Create screens container
        screensContainer = new GameObject("ScreensContainer", typeof(RectTransform), typeof(CanvasRenderer));
        screensContainer.transform.SetParent(mainCanvas.transform, false);
        Undo.RegisterCreatedObjectUndo(screensContainer, "Create ScreensContainer");

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
        EditorMelangeurScreenBuilder melangeurBuilder = new EditorMelangeurScreenBuilder();
        melangeurBuilder.BuildScreen(screensContainer.transform);
        melangeurScreen = melangeurBuilder.GetScreenRoot();

        // Create and build Mini-Jeu screen
        EditorMiniJeuScreenBuilder miniJeuBuilder = new EditorMiniJeuScreenBuilder();
        miniJeuBuilder.BuildScreen(screensContainer.transform);
        miniJeuScreen = miniJeuBuilder.GetScreenRoot();

        // Create and build Potager screen
        EditorPotagerScreenBuilder potagerBuilder = new EditorPotagerScreenBuilder();
        potagerBuilder.BuildScreen(screensContainer.transform);
        potagerScreen = potagerBuilder.GetScreenRoot();

        // Create and build Boutique screen
        EditorBoutiqueScreenBuilder boutiqueBuilder = new EditorBoutiqueScreenBuilder();
        boutiqueBuilder.BuildScreen(screensContainer.transform);
        boutiqueScreen = boutiqueBuilder.GetScreenRoot();

        // Create and build Collection screen
        EditorCollectionScreenBuilder collectionBuilder = new EditorCollectionScreenBuilder();
        collectionBuilder.BuildScreen(screensContainer.transform);
        collectionScreen = collectionBuilder.GetScreenRoot();

        // Create and build Options screen
        EditorOptionsScreenBuilder optionsBuilder = new EditorOptionsScreenBuilder();
        optionsBuilder.BuildScreen(screensContainer.transform);
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
        EditorBottomNavigationBuilder navigationBuilder = new EditorBottomNavigationBuilder();
        navigationBuilder.BuildScreen(mainCanvas.transform);
        bottomNavigation = navigationBuilder.GetScreenRoot();
    }

    private void SetupMenuManager()
    {
        // Check if MenuManager already exists on ScreensContainer
        MenuManager menuManager = screensContainer.GetComponent<MenuManager>();
        if (menuManager == null)
        {
            menuManager = screensContainer.AddComponent<MenuManager>();
        }

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
        MenuManager menuManager = screensContainer.GetComponent<MenuManager>();
        
        if (nav != null && menuManager != null)
        {
            nav.InitializeWithMenuManager(menuManager);
        }
    }
}
#endif
