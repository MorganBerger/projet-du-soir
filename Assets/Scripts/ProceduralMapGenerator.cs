using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

/// <summary>
/// Generates a procedural 2D map using Perlin noise with trees, rocks, and terrain
/// </summary>
public class ProceduralMapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public int mapWidth = 100;
    public int mapHeight = 100;
    public float noiseScale = 20f;
    public int seed = 0;
    
    [Header("Terrain")]
    public Tilemap terrainTilemap;
    public TileBase grassTile;
    public TileBase waterTile;
    public TileBase dirtTile;
    public TileBase sandTile;
    
    [Header("Resources")]
    public GameObject treePrefab;
    public GameObject rockPrefab;
    public Transform resourcesParent;
    
    [Header("Generation Parameters")]
    [Range(0f, 1f)] public float waterThreshold = 0.3f;
    [Range(0f, 1f)] public float sandThreshold = 0.4f;
    [Range(0f, 1f)] public float grassThreshold = 0.7f;
    [Range(0f, 1f)] public float treeSpawnChance = 0.1f;
    [Range(0f, 1f)] public float rockSpawnChance = 0.05f;
    
    private float[,] noiseMap;
    private List<Vector3Int> spawnablePositions = new List<Vector3Int>();

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        // Initialize random seed
        if (seed == 0)
            seed = Random.Range(0, 10000);
        
        Random.InitState(seed);
        
        // Generate noise map
        noiseMap = GenerateNoiseMap();
        
        // Clear existing tiles and resources
        if (terrainTilemap != null)
            terrainTilemap.ClearAllTiles();
        
        if (resourcesParent != null)
        {
            foreach (Transform child in resourcesParent)
                Destroy(child.gameObject);
        }
        
        spawnablePositions.Clear();
        
        // Generate terrain tiles
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float noiseValue = noiseMap[x, y];
                Vector3Int tilePosition = new Vector3Int(x - mapWidth / 2, y - mapHeight / 2, 0);
                
                TileBase tile = GetTileForNoiseValue(noiseValue);
                if (terrainTilemap != null && tile != null)
                    terrainTilemap.SetTile(tilePosition, tile);
                
                // Track positions where we can spawn resources (not water)
                if (noiseValue > waterThreshold)
                    spawnablePositions.Add(tilePosition);
            }
        }
        
        // Spawn resources
        SpawnResources();
    }

    float[,] GenerateNoiseMap()
    {
        float[,] map = new float[mapWidth, mapHeight];
        float offsetX = Random.Range(-10000f, 10000f);
        float offsetY = Random.Range(-10000f, 10000f);
        
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float sampleX = (x + offsetX) / noiseScale;
                float sampleY = (y + offsetY) / noiseScale;
                
                // Use multiple octaves for more interesting terrain
                float noise = 0f;
                float amplitude = 1f;
                float frequency = 1f;
                
                for (int i = 0; i < 4; i++)
                {
                    noise += Mathf.PerlinNoise(sampleX * frequency, sampleY * frequency) * amplitude;
                    amplitude *= 0.5f;
                    frequency *= 2f;
                }
                
                map[x, y] = Mathf.Clamp01(noise);
            }
        }
        
        return map;
    }

    TileBase GetTileForNoiseValue(float value)
    {
        if (value < waterThreshold)
            return waterTile;
        else if (value < sandThreshold)
            return sandTile;
        else if (value < grassThreshold)
            return grassTile;
        else
            return dirtTile;
    }

    void SpawnResources()
    {
        foreach (Vector3Int pos in spawnablePositions)
        {
            float x = pos.x + mapWidth / 2;
            float y = pos.y + mapHeight / 2;
            
            if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight)
                continue;
            
            float noiseValue = noiseMap[(int)x, (int)y];
            
            // Spawn trees on grass
            if (noiseValue >= sandThreshold && noiseValue < grassThreshold)
            {
                if (Random.value < treeSpawnChance && treePrefab != null)
                {
                    Vector3 worldPos = terrainTilemap.CellToWorld(pos) + new Vector3(0.5f, 0.5f, 0);
                    Instantiate(treePrefab, worldPos, Quaternion.identity, resourcesParent);
                }
            }
            
            // Spawn rocks on dirt/higher ground
            if (noiseValue >= grassThreshold)
            {
                if (Random.value < rockSpawnChance && rockPrefab != null)
                {
                    Vector3 worldPos = terrainTilemap.CellToWorld(pos) + new Vector3(0.5f, 0.5f, 0);
                    Instantiate(rockPrefab, worldPos, Quaternion.identity, resourcesParent);
                }
            }
        }
    }

    // Public method to regenerate map with new seed
    public void RegenerateMap(int newSeed = 0)
    {
        seed = newSeed;
        GenerateMap();
    }
}
