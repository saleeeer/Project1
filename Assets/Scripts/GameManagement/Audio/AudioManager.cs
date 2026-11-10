
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music Clips")]
    public AudioClip menuMusic;
    public AudioClip gameplayMusic;

    [Header("SFX Clips")]
    public AudioClip buttonClick;
    public AudioClip shipSpawn;
    public AudioClip shipDestroyed;
    public AudioClip planetCaptured;
    public AudioClip overdriveSound;
    public AudioClip evadeSound;
    public AudioClip energyShieldSound;
    public AudioClip battleAuraSound;
    public AudioClip empSound;
    public AudioClip rapidFireSound;
    public AudioClip victorySound;
    public AudioClip defeatSound;
    public AudioClip eventVideoSound;

    [Header("SFX Control")]
    [Range(1, 10)]
    public int maxSimultaneousSFX = 4;

    [Range(0f, 1f)]
    public float sfxVolumeMultiplier = 0.7f;

    [Range(0f, 1f)]
    public float abilitySoundCooldown = 0.5f;

    private readonly Dictionary<AudioClip, float> lastPlayedTimes =
        new Dictionary<AudioClip, float>();

    private int activeSFX = 0;

    [Header("Volume")]
    [Range(0f, 1f)]
    public float musicVolume = 1f;

    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
    }

    void Start()
    {
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "Menu":
                PlayMusic(menuMusic);
                break;

            case "Level":
                PlayMusic(gameplayMusic);
                break;
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null)
            return;

        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
            return;

        // Evita repetir el mismo sonido demasiado seguido.
        if (lastPlayedTimes.ContainsKey(clip))
        {
            float elapsed = Time.time - lastPlayedTimes[clip];

            if (elapsed < abilitySoundCooldown)
                return;
        }

        // Limita la cantidad de sonidos simultáneos.
        if (activeSFX >= maxSimultaneousSFX)
            return;

        lastPlayedTimes[clip] = Time.time;

        activeSFX++;

        sfxSource.PlayOneShot(
            clip,
            sfxVolumeMultiplier
        );

        StartCoroutine(ReleaseSFXSlot(clip.length));
    }

    private System.Collections.IEnumerator ReleaseSFXSlot(float duration)
    {
        yield return new WaitForSeconds(duration);

        activeSFX = Mathf.Max(0, activeSFX - 1);
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;

        musicSource.volume = volume;

        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;

        sfxSource.volume = volume;

        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }
}
