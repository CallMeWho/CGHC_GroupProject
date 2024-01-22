using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightProcess : MonoBehaviour
{
    public GameObject LightObject;
    private Light2D Light2D;

    private void Start()
    {
        Light2D = LightObject.GetComponent<Light2D>();
    }


}
