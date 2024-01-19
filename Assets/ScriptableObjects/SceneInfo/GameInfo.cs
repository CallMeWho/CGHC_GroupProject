using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameInfo", menuName = "GameManager/GameInfo")]
public class GameInfo : ScriptableObject
{
    [Header("Current Scene")]
    public string CurrentSceneName;

    [Header("Player Current Conditions")]
    public float MoveSpeed;
    public float Oxygen;
    
}
