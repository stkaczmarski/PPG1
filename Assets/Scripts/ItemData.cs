using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemPickup.PickupType type;
    public int amount = 10; // Ile leczy/ile amunicji
    [TextArea] public string description;
}