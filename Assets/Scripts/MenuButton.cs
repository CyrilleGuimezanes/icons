using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles the visual state of menu navigation buttons.
/// Changes appearance when the button is selected or deselected.
/// </summary>
[RequireComponent(typeof(Button))]
public class MenuButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI iconText;
    [SerializeField] private TextMeshProUGUI labelText;

    [Header("Colors")]
    [SerializeField] private Color selectedColor = new Color(0.2f, 0.6f, 1f);
    [SerializeField] private Color normalColor = new Color(0.5f, 0.5f, 0.5f);

    private Button button;
    private bool isSelected;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    /// <summary>
    /// Sets the selected state of this menu button.
    /// </summary>
    /// <param name="selected">Whether the button is selected</param>
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        Color targetColor = isSelected ? selectedColor : normalColor;

        if (iconText != null)
        {
            iconText.color = targetColor;
        }

        if (labelText != null)
        {
            labelText.color = targetColor;
        }
    }
}
