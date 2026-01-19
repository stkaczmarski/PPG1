using Unity.VisualScripting;
using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    public enum PickupType
    {
        Health,
        Ammo,
        Money
    }

    [Header("Settings")]
    public ItemData itemData;
    public PickupType type;
    public int amount = 10;
    public int cost = 0;

    public bool StartInteract(Transform holdPoint, GameObject player)
    {
        PlayerEconomy economy = player.GetComponent<PlayerEconomy>();
        if (economy == null) return false;

        if (type == PickupType.Money)
        {
            economy.AddMoney(amount);
            if (SoundManager.Instance.moneyPickupSound != null)
                SoundManager.Instance.moneyPickupSound.Play();
            Destroy(gameObject);
            return false;
        }

        if (economy.TrySpendMoney(cost))
        {
            bool addedToInventory = InventoryManager.Instance.AddItem(itemData);

            if (addedToInventory)
            {
                Debug.Log("Dodano " + itemData.itemName + " do ekwipunku.");
                Destroy(gameObject);
            }
            else
            {
                economy.AddMoney(cost);
                Debug.Log("Ekwipunek pe³ny!");
            }
        }
        else
        {
            if (SoundManager.Instance.noMoneySound != null)
                SoundManager.Instance.noMoneySound.Play();
        }

        return false;
    }

    public void StopInteract() { }
}