using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public CustomSoundEle[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource, moveSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlaySound("CommonBgm", musicSounds, musicSource, false);
    }

    // Play a sound
    public void PlaySound(string soundName, CustomSoundEle[] soundArray, AudioSource playSource, bool isOneShot)
    {
        CustomSoundEle ele = FindSoundByName(soundName, soundArray);

        if (ele == null || ele.SoundClip == null || playSource == null)
        {
            if (ele == null)
            {
                Debug.Log($"Array {soundArray} has no sound name {soundName}");
            }
            if (ele != null && ele.SoundClip == null)
            {
                Debug.Log($"Sound name {soundName} in array {soundArray}, has NO CLIP.");
            }
            if (playSource == null)
            {
                Debug.Log($"NO AUDIO SOURCE {playSource}.");
            }

            return; // Return early if any null condition is met
        }

        if (isOneShot)
        {
            playSource.PlayOneShot(ele.SoundClip);
        }
        else
        {
            playSource.clip = ele.SoundClip;
            playSource.Play();
        }
    }

    private CustomSoundEle FindSoundByName(string soundName, CustomSoundEle[] soundArray)
    {
        foreach (CustomSoundEle sound in soundArray)
        {
            if (sound.SoundName == soundName)
            {
                return sound;
            }
        }

        return null;
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }

    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
        moveSource.mute = !moveSource.mute;
    }

    public void MusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SFXVolume(float volume)
    {
        sfxSource.volume = volume;
        moveSource.volume = volume;
    }
}
