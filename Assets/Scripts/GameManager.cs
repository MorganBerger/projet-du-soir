using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Main game manager that coordinates all game systems
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("References")]
    public ProceduralMapGenerator mapGenerator;
    public GameObject playerPrefab;
    
    [Header("Settings")]
    public bool generateMapOnStart = true;
    public bool spawnPlayerOnStart = true;
    public Vector3 playerSpawnPosition = Vector3.zero;
    
    private static GameManager instance;
    private GameObject currentPlayer;
    
    public static GameManager Instance
    {
        get { return instance; }
    }
    
    void Awake()
    {
        // Singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    void Start()
    {
        Initialize();
    }
    
    void Initialize()
    {
        // Generate map if requested
        if (generateMapOnStart && mapGenerator != null)
        {
            Debug.Log("Generating procedural map...");
            mapGenerator.GenerateMap();
        }
        
        // Spawn player if requested
        if (spawnPlayerOnStart && playerPrefab != null && currentPlayer == null)
        {
            SpawnPlayer();
        }
    }
    
    public void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab not assigned!");
            return;
        }
        
        // Destroy existing player if any
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
        }
        
        // Instantiate new player
        currentPlayer = Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
        currentPlayer.tag = "Player";
        
        // Setup camera to follow player
        CameraController cam = Camera.main?.GetComponent<CameraController>();
        if (cam != null)
        {
            cam.SetTarget(currentPlayer.transform);
        }
        
        Debug.Log("Player spawned at " + playerSpawnPosition);
    }
    
    public GameObject GetPlayer()
    {
        return currentPlayer;
    }
    
    public void RegenerateMap()
    {
        if (mapGenerator != null)
        {
            Debug.Log("Regenerating map with new seed...");
            mapGenerator.RegenerateMap();
        }
    }
    
    public void RestartGame()
    {
        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
