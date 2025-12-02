using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Builds the "Icon Catcher" mini-game UI at runtime.
/// </summary>
public class IconCatcherGameBuilder : MiniGameBuilderBase
{
    private RectTransform catcherRect;
    private RectTransform spawnArea;
    private TextMeshProUGUI caughtCountText;
    private GameObject iconPrefab;

    public override void BuildGame(Transform parent)
    {
        gameRoot = CreateFullScreenPanel("IconCatcherGame", parent);
        gameRoot.SetActive(false);

        CreateGameHeader(gameRoot.transform, "🧺 Attrape les icônes !");
        CreateInstructionPanel(gameRoot.transform, "Déplace le panier pour attraper les icônes !");
        CreateGameArea(gameRoot.transform);
        BuildCatcherGameUI();
        SetupController();
    }

    private void BuildCatcherGameUI()
    {
        // Spawn area (top area where icons fall from)
        GameObject spawnAreaObj = CreatePanel("SpawnArea", gamePanel.transform);
        spawnArea = spawnAreaObj.GetComponent<RectTransform>();
        spawnArea.anchorMin = new Vector2(0, 0.8f);
        spawnArea.anchorMax = new Vector2(1, 1);
        spawnArea.offsetMin = Vector2.zero;
        spawnArea.offsetMax = Vector2.zero;

        // Fall area (where icons fall through)
        GameObject fallArea = CreatePanel("FallArea", gamePanel.transform);
        RectTransform fallRect = fallArea.GetComponent<RectTransform>();
        fallRect.anchorMin = new Vector2(0, 0.2f);
        fallRect.anchorMax = new Vector2(1, 0.8f);
        fallRect.offsetMin = Vector2.zero;
        fallRect.offsetMax = Vector2.zero;

        // Catcher (basket at bottom)
        GameObject catcher = CreatePanelWithBackground("Catcher", gamePanel.transform, UIColors.Primary);
        catcherRect = catcher.GetComponent<RectTransform>();
        catcherRect.anchorMin = new Vector2(0.35f, 0.05f);
        catcherRect.anchorMax = new Vector2(0.65f, 0.18f);
        catcherRect.offsetMin = Vector2.zero;
        catcherRect.offsetMax = Vector2.zero;

        // Catcher icon
        TextMeshProUGUI catcherIcon = CreateText("CatcherIcon", catcher.transform, "🧺", 40f);
        RectTransform catcherIconRect = catcherIcon.GetComponent<RectTransform>();
        SetFullStretch(catcherIconRect);
        catcherIcon.color = Color.white;

        // Left/Right movement buttons
        GameObject controlsContainer = CreatePanel("Controls", gamePanel.transform);
        RectTransform controlsRect = controlsContainer.GetComponent<RectTransform>();
        controlsRect.anchorMin = new Vector2(0, 0);
        controlsRect.anchorMax = new Vector2(1, 0.18f);
        controlsRect.offsetMin = new Vector2(10, 5);
        controlsRect.offsetMax = new Vector2(-10, 0);

        HorizontalLayoutGroup layout = CreateHorizontalLayout(controlsContainer, 0);

        // Left button
        Button leftBtn = CreateButton("LeftButton", controlsContainer.transform, "◀", UIColors.Secondary);
        AddLayoutElement(leftBtn.gameObject, flexibleWidth: 1);

        // Spacer
        GameObject spacer = CreatePanel("Spacer", controlsContainer.transform);
        AddLayoutElement(spacer, flexibleWidth: 1);

        // Right button
        Button rightBtn = CreateButton("RightButton", controlsContainer.transform, "▶", UIColors.Secondary);
        AddLayoutElement(rightBtn.gameObject, flexibleWidth: 1);

        // Caught count
        caughtCountText = CreateText("CaughtCount", gamePanel.transform, "Attrapés: 0", FontSizes.Body, FontStyles.Bold);
        caughtCountText.color = UIColors.Success;
        RectTransform caughtRect = caughtCountText.GetComponent<RectTransform>();
        caughtRect.anchorMin = new Vector2(0, 0.85f);
        caughtRect.anchorMax = new Vector2(1, 0.95f);
        caughtRect.offsetMin = Vector2.zero;
        caughtRect.offsetMax = Vector2.zero;

        // Create falling icon prefab
        CreateIconPrefab();
    }

    private void CreateIconPrefab()
    {
        iconPrefab = new GameObject("FallingIconPrefab", typeof(RectTransform));
        iconPrefab.SetActive(false);

        RectTransform iconRect = iconPrefab.GetComponent<RectTransform>();
        iconRect.sizeDelta = new Vector2(50, 50);

        Image iconBg = iconPrefab.AddComponent<Image>();
        iconBg.color = UIColors.Warning;

        // Icon text
        GameObject iconTextObj = new GameObject("Icon", typeof(RectTransform), typeof(CanvasRenderer));
        iconTextObj.transform.SetParent(iconPrefab.transform, false);
        TextMeshProUGUI iconText = iconTextObj.AddComponent<TextMeshProUGUI>();
        iconText.fontSize = 28f;
        iconText.alignment = TextAlignmentOptions.Center;
        iconText.color = Color.white;
        RectTransform textRect = iconTextObj.GetComponent<RectTransform>();
        SetFullStretch(textRect);
    }

    private void SetupController()
    {
        miniGame = gameRoot.AddComponent<IconCatcherGame>();

        ((IconCatcherGame)miniGame).InitializeReferences(
            timerText,
            scoreText,
            instructionText,
            gamePanel,
            catcherRect,
            spawnArea,
            caughtCountText,
            iconPrefab
        );
    }
}
