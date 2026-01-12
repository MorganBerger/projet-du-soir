using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Manages the inventory UI display
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject inventoryPanel;
    public Transform slotsContainer;
    public GameObject slotPrefab;
    
    [Header("Settings")]
    public KeyCode toggleKey = KeyCode.I;
    
    private InventorySystem inventorySystem;
    private List<InventorySlotUI> slotUIElements = new List<InventorySlotUI>();
    private bool isOpen = false;
    
    void Start()
    {
        // Find the player's inventory system
        inventorySystem = FindObjectOfType<InventorySystem>();
        
        if (inventorySystem == null)
        {
            Debug.LogWarning("No InventorySystem found in scene!");
            return;
        }
        
        // Subscribe to inventory changes
        inventorySystem.OnInventoryChanged += UpdateUI;
        
        // Initialize UI slots
        InitializeSlots();
        
        // Hide inventory by default
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
    }
    
    void OnDestroy()
    {
        if (inventorySystem != null)
        {
            inventorySystem.OnInventoryChanged -= UpdateUI;
        }
    }
    
    void Update()
    {
        // Toggle inventory with key
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleInventory();
        }
    }
    
    void InitializeSlots()
    {
        if (inventorySystem == null || slotsContainer == null || slotPrefab == null)
            return;
        
        // Clear existing slots
        foreach (var slot in slotUIElements)
        {
            if (slot != null)
                Destroy(slot.gameObject);
        }
        slotUIElements.Clear();
        
        // Create UI slots for each inventory slot
        var inventory = inventorySystem.GetInventory();
        foreach (var slot in inventory)
        {
            GameObject slotObject = Instantiate(slotPrefab, slotsContainer);
            InventorySlotUI slotUI = slotObject.GetComponent<InventorySlotUI>();
            
            if (slotUI != null)
            {
                slotUI.UpdateSlot(slot);
                slotUIElements.Add(slotUI);
            }
        }
    }
    
    void UpdateUI()
    {
        if (inventorySystem == null)
            return;
        
        var inventory = inventorySystem.GetInventory();
        
        for (int i = 0; i < slotUIElements.Count && i < inventory.Count; i++)
        {
            slotUIElements[i].UpdateSlot(inventory[i]);
        }
    }
    
    public void ToggleInventory()
    {
        isOpen = !isOpen;
        
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(isOpen);
            
            // Update UI when opening
            if (isOpen)
                UpdateUI();
        }
    }
    
    public void OpenInventory()
    {
        isOpen = true;
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
            UpdateUI();
        }
    }
    
    public void CloseInventory()
    {
        isOpen = false;
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
    }
}
