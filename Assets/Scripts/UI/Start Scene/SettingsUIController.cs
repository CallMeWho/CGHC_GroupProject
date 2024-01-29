using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingsUIController : MonoBehaviour
{
    public Slider _musicslider, _sfxslider;

    public void ToggleMusic()
    {
        AudioManager.instance.ToggleMusic();
    }

    public void ToggleSFX()
    {
        AudioManager.instance.ToggleSFX();
    }

    public void MusicVolume()
    {
        AudioManager.instance.MusicVolume(_musicslider.value);
    }

    public void SFXVolume()
    {
        AudioManager.instance.SFXVolume(_sfxslider.value);
    }
}
