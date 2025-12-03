#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// <summary>
/// Editor script that automatically builds UI when the scene loads in the Editor.
/// Also provides menu items for manual UI generation.
/// </summary>
[InitializeOnLoad]
public static class EditorSceneUIGenerator
{
    private static bool isSubscribed = false;

    static EditorSceneUIGenerator()
    {
        // Subscribe to scene opened event
        EditorSceneManager.sceneOpened += OnSceneOpened;
        isSubscribed = true;
    }

    /// <summary>
    /// Called when a scene is opened in the Editor.
    /// </summary>
    private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
    {
        // Only auto-generate UI for MainSceneScripted
        if (scene.name == "MainSceneScripted")
        {
            Debug.Log($"Scene '{scene.name}' opened. Auto-generating UI...");
            
            // Delay to ensure scene is fully loaded
            EditorApplication.delayCall += () =>
            {
                BuildUIAndSave();
            };
        }
    }

    [MenuItem("Icons/Generate UI on Scene Load/Enable Auto-Generation")]
    public static void EnableAutoGeneration()
    {
        if (!isSubscribed)
        {
            EditorSceneManager.sceneOpened += OnSceneOpened;
            isSubscribed = true;
            Debug.Log("Auto UI generation enabled.");
        }
        else
        {
            Debug.Log("Auto UI generation is already enabled.");
        }
    }

    [MenuItem("Icons/Generate UI on Scene Load/Disable Auto-Generation")]
    public static void DisableAutoGeneration()
    {
        if (isSubscribed)
        {
            EditorSceneManager.sceneOpened -= OnSceneOpened;
            isSubscribed = false;
            Debug.Log("Auto UI generation disabled.");
        }
        else
        {
            Debug.Log("Auto UI generation is already disabled.");
        }
    }

    /// <summary>
    /// Builds the complete UI and saves the scene.
    /// </summary>
    private static void BuildUIAndSave()
    {
        // Check if we have a Canvas in the scene
        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("No Canvas found in scene. Skipping UI generation.");
            return;
        }

        // Build the UI
        EditorMainUIBuilder builder = new EditorMainUIBuilder();
        builder.BuildUI();

        // Mark scene as dirty and save
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();

        Debug.Log("UI generated and scene saved successfully!");
    }

    [MenuItem("Icons/Build UI/Rebuild All UI")]
    public static void RebuildAllUI()
    {
        BuildUIAndSave();
    }

    [MenuItem("Icons/Build UI/Clear Generated UI")]
    public static void ClearGeneratedUI()
    {
        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("No Canvas found in scene.");
            return;
        }

        // Remove ScreensContainer
        Transform screensContainer = canvas.transform.Find("ScreensContainer");
        if (screensContainer != null)
        {
            Undo.DestroyObjectImmediate(screensContainer.gameObject);
        }

        // Remove BottomNavigation
        Transform bottomNav = canvas.transform.Find("BottomNavigation");
        if (bottomNav != null)
        {
            Undo.DestroyObjectImmediate(bottomNav.gameObject);
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("Generated UI cleared.");
    }
}
#endif
