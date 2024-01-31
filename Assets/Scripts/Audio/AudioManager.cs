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

        EnsureAudioListener();
    }

    private void Start()
    {
        PlaySound("CommonBgm", musicSounds, musicSource, false);
    }

    public void PlaySound(string soundName, CustomSoundEle[] soundArray, AudioSource playSource, bool isOneShot)
    {
        CustomSoundEle ele = Array.Find(soundArray, x => x.SoundName == soundName);

        if (ele != null && ele.SoundClip != null)
        {
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
        }
    }

    public void PlayMusic(string name)
    {
        CustomSoundEle s = Array.Find(musicSounds, x => x.SoundName == name);

        moveSource.clip = s.SoundClip;
        moveSource.Play();
    }

    public void PlaySFX(string name)
    {
        CustomSoundEle s = Array.Find(sfxSounds, x => x.SoundName == name);

        sfxSource.PlayOneShot(s.SoundClip);
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

    private void EnsureAudioListener()
    {
        if (FindObjectOfType<AudioListener>() == null)
        {
            GameObject audioListenerObj = new GameObject("AudioListener");
            audioListenerObj.AddComponent<AudioListener>();
        }
    }
}
