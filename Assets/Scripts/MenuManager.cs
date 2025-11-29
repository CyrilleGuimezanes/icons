using UnityEngine;

/// <summary>
/// Manages the main menu navigation and screen switching.
/// Controls which screen is currently visible based on user interaction.
/// </summary>
public class MenuManager : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private GameObject melangeurScreen;
    [SerializeField] private GameObject miniJeuScreen;
    [SerializeField] private GameObject potagerScreen;
    [SerializeField] private GameObject boutiqueScreen;
    [SerializeField] private GameObject collectionScreen;
    [SerializeField] private GameObject optionsScreen;

    private GameObject currentScreen;
    private GameObject[] allScreens;

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
        // Show the Melangeur screen by default
        ShowScreen(MenuScreen.Melangeur);
    }

    /// <summary>
    /// Shows the specified screen and hides all others.
    /// </summary>
    /// <param name="screen">The screen to display</param>
    public void ShowScreen(MenuScreen screen)
    {
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
