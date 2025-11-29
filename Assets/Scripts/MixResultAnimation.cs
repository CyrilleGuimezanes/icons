using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Handles animations for mix results (success and failure).
/// Provides visual feedback with humorous messages for failures.
/// </summary>
public class MixResultAnimation : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform resultContainer;
    [SerializeField] private TextMeshProUGUI resultIconText;
    [SerializeField] private TextMeshProUGUI resultMessageText;
    [SerializeField] private Image backgroundImage;
    
    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private float displayDuration = 2f;
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve shakeCurve = AnimationCurve.Linear(0, 1, 1, 0);
    
    [Header("Colors")]
    [SerializeField] private Color successColor = new Color(0.2f, 0.8f, 0.2f);
    [SerializeField] private Color failureColor = new Color(0.9f, 0.3f, 0.3f);
    
    [Header("Failure Messages")]
    [SerializeField] private string[] failureMessages = new string[]
    {
        "Bof... Essaie encore !",
        "Ça ne marche pas !",
        "Pas cette fois !",
        "Hum... Non !",
        "Raté !",
        "Presque... Non !",
        "Nope !",
        "Oups !",
        "Bizarre mélange !",
        "Pas compatible !"
    };
    
    [Header("Failure Icons")]
    [SerializeField] private string[] failureIcons = new string[]
    {
        "❌",
        "💥",
        "💨",
        "😅",
        "🤷",
        "🙈",
        "😬",
        "🤔"
    };
    
    private Coroutine currentAnimation;
    
    /// <summary>
    /// Shows a success animation for a successful mix.
    /// </summary>
    /// <param name="resultIcon">The icon ID of the result</param>
    /// <param name="quantity">The quantity created</param>
    public void ShowSuccess(string resultIcon, int quantity = 1)
    {
        string message = quantity > 1 ? $"Succès ! x{quantity}" : "Succès !";
        PlayAnimation(resultIcon, message, true);
    }
    
    /// <summary>
    /// Shows a failure animation with a humorous message.
    /// </summary>
    public void ShowFailure()
    {
        string icon = GetRandomFailureIcon();
        string message = GetRandomFailureMessage();
        PlayAnimation(icon, message, false);
    }
    
    /// <summary>
    /// Shows a failure animation with a custom message.
    /// </summary>
    /// <param name="message">The message to display</param>
    public void ShowFailure(string message)
    {
        string icon = GetRandomFailureIcon();
        PlayAnimation(icon, message, false);
    }
    
    private string GetRandomFailureIcon()
    {
        if (failureIcons == null || failureIcons.Length == 0)
        {
            return "❌";
        }
        return failureIcons[Random.Range(0, failureIcons.Length)];
    }
    
    private string GetRandomFailureMessage()
    {
        if (failureMessages == null || failureMessages.Length == 0)
        {
            return "Raté !";
        }
        return failureMessages[Random.Range(0, failureMessages.Length)];
    }
    
    private void PlayAnimation(string icon, string message, bool isSuccess)
    {
        // Stop any existing animation
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        
        // Update visuals
        if (resultIconText != null)
        {
            resultIconText.text = icon;
        }
        
        if (resultMessageText != null)
        {
            resultMessageText.text = message;
        }
        
        if (backgroundImage != null)
        {
            backgroundImage.color = isSuccess ? successColor : failureColor;
        }
        
        // Start animation
        gameObject.SetActive(true);
        currentAnimation = StartCoroutine(AnimateResult(isSuccess));
    }
    
    private IEnumerator AnimateResult(bool isSuccess)
    {
        // Fade in and scale up
        float elapsed = 0f;
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            
            // Scale
            float scale = scaleCurve.Evaluate(t);
            if (resultContainer != null)
            {
                resultContainer.localScale = Vector3.one * scale;
            }
            
            // Fade
            if (canvasGroup != null)
            {
                canvasGroup.alpha = t;
            }
            
            yield return null;
        }
        
        // Ensure final state
        if (resultContainer != null)
        {
            resultContainer.localScale = Vector3.one;
        }
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }
        
        // Shake animation for failure
        if (!isSuccess)
        {
            yield return StartCoroutine(ShakeAnimation());
        }
        
        // Wait for display duration
        yield return new WaitForSeconds(displayDuration);
        
        // Fade out
        elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = 1f - (elapsed / animationDuration);
            
            if (canvasGroup != null)
            {
                canvasGroup.alpha = t;
            }
            
            yield return null;
        }
        
        // Hide
        gameObject.SetActive(false);
        currentAnimation = null;
    }
    
    private IEnumerator ShakeAnimation()
    {
        float shakeDuration = 0.5f;
        float shakeIntensity = 10f;
        float elapsed = 0f;
        
        Vector2 originalPosition = resultContainer != null ? resultContainer.anchoredPosition : Vector2.zero;
        
        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / shakeDuration;
            float intensity = shakeCurve.Evaluate(t) * shakeIntensity;
            
            if (resultContainer != null)
            {
                float offsetX = Random.Range(-intensity, intensity);
                float offsetY = Random.Range(-intensity, intensity);
                resultContainer.anchoredPosition = originalPosition + new Vector2(offsetX, offsetY);
            }
            
            yield return null;
        }
        
        // Reset position
        if (resultContainer != null)
        {
            resultContainer.anchoredPosition = originalPosition;
        }
    }
    
    /// <summary>
    /// Hides the result panel immediately.
    /// </summary>
    public void Hide()
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            currentAnimation = null;
        }
        
        gameObject.SetActive(false);
    }
}
