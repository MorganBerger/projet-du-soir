using UnityEngine;

/// <summary>
/// Represents a harvestable resource (tree, rock, etc.)
/// </summary>
public class Resource : MonoBehaviour
{
    [Header("Resource Settings")]
    public string resourceName;
    public ResourceType type;
    public Item dropItem;
    public int dropQuantityMin = 1;
    public int dropQuantityMax = 3;
    public int maxHealth = 3;
    
    [Header("Visual Feedback")]
    public GameObject hitEffect;
    public AudioClip hitSound;
    
    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private InventorySystem cachedPlayerInventory;
    
    public enum ResourceType
    {
        Tree,
        Rock,
        Bush
    }
    
    void Awake()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }
    
    /// <summary>
    /// Called when the resource is harvested
    /// </summary>
    public void Harvest(PlayerController player)
    {
        currentHealth--;
        
        // Visual feedback
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }
        
        // Audio feedback
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
        
        // Damage feedback (flash sprite)
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashSprite());
        }
        
        // Check if depleted
        if (currentHealth <= 0)
        {
            DropResources(player);
            Destroy(gameObject, 0.1f);
        }
    }
    
    void DropResources(PlayerController player)
    {
        if (dropItem == null || player == null) return;
        
        int dropQuantity = Random.Range(dropQuantityMin, dropQuantityMax + 1);
        
        // Cache inventory reference if not already cached
        if (cachedPlayerInventory == null)
        {
            cachedPlayerInventory = player.GetComponent<InventorySystem>();
        }
        
        // Try to add to player inventory
        if (cachedPlayerInventory != null)
        {
            bool success = cachedPlayerInventory.AddItem(dropItem, dropQuantity);
            if (success)
            {
                Debug.Log($"Collected {dropQuantity}x {dropItem.itemName}");
            }
            else
            {
                Debug.Log($"Inventory full! Could not collect {dropItem.itemName}");
            }
        }
    }
    
    System.Collections.IEnumerator FlashSprite()
    {
        if (spriteRenderer == null) yield break;
        
        Color original = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = original;
    }
}
