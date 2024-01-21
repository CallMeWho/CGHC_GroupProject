using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeStartUp : MonoBehaviour
{
    FadeInOut Fade;

    private void Start()
    {
        Fade = GetComponent<FadeInOut>();
        Fade.StartFadeOut();
    }
}
