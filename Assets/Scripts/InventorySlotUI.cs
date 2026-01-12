using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI component for displaying an inventory slot
/// </summary>
public class InventorySlotUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image itemIcon;
    public TextMeshProUGUI quantityText;
    public Image background;
    
    [Header("Colors")]
    public Color emptyColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    public Color filledColor = new Color(0.4f, 0.4f, 0.4f, 0.8f);
    
    private InventorySlot slot;
    
    public void UpdateSlot(InventorySlot inventorySlot)
    {
        slot = inventorySlot;
        
        if (slot == null || slot.IsEmpty)
        {
            // Empty slot
            itemIcon.enabled = false;
            quantityText.text = "";
            if (background != null)
                background.color = emptyColor;
        }
        else
        {
            // Filled slot
            itemIcon.enabled = true;
            itemIcon.sprite = slot.item.icon;
            quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";
            if (background != null)
                background.color = filledColor;
        }
    }
    
    public InventorySlot GetSlot()
    {
        return slot;
    }
}
