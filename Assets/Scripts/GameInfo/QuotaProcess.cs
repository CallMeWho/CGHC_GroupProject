using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuotaProcess : MonoBehaviour
{
    [Header("Data Keeper")]
    [SerializeField] public GameInfo GameInfo;

    private int previousLevel;

    private void Start()
    {
        int currentLevel = GameInfo.CaveLevel; // Get the current level

        if (currentLevel > previousLevel && GameInfo.CurrentSceneName != "Cave")
        {
            int levelDifference = currentLevel - previousLevel;
            GameInfo.Quota += levelDifference * 100;
            previousLevel = currentLevel;
        }
    }

    private void Update()
    {
        if (GameInfo.CurrentCredit >= GameInfo.Quota)
        {
            GameInfo.HasMetQuota = true;
        }
        else
        {
            GameInfo.HasMetQuota = false;
        }

        ShopCheckInRequirement();
    }

    private void ShopCheckInRequirement()
    {
        if (GameInfo.CurrentCredit >= GameInfo.ShopCost)
        {
            GameInfo.CanBuySkill = true;
        }
        else
        {
            GameInfo.CanBuySkill = false;
        }
    }
}