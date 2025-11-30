using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages In-App Purchases using Unity IAP.
/// Handles initialization, purchase processing, and receipt validation.
/// </summary>
public class IAPManager : MonoBehaviour, IDetailedStoreListener
{
    /// <summary>
    /// Singleton instance for global access.
    /// </summary>
    public static IAPManager Instance { get; private set; }

    /// <summary>
    /// Event triggered when a purchase is successful.
    /// </summary>
    public event Action<string> OnPurchaseSuccess;

    /// <summary>
    /// Event triggered when a purchase fails.
    /// </summary>
    public event Action<string, string> OnPurchaseFailure;

    /// <summary>
    /// Event triggered when IAP initialization is complete.
    /// </summary>
    public event Action<bool> OnIAPInitialized;

    private IStoreController storeController;
    private IExtensionProvider extensionProvider;

    /// <summary>
    /// Returns true if IAP is initialized and ready.
    /// </summary>
    public bool IsInitialized => storeController != null && extensionProvider != null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePurchasing();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Initializes Unity IAP with the available products.
    /// </summary>
    private void InitializePurchasing()
    {
        if (IsInitialized)
        {
            return;
        }

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Add all icon pack products
        foreach (var pack in IconPacks.GetAllPacks())
        {
            builder.AddProduct(pack.packId, ProductType.Consumable);
        }

        UnityPurchasing.Initialize(this, builder);
    }

    /// <summary>
    /// Called when Unity IAP is ready and has retrieved the product list.
    /// </summary>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        extensionProvider = extensions;
        Debug.Log("IAP initialized successfully");
        OnIAPInitialized?.Invoke(true);
    }

    /// <summary>
    /// Called when Unity IAP fails to initialize.
    /// </summary>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError($"IAP initialization failed: {error}");
        OnIAPInitialized?.Invoke(false);
    }

    /// <summary>
    /// Called when Unity IAP fails to initialize with additional details.
    /// </summary>
    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError($"IAP initialization failed: {error} - {message}");
        OnIAPInitialized?.Invoke(false);
    }

    /// <summary>
    /// Initiates a purchase for the specified product.
    /// </summary>
    /// <param name="productId">The product ID to purchase.</param>
    public void PurchaseProduct(string productId)
    {
        if (!IsInitialized)
        {
            Debug.LogError("IAP not initialized");
            OnPurchaseFailure?.Invoke(productId, "IAP not initialized");
            return;
        }

        Product product = storeController.products.WithID(productId);
        if (product != null && product.availableToPurchase)
        {
            Debug.Log($"Purchasing product: {productId}");
            storeController.InitiatePurchase(product);
        }
        else
        {
            Debug.LogError($"Product not available: {productId}");
            OnPurchaseFailure?.Invoke(productId, "Product not available");
        }
    }

    /// <summary>
    /// Called when a purchase is processed.
    /// </summary>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        string productId = args.purchasedProduct.definition.id;
        Debug.Log($"Purchase successful: {productId}");

        // Process the pack purchase through ShopManager
        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.ProcessIAPPurchase(productId);
        }
        else
        {
            Debug.LogError($"ShopManager not available to process IAP purchase: {productId}. Purchase may be lost!");
        }

        OnPurchaseSuccess?.Invoke(productId);
        return PurchaseProcessingResult.Complete;
    }

    /// <summary>
    /// Called when a purchase fails.
    /// </summary>
    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.LogError($"Purchase failed: {product.definition.id} - {reason}");
        OnPurchaseFailure?.Invoke(product.definition.id, reason.ToString());
    }

    /// <summary>
    /// Called when a purchase fails with detailed information.
    /// </summary>
    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.LogError($"Purchase failed: {product.definition.id} - {failureDescription.message}");
        OnPurchaseFailure?.Invoke(product.definition.id, failureDescription.message);
    }

    /// <summary>
    /// Gets the localized price string for a product.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <returns>The localized price string, or the default price if not available.</returns>
    public string GetLocalizedPrice(string productId)
    {
        if (!IsInitialized)
        {
            // Return default price from IconPacks
            foreach (var pack in IconPacks.GetAllPacks())
            {
                if (pack.packId == productId)
                {
                    return pack.realMoneyPrice;
                }
            }
            return "$?.??";
        }

        Product product = storeController.products.WithID(productId);
        if (product != null)
        {
            return product.metadata.localizedPriceString;
        }

        return "$?.??";
    }

    /// <summary>
    /// Checks if a product is available for purchase.
    /// </summary>
    /// <param name="productId">The product ID to check.</param>
    /// <returns>True if the product is available.</returns>
    public bool IsProductAvailable(string productId)
    {
        if (!IsInitialized)
        {
            return false;
        }

        Product product = storeController.products.WithID(productId);
        return product != null && product.availableToPurchase;
    }

    /// <summary>
    /// Restores previous purchases (for non-consumables on iOS).
    /// </summary>
    public void RestorePurchases()
    {
        if (!IsInitialized)
        {
            Debug.LogError("IAP not initialized");
            return;
        }

#if UNITY_IOS
        var apple = extensionProvider.GetExtension<IAppleExtensions>();
        apple.RestoreTransactions((result, error) =>
        {
            if (result)
            {
                Debug.Log("Restore purchases successful");
            }
            else
            {
                Debug.LogError($"Restore purchases failed: {error}");
            }
        });
#else
        Debug.Log("Restore purchases is only needed on iOS");
#endif
    }
}
