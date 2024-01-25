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
    [SerializeField] public UnityEngine.UI.Image OxygenIconImage;
    [SerializeField] public GameObject OxygenTextObject;
    [SerializeField] public GameObject PressureIcon;
    [SerializeField] public GameObject PressureTicker;
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
        float rotationSpeed = 50f;

        if (isReverting)
        {
            rotationSpeed = -rotationSpeed;
        }
        
        if (isStopped)
        {
            rotationSpeed = 0;
        }

        float nextFrameRotationZ = PressureTicker.transform.eulerAngles.z + rotationSpeed * Time.deltaTime; // rotate ticker value only in z

        PressureTicker.transform.eulerAngles = new Vector3(
            PressureTicker.transform.eulerAngles.x, 
            PressureTicker.transform.eulerAngles.y,
            nextFrameRotationZ);

        if (PressureTicker.transform.eulerAngles.z >= 180 || PressureTicker.transform.eulerAngles.z <= -90)
        {
            isStopped = true;
            isReverting = true;
            isStopped = false;
        }
    }

    // check player oxygen and show hurt screen
    private void ProcessHurtScreen()
    {
        if (GameInfo.CurrentOxygen <= GameInfo.MaxOxygen * 0.2f)    // if lower than 20% of max oxygen
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
