using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BookProcess : MonoBehaviour
{
    [SerializeField] float MoveSpeedIncreaseVal = 1f;
    [SerializeField] float MaxOxygenIncreasePer = 0.2f;
    [SerializeField] float OxygenConsRate = 0.1f;
    [SerializeField] float MaxPressureIncreasePer = 0.2f;
    [SerializeField] float PressureChangeRate = 0.1f;

    [Header("Data Keeper")]
    [SerializeField] public GameInfo GameInfo;
    
    // after player clicks the book, this activates.
    public void BuySkill()
    {
        AudioManager.instance.PlaySound("BookBuySound", AudioManager.instance.sfxSounds, AudioManager.instance.sfxSource, true);
        GameInfo.CurrentCredit -= GameInfo.ShopCost;
        GameInfo.ShopCost += Mathf.RoundToInt(GameInfo.ShopCost * GameInfo.CostIncrementPercent);

        ShopUIUpdateProcess.ShopCanvasObj.SetActive(false);

        InGameUIProcess.InGameCanvasObj.SetActive(true);
        
        //GameScenesManager.GameScenesManagerInstance.LoadGameScene("Company");
    }

    // move speed
    public void IncreaseMoveSpeed()
    {
        GameInfo.MaxSpeed += 5;
        GameInfo.MaxAcceleration += 5;
    }

    // oxygen
    public void IncreaseMaxOxygen()
    {
        GameInfo.MaxOxygen += (100 * MaxOxygenIncreasePer);
    }

    public void DecreaseOxygenConsRate()
    {
        GameInfo.OxygenConsumptionRate -= (0.5f * OxygenConsRate);

        if (GameInfo.OxygenConsumptionRate < 0.1f)
        {
            GameInfo.OxygenConsumptionRate = 0.1f;
        }
    }

    // water pressure
    public void IncreaseMaxPressure()
    {
        GameInfo.MaxPressureCapacity += (100 * MaxPressureIncreasePer);
    }

    public void DecreasePressureConsRate()
    {
        GameInfo.PressureChangeRate -= (0.5f * PressureChangeRate);

        if (GameInfo.PressureChangeRate < 0.1f)
        {
            GameInfo.PressureChangeRate = 0.1f;
        }
    }

    // vision
    public void IncreaseVision()
    {
        GameInfo.LightIntensity += 0.5f;
        if (GameInfo.LightIntensity >= 8f)
        {
            GameInfo.LightIntensity = 8f;
        }

        GameInfo.LightInnerRadius += 2f;
        GameInfo.LightOuterRadius += 2f;
    }
}
