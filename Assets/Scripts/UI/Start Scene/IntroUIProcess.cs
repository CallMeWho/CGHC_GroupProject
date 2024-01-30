using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroUIProcess : MonoBehaviour
{
    private FadeInOut Fade;

    private void Awake()
    {
        AudioListener[] audioListeners = FindObjectsOfType<AudioListener>();

        // if no audio listener
        if (audioListeners.Length == 0)
        {
            gameObject.AddComponent<AudioListener>();
        }
        // remove extra audio listener
        else if (audioListeners.Length > 1)
        {
            AudioListener currentListener = gameObject.GetComponent<AudioListener>();

            foreach (AudioListener listener in audioListeners)
            {
                if (listener != currentListener) Destroy(listener);
            }
        }
    }

    private void Start()
    {
        Fade = GetComponent<FadeInOut>();

        AudioManager.instance.PlaySound("CommonBgm", AudioManager.instance.musicSounds, AudioManager.instance.musicSource, false);

        if (InGameUIProcess.InGameCanvasObj != null) 
        {
            InGameUIProcess.InGameCanvasObj.SetActive(false);
        }
    }

    public void LoadScene()
    {
        StartCoroutine(FadeInLoadScene());
    }

    public void OpenSetting()
    {
        SettingsUIController.SettingsCanvas.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator FadeInLoadScene()
    {
        Fade.StartFadeIn();
        yield return new WaitForSeconds(1);
        //GameScenesManager.GameScenesManagerInstance.LoadGameScene("Company");
        SceneManager.LoadSceneAsync("Company");

        new WaitForSeconds(3);
        if (InGameUIProcess.InGameCanvasObj != null)
        {
            InGameUIProcess.InGameCanvasObj.SetActive(true);
        }
    }

    private IEnumerator CoolQuitScene()
    {
        Fade.StartFadeOut();
        yield return new WaitForSeconds(1);
    }
}
