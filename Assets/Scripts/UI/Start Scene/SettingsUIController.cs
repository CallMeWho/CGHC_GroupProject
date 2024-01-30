using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingsUIController : MonoBehaviour
{
    public Slider MusicSlider, SfxSlider;

    public void ToggleMusic()
    {
        AudioManager.instance.ToggleMusic();
    }

    public void ToggleSFX()
    {
        AudioManager.instance.ToggleSFX();  // included move source
    }

    public void MusicVolume()
    {
        AudioManager.instance.MusicVolume(MusicSlider.value);
    }

    public void SFXVolume()
    {
        AudioManager.instance.SFXVolume(SfxSlider.value);   // included move source
    }
}
