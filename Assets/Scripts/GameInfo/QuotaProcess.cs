using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuotaProcess : MonoBehaviour
{
    [Header("Data Keeper")]
    [SerializeField] public GameInfo GameInfo;

    private int previousLevel;
    private bool hasPlayedQuotaSound = false;

    private void Start()
    {
        int currentLevel = GameInfo.CaveLevel; // Get the current level

        if (currentLevel > previousLevel && GameInfo.CurrentSceneName != "Cave")
        {
            int levelDifference = currentLevel - previousLevel;
            GameInfo.Quota += levelDifference * 100;
            previousLevel = currentLevel;
        }

        hasPlayedQuotaSound = false;
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
        ReachQuotaSound();
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

    private void ReachQuotaSound()  //maybe this one can apply in shopbuysound there
    {
        if (GameInfo.HasMetQuota && !hasPlayedQuotaSound && GameInfo.CurrentSceneName == "Cave")
        {
            AudioManager.instance.PlaySFX("ReachQuotaSound");
            hasPlayedQuotaSound = true;
        }
    }
}
