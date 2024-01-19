using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenProcess : MonoBehaviour
{
    [Header("Data Keeper")]
    [SerializeField] public GameInfo GameInfo;

    private void Start()
    {
        GameInfo.HasNoOxygen = false;
    }

    private void Update()
    {
        ProcessBasedOnScene();
    }

    private void ProcessBasedOnScene()
    {
        if (GameInfo.HasNoOxygen)
        {
            return;
        }

        else if (GameInfo.CurrentSceneName == "Company")
        {
            RecoverOxygen();
        }
        else if (GameInfo.CurrentSceneName == "Cave")
        {
            ConsumeOxygen();
        }
    }

    private void ConsumeOxygen()
    {
        if (GameInfo.CurrentOxygen <= GameInfo.MaxOxygen)
        {
            GameInfo.CurrentOxygen -= GameInfo.OxygenConsumptionRate * Time.deltaTime;

            if (GameInfo.CurrentOxygen < 0)
            {
                GameInfo.CurrentOxygen = 0;
                GameInfo.HasNoOxygen = true;

            }
        }

        else if (GameInfo.CurrentOxygen > GameInfo.MaxOxygen)
        {
            GameInfo.CurrentOxygen = GameInfo.MaxOxygen;
        }
    }

    private void RecoverOxygen()
    {
        if (GameInfo.CurrentOxygen == 0 || GameInfo.CurrentOxygen < GameInfo.MaxOxygen)
        {
            GameInfo.CurrentOxygen += GameInfo.OxygenRecoverRate * Time.deltaTime;
        }

        else if (GameInfo.CurrentOxygen >= GameInfo.MaxOxygen)
        {
            GameInfo.CurrentOxygen = GameInfo.MaxOxygen;
        }

        GameInfo.HasNoOxygen = false;
    }
}