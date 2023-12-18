using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralTerrainGeneration : MonoBehaviour
{
    [Header("Terrain Size")]
    [SerializeField] int Width;
    [SerializeField] int MinInnerTileInterval, MaxInnerTileInterval;
    [Range(0,100)][SerializeField] float HeightValue, Smoothness;

    [Header("Tilemaps")]
    [SerializeField] Tilemap TerrainTilemap;

    [Header("Tiles")]
    [SerializeField] TileBase UpperTile;
    [SerializeField] TileBase LowerTile;
    [SerializeField] TileBase GrassTile;

    [Header("Random Generate")]
    [SerializeField] float Seed;

    void Start()
    {
        Seed = Random.Range(-100000, 100000);
        TerrainGeneration();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TerrainTilemap.ClearAllTiles();
            Seed = Random.Range(-100000, 100000);
            TerrainGeneration();
        }
    }

    void TerrainGeneration()
    {
        for (int x = 0; x < Width; x++)
        {
            int perlinHeight = Mathf.RoundToInt(HeightValue * Mathf.PerlinNoise(x / Smoothness, Seed));

            int highestInnerTileHeight = perlinHeight - MinInnerTileInterval;
            int lowestInnerTileHeight = perlinHeight - MaxInnerTileInterval;
            int randomInnerTileHeight = Random.Range(lowestInnerTileHeight, highestInnerTileHeight);

            for (int y =  0; y < perlinHeight; y++)
            {
                if (y < randomInnerTileHeight)
                {
                    SpawnTile(LowerTile, x, y);
                }
                else
                {
                    SpawnTile(UpperTile, x, y);
                }
            }

            if (randomInnerTileHeight == perlinHeight)
            {
                SpawnTile(LowerTile, x, perlinHeight);
            }
            else
            {
                SpawnTile(GrassTile, x, perlinHeight);
            }
        }
    }

    void SpawnTile(TileBase tile, int width, int height)
    {
        Vector3Int tilePosition = new Vector3Int(width, height, 0);
        TerrainTilemap.SetTile(tilePosition, tile);
    }
}
