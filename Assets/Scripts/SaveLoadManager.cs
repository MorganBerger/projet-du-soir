using UnityEngine;
using System.IO;
using System;

/// <summary>
/// Handles saving and loading game state
/// </summary>
[System.Serializable]
public class GameSaveData
{
    public int mapSeed;
    public float playerX;
    public float playerY;
    public string[] inventoryItemIDs;
    public int[] inventoryQuantities;
    public DateTime saveTime;
}

/// <summary>
/// Manages game state persistence
/// </summary>
public class SaveLoadManager : MonoBehaviour
{
    private const string SAVE_FILE_NAME = "gamesave.json";
    
    private static SaveLoadManager instance;
    
    public static SaveLoadManager Instance
    {
        get { return instance; }
    }
    
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    /// <summary>
    /// Gets the full path to the save file
    /// </summary>
    string GetSaveFilePath()
    {
        return Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
    }
    
    /// <summary>
    /// Saves the current game state
    /// </summary>
    public bool SaveGame()
    {
        try
        {
            GameSaveData saveData = new GameSaveData();
            
            // Save map seed
            ProceduralMapGenerator mapGen = FindObjectOfType<ProceduralMapGenerator>();
            if (mapGen != null)
            {
                saveData.mapSeed = mapGen.seed;
            }
            
            // Save player position
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                saveData.playerX = player.transform.position.x;
                saveData.playerY = player.transform.position.y;
                
                // Save inventory
                InventorySystem inventory = player.GetComponent<InventorySystem>();
                if (inventory != null)
                {
                    var slots = inventory.GetInventory();
                    saveData.inventoryItemIDs = new string[slots.Count];
                    saveData.inventoryQuantities = new int[slots.Count];
                    
                    for (int i = 0; i < slots.Count; i++)
                    {
                        if (!slots[i].IsEmpty)
                        {
                            saveData.inventoryItemIDs[i] = slots[i].item.itemID;
                            saveData.inventoryQuantities[i] = slots[i].quantity;
                        }
                        else
                        {
                            saveData.inventoryItemIDs[i] = "";
                            saveData.inventoryQuantities[i] = 0;
                        }
                    }
                }
            }
            
            saveData.saveTime = DateTime.Now;
            
            // Convert to JSON
            string json = JsonUtility.ToJson(saveData, true);
            
            // Write to file
            File.WriteAllText(GetSaveFilePath(), json);
            
            Debug.Log($"Game saved successfully to {GetSaveFilePath()}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save game: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Loads the saved game state
    /// </summary>
    public bool LoadGame()
    {
        try
        {
            string filePath = GetSaveFilePath();
            
            if (!File.Exists(filePath))
            {
                Debug.LogWarning("No save file found");
                return false;
            }
            
            // Read JSON
            string json = File.ReadAllText(filePath);
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);
            
            // Load map with saved seed
            ProceduralMapGenerator mapGen = FindObjectOfType<ProceduralMapGenerator>();
            if (mapGen != null)
            {
                mapGen.RegenerateMap(saveData.mapSeed);
            }
            
            // Load player position
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = new Vector3(saveData.playerX, saveData.playerY, 0);
                
                // Load inventory
                InventorySystem inventory = player.GetComponent<InventorySystem>();
                if (inventory != null && saveData.inventoryItemIDs != null)
                {
                    inventory.ClearInventory();
                    
                    // Note: This requires a way to get Item references from IDs
                    // You would need an ItemDatabase or ResourceManager
                    // For now, this is a placeholder
                    
                    Debug.Log("Inventory loading requires ItemDatabase implementation");
                }
            }
            
            Debug.Log($"Game loaded successfully from {filePath}");
            Debug.Log($"Save date: {saveData.saveTime}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load game: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Checks if a save file exists
    /// </summary>
    public bool SaveFileExists()
    {
        return File.Exists(GetSaveFilePath());
    }
    
    /// <summary>
    /// Deletes the save file
    /// </summary>
    public bool DeleteSave()
    {
        try
        {
            string filePath = GetSaveFilePath();
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log("Save file deleted");
                return true;
            }
            
            return false;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to delete save: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Gets information about the save file
    /// </summary>
    public string GetSaveInfo()
    {
        try
        {
            string filePath = GetSaveFilePath();
            
            if (!File.Exists(filePath))
                return "No save file found";
            
            string json = File.ReadAllText(filePath);
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);
            
            return $"Save Date: {saveData.saveTime}\nMap Seed: {saveData.mapSeed}";
        }
        catch
        {
            return "Error reading save file";
        }
    }
}
