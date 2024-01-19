using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameInfo", menuName = "GameManager/GameInfo")]
public class GameInfo : ScriptableObject
{
    [Header("Current Scene")]
    public string CurrentSceneName;

    [Header("Player Move Condition")]
    public float MaxSpeed = 4f;
    public float MaxAcceleration = 35f;
    public float GroundFriction;

    [Header("Player Oxygen Condition")]
    public float CurrentOxygen;
    public float MaxOxygen;
    public float OxygenRecoverRate;
    public float OxygenConsumptionRate;

    [Header("Player Current States")]
    public bool IsOnGround = false;
    public bool HasNoOxygen = false;
}
