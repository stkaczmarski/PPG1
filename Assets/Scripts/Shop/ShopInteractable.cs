using UnityEngine;

public class ShopInteractable : MonoBehaviour, IInteractable
{
    public ShopManager shopManager;

    private void Start()
    {
        //shopManager = FindAnyObjectByType<ShopManager>();
    }

    public bool StartInteract(Transform holdPoint, GameObject player)
    {
        if (shopManager != null)
        {
            shopManager.ToggleShop(this);
            return true;
        }
        return false;
    }

    public void StopInteract()
    {
        //if (shopManager != null)
        //{
        //    shopManager.CloseShop();
        //}
    }
}