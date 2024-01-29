using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightProcess : MonoBehaviour
{
    public GameObject LightObject;
    private Light2D Light2D;

    [Header("Data Keeper")]
    [SerializeField] public GameInfo GameInfo;

    private void Start()
    {
        Light2D = LightObject.GetComponent<Light2D>();
        PlayerLightSetting();
    }

    private void PlayerLightSetting()
    {
        Light2D.intensity = GameInfo.LightIntensity;
        Light2D.pointLightInnerRadius = GameInfo.LightInnerRadius;
        Light2D.pointLightOuterRadius = GameInfo.LightOuterRadius;
    }
}
