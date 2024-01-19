using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPressureProcess : MonoBehaviour
{
    [Header("Data Keeper")]
    [SerializeField] public GameInfo GameInfo;

    private Transform playerTransform;
    private float playerInitialHeight;
    private float previousHeight;

    private void Awake()
    {
        playerTransform = GetComponent<Transform>();
    }

    private void Update()
    {
        UpdateWaterPressure();
    }

    private void UpdateWaterPressure()
    {
        if (GameInfo.CurrentSceneName != "Cave")
        {
            GameInfo.CurrentWaterPressure = 0;
            GameInfo.IsVerticalMoving = false;
            return;
        }

        float currentHeight = playerTransform.position.y;

        if (currentHeight != previousHeight)
        {
            GameInfo.IsVerticalMoving = true;
        }
        else
        {
            GameInfo.IsVerticalMoving = false;
        }

        float heightDifference = Mathf.Abs(previousHeight - currentHeight);

        if (GameInfo.CurrentWaterPressure < GameInfo.MaxPressureCapacity)
        {
            if (currentHeight < previousHeight)
            {
                GameInfo.CurrentWaterPressure += heightDifference * GameInfo.PressureChangeRate;
            }
            else
            {
                GameInfo.CurrentWaterPressure -= heightDifference * GameInfo.PressureChangeRate;
            }
        }

        GameInfo.CurrentWaterPressure = Mathf.Clamp(GameInfo.CurrentWaterPressure, 1f, GameInfo.MaxPressureCapacity);

        previousHeight = currentHeight;
    }
}
