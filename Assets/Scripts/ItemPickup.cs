using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    public enum PickupType
    {
        Health,
        Ammo
    }

    [Header("Settings")]
    public PickupType type;
    public int amount = 10;
    public Weapon weaponScript;

    public bool StartInteract(Transform holdPoint, GameObject player)
    {
        bool wasUsed = false;

        if (type == PickupType.Health)
        {
            PlayerHealth healthScript = player.GetComponent<PlayerHealth>();
            if (healthScript != null)
            {
                if (healthScript.currentHealth < healthScript.maxHealth)
                {
                    healthScript.Heal(amount);
                    if (SoundManager.Instance.healthPickupSound != null)
                        SoundManager.Instance.healthPickupSound.Play();

                    wasUsed = true;
                }
                else
                {
                    Debug.Log("Nie mo¿na podnieœæ z pe³nym ¿yciem.");
                }
            }
        }
        else if (type == PickupType.Ammo)
        {
            Weapon weaponToReload = null;
            PlayerMovement movement = player.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                weaponToReload = movement.weaponScript;
            }
            if (weaponToReload != null && weaponToReload.ammoLeft < 30)
            {
                weaponToReload.ammoLeft += amount;
                if (weaponToReload.ammoLeft > 30)
                    weaponToReload.ammoLeft = 30;

                weaponToReload.UpdateAmmoUI();

                if (SoundManager.Instance.ammoPickupSound != null)
                    SoundManager.Instance.ammoPickupSound.Play();

                wasUsed = true;
                Debug.Log("Podniesiono amunicjê. Razem: " + weaponToReload.ammoLeft);
            }
            else
            {
                Debug.Log("Nie znaleziono skryptu Weapon!");
            }
        }

        if (wasUsed)
        {
            Destroy(gameObject);
        }

        return false;
    }

    public void StopInteract()
    {
        
    }
}