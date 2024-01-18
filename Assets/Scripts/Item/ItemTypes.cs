using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemTypes
{
    public string ItemName;
    public GameObject[] PrefabList;
    [Range(0, 100)] public float FillPercent;
    [Range(0, 100)] public float SpawnProbability;
    public int DetectRadius;
    [Range(2, 4)] public int ArrayIndex;
    public Vector3 SpawnOffset;
}
