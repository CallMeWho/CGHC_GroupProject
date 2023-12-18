using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralTrapGeneration : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField] Tilemap TrapTilemap;
    [SerializeField] Tilemap TerrainTilemap;

    [Header("Tiles")]
    [SerializeField] TileBase TrapTile;

    [Header("Trap Placement")]
    [SerializeField] int Height;

    void Start()
    {
        
    }

    
}
