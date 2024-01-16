using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class WaterPressureBearing : MonoBehaviour
{
    [SerializeField] public float CurrentWaterPressure = 1f;
    [SerializeField] public float MaxWaterPressureBearing = 100f;
    [SerializeField] private float pressureChangeRate = 1f;

    private Transform playerTransform;
    private Dive dive;

    private float playerInitialHeight;
    private float previousHeight;
    private bool isMoving;

    private void Awake()
    {
        playerTransform = GetComponent<Transform>();
        dive = GetComponent<Dive>();
    }

    private void Start()
    {
        playerInitialHeight = playerTransform.position.y;
        previousHeight = playerInitialHeight;
    }

    private void Update()
    {
        UpdateWaterPressure();

        if (dive.GetIsDead())
        {
            return;
        }
    }

    #region Public Call Functions
    public bool GetIsOverWaterPressure()
    {
        bool isOverWaterPressure = false;

        if (CurrentWaterPressure >= MaxWaterPressureBearing)
        {
            CurrentWaterPressure = MaxWaterPressureBearing;
            isOverWaterPressure = true;
        }
        return isOverWaterPressure;
    }
    #endregion

    #region Private Main Function
    private void UpdateWaterPressure()
    {
        float currentHeight = playerTransform.position.y;

        isMoving = GetIsMoving(currentHeight, previousHeight);

        if (isMoving)
        {
            CurrentWaterPressure = SetNewWaterPressure(CurrentWaterPressure, pressureChangeRate, 
                currentHeight, previousHeight, MaxWaterPressureBearing);
        }

        CurrentWaterPressure = Mathf.Clamp(CurrentWaterPressure, 1f, MaxWaterPressureBearing);

        previousHeight = currentHeight;
    }
    #endregion

    #region Private Quick Functions
    private bool GetIsMoving(float currentHeight, float previousHeight)
    {
        bool isMoving = false;

        if (currentHeight != previousHeight)
        {
            isMoving = true;
        }
        return isMoving;
    }

    private float SetNewWaterPressure(float currentWaterPressure, float pressureChangeRate, 
        float currentHeight, float previousHeight, float maxWaterPressureBearing)
    {
        float heightDifference = Mathf.Abs(previousHeight - currentHeight);

        if (currentWaterPressure >= maxWaterPressureBearing)
        {
            return currentWaterPressure;
        }

        else
        {
            if (currentHeight < previousHeight)
            {
                currentWaterPressure += heightDifference * pressureChangeRate;
            }
            else
            {
                currentWaterPressure -= heightDifference * pressureChangeRate;
            }
            return currentWaterPressure;
        }
    }
    #endregion
}