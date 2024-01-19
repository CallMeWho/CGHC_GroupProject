using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreatheOxygen : MonoBehaviour
{
    [SerializeField] public float CurrentOxygenLevel;
    [SerializeField] public float MaxOxygenLevel = 100f;
    [SerializeField] public float OxygenRecoverRate = 20f;
    [SerializeField] public float OxygenConsumptionRate = 1f;

    [Header("Data Keeper")]
    [SerializeField] public GameInfo GameInfo;

    private Spawn spawn;
    private Dive dive;

    private string sceneName;
    private bool isNoOxygen;

    private void Awake()
    {
        spawn = GetComponent<Spawn>();
        dive = GetComponent<Dive>();
    }

    private void Start()
    {
        CurrentOxygenLevel = MaxOxygenLevel;
        isNoOxygen = false;

        if (GameInfo != null)
        {
            GameInfo.Oxygen = 100;
        }
        else
        {
            Debug.LogError("GameInfo scriptable object is not assigned to BreatheOxygen script!");
        }
    }

    private void Update()
    {
        if (dive.GetIsDead())
        {
            return;
        }

        sceneName = spawn.GetSceneName();

        if (sceneName == "Company")
        {
            RecoverOxygen();
        }

        if (sceneName == "Cave")
        {
            ConsumeOxygen();
        }
    }

    private void ConsumeOxygen()
    {
        CurrentOxygenLevel -= OxygenConsumptionRate * Time.deltaTime;

        if (CurrentOxygenLevel < 0) //if player lacks oxygen during diving
        {
            CurrentOxygenLevel = 0;
            isNoOxygen = true;
            //code player behavior when out of oxygen
        }
    }

    private void RecoverOxygen()
    {
        if (CurrentOxygenLevel < MaxOxygenLevel)
        {
            CurrentOxygenLevel += OxygenRecoverRate * Time.deltaTime;   //oxygen will recover gradually

            if (CurrentOxygenLevel > MaxOxygenLevel)
            //OxygenRecoverRate will sometimes cause oxygen larger than max one, so if it happens that set oxygen = max
            {
                CurrentOxygenLevel = MaxOxygenLevel;
                isNoOxygen = false;
            }
        }

        GameInfo.Oxygen = CurrentOxygenLevel;
    }

    public bool GetIsNoOxygen()
    {
        return isNoOxygen;
    }
}
