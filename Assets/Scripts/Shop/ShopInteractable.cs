using UnityEngine;

public class ShopInteractable : MonoBehaviour, IInteractable
{
    private ShopManager shopManager;

    private void Start()
    {
        shopManager = FindAnyObjectByType<ShopManager>();
    }

    public bool StartInteract(Transform holdPoint, GameObject player)
    {
        if (shopManager != null)
        {
            shopManager.OpenShop(this);
            return true;
        }
        return false;
    }

    public void StopInteract()
    {
        if (shopManager != null)
        {
            shopManager.CloseShop();
        }
    }
}