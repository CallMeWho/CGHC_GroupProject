using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum DiverState
{
    Walking,
    Diving,
    Dead
}

public class DiverConditions : MonoBehaviour
{
    [Header("Diver Current State")]
    public DiverState currentState = DiverState.Walking;

    [Header("Diver Conditions")]
    public float CurrentMoveSpeed;
    public float CurrentOxygen;
    public float WaterPressureEndurance; //water pressure bearing capacity

    [Header("Diver Current Quota")]
    public int CurrentMoney;
}
