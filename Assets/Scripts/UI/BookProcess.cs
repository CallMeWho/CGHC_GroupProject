using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BookProcess : MonoBehaviour
{
    [SerializeField] float MaxPressureIncreasePer = 0.2f;

    [Header("Data Keeper")]
    [SerializeField] public GameInfo GameInfo;

    public void BuySkill()
    {
        GameInfo.CurrentCredit -= GameInfo.ShopCost;
        GameInfo.ShopCost += Mathf.RoundToInt(GameInfo.ShopCost * GameInfo.CostIncrementPercent);
        SceneManager.LoadSceneAsync("Company");
    }

    // movespeed


    // oxygen


    // water pressure
    public void IncreaseMaxPressure()
    {
        GameInfo.MaxPressureCapacity += (100 * MaxPressureIncreasePer);
    }

    // vision

}
