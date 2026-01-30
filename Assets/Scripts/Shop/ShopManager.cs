using System;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("References")]
    public GameObject shopBarPanel;
    public ShopSlot[] buySlots;
    public ShopSlot sellSlot;

    [Header("Shop Content")]
    public ItemData[] itemsForSale;

    [Header("Scripts")]
    public PlayerEconomy playerEconomy;
    public PlayerMovement playerMovement;
    public Weapon weaponScript;

    private ShopInteractable currentShopInteractable;
    private bool isOpen = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (playerEconomy == null)
             playerEconomy = FindAnyObjectByType<PlayerEconomy>();
             shopBarPanel.SetActive(false);
            UpdateShopUI();

    }
    private void Update()
    {
        if (isOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseShop();
            }
        }
    }

    private void UpdateShopUI()
    {
        for (int i = 0; i < buySlots.Length; i++)
        {
            if (i < itemsForSale.Length)
            {
                buySlots[i].SetupShopSlot(itemsForSale[i]);
            }
            else
            {
                buySlots[i].SetupShopSlot(null);
            }
        }

        if (sellSlot != null) sellSlot.isSellSlot = true;
    }

    public void OpenShop(ShopInteractable interactable)
    {
        isOpen = true;
        currentShopInteractable = interactable;
        shopBarPanel.SetActive(true);
        if (playerMovement != null) playerMovement.enabled = false;
        if (weaponScript != null) weaponScript.enabled = false;


        InventoryManager.Instance.SetInventoryState(true);
    }

    public void CloseShop()
    {
        isOpen = false;
        shopBarPanel.SetActive(false);
        currentShopInteractable = null;
        if (playerMovement != null) playerMovement.enabled = true;
        if (weaponScript != null) weaponScript.enabled = true;

        InventoryManager.Instance.SetInventoryState(false);
    }

    public void BuyItem(ItemData item)
    {
        if (playerEconomy.TrySpendMoney(item.buyPrice))
        {
            bool added = InventoryManager.Instance.AddItem(item);
            if (added)
            {
                Debug.Log("Kupiono: " + item.itemName);
                if (SoundManager.Instance.moneyPickupSound != null) 
                    SoundManager.Instance.moneyPickupSound.Play();
            }
            else
            {
                playerEconomy.AddMoney(item.buyPrice);
                Debug.Log("Brak miejsca w ekwipunku");
            }
        }
        else
        {
            if (SoundManager.Instance.noMoneySound != null)
                SoundManager.Instance.noMoneySound.Play();
            Debug.Log("Brak pieniêdzy");
        }
    }

    public void SellItem(InventorySlot inventorySlot)
    {
        ItemData itemToSell = inventorySlot.item;

        if (itemToSell != null)
        {
            int sellValue = itemToSell.sellPrice;

            playerEconomy.AddMoney(sellValue);

            InventoryManager.Instance.RemoveItem(inventorySlot);

            Debug.Log("Sprzedano " + itemToSell.itemName + " za $" + sellValue);

            if (SoundManager.Instance.moneyPickupSound != null)
                SoundManager.Instance.moneyPickupSound.Play();
        }
    }

    internal void ToggleShop(ShopInteractable shopInteractable)
    {
        if(isOpen)
        {
            CloseShop();
        }
        else
        {
            OpenShop(shopInteractable);
        }
    }
}