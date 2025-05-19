using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    public Slider MusicSlider;
    public Slider SFXSlider;

    public AudioClip background;
    public AudioClip deathPlayer;
    public AudioClip deathEnemy;
    public AudioClip сollectedPotion;
    public AudioClip сollectedEssence;

    [Header("Random Sounds")]
    public AudioClip[] randomSounds; // Добавьте сюда 4 звука через инспектор
    public float soundInterval = 15f; // Интервал в секундах

    private Coroutine soundRoutine;

    private void Start()
    {
        // Загружаем настройки (значение по умолчанию 0.7f если нет сохраненных)
        MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.7f);

        // Применяем настройки
        musicSource.volume = MusicSlider.value;
        SFXSource.volume = SFXSlider.value;

        musicSource.clip = background;
        musicSource.Play();

        // Запускаем воспроизведение случайных звуков
        soundRoutine = StartCoroutine(PlayRandomSoundsRoutine());
    }

    private IEnumerator PlayRandomSoundsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(soundInterval);

            if (randomSounds != null && randomSounds.Length > 0)
            {
                // Выбираем случайный звук
                AudioClip randomClip = randomSounds[UnityEngine.Random.Range(0, randomSounds.Length)];
                PlaySFX(randomClip);
            }
        }
    }

    void Update()
    {
        if (musicSource.volume != MusicSlider.value)
        {
            musicSource.volume = MusicSlider.value;
            PlayerPrefs.SetFloat("MusicVolume", MusicSlider.value);
        }

        if (SFXSource.volume != SFXSlider.value)
        {
            SFXSource.volume = SFXSlider.value;
            PlayerPrefs.SetFloat("SFXVolume", SFXSlider.value);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            SFXSource.PlayOneShot(clip);
        }
    }

    // Для изменения интервала во время игры
    public void SetSoundInterval(float newInterval)
    {
        soundInterval = newInterval;

        // Перезапускаем корутину с новым интервалом
        if (soundRoutine != null)
        {
            StopCoroutine(soundRoutine);
        }
        soundRoutine = StartCoroutine(PlayRandomSoundsRoutine());
    }

}
