using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    public AudioClip backgroundGame;
    public AudioClip backgroundMenu;

    public AudioClip deathPlayer;
    public AudioClip deathEnemy;
    public AudioClip CollectedPotion;
    public AudioClip CollectedEssence;

    [Header("Random Sounds")]
    public AudioClip[] randomSounds;
    public float soundInterval = 15f;

    private Coroutine soundRoutine;
    private int currentSceneIndex;

    private static bool _deathSoundPlayed = false; // Статический флаг

    // Singleton Instance
    private static AudioManager _instance;
    public static AudioManager Instance { get { return _instance; } }

    private void Awake()
    {
        // Singleton Pattern implementation
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // Load volumes from PlayerPrefs
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.2f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.2f);

        // Set source volume values
        musicSource.volume = musicVolume;
        SFXSource.volume = sfxVolume;

        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SetMusicForScene(currentSceneIndex);
        ManageRandomSounds();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneIndex = scene.buildIndex;
        SetMusicForScene(currentSceneIndex);
        ManageRandomSounds();
    }

    private void SetMusicForScene(int sceneIndex)
    {
        AudioClip clipToPlay = sceneIndex == 0 ? backgroundMenu : backgroundGame;

        if (musicSource.clip == clipToPlay && musicSource.isPlaying)
            return;

        musicSource.clip = clipToPlay;
        musicSource.loop = true;
        musicSource.Play();
    }

    private void ManageRandomSounds()
    {
        // Останавливаем текущую корутину, если она есть
        if (soundRoutine != null)
        {
            StopCoroutine(soundRoutine);
            soundRoutine = null;
        }

        // Запускаем новую корутину только если это вторая сцена (индекс 1)
        if (currentSceneIndex == 1)
        {
            soundRoutine = StartCoroutine(PlayRandomSoundsRoutine());
        }
    }

    private IEnumerator PlayRandomSoundsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(soundInterval);

            if (randomSounds != null && randomSounds.Length > 0)
            {
                AudioClip randomClip = randomSounds[UnityEngine.Random.Range(0, randomSounds.Length)];
                PlaySFX(randomClip);
            }
        }
    }

    public void SetMusicVolume(float value)
    {
        musicSource.volume = value;
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        SFXSource.volume = value;
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            SFXSource.PlayOneShot(clip);
        }
    }

    public void PlayDeathSound()
    {
        if (!_deathSoundPlayed)
        {
            if (deathPlayer != null)
            {
                PlaySFX(deathPlayer);
                _deathSoundPlayed = true;
            }
            else
            {
                Debug.LogWarning("Звук deathPlayer не найден!");
            }
        }
    }

    // Метод для сброса флага (вызывается, когда игрок возрождается, например)
    public void ResetDeathSoundFlag()
    {
        _deathSoundPlayed = false;
    }

    public void SetSoundInterval(float newInterval)
    {
        soundInterval = newInterval;

        // Перезапускаем корутину только если мы на второй сцене
        if (currentSceneIndex == 1)
        {
            if (soundRoutine != null)
            {
                StopCoroutine(soundRoutine);
            }
            soundRoutine = StartCoroutine(PlayRandomSoundsRoutine());
        }
    }

  




}