using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Simple main menu UI controller
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject multiplayerPanel;
    public GameObject settingsPanel;
    
    [Header("UI Elements")]
    public TMP_InputField serverIPInput;
    public TextMeshProUGUI statusText;
    
    void Start()
    {
        ShowMainMenu();
    }
    
    public void ShowMainMenu()
    {
        SetActivePanel(mainMenuPanel);
    }
    
    public void ShowMultiplayerMenu()
    {
        SetActivePanel(multiplayerPanel);
    }
    
    public void ShowSettings()
    {
        SetActivePanel(settingsPanel);
    }
    
    void SetActivePanel(GameObject panel)
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (multiplayerPanel != null) multiplayerPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        
        if (panel != null) panel.SetActive(true);
    }
    
    // Menu Actions
    public void StartSinglePlayer()
    {
        Debug.Log("Starting single player game...");
        // Load game scene
        SceneManager.LoadScene("MainGame");
    }
    
    public void HostMultiplayerGame()
    {
        Debug.Log("Hosting multiplayer game...");
        
        NetworkGameManager networkManager = FindObjectOfType<NetworkGameManager>();
        if (networkManager != null)
        {
            networkManager.StartHost();
            UpdateStatus("Hosting game...");
        }
        else
        {
            UpdateStatus("Error: NetworkManager not found!");
        }
    }
    
    public void JoinMultiplayerGame()
    {
        Debug.Log("Joining multiplayer game...");
        
        NetworkGameManager networkManager = FindObjectOfType<NetworkGameManager>();
        if (networkManager != null)
        {
            // You might want to set server IP here if needed
            networkManager.StartClient();
            UpdateStatus("Connecting to game...");
        }
        else
        {
            UpdateStatus("Error: NetworkManager not found!");
        }
    }
    
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    
    void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
        Debug.Log(message);
    }
}
