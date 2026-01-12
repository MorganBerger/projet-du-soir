using UnityEngine;

/// <summary>
/// Handles player interaction with resources (trees, rocks, etc.)
/// </summary>
public class ResourceHarvester : MonoBehaviour
{
    [Header("Harvesting Settings")]
    public float harvestRange = 2f;
    public float harvestCooldown = 0.5f;
    public LayerMask resourceLayer;
    
    [Header("Tools")]
    public bool hasAxe = true;  // For trees
    public bool hasPickaxe = true;  // For rocks
    
    private float lastHarvestTime;
    private PlayerController playerController;
    
    void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }
    
    void Update()
    {
        // Check for harvest input
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            TryHarvest();
        }
    }
    
    void TryHarvest()
    {
        // Check cooldown
        if (Time.time < lastHarvestTime + harvestCooldown)
            return;
        
        // Find nearby resources
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, harvestRange, resourceLayer);
        
        if (hits.Length == 0)
        {
            Debug.Log("No resources in range");
            return;
        }
        
        // Find the closest resource
        Resource closestResource = null;
        float closestDistance = float.MaxValue;
        
        foreach (var hit in hits)
        {
            Resource resource = hit.GetComponent<Resource>();
            if (resource != null)
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestResource = resource;
                }
            }
        }
        
        // Harvest the closest resource
        if (closestResource != null)
        {
            // Check if player has the right tool
            bool canHarvest = false;
            
            switch (closestResource.type)
            {
                case Resource.ResourceType.Tree:
                    canHarvest = hasAxe;
                    if (!canHarvest)
                        Debug.Log("You need an axe to harvest trees!");
                    break;
                    
                case Resource.ResourceType.Rock:
                    canHarvest = hasPickaxe;
                    if (!canHarvest)
                        Debug.Log("You need a pickaxe to harvest rocks!");
                    break;
                    
                case Resource.ResourceType.Bush:
                    canHarvest = true; // Can harvest bushes without tools
                    break;
            }
            
            if (canHarvest)
            {
                closestResource.Harvest(playerController);
                lastHarvestTime = Time.time;
            }
        }
    }
    
    // Visualize harvest range in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, harvestRange);
    }
}
