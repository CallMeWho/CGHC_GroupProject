using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuotaProcess : MonoBehaviour
{
    [Header("Data Keeper")]
    [SerializeField] public GameInfo GameInfo;

    private int previousLevel;
    private int currentLevel;
    private bool hasPlayedQuotaSound;
    private bool increaseCaveLevel;

    private void Start()
    {
        increaseCaveLevel = false;
        hasPlayedQuotaSound = false;
    }

    private void Update()
    {
        GameInfo.CanBuySkill = GameInfo.CurrentCredit >= GameInfo.ShopCost;
        GameInfo.HasMetQuota = (GameInfo.CurrentCredit >= GameInfo.Quota && GameInfo.Quota != 0);
        ReachQuotaSound();

        if (GameInfo.CurrentSceneName == "Cave" && !increaseCaveLevel)
        {
            IncreaseDifficulty();
            increaseCaveLevel = true;
        }
    }

    private void IncreaseDifficulty()
    {
        int currentLevel = GameInfo.CaveLevel; // Get the current level
        int levelDifference = currentLevel - previousLevel;
        GameInfo.Quota += levelDifference * 100;
        previousLevel = currentLevel;
    }

    private void ReachQuotaSound()  //maybe this one can apply in shopbuysound there
    {
        if (GameInfo.HasMetQuota && !hasPlayedQuotaSound && GameInfo.CurrentSceneName == "Cave")
        {
            AudioManager.instance.PlaySound("ReachQuotaSound", AudioManager.instance.sfxSounds, AudioManager.instance.sfxSource, true);
            hasPlayedQuotaSound = true;
        }
    }
}
