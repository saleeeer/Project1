using UnityEngine;
using UnityEngine.SceneManagement;

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

        sfxSource.PlayOneShot(clip);
    }
}