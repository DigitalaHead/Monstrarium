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
    public AudioClip CollectedPotion;
    public AudioClip CollectedEssence;

    [Header("Random Sounds")]
    public AudioClip[] randomSounds; // �������� ���� 4 ����� ����� ���������
    public float soundInterval = 15f; // �������� � ��������

    private Coroutine soundRoutine;

    private void Start()
    {
        // ��������� ��������� (�������� �� ��������� 0.7f ���� ��� �����������)
        MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.7f);

        // ��������� ���������
        musicSource.volume = MusicSlider.value;
        SFXSource.volume = SFXSlider.value;

        musicSource.clip = background;
        musicSource.Play();

        // ��������� ��������������� ��������� ������
        soundRoutine = StartCoroutine(PlayRandomSoundsRoutine());
    }

    private IEnumerator PlayRandomSoundsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(soundInterval);

            if (randomSounds != null && randomSounds.Length > 0)
            {
                // �������� ��������� ����
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

    // ��� ��������� ��������� �� ����� ����
    public void SetSoundInterval(float newInterval)
    {
        soundInterval = newInterval;

        // ������������� �������� � ����� ����������
        if (soundRoutine != null)
        {
            StopCoroutine(soundRoutine);
        }
        soundRoutine = StartCoroutine(PlayRandomSoundsRoutine());
    }

}
