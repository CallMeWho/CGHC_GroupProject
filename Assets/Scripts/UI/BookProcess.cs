using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BookProcess : MonoBehaviour
{
    [Header("Data Keeper")]
    [SerializeField] public GameInfo GameInfo;

    public void BuySkill()
    {
        GameInfo.CurrentCredit -= GameInfo.ShopCost;
        GameInfo.ShopCost += Mathf.RoundToInt(GameInfo.ShopCost * GameInfo.CostIncrementPercent);
        SceneManager.LoadScene("Company");
    }
}
