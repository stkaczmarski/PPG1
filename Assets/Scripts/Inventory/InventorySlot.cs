using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("References")]
    public Image iconImage;
    public Image selectionOutline;

    
    public ItemData item;
    public int slotIndex;
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
            iconImage.enabled = true;
            iconImage.sprite = item.icon;
        }
        else
        {
            iconImage.enabled = false;
            iconImage.sprite = null;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item == null) return;
        
        originalParent = transform;
        iconImage.transform.SetParent(inventoryManager.transform);
        //canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item == null) return;
        iconImage.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //if (item == null) return;

        iconImage.transform.SetParent(originalParent);
        iconImage.transform.localPosition = Vector3.zero;
        //canvasGroup.blocksRaycasts = true;


    }

    public void OnDrop(PointerEventData eventData)
    {
       
        InventorySlot draggedSlot = eventData.pointerDrag.GetComponent<InventorySlot>();

        if (draggedSlot != null && draggedSlot.item != null)
        {
            inventoryManager.SwapItems(draggedSlot, this);
        }
        inventoryManager.RefreshUI();
    }
}