using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralTerrainGeneration : MonoBehaviour
{
    [SerializeField] int Width;
    [SerializeField] int MinStoneHeight, MaxStoneHeight;
    [Range(0,100)]
    [SerializeField] float HeightValue, Smoothness;
    [SerializeField] TileBase PlaneTile, GrassTile, StoneTile;
    [SerializeField] Tilemap TerrainTilemap;
    [SerializeField] float Seed;

    void Start()
    {
        Seed = Random.Range(-100000, 100000);
        Generation();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Seed = Random.Range(-100000, 100000);
            Generation();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            TerrainTilemap.ClearAllTiles();
        }
    }

    void Generation()
    {
        for (int x = 0; x < Width; x++)
        {
            int height = Mathf.RoundToInt(HeightValue * Mathf.PerlinNoise(x / Smoothness, Seed));

            int minStoneSpawnDistance = height - MinStoneHeight;
            int maxStoneSpawnDistance = height - MaxStoneHeight;
            int totalStoneSpawnDistance = Random.Range(maxStoneSpawnDistance, minStoneSpawnDistance);

            for (int y =  0; y < height; y++)
            {
                if (y < totalStoneSpawnDistance)
                {
                    SpawnTile(StoneTile, x, y);
                }
                else
                {
                    SpawnTile(PlaneTile, x, y);
                }
            }

            if (totalStoneSpawnDistance == height)
            {
                SpawnTile(StoneTile, x, height);
            }
            else
            {
                SpawnTile(GrassTile, x, height);
            }
            
        }
    }

    void SpawnTile(TileBase tile, int width, int height)
    {
        Vector3Int tilePosition = new Vector3Int(width, height, 0);
        TerrainTilemap.SetTile(tilePosition, tile);
    }
}
