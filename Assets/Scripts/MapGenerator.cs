using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int mapWidth;
    public int mapHeight;

    public int[,] tiles;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tiles = new int[mapWidth, mapHeight];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        float realWidth = mapWidth * 10f;
        float realHeight = mapHeight * 10f;

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
