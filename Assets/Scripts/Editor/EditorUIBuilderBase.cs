#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Base class for Editor UI builders.
/// Provides common utility methods for creating UI components in the Editor and saving the scene.
/// </summary>
public abstract class EditorUIBuilderBase
{
    /// <summary>
    /// Reference to the parent canvas for UI elements.
    /// </summary>
    protected Canvas parentCanvas;

    /// <summary>
    /// The root GameObject of this screen.
    /// </summary>
    protected GameObject screenRoot;

    /// <summary>
    /// Standard colors used throughout the UI.
    /// </summary>
    protected static class UIColors
    {
        public static readonly Color Background = new Color(0.96f, 0.96f, 0.96f, 1f);
        public static readonly Color Primary = new Color(0.2f, 0.6f, 1f, 1f);
        public static readonly Color Secondary = new Color(0.5f, 0.5f, 0.5f, 1f);
        public static readonly Color Success = new Color(0.3f, 0.8f, 0.3f, 1f);
        public static readonly Color Danger = new Color(0.9f, 0.3f, 0.3f, 1f);
        public static readonly Color Warning = new Color(1f, 0.6f, 0.2f, 1f);
        public static readonly Color Text = new Color(0.2f, 0.2f, 0.2f, 1f);
        public static readonly Color TextLight = new Color(0.5f, 0.5f, 0.5f, 1f);
        public static readonly Color White = Color.white;
        public static readonly Color Overlay = new Color(0, 0, 0, 0.5f);

        // Rarity colors
        public static readonly Color Common = new Color(0.4f, 0.8f, 0.4f, 1f);
        public static readonly Color Uncommon = new Color(0.3f, 0.5f, 0.9f, 1f);
        public static readonly Color Rare = new Color(0.6f, 0.3f, 0.8f, 1f);
        public static readonly Color Legendary = new Color(1f, 0.6f, 0.2f, 1f);
    }

    /// <summary>
    /// Standard font sizes used throughout the UI.
    /// </summary>
    protected static class FontSizes
    {
        public const float Title = 36f;
        public const float Subtitle = 24f;
        public const float Body = 18f;
        public const float Small = 14f;
        public const float Tiny = 12f;
        public const float Button = 16f;
        public const float Icon = 48f;
        public const float LargeIcon = 64f;
    }

    /// <summary>
    /// Finds the parent canvas in the scene.
    /// </summary>
    protected void FindCanvas()
    {
        if (parentCanvas == null)
        {
            parentCanvas = Object.FindAnyObjectByType<Canvas>();
        }
    }

    /// <summary>
    /// Builds the screen UI. Override in subclasses.
    /// </summary>
    public abstract void BuildScreen(Transform parent);

    /// <summary>
    /// Gets the screen root GameObject.
    /// </summary>
    public GameObject GetScreenRoot() => screenRoot;

    /// <summary>
    /// Saves the current scene after building UI.
    /// </summary>
    protected void SaveScene()
    {
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log($"Scene saved after building UI.");
    }

    #region Panel Creation

    /// <summary>
    /// Creates a panel GameObject with a RectTransform.
    /// </summary>
    protected GameObject CreatePanel(string name, Transform parent)
    {
        GameObject panel = new GameObject(name, typeof(RectTransform));
        panel.transform.SetParent(parent, false);
        Undo.RegisterCreatedObjectUndo(panel, $"Create {name}");
        return panel;
    }

    /// <summary>
    /// Creates a panel that fills its parent.
    /// </summary>
    protected GameObject CreateFullScreenPanel(string name, Transform parent, Color? backgroundColor = null)
    {
        GameObject panel = CreatePanel(name, parent);
        RectTransform rect = panel.GetComponent<RectTransform>();
        SetFullStretch(rect);

        if (backgroundColor.HasValue)
        {
            Image bg = panel.AddComponent<Image>();
            bg.color = backgroundColor.Value;
        }

        return panel;
    }

    /// <summary>
    /// Creates a panel with a background image.
    /// </summary>
    protected GameObject CreatePanelWithBackground(string name, Transform parent, Color backgroundColor)
    {
        GameObject panel = CreatePanel(name, parent);
        Image bg = panel.AddComponent<Image>();
        bg.color = backgroundColor;
        return panel;
    }

