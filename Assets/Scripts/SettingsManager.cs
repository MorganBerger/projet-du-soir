using UnityEngine;

/// <summary>
/// Manages game settings and player preferences
/// Uses PlayerPrefs for persistence
/// </summary>
public class SettingsManager : MonoBehaviour
{
    [Header("Default Settings")]
    public float defaultMusicVolume = 0.7f;
    public float defaultSFXVolume = 1f;
    public bool defaultFullscreen = true;
    public int defaultResolutionIndex = 0;
    
    private static SettingsManager instance;
    
    public static SettingsManager Instance
    {
        get { return instance; }
    }
    
    // Settings keys
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";
    private const string FULLSCREEN_KEY = "Fullscreen";
    private const string RESOLUTION_INDEX_KEY = "ResolutionIndex";
    private const string MASTER_VOLUME_KEY = "MasterVolume";
    
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
        
        LoadSettings();
    }
    
    /// <summary>
    /// Loads all settings from PlayerPrefs
    /// </summary>
    public void LoadSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, defaultMusicVolume);
        float sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, defaultSFXVolume);
        bool fullscreen = PlayerPrefs.GetInt(FULLSCREEN_KEY, defaultFullscreen ? 1 : 0) == 1;
        int resolutionIndex = PlayerPrefs.GetInt(RESOLUTION_INDEX_KEY, defaultResolutionIndex);
        
        ApplySettings(musicVolume, sfxVolume, fullscreen, resolutionIndex);
    }
    
    /// <summary>
    /// Applies settings to the game
    /// </summary>
    void ApplySettings(float musicVolume, float sfxVolume, bool fullscreen, int resolutionIndex)
    {
        // Apply audio settings
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(musicVolume);
            AudioManager.Instance.SetSFXVolume(sfxVolume);
        }
        
        // Apply graphics settings
        Screen.fullScreen = fullscreen;
        
        // Apply resolution if valid
        if (resolutionIndex >= 0 && resolutionIndex < Screen.resolutions.Length)
        {
            Resolution res = Screen.resolutions[resolutionIndex];
            Screen.SetResolution(res.width, res.height, fullscreen);
        }
    }
    
    /// <summary>
    /// Saves current settings to PlayerPrefs
    /// </summary>
    public void SaveSettings()
    {
        if (AudioManager.Instance != null)
        {
            PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, AudioManager.Instance.musicVolume);
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, AudioManager.Instance.sfxVolume);
        }
        
        PlayerPrefs.SetInt(FULLSCREEN_KEY, Screen.fullScreen ? 1 : 0);
        PlayerPrefs.Save();
        
        Debug.Log("Settings saved");
    }
    
    /// <summary>
    /// Sets and saves music volume
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(volume);
        }
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, volume);
    }
    
    /// <summary>
    /// Sets and saves SFX volume
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(volume);
        }
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
    }
    
    /// <summary>
    /// Sets and saves fullscreen mode
    /// </summary>
    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
        PlayerPrefs.SetInt(FULLSCREEN_KEY, fullscreen ? 1 : 0);
    }
    
    /// <summary>
    /// Sets and saves resolution
    /// </summary>
    public void SetResolution(int resolutionIndex)
    {
        if (resolutionIndex >= 0 && resolutionIndex < Screen.resolutions.Length)
        {
            Resolution res = Screen.resolutions[resolutionIndex];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);
            PlayerPrefs.SetInt(RESOLUTION_INDEX_KEY, resolutionIndex);
        }
    }
    
    /// <summary>
    /// Resets all settings to defaults
    /// </summary>
    public void ResetToDefaults()
    {
        PlayerPrefs.DeleteAll();
        ApplySettings(defaultMusicVolume, defaultSFXVolume, defaultFullscreen, defaultResolutionIndex);
        SaveSettings();
        
        Debug.Log("Settings reset to defaults");
    }
    
    /// <summary>
    /// Gets current music volume
    /// </summary>
    public float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, defaultMusicVolume);
    }
    
    /// <summary>
    /// Gets current SFX volume
    /// </summary>
    public float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat(SFX_VOLUME_KEY, defaultSFXVolume);
    }
    
    /// <summary>
    /// Gets current fullscreen setting
    /// </summary>
    public bool GetFullscreen()
    {
        return PlayerPrefs.GetInt(FULLSCREEN_KEY, defaultFullscreen ? 1 : 0) == 1;
    }
}
