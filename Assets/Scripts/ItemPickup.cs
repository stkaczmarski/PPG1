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
    public PickupType type;
    public int amount = 10;
    public int cost = 0;

    public bool StartInteract(Transform holdPoint, GameObject player)
    {
        PlayerEconomy economy = player.GetComponent<PlayerEconomy>();

        if(economy == null)
        {
            Debug.LogError("Brak skryptu PlayerEconomy");
            return false;
        }

        if(type == PickupType.Money)
        {
            economy.AddMoney(amount);

            if(SoundManager.Instance.moneyPickupSound != null)
                SoundManager.Instance.moneyPickupSound.Play();

            Destroy(gameObject);
            return false;
        }

        if(economy.TrySpendMoney(cost))
        {
            bool itemUsed = ApplyEffect(player);

            if(itemUsed)
            {
                Destroy(gameObject);
            }
            else
            {
                economy.AddMoney(cost);
                Debug.Log("Pe³ne ¿ycie lub amunicja");
            }
        }
        else
        {
            if (SoundManager.Instance.noMoneySound != null)
                SoundManager.Instance.noMoneySound.Play();
        }

        return false;
    }

    private bool ApplyEffect(GameObject player)
    {
        if(type == PickupType.Health)
        {
            PlayerHealth healthScript = player.GetComponent<PlayerHealth>();
            if (healthScript != null && healthScript.currentHealth < healthScript.maxHealth)
            {
                healthScript.Heal(amount);
                if (SoundManager.Instance.healthPickupSound != null)
                    SoundManager.Instance.healthPickupSound.Play();

                return true;
            }
        }
        else if(type == PickupType.Ammo)
        {
            Weapon weaponToReload = null;
            PlayerMovement movement = player.GetComponent<PlayerMovement>();

            if (movement != null) weaponToReload = movement.weaponScript;

            if (weaponToReload != null && weaponToReload.ammoLeft < 30)
            {
                weaponToReload.ammoLeft += amount;
                if (weaponToReload.ammoLeft > 30)
                    weaponToReload.ammoLeft = 30;

                weaponToReload.UpdateAmmoUI();

                if (SoundManager.Instance.ammoPickupSound != null)
                    SoundManager.Instance.ammoPickupSound.Play();

                Debug.Log("Kupiono amunicjê za " + cost + ". Razem: " + weaponToReload.ammoLeft);
                return true;
            }
        }
        return false;
    }

    public void StopInteract()
    {
        
    }
}