    #endregion

    #region Text Creation

    /// <summary>
    /// Creates a TextMeshProUGUI component.
    /// </summary>
    protected TextMeshProUGUI CreateText(string name, Transform parent, string text, float fontSize, FontStyles style = FontStyles.Normal)
    {
        GameObject textObj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));
        textObj.transform.SetParent(parent, false);
        Undo.RegisterCreatedObjectUndo(textObj, $"Create {name}");

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = style;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = UIColors.Text;

        return tmp;
    }

    /// <summary>
    /// Creates a title text element.
    /// </summary>
    protected TextMeshProUGUI CreateTitle(string name, Transform parent, string text)
    {
        return CreateText(name, parent, text, FontSizes.Title, FontStyles.Bold);
    }

    /// <summary>
    /// Creates a subtitle text element.
    /// </summary>
    protected TextMeshProUGUI CreateSubtitle(string name, Transform parent, string text)
    {
        return CreateText(name, parent, text, FontSizes.Subtitle, FontStyles.Bold);
    }

    /// <summary>
    /// Creates a body text element.
    /// </summary>
    protected TextMeshProUGUI CreateBodyText(string name, Transform parent, string text)
    {
        return CreateText(name, parent, text, FontSizes.Body);
    }

    /// <summary>
    /// Creates an icon text element using Material Icons font.
    /// </summary>
    protected TextMeshProUGUI CreateIconText(string name, Transform parent, string iconId, float fontSize = 48f)
    {
        TextMeshProUGUI text = CreateText(name, parent, iconId, fontSize);
        return text;
    }

    #endregion

    #region Button Creation

    /// <summary>
    /// Creates a button with text.
    /// </summary>
    protected Button CreateButton(string name, Transform parent, string text, Color backgroundColor)
    {
        GameObject btnObj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));
        btnObj.transform.SetParent(parent, false);
        Undo.RegisterCreatedObjectUndo(btnObj, $"Create {name}");

        Image btnImage = btnObj.AddComponent<Image>();
        btnImage.color = backgroundColor;

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnImage;

        // Button text
        TextMeshProUGUI btnText = CreateText("Text", btnObj.transform, text, FontSizes.Button, FontStyles.Bold);
        RectTransform btnTextRect = btnText.GetComponent<RectTransform>();
        SetFullStretch(btnTextRect);
        btnText.color = Color.white;

        return btn;
    }

    /// <summary>
    /// Creates a button with an icon.
    /// </summary>
    protected Button CreateIconButton(string name, Transform parent, string iconId, Color backgroundColor)
    {
        GameObject btnObj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));
        btnObj.transform.SetParent(parent, false);
        Undo.RegisterCreatedObjectUndo(btnObj, $"Create {name}");

        Image btnImage = btnObj.AddComponent<Image>();
        btnImage.color = backgroundColor;

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnImage;

        // Button icon
        TextMeshProUGUI iconText = CreateText("Icon", btnObj.transform, iconId, FontSizes.Icon);
        RectTransform iconRect = iconText.GetComponent<RectTransform>();
        SetFullStretch(iconRect);
        iconText.color = Color.white;

        return btn;
    }

    /// <summary>
    /// Creates a button with both icon and text label.
    /// </summary>
    protected Button CreateLabeledIconButton(string name, Transform parent, string iconId, string label, Color backgroundColor)
    {
        GameObject btnObj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));
        btnObj.transform.SetParent(parent, false);
        Undo.RegisterCreatedObjectUndo(btnObj, $"Create {name}");

        Image btnImage = btnObj.AddComponent<Image>();
        btnImage.color = backgroundColor;

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnImage;

        // Icon
        TextMeshProUGUI iconText = CreateText("Icon", btnObj.transform, iconId, FontSizes.Icon);
        RectTransform iconRect = iconText.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0, 0.35f);
        iconRect.anchorMax = new Vector2(1, 1);
        iconRect.offsetMin = Vector2.zero;
        iconRect.offsetMax = Vector2.zero;
        iconText.color = Color.white;

        // Label
        TextMeshProUGUI labelText = CreateText("Label", btnObj.transform, label, FontSizes.Small);
        RectTransform labelRect = labelText.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0);
        labelRect.anchorMax = new Vector2(1, 0.35f);
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        labelText.color = Color.white;

        return btn;
    }

    #endregion

    #region Input Creation

    /// <summary>
    /// Creates a TMP InputField.
    /// </summary>
    protected TMP_InputField CreateInputField(string name, Transform parent, string placeholder = "")
    {
        GameObject inputObj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));
        inputObj.transform.SetParent(parent, false);
        Undo.RegisterCreatedObjectUndo(inputObj, $"Create {name}");

        Image inputBg = inputObj.AddComponent<Image>();
        inputBg.color = Color.white;

        // Text area
        GameObject textArea = CreatePanel("TextArea", inputObj.transform);
        RectTransform textAreaRect = textArea.GetComponent<RectTransform>();
        SetFullStretch(textAreaRect, new Vector2(10, 5), new Vector2(-10, -5));
        textArea.AddComponent<RectMask2D>();

        // Input text
        TextMeshProUGUI inputText = CreateText("Text", textArea.transform, "", FontSizes.Body);
        RectTransform inputTextRect = inputText.GetComponent<RectTransform>();
        SetFullStretch(inputTextRect);
        inputText.alignment = TextAlignmentOptions.Left;

        // Placeholder
        TextMeshProUGUI placeholderText = CreateText("Placeholder", textArea.transform, placeholder, FontSizes.Body, FontStyles.Italic);
        RectTransform placeholderRect = placeholderText.GetComponent<RectTransform>();
        SetFullStretch(placeholderRect);
        placeholderText.alignment = TextAlignmentOptions.Left;
        placeholderText.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        TMP_InputField inputField = inputObj.AddComponent<TMP_InputField>();
        inputField.textViewport = textAreaRect;
        inputField.textComponent = inputText;
        inputField.placeholder = placeholderText;

        return inputField;
    }

    /// <summary>
    /// Creates a Toggle component.
    /// </summary>
    protected Toggle CreateToggle(string name, Transform parent, string label, bool defaultValue = true)
    {
        GameObject toggleObj = new GameObject(name, typeof(RectTransform));
        toggleObj.transform.SetParent(parent, false);
        Undo.RegisterCreatedObjectUndo(toggleObj, $"Create {name}");

        // Background
        GameObject background = CreatePanelWithBackground("Background", toggleObj.transform, UIColors.Secondary);
        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 0.5f);
        bgRect.anchorMax = new Vector2(0, 0.5f);
        bgRect.sizeDelta = new Vector2(50, 30);
        bgRect.anchoredPosition = new Vector2(25, 0);

        // Checkmark
        GameObject checkmark = CreatePanelWithBackground("Checkmark", background.transform, UIColors.Primary);
        RectTransform checkRect = checkmark.GetComponent<RectTransform>();
        checkRect.anchorMin = new Vector2(0.1f, 0.1f);
        checkRect.anchorMax = new Vector2(0.9f, 0.9f);
        checkRect.offsetMin = Vector2.zero;
        checkRect.offsetMax = Vector2.zero;

        // Label
        TextMeshProUGUI labelText = CreateText("Label", toggleObj.transform, label, FontSizes.Body);
        RectTransform labelRect = labelText.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0);
        labelRect.anchorMax = new Vector2(1, 1);
        labelRect.offsetMin = new Vector2(60, 0);
        labelRect.offsetMax = Vector2.zero;
        labelText.alignment = TextAlignmentOptions.Left;

        Toggle toggle = toggleObj.AddComponent<Toggle>();
        toggle.targetGraphic = background.GetComponent<Image>();
        toggle.graphic = checkmark.GetComponent<Image>();
        toggle.isOn = defaultValue;

        return toggle;
    }

    /// <summary>
    /// Creates a Slider component.
    /// </summary>
    protected Slider CreateSlider(string name, Transform parent, float minValue = 0f, float maxValue = 1f, float defaultValue = 1f)
    {
        GameObject sliderObj = new GameObject(name, typeof(RectTransform));
        sliderObj.transform.SetParent(parent, false);
        Undo.RegisterCreatedObjectUndo(sliderObj, $"Create {name}");

        // Background
        GameObject background = CreatePanelWithBackground("Background", sliderObj.transform, UIColors.Secondary);
        RectTransform bgRect = background.GetComponent<RectTransform>();
        SetFullStretch(bgRect);

        // Fill Area
        GameObject fillArea = CreatePanel("FillArea", sliderObj.transform);
        RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
        SetFullStretch(fillAreaRect, new Vector2(5, 0), new Vector2(-5, 0));

        // Fill
        GameObject fill = CreatePanelWithBackground("Fill", fillArea.transform, UIColors.Primary);
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = new Vector2(0, 1);
        fillRect.sizeDelta = new Vector2(10, 0);

        // Handle Area
        GameObject handleArea = CreatePanel("HandleArea", sliderObj.transform);
        RectTransform handleAreaRect = handleArea.GetComponent<RectTransform>();
        SetFullStretch(handleAreaRect, new Vector2(10, 0), new Vector2(-10, 0));

        // Handle
        GameObject handle = CreatePanelWithBackground("Handle", handleArea.transform, UIColors.White);
        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(20, 0);

        Slider slider = sliderObj.AddComponent<Slider>();
        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        slider.value = defaultValue;
        slider.targetGraphic = handle.GetComponent<Image>();

        return slider;
    }

    #endregion

    #region Layout Creation

    /// <summary>
    /// Creates a vertical layout group.
    /// </summary>
    protected VerticalLayoutGroup CreateVerticalLayout(GameObject obj, float spacing = 10f, RectOffset padding = null)
    {
        VerticalLayoutGroup layout = obj.AddComponent<VerticalLayoutGroup>();
        layout.spacing = spacing;
        layout.padding = padding ?? new RectOffset(0, 0, 0, 0);
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        return layout;
    }

    /// <summary>
    /// Creates a horizontal layout group.
    /// </summary>
    protected HorizontalLayoutGroup CreateHorizontalLayout(GameObject obj, float spacing = 10f, RectOffset padding = null)
    {
        HorizontalLayoutGroup layout = obj.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = spacing;
        layout.padding = padding ?? new RectOffset(0, 0, 0, 0);
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;
        return layout;
    }

    /// <summary>
    /// Creates a grid layout group.
    /// </summary>
    protected GridLayoutGroup CreateGridLayout(GameObject obj, Vector2 cellSize, Vector2 spacing, GridLayoutGroup.Corner startCorner = GridLayoutGroup.Corner.UpperLeft)
    {
        GridLayoutGroup layout = obj.AddComponent<GridLayoutGroup>();
        layout.cellSize = cellSize;
        layout.spacing = spacing;
        layout.startCorner = startCorner;
        layout.startAxis = GridLayoutGroup.Axis.Horizontal;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.constraint = GridLayoutGroup.Constraint.Flexible;
        return layout;
    }

    /// <summary>
    /// Adds a LayoutElement to control sizing in layouts.
    /// </summary>
    protected LayoutElement AddLayoutElement(GameObject obj, float? preferredHeight = null, float? preferredWidth = null, 
        float? minHeight = null, float? minWidth = null, float? flexibleWidth = null, float? flexibleHeight = null)
    {
        LayoutElement element = obj.AddComponent<LayoutElement>();
        if (preferredHeight.HasValue) element.preferredHeight = preferredHeight.Value;
        if (preferredWidth.HasValue) element.preferredWidth = preferredWidth.Value;
        if (minHeight.HasValue) element.minHeight = minHeight.Value;
        if (minWidth.HasValue) element.minWidth = minWidth.Value;
        if (flexibleWidth.HasValue) element.flexibleWidth = flexibleWidth.Value;
        if (flexibleHeight.HasValue) element.flexibleHeight = flexibleHeight.Value;
        return element;
    }

    /// <summary>
    /// Adds a ContentSizeFitter to auto-size content.
    /// </summary>
    protected ContentSizeFitter AddContentSizeFitter(GameObject obj, ContentSizeFitter.FitMode horizontalFit = ContentSizeFitter.FitMode.Unconstrained, 
        ContentSizeFitter.FitMode verticalFit = ContentSizeFitter.FitMode.Unconstrained)
    {
        ContentSizeFitter fitter = obj.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = horizontalFit;
        fitter.verticalFit = verticalFit;
        return fitter;
    }

    #endregion

    #region ScrollView Creation

    /// <summary>
    /// Creates a ScrollRect with content container.
    /// </summary>
    protected ScrollRect CreateScrollView(string name, Transform parent, bool horizontal = false, bool vertical = true)
    {
        GameObject scrollObj = new GameObject(name, typeof(RectTransform));
        scrollObj.transform.SetParent(parent, false);
        Undo.RegisterCreatedObjectUndo(scrollObj, $"Create {name}");

        Image scrollBg = scrollObj.AddComponent<Image>();
        scrollBg.color = Color.clear;

        ScrollRect scrollRect = scrollObj.AddComponent<ScrollRect>();

        // Viewport
        GameObject viewport = CreatePanel("Viewport", scrollObj.transform);
        RectTransform viewportRect = viewport.GetComponent<RectTransform>();
        SetFullStretch(viewportRect);
        viewport.AddComponent<Mask>().showMaskGraphic = false;
        viewport.AddComponent<Image>().color = Color.white;

        // Content
        GameObject content = CreatePanel("Content", viewport.transform);
        RectTransform contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.sizeDelta = new Vector2(0, 0);

        scrollRect.content = contentRect;
        scrollRect.viewport = viewportRect;
        scrollRect.horizontal = horizontal;
        scrollRect.vertical = vertical;
        scrollRect.movementType = ScrollRect.MovementType.Elastic;
        scrollRect.elasticity = 0.1f;

        return scrollRect;
    }

    #endregion

    #region RectTransform Helpers

    /// <summary>
    /// Sets RectTransform to stretch and fill parent.
    /// </summary>
    protected void SetFullStretch(RectTransform rect, Vector2? offsetMin = null, Vector2? offsetMax = null)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = offsetMin ?? Vector2.zero;
        rect.offsetMax = offsetMax ?? Vector2.zero;
    }

    /// <summary>
    /// Sets RectTransform anchors and position.
    /// </summary>
    protected void SetAnchors(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2? anchoredPosition = null, Vector2? sizeDelta = null)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        if (anchoredPosition.HasValue) rect.anchoredPosition = anchoredPosition.Value;
        if (sizeDelta.HasValue) rect.sizeDelta = sizeDelta.Value;
    }

    /// <summary>
    /// Sets RectTransform to be centered with specific size.
    /// </summary>
    protected void SetCentered(RectTransform rect, Vector2 size)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = Vector2.zero;
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Gets a rarity color.
    /// </summary>
    protected Color GetRarityColor(IconRarity rarity)
    {
        return rarity switch
        {
            IconRarity.Common => UIColors.Common,
            IconRarity.Uncommon => UIColors.Uncommon,
            IconRarity.Rare => UIColors.Rare,
            IconRarity.Legendary => UIColors.Legendary,
            _ => UIColors.White
        };
    }

    /// <summary>
    /// Creates a modal overlay panel.
    /// </summary>
    protected GameObject CreateModalOverlay(string name, Transform parent)
    {
        GameObject overlay = CreateFullScreenPanel(name, parent, UIColors.Overlay);
        
        // Add a button to block clicks on background
        Button blocker = overlay.AddComponent<Button>();
        blocker.transition = Selectable.Transition.None;
        
        return overlay;
    }

    /// <summary>
    /// Creates a dialog box within a modal.
    /// </summary>
    protected GameObject CreateDialogBox(string name, Transform parent, Vector2? size = null)
    {
        GameObject dialog = CreatePanelWithBackground(name, parent, UIColors.White);
        RectTransform dialogRect = dialog.GetComponent<RectTransform>();
        
        Vector2 dialogSize = size ?? new Vector2(600, 400);
        SetCentered(dialogRect, dialogSize);
        
        return dialog;
    }

    #endregion
}
#endif
