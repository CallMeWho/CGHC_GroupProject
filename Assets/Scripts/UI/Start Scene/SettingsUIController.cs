using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingsUIController : MonoBehaviour
{
    public static GameObject SettingsCanvas;
    public static SettingsUIController Instance;

    public Slider MusicSlider, SfxSlider;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

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
        //Instance.gameObject.SetActive(false);
        gameObject.SetActive(false);

        InGameUIProcess.isPaused = false;
        Time.timeScale = 1;
    }

    public void Leave()
    {
        CloseCanvas();
        EndSceneSettings.BackToMenu();
    }
}
