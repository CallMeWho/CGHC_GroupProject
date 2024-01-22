using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TerrainInfo", menuName = "GameManager/TerrainInfo")]
public class TerrainInfo : ScriptableObject
{
    public int[,] TerrainArray;

    [Header("Current Terrain")]
    public int Width;
    public int Height;
    public int Size;
    public int ActualSize;

    [Header("Current Boundary Points")]
    public Vector2 BoundaryMaxPoint;
    public Vector2 BoundaryMinPoint;

    [Header("Randomization")]
    public float Seed;

    [Header("Walk Cave")]
    public Vector2 EntryPoint1;
    [Range(0, 94)] public int PlayerMovingAreaPercent;
    public int PlayerMovingAreaCount;

    [Header("Cellular Automata")]
    public Vector2 EntryPoint2;
    public int FillChance;

    [Header("Smooth Cellular")]
    public bool WallEdges;
    [Range(0, 20)] public int SmoothCount;
}
