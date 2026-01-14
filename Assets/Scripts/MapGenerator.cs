using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int mapWidth;
    public int mapHeight;
    public int tileSize = 10; // Plans size are 10
    public int tileOffset = 5;
    public float perlinNoiseScale;
    public GameObject map;
    public GameObject tilePrefab;
    public GameObject[] tilesGameObjects;
    public Material waterMaterial;
    public Material dirtMaterial;
    public Material grassMaterial;
    public Material sandMaterial;

    public int[,] tiles;

    void Start()
    {
        tiles = new int[mapWidth, mapHeight];
        map = GameObject.Find("Map");

        if (map == null) 
        {
            Debug.LogWarning("Map game object not found, creating a new one.");
            map = new GameObject("Map");
        }
        GenerateMap();
    }

    void GenerateMap()
    {
        float realWidth = mapWidth * tileSize;
        float realHeight = mapHeight * tileSize;
        
        float mapStartX = -realWidth / 2f;
        float mapStartZ = -realHeight / 2f;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float posX = mapStartX + x * tileSize + tileOffset;
                float posZ = mapStartZ + y * tileSize + tileOffset;
                
                GameObject tile = Instantiate(tilePrefab, new Vector3(posX, 0, posZ), Quaternion.identity);
                tilesGameObjects.Append(tile);
                tile.transform.SetParent(map.transform);
                float noiseValue = GetPerlinNoiseValue(x, y);
                
                // Set material based on noise value
                Material tileMaterial;
                if (noiseValue < 0.2f)
                {
                    tileMaterial = waterMaterial;
                }
                else if (noiseValue < 0.32f)
                {
                    tileMaterial = sandMaterial;
                }
                else if (noiseValue < 0.65f)
                {
                    tileMaterial = grassMaterial;
                }
                else
                {
                    tileMaterial = dirtMaterial;
                }
                
                tile.GetComponent<Renderer>().sharedMaterial = tileMaterial;
            }
        }
    }

    float GetPerlinNoiseValue(int x, int y)
    {
        float xCoord = (float)x / mapWidth * perlinNoiseScale;
        float yCoord = (float)y / mapHeight * perlinNoiseScale;

        float sample = Mathf.PerlinNoise(xCoord, yCoord);
        return sample;
    }

    /**
    * Draw the map grid and borders in the editor
    */
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        float realWidth = mapWidth * tileSize;
        float realHeight = mapHeight * tileSize;

        float mapStartX = -realWidth / 2f;
        float mapStartZ = -realHeight / 2f;

        float mapEndX = mapStartX + realWidth;
        float mapEndZ = mapStartZ + realHeight;

        // Draw border
        Gizmos.DrawLine(new Vector3(mapStartX, 0, mapStartZ), new Vector3(mapEndX, 0, mapStartZ));
        Gizmos.DrawLine(new Vector3(mapStartX, 0, mapStartZ), new Vector3(mapStartX, 0, mapEndZ));

        Gizmos.DrawLine(new Vector3(mapEndX, 0, mapStartZ), new Vector3(mapEndX, 0, mapEndZ));
        Gizmos.DrawLine(new Vector3(mapStartX, 0, mapEndZ), new Vector3(mapEndX, 0, mapEndZ));


        // Draw grid
        for (int x = 0; x <= mapWidth; x++)
        {
            float posX = mapStartX + x * 10f;
            Gizmos.DrawLine(new Vector3(posX, 0, mapStartZ), new Vector3(posX, 0, mapEndZ));
        }
        for (int z = 0; z <= mapHeight; z++)
        {
            float posZ = mapStartZ + z * 10f;
            Gizmos.DrawLine(new Vector3(mapStartX, 0, posZ), new Vector3(mapEndX, 0, posZ));
        }
    }
}
