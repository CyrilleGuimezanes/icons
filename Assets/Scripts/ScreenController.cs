using UnityEngine;
using TMPro;

/// <summary>
/// Base class for screen controllers.
/// Provides common functionality for all game screens.
/// </summary>
public class ScreenController : MonoBehaviour
{
    [Header("Screen Info")]
    [SerializeField] protected string screenTitle;
    [SerializeField] protected TextMeshProUGUI titleText;

    protected virtual void OnEnable()
    {
        UpdateTitle();
    }

    protected virtual void UpdateTitle()
    {
        if (titleText != null)
        {
            titleText.text = screenTitle;
        }
    }
}
