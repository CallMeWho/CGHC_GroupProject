using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;

public class InGameUIProcess : MonoBehaviour
{
    [SerializeField] public Canvas Canvas;

    [Header("Oxygen")]
    [SerializeField] public UnityEngine.UI.Image OxygenIconImage;
    [SerializeField] public GameObject OxygenTextObject;

    [Header("Water Pressure")]
    [SerializeField] public GameObject PressureIcon;
    [SerializeField] public RectTransform PressureTicker;

    [Header("Quota")]
    [SerializeField] TextMeshProUGUI QuotaText;

    [Header("Hurt")]
    [SerializeField] public GameObject HurtScreen;

    [Header("Data Keeper")]
    [SerializeField] public GameInfo GameInfo;
    [SerializeField] public TerrainInfo TerrainInfo;

    public bool isReverting; // testing use only
    public bool isStopped; // testing use only

    public static InGameUIProcess InGameUIProcessInstance;

    private UnityEngine.UI.Image HurtScreenImage;
    private TextMeshProUGUI OxygenText;

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

        HurtScreenImage = HurtScreen.GetComponent<UnityEngine.UI.Image>();
        OxygenText = OxygenTextObject.GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        HurtScreen.SetActive(false);

        // set the ticker to initial point
        PressureTicker.transform.eulerAngles = new Vector3(
            PressureTicker.transform.eulerAngles.x, 
            PressureTicker.transform.eulerAngles.y, 
            0);
    }

    private void Update()
    {
        ProcessOxygenUI();
        ProcessHurtScreen();
        ProcessPressureUI();
        ProcessQuota();
    }

    private void ProcessOxygenUI()
    {
        // oxygen icon fill amount
        OxygenIconImage.fillAmount = GameInfo.CurrentOxygen / GameInfo.MaxOxygen;

        // oxygen percent show in int
        OxygenText.text = $"{Mathf.RoundToInt(GameInfo.CurrentOxygen / GameInfo.MaxOxygen * 100f)}%";
    }

    private void ProcessPressureUI()
    {
        // ticker will rotate based on current pressure
        PressureTicker.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(130, -130, GameInfo.CurrentWaterPressure / GameInfo.MaxPressureCapacity));
    }

    private void ProcessQuota()
    {
        QuotaText.text = $"QUOTA \n{GameInfo.CurrentCredit} / {GameInfo.Quota}";
    }

    // check player oxygen and show hurt screen
    private void ProcessHurtScreen()
    {
        if (GameInfo.CurrentOxygen <= GameInfo.MaxOxygen * 0.2f || GameInfo.CurrentWaterPressure >= GameInfo.MaxPressureCapacity * 0.9f)    // if lower than 20% of max oxygen
        {
            HurtScreen.SetActive(true);

            StartCoroutine(FadeHurtScreen(0f, 0.2f));
        }

        else
        {
            HurtScreen.SetActive(false);
        }
    }

    // future try if can use animation and timeline to do, instead of code here to make the effect
    private IEnumerator FadeHurtScreen(float minAlpha, float maxAlpha)  // max alpha is 1
    {
        Color targetColor = HurtScreenImage.color;
        targetColor.a = 0;  // set initial alpha to screen

        float duration = 1f; // fade duration (editable)
        float timer = 0f;    // elapsed time

        while (true) // this line is a must, or the update will be very quick, which has no kinda like waiting order.
        {
            // min alpha -> max alpha
            while (timer < duration)
            {
                float alpha = Mathf.Lerp(minAlpha, maxAlpha, timer / duration); // get current alpha
                HurtScreenImage.color = new Color(targetColor.r, targetColor.g, targetColor.b, alpha);

                timer += Time.deltaTime;
                yield return null;
            }

            timer = 0f; // reset timer

            // max alpha -> min alpha
            while (timer < duration)
            {
                float alpha = Mathf.Lerp(maxAlpha, minAlpha, timer / duration); 
                HurtScreenImage.color = new Color(targetColor.r, targetColor.g, targetColor.b, alpha);

                timer += Time.deltaTime;
                yield return null;
            }

            timer = 0f; // reset timer
        }
    }
}
