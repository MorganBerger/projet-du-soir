using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents an item that can be stored in inventory
/// </summary>
[System.Serializable]
public class Item
{
    public string itemName;
    public string itemID;
    public Sprite icon;
    public int maxStackSize = 99;
    public ItemType type;
    
    public enum ItemType
    {
        Resource,
        Tool,
        Craftable
    }
}

/// <summary>
/// Represents a slot in the inventory containing an item and quantity
/// </summary>
[System.Serializable]
public class InventorySlot
{
    public Item item;
    public int quantity;
    
    public InventorySlot(Item item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
    
    public bool IsEmpty => item == null || quantity <= 0;
    
    public bool CanAddItem(Item newItem)
    {
        return IsEmpty || (item.itemID == newItem.itemID && quantity < item.maxStackSize);
    }
    
    public int AddItem(Item newItem, int amount)
    {
        if (IsEmpty)
        {
            item = newItem;
            quantity = Mathf.Min(amount, newItem.maxStackSize);
            return amount - quantity;
        }
        else if (item.itemID == newItem.itemID)
        {
            int spaceLeft = item.maxStackSize - quantity;
            int toAdd = Mathf.Min(amount, spaceLeft);
            quantity += toAdd;
            return amount - toAdd;
        }
        return amount;
    }
    
    public void Clear()
    {
        item = null;
        quantity = 0;
    }
}

/// <summary>
/// Manages the player's inventory system
/// </summary>
public class InventorySystem : MonoBehaviour
{
    [Header("Inventory Settings")]
    public int inventorySize = 20;
    
    private List<InventorySlot> inventory;
    
    // Events for UI updates
    public delegate void InventoryChanged();
    public event InventoryChanged OnInventoryChanged;
    
    void Awake()
    {
        InitializeInventory();
    }
    
    void InitializeInventory()
    {
        inventory = new List<InventorySlot>();
        for (int i = 0; i < inventorySize; i++)
        {
            inventory.Add(new InventorySlot(null, 0));
        }
    }
    
    /// <summary>
    /// Adds an item to the inventory
    /// </summary>
    public bool AddItem(Item item, int quantity = 1)
    {
        if (item == null) return false;
        
        int remainingQuantity = quantity;
        
        // First, try to stack with existing items
        foreach (var slot in inventory)
        {
            if (!slot.IsEmpty && slot.item.itemID == item.itemID)
            {
                remainingQuantity = slot.AddItem(item, remainingQuantity);
                if (remainingQuantity <= 0)
                {
                    OnInventoryChanged?.Invoke();
                    return true;
                }
            }
        }
        
        // Then, try to add to empty slots
        foreach (var slot in inventory)
        {
            if (slot.IsEmpty)
            {
                remainingQuantity = slot.AddItem(item, remainingQuantity);
                if (remainingQuantity <= 0)
                {
                    OnInventoryChanged?.Invoke();
                    return true;
                }
            }
        }
        
        // If there's still remaining quantity, inventory is full
        if (remainingQuantity > 0)
        {
            Debug.Log($"Inventory full! Could not add {remainingQuantity} of {item.itemName}");
            OnInventoryChanged?.Invoke();
            return false;
        }
        
        OnInventoryChanged?.Invoke();
        return true;
    }
    
    /// <summary>
    /// Removes an item from the inventory
    /// </summary>
    public bool RemoveItem(string itemID, int quantity = 1)
    {
        int remainingToRemove = quantity;
        
        for (int i = inventory.Count - 1; i >= 0; i--)
        {
            var slot = inventory[i];
            if (!slot.IsEmpty && slot.item.itemID == itemID)
            {
                if (slot.quantity >= remainingToRemove)
                {
                    slot.quantity -= remainingToRemove;
                    if (slot.quantity <= 0)
                        slot.Clear();
                    
                    OnInventoryChanged?.Invoke();
                    return true;
                }
                else
                {
                    remainingToRemove -= slot.quantity;
                    slot.Clear();
                }
            }
        }
        
        OnInventoryChanged?.Invoke();
        return remainingToRemove <= 0;
    }
    
    /// <summary>
    /// Checks if the inventory contains a specific item in the required quantity
    /// </summary>
    public bool HasItem(string itemID, int quantity = 1)
    {
        int count = 0;
        foreach (var slot in inventory)
        {
            if (!slot.IsEmpty && slot.item.itemID == itemID)
            {
                count += slot.quantity;
                if (count >= quantity)
                    return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// Gets the total quantity of an item in the inventory
    /// </summary>
    public int GetItemCount(string itemID)
    {
        int count = 0;
        foreach (var slot in inventory)
        {
            if (!slot.IsEmpty && slot.item.itemID == itemID)
                count += slot.quantity;
        }
        return count;
    }
    
    /// <summary>
    /// Gets the inventory slots (for UI display)
    /// </summary>
    public List<InventorySlot> GetInventory()
    {
        return inventory;
    }
    
    /// <summary>
    /// Clears the entire inventory
    /// </summary>
    public void ClearInventory()
    {
        foreach (var slot in inventory)
        {
            slot.Clear();
        }
        OnInventoryChanged?.Invoke();
    }
}
