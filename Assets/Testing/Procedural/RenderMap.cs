using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RenderMap : MonoBehaviour
{
    public int[,] TerrainArray;
    [SerializeField] Tilemap TerrainTilemap;
    [SerializeField] TileBase LowerTile;
    private ProceduralTerrainGeneration _ProceduralTerrainGenerationScript;

    void Start()
    {
        ProceduralTerrainGeneration.OnArrayGenerated += HandleArrayGenerated;
    }

    void Update()
    {
        
    }

    void HandleArrayGenerated(int[,] generatedArray)
    {
        TerrainArray = generatedArray;
    }

    public void RenderTerrainArray(int[,] terrainArray, Tilemap terrainTilemap)
    {
        _ProceduralTerrainGenerationScript = FindObjectOfType<ProceduralTerrainGeneration>();

        terrainTilemap.ClearAllTiles();
        int terrainWidth = terrainArray.GetUpperBound(0);
        int terrainHeight = terrainArray.GetUpperBound(1);

        for (int x = 0; x < terrainWidth; x++)
        {
            for (int y = 0; y < terrainHeight; y++)
            {
                if (terrainArray[x, y] == 0)
                {
                    SpawnTile(null, x, y);
                }

                if (terrainArray[x, y] == 1)
                {
                    SpawnTile(LowerTile, x, y);
                }
            }
        }
    }

    void SpawnTile(TileBase tile, int xPos, int yPos)
    {
        Vector3Int tilePosition = new Vector3Int(xPos, yPos, 0);
        TerrainTilemap.SetTile(tilePosition, tile);
    }
}
