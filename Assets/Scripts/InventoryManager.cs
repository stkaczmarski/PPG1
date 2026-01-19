using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("UI References")]
    public GameObject inventoryPanel;
    public Transform mainInventoryGrid;
    public Transform toolbarGrid;

    [Header("Player References")]
    public GameObject player;

    private ItemData[] mainInventoryItems = new ItemData[40];
    private ItemData[] toolbarItems = new ItemData[8];

    private InventorySlot[] mainSlotsUI;
    private InventorySlot[] toolbarSlotsUI;

    private bool isInventoryOpen = false;
    public bool IsInventoryOpen { get => isInventoryOpen;  }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        mainSlotsUI = mainInventoryGrid.GetComponentsInChildren<InventorySlot>();
        toolbarSlotsUI = toolbarGrid.GetComponentsInChildren<InventorySlot>();

        for (int i = 0; i < mainSlotsUI.Length; i++)
        {
            mainSlotsUI[i].slotIndex = i;
            mainSlotsUI[i].isToolbarSlot = false;
        }

        for (int i = 0; i < toolbarSlotsUI.Length; i++)
        {
            toolbarSlotsUI[i].slotIndex = i;
            toolbarSlotsUI[i].isToolbarSlot = true;
        }

        inventoryPanel.SetActive(false);
        RefreshUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }

        HandleToolbarInput();
    }

    public bool AddItem(ItemData newItem)
    {
        for (int i = 0; i < toolbarItems.Length; i++)
        {
            if (toolbarItems[i] == null)
            {
                toolbarItems[i] = newItem;
                RefreshUI();
                return true;
            }
        }

        for (int i = 0; i < mainInventoryItems.Length; i++)
        {
            if (mainInventoryItems[i] == null)
            {
                mainInventoryItems[i] = newItem;
                RefreshUI();
                return true;
            }
        }

        Debug.Log("Ekwipunek pe³ny!");
        return false;
    }

    public void SwapItems(InventorySlot slotA, InventorySlot slotB)
    {
        ItemData itemA = GetItemFromSlot(slotA);
        ItemData itemB = GetItemFromSlot(slotB);

        // Zamiana danych w tablicach
        SetItemInSlot(slotA, itemB);
        SetItemInSlot(slotB, itemA);

        RefreshUI();
    }

    private ItemData GetItemFromSlot(InventorySlot slot)
    {
        if (slot.isToolbarSlot) return toolbarItems[slot.slotIndex];
        return mainInventoryItems[slot.slotIndex];
    }

    private void SetItemInSlot(InventorySlot slot, ItemData item)
    {
        if (slot.isToolbarSlot) toolbarItems[slot.slotIndex] = item;
        else mainInventoryItems[slot.slotIndex] = item;
    }

    public void RefreshUI()
    {
        for (int i = 0; i < mainSlotsUI.Length; i++)
        {
            mainSlotsUI[i].Setup(i < mainInventoryItems.Length ? mainInventoryItems[i] : null);
        }

        for (int i = 0; i < toolbarSlotsUI.Length; i++)
        {
            toolbarSlotsUI[i].Setup(i < toolbarItems.Length ? toolbarItems[i] : null);
        }
    }

    private void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);

        // Obs³uga kursora
        if (isInventoryOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void HandleToolbarInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) UseItem(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) UseItem(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) UseItem(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) UseItem(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) UseItem(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) UseItem(5);
        if (Input.GetKeyDown(KeyCode.Alpha7)) UseItem(6);
        if (Input.GetKeyDown(KeyCode.Alpha8)) UseItem(7);
    }

    private void UseItem(int index)
    {
        if (index < 0 || index >= toolbarItems.Length) return;
        ItemData item = toolbarItems[index];

        if (item != null)
        {
            bool used = ApplyItemEffect(item);
            if (used)
            {
                toolbarItems[index] = null; // Usuñ przedmiot po zu¿yciu
                RefreshUI();
            }
        }
    }

    private bool ApplyItemEffect(ItemData item)
    {
        if (player == null) return false;

        if (item.type == ItemPickup.PickupType.Health)
        {
            PlayerHealth healthScript = player.GetComponent<PlayerHealth>();
            if (healthScript != null && healthScript.currentHealth < healthScript.maxHealth)
            {
                healthScript.Heal(item.amount);
                if (SoundManager.Instance.healthPickupSound != null)
                    SoundManager.Instance.healthPickupSound.Play();
                return true;
            }
        }
        else if (item.type == ItemPickup.PickupType.Ammo)
        {
            PlayerMovement movement = player.GetComponent<PlayerMovement>();
            Weapon weaponToReload = movement != null ? movement.weaponScript : null;

            if (weaponToReload != null && weaponToReload.ammoLeft < 30)
            {
                weaponToReload.ammoLeft += item.amount;
                if (weaponToReload.ammoLeft > 30) weaponToReload.ammoLeft = 30;

                weaponToReload.UpdateAmmoUI();

                if (SoundManager.Instance.ammoPickupSound != null)
                    SoundManager.Instance.ammoPickupSound.Play();
                return true;
            }
        }
        return false;
    }
}