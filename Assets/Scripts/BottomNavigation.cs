using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the bottom navigation bar behavior.
/// Manages button selection states and communicates with MenuManager.
/// </summary>
public class BottomNavigation : MonoBehaviour
{
    [Header("Menu Manager")]
    [SerializeField] private MenuManager menuManager;

    [Header("Menu Buttons")]
    [SerializeField] private MenuButton[] menuButtons;

    private int currentIndex = 0;

    private void Start()
    {
        // Select the first button by default
        SelectButton(0);
    }

    /// <summary>
    /// Called when a menu button is clicked.
    /// </summary>
    /// <param name="index">Index of the clicked button (0-5)</param>
    public void OnMenuButtonClicked(int index)
    {
        if (index == currentIndex) return;

        SelectButton(index);
        menuManager?.ShowScreenByIndex(index);
    }

    private void SelectButton(int index)
    {
        // Deselect all buttons
        foreach (var button in menuButtons)
        {
            if (button != null)
            {
                button.SetSelected(false);
            }
        }

        // Select the target button
        if (index >= 0 && index < menuButtons.Length && menuButtons[index] != null)
        {
            menuButtons[index].SetSelected(true);
            currentIndex = index;
        }
    }
}
