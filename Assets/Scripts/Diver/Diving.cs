using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diving : MonoBehaviour
{
    public bool IsDiving;
    public float CurrentOxygenLevel;

    private float MaxOxygenLevel = 100f;
    private float OxygenRecoverRate = 20f;
    private float OxygenConsumptionRate = 1f;

    public void StartDiving()
    {
        IsDiving = true;
        //effect when diving
    }

    public void StopDiving()
    {
        IsDiving = false;
        //effect when surfacing
    }

    public void Start()
    {
        IsDiving = true;
        CurrentOxygenLevel = MaxOxygenLevel;
    }

    public void Update()
    {
        if (IsDiving)   //if player start diving
        {
            CurrentOxygenLevel -= OxygenConsumptionRate * Time.deltaTime;

            if (CurrentOxygenLevel < 0) //if player lacks oxygen during diving
            {
                CurrentOxygenLevel = 0;
                StopDiving();
                //code player behavior when out of oxygen
            }
        }

        else if (!IsDiving) //if player surfacing back from diving
        {
            if (CurrentOxygenLevel < MaxOxygenLevel)    
            {
                CurrentOxygenLevel += OxygenRecoverRate * Time.deltaTime;   //oxygen will recover gradually

                if (CurrentOxygenLevel > MaxOxygenLevel)
                //OxygenRecoverRate will sometimes cause oxygen larger than max one, so if it happens that set oxygen = max
                {
                    CurrentOxygenLevel = MaxOxygenLevel;
                }
            }
            StopDiving();
        } 
    }
}
