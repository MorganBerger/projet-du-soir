using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.IO;

/// <summary>
/// Editor utility to create placeholder tiles and sprites for the game
/// Access via: Tools > Game Setup > Create Placeholder Assets
/// </summary>
public class AssetCreatorEditor : EditorWindow
{
    private int tileSize = 32;
    private int resourceSize = 64;
    private int playerSize = 64;
    
    [MenuItem("Tools/Game Setup/Create Placeholder Assets")]
    static void ShowWindow()
    {
        AssetCreatorEditor window = GetWindow<AssetCreatorEditor>("Asset Creator");
        window.Show();
    }
    
    void OnGUI()
    {
        GUILayout.Label("Placeholder Asset Creator", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        GUILayout.Label("Sprite Sizes:", EditorStyles.label);
        tileSize = EditorGUILayout.IntField("Tile Size", tileSize);
        resourceSize = EditorGUILayout.IntField("Resource Size", resourceSize);
        playerSize = EditorGUILayout.IntField("Player Size", playerSize);
        
        GUILayout.Space(20);
        
        if (GUILayout.Button("Create All Assets", GUILayout.Height(40)))
        {
            CreateAllAssets();
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Create Tile Assets Only"))
        {
            CreateTileAssets();
        }
        
        if (GUILayout.Button("Create Resource Prefabs Only"))
        {
            CreateResourcePrefabs();
        }
        
        if (GUILayout.Button("Create Player Prefab Only"))
        {
            CreatePlayerPrefab();
        }
    }
    
    void CreateAllAssets()
    {
        CreateTileAssets();
        CreateResourcePrefabs();
        CreatePlayerPrefab();
        
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Success", "All placeholder assets created successfully!", "OK");
    }
    
    void CreateTileAssets()
    {
        // Create directories if they don't exist
        CreateDirectoryIfNotExists("Assets/Sprites");
        CreateDirectoryIfNotExists("Assets/Sprites/Tiles");
        CreateDirectoryIfNotExists("Assets/Tiles");
        
        // Create tile sprites
        CreateAndSaveTileSprite("GrassTile", new Color(0.3f, 0.7f, 0.3f));
        CreateAndSaveTileSprite("WaterTile", new Color(0.2f, 0.4f, 0.8f));
        CreateAndSaveTileSprite("DirtTile", new Color(0.5f, 0.35f, 0.2f));
        CreateAndSaveTileSprite("SandTile", new Color(0.9f, 0.8f, 0.5f));
        
        AssetDatabase.Refresh();
        
        // Create Tile assets from sprites
        CreateTileFromSprite("Assets/Sprites/Tiles/GrassTile.png", "Assets/Tiles/GrassTile.asset");
        CreateTileFromSprite("Assets/Sprites/Tiles/WaterTile.png", "Assets/Tiles/WaterTile.asset");
        CreateTileFromSprite("Assets/Sprites/Tiles/DirtTile.png", "Assets/Tiles/DirtTile.asset");
        CreateTileFromSprite("Assets/Sprites/Tiles/SandTile.png", "Assets/Tiles/SandTile.asset");
        
        Debug.Log("Tile assets created successfully!");
    }
    
    void CreateAndSaveTileSprite(string name, Color color)
    {
        Texture2D texture = CreateColorTexture(tileSize, tileSize, color);
        byte[] bytes = texture.EncodeToPNG();
        string path = $"Assets/Sprites/Tiles/{name}.png";
        
        File.WriteAllBytes(path, bytes);
        AssetDatabase.ImportAsset(path);
        
        // Configure as sprite
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = tileSize;
            importer.filterMode = FilterMode.Point;
            importer.SaveAndReimport();
        }
    }
    
    void CreateTileFromSprite(string spritePath, string tilePath)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        if (sprite != null)
        {
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;
            AssetDatabase.CreateAsset(tile, tilePath);
        }
    }
    
