using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages audio playback for music and sound effects
/// Singleton pattern for easy access from anywhere
/// </summary>
public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    
    [Header("Music Tracks")]
    public AudioClip mainMenuMusic;
    public AudioClip gameplayMusic;
    
    [Header("Sound Effects")]
    public AudioClip buttonClickSound;
    public AudioClip itemPickupSound;
    public AudioClip craftingSound;
    public AudioClip harvestSound;
    
    [Header("Settings")]
    [Range(0f, 1f)] public float musicVolume = 0.7f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    
    private static AudioManager instance;
    
    public static AudioManager Instance
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
        
        // Create audio sources if not assigned
        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("Music");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }
        
        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFX");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
        
        UpdateVolumes();
    }
    
    void Start()
    {
        // Start with gameplay music if in game scene
        if (gameplayMusic != null)
        {
            PlayMusic(gameplayMusic);
        }
    }
    
    /// <summary>
    /// Plays a music track, replacing the current one
    /// </summary>
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null) return;
        
        if (musicSource.clip == clip && musicSource.isPlaying)
            return;
        
        musicSource.clip = clip;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }
    
    /// <summary>
    /// Stops the currently playing music
    /// </summary>
    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }
    
    /// <summary>
    /// Plays a sound effect once
    /// </summary>
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        
        sfxSource.PlayOneShot(clip, sfxVolume);
    }
    
    /// <summary>
    /// Plays a sound effect at a specific position in 3D space
    /// </summary>
    public void PlaySFXAtPosition(AudioClip clip, Vector3 position)
    {
        if (clip == null) return;
        
        AudioSource.PlayClipAtPoint(clip, position, sfxVolume);
    }
    
    /// <summary>
    /// Sets the music volume
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }
    
    /// <summary>
    /// Sets the sound effects volume
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }
    
    /// <summary>
    /// Updates both audio sources with current volume settings
    /// </summary>
    public void UpdateVolumes()
    {
        if (musicSource != null)
            musicSource.volume = musicVolume;
        if (sfxSource != null)
            sfxSource.volume = sfxVolume;
    }
    
    /// <summary>
    /// Mutes or unmutes all audio
    /// </summary>
    public void SetMute(bool mute)
    {
        if (musicSource != null)
            musicSource.mute = mute;
        if (sfxSource != null)
            sfxSource.mute = mute;
    }
    
    // Convenience methods for common sounds
    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSound);
    }
    
    public void PlayItemPickup()
    {
        PlaySFX(itemPickupSound);
    }
    
    public void PlayCrafting()
    {
        PlaySFX(craftingSound);
    }
    
    public void PlayHarvest()
    {
        PlaySFX(harvestSound);
    }
}
