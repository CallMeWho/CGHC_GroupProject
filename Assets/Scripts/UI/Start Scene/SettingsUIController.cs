using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingsUIController : MonoBehaviour
{
    public static GameObject SettingsCanvas;
    
    public Slider MusicSlider, SfxSlider;

    private void Start()
    {
        SettingsCanvas = gameObject;
        gameObject.SetActive(false);
    }

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

    public void CloseCanvas()
    {
        gameObject.SetActive(false);
    }
}
