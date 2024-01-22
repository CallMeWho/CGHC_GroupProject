using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class InGameUIProcess : MonoBehaviour
{
    [SerializeField] public Canvas Canvas;
    [SerializeField] public UnityEngine.UI.Image OxygenIconImage;
    [SerializeField] public GameObject PressureIcon;
    [SerializeField] public GameObject HurtScreen;

    [Header("Data Keeper")]
    [SerializeField] public GameInfo GameInfo;
    [SerializeField] public TerrainInfo TerrainInfo;

    public static InGameUIProcess InGameUIProcessInstance;

    private void Awake()
    {
        if (InGameUIProcessInstance == null)
        {
            InGameUIProcessInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        ProcessOxygenIcon();
    }

    private void ProcessOxygenIcon()
    {
        OxygenIconImage.fillAmount = GameInfo.CurrentOxygen / GameInfo.MaxOxygen;
    }
}
