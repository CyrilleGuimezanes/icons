using UnityEngine;

/// <summary>
/// Main game initialization manager.
/// Ensures all game systems are initialized in the correct order.
/// </summary>
public class GameInitializer : MonoBehaviour
{
    [Header("Manager Prefabs")]
    [SerializeField] private bool createManagers = true;

    private void Awake()
    {
        if (createManagers)
        {
            InitializeManagers();
        }
    }

    private void InitializeManagers()
    {
        // Create GameSlotsManager first (other managers depend on it)
        if (GameSlotsManager.Instance == null)
        {
            GameObject slotsManagerObj = new GameObject("GameSlotsManager");
            slotsManagerObj.AddComponent<GameSlotsManager>();
        }

        // Create UnlockedIconsManager
        if (UnlockedIconsManager.Instance == null)
        {
            GameObject unlockedObj = new GameObject("UnlockedIconsManager");
            unlockedObj.AddComponent<UnlockedIconsManager>();
        }

        // Create PlayerInventory
        if (PlayerInventory.Instance == null)
        {
            GameObject inventoryObj = new GameObject("PlayerInventory");
            inventoryObj.AddComponent<PlayerInventory>();
        }

        // Create CurrencyManager
        if (CurrencyManager.Instance == null)
        {
            GameObject currencyObj = new GameObject("CurrencyManager");
            currencyObj.AddComponent<CurrencyManager>();
        }
    }
}
