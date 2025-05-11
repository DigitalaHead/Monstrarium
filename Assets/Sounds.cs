using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class Sounds : MonoBehaviour
{
    public AudioClip[] sounds;

    private AudioSource audioScr => GetComponent<AudioSource>();

    public void PlaySound(AudioClip clip, float volume = 1f, bool destroyed = false, float p1 = 0.85f, float p2 = 1.2f)
    {
        audioScr.pitch = UnityEngine.Random.Range(p1, p2);
        audioScr.PlayOneShot(clip, volume);
    }

    protected void PlayRandomSound()
    {
        AudioClip randomClip = sounds[Random.Range(0, sounds.Length)];
        PlaySound(randomClip);
    }
}
