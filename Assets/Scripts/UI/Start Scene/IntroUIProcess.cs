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
        if (audioListeners.Length == 0)
        {
            gameObject.AddComponent<AudioListener>();
        }
        else if (audioListeners.Length > 0)
        {
            for (int i = 0; i < audioListeners.Length; i++)
            {
                if (audioListeners[i] != GetComponent<AudioListener>())
                {
                    Debug.Log("Extra audio listener found at: " + audioListeners[i].gameObject.name);
                    //audioListeners[i].enabled = false;
                    //Destroy(audioListeners[i]);
                }
            }
        }
    }

    private void Start()
    {
        Fade = GetComponent<FadeInOut>();

        AudioManager.instance.PlaySound("CommonBgm", AudioManager.instance.musicSounds, AudioManager.instance.musicSource, false);
    }

    public void LoadScene()
    {
        StartCoroutine(FadeInLoadScene());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator FadeInLoadScene()
    {
        Fade.StartFadeIn();
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync("Company");
    }

    private IEnumerator CoolQuitScene()
    {
        Fade.StartFadeOut();
        yield return new WaitForSeconds(1);
    }
}
