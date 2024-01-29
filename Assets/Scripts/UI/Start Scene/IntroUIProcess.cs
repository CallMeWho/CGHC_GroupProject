using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroUIProcess : MonoBehaviour
{
    private FadeInOut Fade;

    private void Start()
    {
        Fade = GetComponent<FadeInOut>();
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
