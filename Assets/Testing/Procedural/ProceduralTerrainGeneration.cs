using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TerrainUtils;
using UnityEngine.Tilemaps;

public class ProceduralTerrainGeneration : MonoBehaviour
{
    [Header("Boundary")]
    [SerializeField] int BoundaryWidth;
    [SerializeField] int BoundaryHeight;
    int[,] BoundaryArray;
    int[,] TerrainArray;

    [Header("Terrain")]
    [SerializeField] int TerrainMaxHeight;
    [SerializeField] int MinInnerTileInterval;
    [SerializeField] int MaxInnerTileInterval;
    [Range(0,100)][SerializeField] float HeightValue, Smoothness;
    int RandomInnerTileHeight;

    [Header("Tilemaps")]
    [SerializeField] Tilemap TerrainTilemap;


    [Header("Tiles")]
    [SerializeField] TileBase UpperTile;
    [SerializeField] TileBase LowerTile;
    [SerializeField] TileBase GrassTile;

    [Header("Randomization")]
    [SerializeField] float Seed;

    void Start()
    {
        BoundaryArray = GeneratedBoundaryArray(BoundaryWidth, BoundaryHeight, true);
        TerrainArray = GeneratedTerrainArray(BoundaryArray);
        RenderTerrainArray(BoundaryArray, TerrainTilemap);

        Seed = Random.Range(-100000, 100000);
        //TerrainGeneration();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TerrainTilemap.ClearAllTiles();
            BoundaryArray = GeneratedBoundaryArray(BoundaryWidth, BoundaryHeight, true);
            TerrainArray = GeneratedTerrainArray(BoundaryArray);
            RenderTerrainArray(BoundaryArray, TerrainTilemap);

            Seed = Random.Range(-100000, 100000);
            //TerrainGeneration();
        }
    }

    public int[,] GeneratedBoundaryArray(int width, int height, bool isEmpty)
    {
        int[,] boundaryArray = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (isEmpty)
                {
                    boundaryArray[x, y] = 0;
                }
                else
                {
                    boundaryArray[x, y] = 1;
                }
            }
        }
        return boundaryArray;
    } 

    public int[,] GeneratedTerrainArray(int[,] boundaryArray)
    {
        int[,] terrainArray = boundaryArray;

        for (int x = 0; x < BoundaryWidth; x++)
        {
            int perlinHeight = Mathf.RoundToInt(HeightValue * Mathf.PerlinNoise(x / Smoothness, Seed));

            int highestInnerTileHeight = perlinHeight - MinInnerTileInterval;
            int lowestInnerTileHeight = perlinHeight - MaxInnerTileInterval;
            int randomInnerTileHeight = Random.Range(lowestInnerTileHeight, highestInnerTileHeight);

            for (int y = 0; y < perlinHeight; y++)
            {
                if (y < randomInnerTileHeight)
                {
                    terrainArray[x, y] = 1;
                }
                else
                {
                    terrainArray[x, y] = 2;
                }
            }

            if (randomInnerTileHeight == perlinHeight)
            {
                terrainArray[x, perlinHeight] = 1;
            }
            else
            {
                terrainArray[x, perlinHeight] = 3;
            }
        }
        return terrainArray;
    }

    public void RenderTerrainArray(int[,] terrainMap, Tilemap terrainTilemap)
    {
        for (int x = 0; x < BoundaryWidth; x++)
        {
            for (int y = 0; y < TerrainMaxHeight; y++)
            {
                if (terrainMap[x, y] == 1)
                {
                    SpawnTile(LowerTile, x, y);
                }

                if (terrainMap[x, y] == 2)
                {
                    SpawnTile(UpperTile, x, y);
                }

                if (terrainMap[x, y] == 3)
                {
                    SpawnTile(GrassTile, x, y);
                }
            }
        }
    }

    void TerrainGeneration()
    {
        for (int x = 0; x < BoundaryWidth; x++)
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
