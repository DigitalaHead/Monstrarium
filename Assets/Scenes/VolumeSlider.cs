using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class VolumeSlider : MonoBehaviour
{
    public Slider slider;
    public bool isMusicSlider; // True для MusicSlider, False для SFXSlider

    void Start()
    {
        // Load initial volume from PlayerPrefs
        float initialVolume = isMusicSlider ? PlayerPrefs.GetFloat("MusicVolume", 0.2f) : PlayerPrefs.GetFloat("SFXVolume", 0.2f);
        slider.value = initialVolume;

        // Set initial volume on AudioManager (if it exists)
        if (AudioManager.Instance != null)
        {
            if (isMusicSlider)
            {
                AudioManager.Instance.SetMusicVolume(initialVolume);
            }
            else
            {
                AudioManager.Instance.SetSFXVolume(initialVolume);
            }
        }
        else
        {
            Debug.LogWarning("AudioManager not found in the scene.  Make sure it exists and DontDestroyOnLoad is called.");
        }


        // Add listener for value changes
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    public void OnSliderValueChanged(float value)
    {
        // Update volume in AudioManager
        if (AudioManager.Instance != null)
        {
            if (isMusicSlider)
            {
                AudioManager.Instance.SetMusicVolume(value);
            }
            else
            {
                AudioManager.Instance.SetSFXVolume(value);
            }
        }
        else
        {
            Debug.LogWarning("AudioManager not found in the scene.  Make sure it exists and DontDestroyOnLoad is called.");
        }
    }

}
