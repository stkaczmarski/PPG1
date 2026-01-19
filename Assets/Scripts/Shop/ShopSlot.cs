using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ShopSlot : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    [Header("Settings")]
    public bool isSellSlot = false;

    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI priceText;

    private ItemData itemForSale;
    private ShopManager shopManager;

    private void Start()
    {
        shopManager = FindAnyObjectByType<ShopManager>();
    }

    public void SetupShopSlot(ItemData item)
    {
        itemForSale = item;

        if (itemForSale != null)
        {
            iconImage.sprite = item.icon;
            iconImage.enabled = true;
            if (priceText != null) priceText.text = "$" + item.buyPrice;
        }
        else
        {
            if (!isSellSlot)
            {
                iconImage.enabled = false;
                if (priceText != null) priceText.text = "";
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isSellSlot) return;
        if (itemForSale == null) return;

        shopManager.BuyItem(itemForSale);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!isSellSlot) return;

        InventorySlot draggedSlot = eventData.pointerDrag.GetComponent<InventorySlot>();

        if (draggedSlot != null && draggedSlot.item != null)
        {
            shopManager.SellItem(draggedSlot);
        }
    }
}