    void CreateResourcePrefabs()
    {
        CreateDirectoryIfNotExists("Assets/Prefabs");
        CreateDirectoryIfNotExists("Assets/Sprites/Resources");
        
        // Create Tree
        CreateAndSaveSprite("Tree", SpriteGenerator.CreateTreeSprite(resourceSize), "Assets/Sprites/Resources");
        CreateResourcePrefab("Tree", Resource.ResourceType.Tree, 3, 2, 5);
        
        // Create Rock
        CreateAndSaveSprite("Rock", SpriteGenerator.CreateRockSprite(resourceSize), "Assets/Sprites/Resources");
        CreateResourcePrefab("Rock", Resource.ResourceType.Rock, 5, 1, 3);
        
        Debug.Log("Resource prefabs created successfully!");
    }
    
    void CreateResourcePrefab(string name, Resource.ResourceType type, int health, int minDrop, int maxDrop)
    {
        GameObject obj = new GameObject(name);
        
        // Sprite
        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Sprites/Resources/{name}.png");
        sr.sortingLayerName = "Resources";
        
        // Collider
        CircleCollider2D collider = obj.AddComponent<CircleCollider2D>();
        collider.radius = 0.3f;
        
        // Resource component
        Resource resource = obj.AddComponent<Resource>();
        resource.resourceName = name;
        resource.type = type;
        resource.maxHealth = health;
        resource.dropQuantityMin = minDrop;
        resource.dropQuantityMax = maxDrop;
        
        obj.layer = LayerMask.NameToLayer("Resource");
        
        // Save as prefab
        string prefabPath = $"Assets/Prefabs/{name}.prefab";
        PrefabUtility.SaveAsPrefabAsset(obj, prefabPath);
        DestroyImmediate(obj);
    }
    
    void CreatePlayerPrefab()
    {
        CreateDirectoryIfNotExists("Assets/Prefabs");
        CreateDirectoryIfNotExists("Assets/Sprites/Characters");
        
        // Create Player sprite
        CreateAndSaveSprite("Player", SpriteGenerator.CreatePlayerSprite(playerSize), "Assets/Sprites/Characters");
        
        // Create Player prefab
        GameObject player = new GameObject("Player");
        player.tag = "Player";
        player.layer = LayerMask.NameToLayer("Player");
        
        // Sprite
        SpriteRenderer sr = player.AddComponent<SpriteRenderer>();
        sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Characters/Player.png");
        sr.sortingLayerName = "Player";
        
        // Physics
        Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        CircleCollider2D collider = player.AddComponent<CircleCollider2D>();
        collider.radius = 0.4f;
        
        // Components
        player.AddComponent<PlayerController>();
        player.AddComponent<ResourceHarvester>();
        player.AddComponent<InventorySystem>();
        player.AddComponent<CraftingSystem>();
        
        // Save prefab
        string prefabPath = "Assets/Prefabs/Player.prefab";
        PrefabUtility.SaveAsPrefabAsset(player, prefabPath);
        DestroyImmediate(player);
        
        Debug.Log("Player prefab created successfully!");
    }
    
    void CreateAndSaveSprite(string name, Sprite sprite, string directory)
    {
        Texture2D texture = sprite.texture;
        byte[] bytes = texture.EncodeToPNG();
        string path = $"{directory}/{name}.png";
        
        File.WriteAllBytes(path, bytes);
        AssetDatabase.ImportAsset(path);
        
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 64;
            importer.filterMode = FilterMode.Point;
            importer.SaveAndReimport();
        }
    }
    
    Texture2D CreateColorTexture(int width, int height, Color color)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];
        
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        return texture;
    }
    
    void CreateDirectoryIfNotExists(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string parentFolder = Path.GetDirectoryName(path);
            string newFolder = Path.GetFileName(path);
            
            if (!string.IsNullOrEmpty(parentFolder) && !AssetDatabase.IsValidFolder(parentFolder))
            {
                CreateDirectoryIfNotExists(parentFolder);
            }
            
            AssetDatabase.CreateFolder(parentFolder, newFolder);
        }
    }
}
