using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemTypes
{
    public string ItemName;
    public GameObject[] PrefabList;
    [Range(0, 1)] public float FillPercent;
    [Range(0, 1)] public float SpawnProbability;
    [Range(0, 1)] public float LevelIncrementPer;
    public int DetectRadius;
    [Range(2, 4)] public int ArrayIndex;
    public Vector3 SpawnOffset;
}
