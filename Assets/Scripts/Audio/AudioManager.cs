using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public CustomSoundEle[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource, moveSource;

    // dont destroy on load
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
        AudioManager.instance.PlaySound("CommonBgm", AudioManager.instance.musicSounds, AudioManager.instance.musicSource, false);
    }

    public void PlaySound(string soundName, CustomSoundEle[] soundArray, AudioSource playSource, bool isOneShot)
    {
        // find music
        CustomSoundEle ele = Array.Find(soundArray, x => x.SoundName == soundName);

        // play the clip
        if (ele != null && ele.SoundClip != null)
        {
            if (isOneShot)
            {
                // play once
                playSource.PlayOneShot(ele.SoundClip);
            }
            else
            {
                // play repeat
                playSource.clip = ele.SoundClip;
                playSource.Play();
            }
        }

        // if errors, show reasons
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

    // for button use
    public void PlayMusic(string name)
    {
        // find music
        CustomSoundEle s = Array.Find(musicSounds, x => x.SoundName == name);

        // put music into audio source, and play
        moveSource.clip = s.SoundClip;
        moveSource.Play();
    }

    public void PlaySFX(string name)
    {
        CustomSoundEle s = Array.Find(sfxSounds, x => x.SoundName == name);

        sfxSource.PlayOneShot(s.SoundClip);
    }

    // for setting ui use
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
