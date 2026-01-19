using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("References")]
    public Image iconImage;
    public Image selectionOutline;

    [HideInInspector] public ItemData item;
    [HideInInspector] public int slotIndex;
    [HideInInspector] public bool isToolbarSlot;

    private Transform originalParent;
    private CanvasGroup canvasGroup;
    private InventoryManager inventoryManager;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        inventoryManager = FindAnyObjectByType<InventoryManager>();
    }

    public void Setup(ItemData newItem)
    {
        item = newItem;
        if (item != null)
        {
            iconImage.sprite = item.icon;
            iconImage.enabled = true;
        }
        else
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item == null) return;
        
        originalParent = iconImage.transform.parent;
        iconImage.transform.SetParent(inventoryManager.transform);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item == null) return;
        iconImage.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (item == null) return;

        iconImage.transform.SetParent(originalParent);
        iconImage.transform.localPosition = Vector3.zero;
        canvasGroup.blocksRaycasts = true;

        inventoryManager.RefreshUI();
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlot draggedSlot = eventData.pointerDrag.GetComponent<InventorySlot>();

        if (draggedSlot != null)
        {
            inventoryManager.SwapItems(draggedSlot, this);
        }
    }
}