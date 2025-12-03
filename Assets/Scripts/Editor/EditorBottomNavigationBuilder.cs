#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

/// <summary>
/// Editor script that builds the bottom navigation bar UI.
/// </summary>
public class EditorBottomNavigationBuilder : EditorUIBuilderBase
{
    private struct NavButtonInfo
    {
        public string icon;
        public string label;
        public MenuScreen screen;

        public NavButtonInfo(string icon, string label, MenuScreen screen)
        {
            this.icon = icon;
            this.label = label;
            this.screen = screen;
        }
    }

    private readonly NavButtonInfo[] navButtons = new NavButtonInfo[]
    {
        new NavButtonInfo("🧪", "Mélangeur", MenuScreen.Melangeur),
        new NavButtonInfo("🎲", "Mini-Jeu", MenuScreen.MiniJeu),
        new NavButtonInfo("🌱", "Potager", MenuScreen.Potager),
        new NavButtonInfo("🛒", "Boutique", MenuScreen.Boutique),
        new NavButtonInfo("📚", "Collection", MenuScreen.Collection),
        new NavButtonInfo("⚙️", "Options", MenuScreen.Options)
    };

    private BottomNavigation navigationComponent;
    private MenuButton[] menuButtons;

    [MenuItem("Icons/Build UI/Build Bottom Navigation")]
    public static void BuildBottomNavigation()
    {
        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No Canvas found in scene!");
            return;
        }

        // Remove existing navigation
        Transform existing = canvas.transform.Find("BottomNavigation");
        if (existing != null)
        {
            Undo.DestroyObjectImmediate(existing.gameObject);
        }

        EditorBottomNavigationBuilder builder = new EditorBottomNavigationBuilder();
        builder.BuildScreen(canvas.transform);
        
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("Bottom navigation built successfully!");
    }

    public override void BuildScreen(Transform parent)
    {
        FindCanvas();

        if (parentCanvas == null && parent != null)
        {
            parentCanvas = parent.GetComponentInParent<Canvas>();
        }

        if (parentCanvas == null)
        {
            Debug.LogError("EditorBottomNavigationBuilder: No canvas found!");
            return;
        }

        // Create root navigation panel
        screenRoot = CreatePanel("BottomNavigation", parent);
        RectTransform rootRect = screenRoot.GetComponent<RectTransform>();
        rootRect.anchorMin = new Vector2(0, 0);
        rootRect.anchorMax = new Vector2(1, 0);
        rootRect.pivot = new Vector2(0.5f, 0);
        rootRect.sizeDelta = new Vector2(0, 80);
        rootRect.anchoredPosition = Vector2.zero;

        // Add background
        Image bg = screenRoot.AddComponent<Image>();
        bg.color = UIColors.White;

        // Add shadow effect (simple line at top)
        CreateShadowLine();

        // Create button container
        GameObject buttonsContainer = CreatePanel("ButtonsContainer", screenRoot.transform);
        RectTransform buttonsRect = buttonsContainer.GetComponent<RectTransform>();
        SetFullStretch(buttonsRect, new Vector2(10, 5), new Vector2(-10, -5));

        // Add horizontal layout
        HorizontalLayoutGroup layout = CreateHorizontalLayout(buttonsContainer, 0);
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;

        // Create menu buttons
        menuButtons = new MenuButton[navButtons.Length];
        for (int i = 0; i < navButtons.Length; i++)
        {
            menuButtons[i] = CreateNavButton(buttonsContainer.transform, navButtons[i], i);
        }

        // Add BottomNavigation component
        navigationComponent = screenRoot.GetComponent<BottomNavigation>();
        if (navigationComponent == null)
        {
            navigationComponent = screenRoot.AddComponent<BottomNavigation>();
        }
        navigationComponent.SetMenuButtons(menuButtons);
    }

    private void CreateShadowLine()
    {
        GameObject shadow = CreatePanel("Shadow", screenRoot.transform);
        RectTransform shadowRect = shadow.GetComponent<RectTransform>();
        shadowRect.anchorMin = new Vector2(0, 1);
        shadowRect.anchorMax = new Vector2(1, 1);
        shadowRect.pivot = new Vector2(0.5f, 1);
        shadowRect.sizeDelta = new Vector2(0, 1);
        shadowRect.anchoredPosition = Vector2.zero;

        Image shadowImage = shadow.AddComponent<Image>();
        shadowImage.color = new Color(0.9f, 0.9f, 0.9f, 1f);
    }

    private MenuButton CreateNavButton(Transform parent, NavButtonInfo info, int index)
    {
        GameObject buttonObj = new GameObject($"NavButton_{info.screen}", typeof(RectTransform), typeof(CanvasRenderer));
        buttonObj.transform.SetParent(parent, false);
        Undo.RegisterCreatedObjectUndo(buttonObj, $"Create NavButton_{info.screen}");

        // Background (transparent by default)
        Image btnBg = buttonObj.AddComponent<Image>();
        btnBg.color = Color.clear;

        // Button component
        Button btn = buttonObj.AddComponent<Button>();
        btn.targetGraphic = btnBg;
        btn.transition = Selectable.Transition.None;

        // Icon
        TextMeshProUGUI iconText = CreateText("Icon", buttonObj.transform, info.icon, 28f);
        RectTransform iconRect = iconText.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0, 0.4f);
        iconRect.anchorMax = new Vector2(1, 1);
        iconRect.offsetMin = Vector2.zero;
        iconRect.offsetMax = new Vector2(0, -5);
        iconText.color = UIColors.Secondary;

        // Label
        TextMeshProUGUI labelText = CreateText("Label", buttonObj.transform, info.label, 10f);
        RectTransform labelRect = labelText.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0);
        labelRect.anchorMax = new Vector2(1, 0.4f);
        labelRect.offsetMin = new Vector2(0, 2);
        labelRect.offsetMax = Vector2.zero;
        labelText.color = UIColors.Secondary;

        // Add MenuButton component
        MenuButton menuButton = buttonObj.GetComponent<MenuButton>();
        if (menuButton == null)
        {
            menuButton = buttonObj.AddComponent<MenuButton>();
        }
        menuButton.InitializeReferences(iconText, labelText);

        return menuButton;
    }

    /// <summary>
    /// Gets the BottomNavigation component.
    /// </summary>
    public BottomNavigation GetNavigationComponent() => navigationComponent;
}
#endif
