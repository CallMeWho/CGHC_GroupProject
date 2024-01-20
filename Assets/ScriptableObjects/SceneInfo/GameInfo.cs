using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameInfo", menuName = "GameManager/GameInfo")]
public class GameInfo : ScriptableObject
{
    [Header("Current Scene")]
    public string CurrentSceneName;
    public int CaveLevel = 0;

    [Header("Player Move Condition")]
    public float MaxSpeed = 4f;
    public float MaxAcceleration = 35f;
    public float RotationSpeed = 720f;
    public float GroundFriction;

    [Header("Player Oxygen Condition")]
    public float CurrentOxygen;
    public float MaxOxygen = 100f;
    public float OxygenRecoverRate = 20f;
    public float OxygenConsumptionRate = 1f;

    [Header("Player Water Pressure Condition")]
    public float CurrentWaterPressure = 0f;
    public float MaxPressureCapacity = 100f;
    public float PressureChangeRate = 1f;

    [Header("Player Quota Condition")]
    public int CurrentCredit = 0;
    public int Quota = 100;
    public int ShopCost = 50;

    [Header("Player Current States")]
    public bool IsOnGround = false;
    public bool IsVerticalMoving = false;
    public bool HasNoOxygen = false;
    public bool HasOverWaterPressure = false;
    public bool HasMetQuota = false;
    public bool CanBuySkill = false;

    [Header("Player Interaction Status")]
    public bool IsTouchingObject = false;
    public bool IsPressingKey = false;
    public bool HasInteracted = false;  //useless for now, interacting got prob
}
