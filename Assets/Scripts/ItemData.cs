using UnityEngine;

/// <summary>
/// ScriptableObject for defining items that can exist in the game
/// Create via: Assets > Create > Game > Item
/// </summary>
[CreateAssetMenu(fileName = "New Item", menuName = "Game/Item", order = 1)]
public class ItemData : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    public string itemID;
    [TextArea(3, 5)]
    public string description;
    
    [Header("Visual")]
    public Sprite icon;
    
    [Header("Properties")]
    public Item.ItemType itemType;
    public int maxStackSize = 99;
    public bool isConsumable = false;
    
    /// <summary>
    /// Converts this ScriptableObject to an Item instance
    /// </summary>
    public Item ToItem()
    {
        return new Item
        {
            itemName = this.itemName,
            itemID = this.itemID,
            icon = this.icon,
            maxStackSize = this.maxStackSize,
            type = this.itemType
        };
    }
}
