using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInOut : MonoBehaviour
{
    public CanvasGroup CanvasGroup;
    public bool FadeIn;
    public bool FadeOut;
    public float TimeToFade;

    private void Update()
    {
        fadeIn();
        fadeOut();
    }

    private void fadeIn()
    {
        if (FadeIn)
        {
            if (CanvasGroup.alpha < 1)
            {
                CanvasGroup.alpha += TimeToFade * Time.deltaTime;

                if (CanvasGroup.alpha >= 1)
                {
                    FadeIn = false;
                }
            }
        }
    }

    private void fadeOut()
    {
        if (FadeOut)
        {
            if (CanvasGroup.alpha >= 0)
            {
                CanvasGroup.alpha -= TimeToFade * Time.deltaTime;

                if (CanvasGroup.alpha == 0)
                {
                    FadeOut = false;
                }
            }
        }
    }

    public void StartFadeIn()
    {
        FadeIn = true;
    }

    public void StartFadeOut()
    {
        FadeOut = true;
    }
}
