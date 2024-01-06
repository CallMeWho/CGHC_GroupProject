using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ItemGeneration : MonoBehaviour
{
    public GameObject prefabToSpawn; // The prefab to be spawned
    public int spawnCount = 10; // Number of prefabs to spawn
    public Tilemap targetTilemap; // The tilemap to spawn the prefabs on

    private int[,] _TerrainArray;

    void Start()
    {
        
    }

}