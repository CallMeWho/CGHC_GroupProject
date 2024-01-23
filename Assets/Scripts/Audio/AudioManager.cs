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
        if (ele == null)
        {
            Debug.Log($"Array {soundArray} has no sound name {soundName}");
        }
        if (ele.SoundClip == null)
        {
            Debug.Log($"Sound name {soundName} in array {soundArray}, has NO CLIP.");
        }
        if (playSource == null)
        {
            Debug.Log($"NO AUDIO SOURCE {playSource}.");
        }
    }

    public void PlaySFX(string name)
    {
        CustomSoundEle s = Array.Find(sfxSounds, x => x.SoundName == name);

        sfxSource.PlayOneShot(s.SoundClip);
    }

    public void PlayMoveSound(string name)
    {
        // find music
        CustomSoundEle s = Array.Find(sfxSounds, x => x.SoundName == name);

        // put music into audio source, and play
        moveSource.clip = s.SoundClip;
        moveSource.Play();
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }

    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
    }

    public void MusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
