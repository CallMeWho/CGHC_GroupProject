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

    public static GameObject InGameCanvasObj;

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

    [Header("Pause")]
    [SerializeField] private InputController input = null;
    [SerializeField] private GameObject PauseMenu;
    public static bool isPaused = false;

    [Header("Data Keeper")]
    [SerializeField] public GameInfo GameInfo;
    [SerializeField] public TerrainInfo TerrainInfo;

    public bool isReverting; // testing use only
    public bool isStopped; // testing use only

    public static InGameUIProcess InGameUIProcessInstance;

    private UnityEngine.UI.Image HurtScreenImage;
    private TextMeshProUGUI OxygenText;

    private GameInfo gameInfo;

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
        InGameCanvasObj = gameObject;

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

        CheckPauseInput();
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
        while (true)
        {
            Color targetColor = HurtScreenImage.color;

            float duration = 1f; // fade duration (editable)
            int numSteps = 100; // number of steps for the fade

            for (int i = 0; i <= numSteps; i++)
            {
                float t = i / (float)numSteps;

                // min alpha -> max alpha
                float alpha = Mathf.SmoothStep(minAlpha, maxAlpha, t);
                targetColor.a = alpha;
                HurtScreenImage.color = targetColor;

                yield return null;
            }

            for (int i = 0; i <= numSteps; i++)
            {
                float t = i / (float)numSteps;

                // max alpha -> min alpha
                float alpha = Mathf.SmoothStep(maxAlpha, minAlpha, t);
                targetColor.a = alpha;
                HurtScreenImage.color = targetColor;

                yield return null;
            }
        }
    }

    private void CheckPauseInput()
    {
        if (input.RetrievePauseInput() && !isPaused)
        {
            PauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
    }
}
