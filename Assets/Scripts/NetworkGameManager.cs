using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Manages multiplayer game sessions
/// </summary>
public class NetworkGameManager : MonoBehaviour
{
    [Header("Player Spawning")]
    public GameObject playerPrefab;
    public Transform[] spawnPoints;
    public float spawnRadius = 5f;
    
    private NetworkManager networkManager;
    
    void Awake()
    {
        networkManager = GetComponent<NetworkManager>();
        
        if (networkManager == null)
        {
            Debug.LogError("NetworkManager component not found!");
        }
    }
    
    void Start()
    {
        // Subscribe to network events
        if (networkManager != null)
        {
            networkManager.OnServerStarted += OnServerStarted;
            networkManager.OnClientConnectedCallback += OnClientConnected;
            networkManager.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (networkManager != null)
        {
            networkManager.OnServerStarted -= OnServerStarted;
            networkManager.OnClientConnectedCallback -= OnClientConnected;
            networkManager.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }
    
    void OnServerStarted()
    {
        Debug.Log("Server started successfully!");
    }
    
    void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client {clientId} connected!");
        
        if (NetworkManager.Singleton.IsServer)
        {
            SpawnPlayerForClient(clientId);
        }
    }
    
    void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Client {clientId} disconnected!");
    }
    
    void SpawnPlayerForClient(ulong clientId)
    {
        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab not assigned!");
            return;
        }
        
        // Get spawn position
        Vector3 spawnPosition = GetSpawnPosition();
        
        // Instantiate player
        GameObject playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        
        // Spawn as network object
        NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.SpawnAsPlayerObject(clientId);
        }
    }
    
    Vector3 GetSpawnPosition()
    {
        // Use spawn points if available
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            return spawnPoints[randomIndex].position;
        }
        
        // Otherwise, spawn in a random position within radius
        Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
        return new Vector3(randomOffset.x, randomOffset.y, 0);
    }
    
    // UI Buttons
    public void StartHost()
    {
        if (networkManager != null)
        {
            networkManager.StartHost();
            Debug.Log("Starting as Host...");
        }
    }
    
    public void StartServer()
    {
        if (networkManager != null)
        {
            networkManager.StartServer();
            Debug.Log("Starting as Server...");
        }
    }
    
    public void StartClient()
    {
        if (networkManager != null)
        {
            networkManager.StartClient();
            Debug.Log("Starting as Client...");
        }
    }
    
    public void Shutdown()
    {
        if (networkManager != null)
        {
            networkManager.Shutdown();
            Debug.Log("Network shutdown");
        }
    }
}
