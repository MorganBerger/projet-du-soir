using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Helper script to bootstrap the game with placeholder graphics
/// Useful for testing when no sprite assets are available
/// Attach to a GameObject and run in Play mode to auto-setup
/// </summary>
public class GameBootstrap : MonoBehaviour
{
    [Header("Auto Setup")]
    public bool autoSetupOnStart = true;
    public bool createPlaceholderSprites = true;
    
    [Header("Sprite Sizes")]
    public int tileSize = 32;
    public int resourceSize = 64;
    public int playerSize = 64;
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            BootstrapGame();
        }
    }
    
    [ContextMenu("Bootstrap Game")]
    public void BootstrapGame()
    {
        Debug.Log("Starting game bootstrap...");
        
        if (createPlaceholderSprites)
        {
            CreatePlaceholderAssets();
        }
        
        SetupMapGenerator();
        SetupPlayer();
        SetupCamera();
        
        Debug.Log("Game bootstrap complete!");
    }
    
    void CreatePlaceholderAssets()
    {
        Debug.Log("Creating placeholder sprites...");
        
        // Note: In a real scenario, you'd create these as persistent assets
        // For now, we'll store them in Resources at runtime
        // These will need to be properly saved as assets in the Unity Editor
    }
    
    void SetupMapGenerator()
    {
        ProceduralMapGenerator generator = FindObjectOfType<ProceduralMapGenerator>();
        
        if (generator == null)
        {
            Debug.LogWarning("No ProceduralMapGenerator found. Creating one...");
            GameObject generatorObj = new GameObject("MapGenerator");
            generator = generatorObj.AddComponent<ProceduralMapGenerator>();
        }
        
        // Check if tilemap exists
        if (generator.terrainTilemap == null)
        {
            Tilemap tilemap = FindObjectOfType<Tilemap>();
            
            if (tilemap == null)
            {
                Debug.Log("Creating Grid and Tilemap...");
                
                // Create Grid
                GameObject gridObj = new GameObject("Grid");
                Grid grid = gridObj.AddComponent<Grid>();
                grid.cellSize = new Vector3(1, 1, 0);
                
                // Create Tilemap
                GameObject tilemapObj = new GameObject("Tilemap");
                tilemapObj.transform.SetParent(gridObj.transform);
                tilemap = tilemapObj.AddComponent<Tilemap>();
                TilemapRenderer renderer = tilemapObj.AddComponent<TilemapRenderer>();
                renderer.sortingLayerName = "Ground";
            }
            
            generator.terrainTilemap = tilemap;
        }
        
        // Create resource parent if needed
        if (generator.resourcesParent == null)
        {
            GameObject resourcesObj = new GameObject("Resources");
            generator.resourcesParent = resourcesObj.transform;
        }
        
        // Create placeholder tiles if needed
        if (createPlaceholderSprites)
        {
            CreatePlaceholderTiles(generator);
        }
        
        // Create placeholder resource prefabs
        if (generator.treePrefab == null || generator.rockPrefab == null)
        {
            CreatePlaceholderResourcePrefabs(generator);
        }
        
        Debug.Log("MapGenerator setup complete");
    }
    
    void CreatePlaceholderTiles(ProceduralMapGenerator generator)
    {
        // Create temporary tiles with solid colors
        // Note: These won't persist after play mode
        // In Unity Editor, you'd need to create proper Tile assets
        
        Debug.Log("Creating placeholder tiles (temporary for play mode)...");
        
        // For now, just log that tiles need to be created manually
        // Creating Tile assets requires Editor scripts
        if (generator.grassTile == null)
            Debug.LogWarning("Grass tile not assigned - create Tile assets in Unity Editor");
        if (generator.waterTile == null)
            Debug.LogWarning("Water tile not assigned - create Tile assets in Unity Editor");
        if (generator.dirtTile == null)
            Debug.LogWarning("Dirt tile not assigned - create Tile assets in Unity Editor");
        if (generator.sandTile == null)
            Debug.LogWarning("Sand tile not assigned - create Tile assets in Unity Editor");
    }
    
    void CreatePlaceholderResourcePrefabs(ProceduralMapGenerator generator)
    {
        Debug.Log("Creating placeholder resource prefabs...");
        Debug.Log("WARNING: These are temporary runtime prefabs. Use Asset Creator Editor tool to create persistent prefabs.");
        
        // Create Tree prefab
        if (generator.treePrefab == null)
        {
            GameObject tree = new GameObject("Tree_Placeholder");
            SpriteRenderer sr = tree.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteGenerator.CreateTreeSprite(resourceSize);
            sr.sortingLayerName = "Resources";
            
            CircleCollider2D collider = tree.AddComponent<CircleCollider2D>();
            collider.radius = 0.3f;
            collider.isTrigger = false;
            
            Resource resource = tree.AddComponent<Resource>();
            resource.resourceName = "Tree";
            resource.type = Resource.ResourceType.Tree;
            resource.maxHealth = 3;
            resource.dropQuantityMin = 2;
            resource.dropQuantityMax = 5;
            
            int resourceLayer = LayerMask.NameToLayer("Resource");
            if (resourceLayer != -1)
                tree.layer = resourceLayer;
            else
                Debug.LogWarning("Resource layer not found - please add it in Tag Manager");
                
            tree.SetActive(false); // Will be instantiated by map generator
            
            // Note: This creates a temporary prefab that won't persist
            // In Unity Editor, you'd save this as a proper prefab asset
        }
        
        // Create Rock prefab
        if (generator.rockPrefab == null)
        {
            GameObject rock = new GameObject("Rock_Placeholder");
            SpriteRenderer sr = rock.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteGenerator.CreateRockSprite(resourceSize);
            sr.sortingLayerName = "Resources";
            
            CircleCollider2D collider = rock.AddComponent<CircleCollider2D>();
            collider.radius = 0.3f;
            collider.isTrigger = false;
            
            Resource resource = rock.AddComponent<Resource>();
            resource.resourceName = "Rock";
            resource.type = Resource.ResourceType.Rock;
            resource.maxHealth = 5;
            resource.dropQuantityMin = 1;
            resource.dropQuantityMax = 3;
            
            int resourceLayer = LayerMask.NameToLayer("Resource");
            if (resourceLayer != -1)
                rock.layer = resourceLayer;
            else
                Debug.LogWarning("Resource layer not found - please add it in Tag Manager");
                
            rock.SetActive(false);
        }
    }
    
    void SetupPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player == null)
        {
            Debug.Log("Creating player...");
            
            player = new GameObject("Player");
            player.tag = "Player";
            
            int playerLayer = LayerMask.NameToLayer("Player");
            if (playerLayer != -1)
                player.layer = playerLayer;
            else
                Debug.LogWarning("Player layer not found - please add it in Tag Manager");
            
            // Sprite
            SpriteRenderer sr = player.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteGenerator.CreatePlayerSprite(playerSize);
            sr.sortingLayerName = "Player";
            
            // Physics
            Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            
            CircleCollider2D collider = player.AddComponent<CircleCollider2D>();
            collider.radius = 0.4f;
            
            // Scripts
            player.AddComponent<PlayerController>();
            
            ResourceHarvester harvester = player.AddComponent<ResourceHarvester>();
            harvester.harvestRange = 2f;
            harvester.resourceLayer = LayerMask.GetMask("Resource");
            
            player.AddComponent<InventorySystem>();
            player.AddComponent<CraftingSystem>();
            
            player.transform.position = Vector3.zero;
        }
        
        Debug.Log("Player setup complete");
    }
    
    void SetupCamera()
    {
        Camera mainCam = Camera.main;
        
        if (mainCam != null)
        {
            CameraController camController = mainCam.GetComponent<CameraController>();
            
            if (camController == null)
            {
                Debug.Log("Adding CameraController to Main Camera...");
                camController = mainCam.AddComponent<CameraController>();
            }
            
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                camController.SetTarget(player.transform);
            }
        }
        
        Debug.Log("Camera setup complete");
    }
}
