using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class TerrainGeneration : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField] Tilemap TerrainTilemap;

    [Header("Tiles")]
    [SerializeField] TileBase CaveTile;

    [Header("Check Points")]
    [SerializeField] GameObject Player;
    [SerializeField] GameObject PlayerSpawnPoint;
    [SerializeField] GameObject PlayerTeleportPoint;

    [Header("Lights")]
    [SerializeField] GameObject GlobalLightPrefab;

    [Header("Data Keeper")]
    [SerializeField] public GameInfo GameInfo;
    [SerializeField] public TerrainInfo TerrainInfo;

    private Light2D Light;

    private void Awake()
    {
        Light = GlobalLightPrefab.GetComponent<Light2D>();
        Light.intensity = TerrainInfo.TerrainLightIntensity;
    }

    private void Start()
    {
        if (TerrainInfo.TerrainLevel < GameInfo.CaveLevel)
        {
            TerrainInfo.TerrainLevel++;
            TerrainInfo.Width += 15;
            TerrainInfo.Height += 30;
            
            TerrainInfo.TerrainLightIntensity -= 0.1f;
            if (TerrainInfo.TerrainLightIntensity <= 0)
            {
                TerrainInfo.TerrainLightIntensity = 0;
            }
        }


        GenerateBaseTerrainArray();
        RandomWalkCave(TerrainInfo.TerrainArray, TerrainInfo.Seed, TerrainInfo.PlayerMovingAreaPercent);
        GenerateCellularAutomata(TerrainInfo.TerrainArray, TerrainInfo.Seed, TerrainInfo.FillChance, TerrainInfo.WallEdges);
        SmoothMooreCellularAutomata(TerrainInfo.TerrainArray, TerrainInfo.WallEdges, TerrainInfo.SmoothCount);
        GeneratePlayerSpawnPoint(TerrainInfo.TerrainArray);
        PlayerSpawner(TerrainInfo.TerrainArray);
        SetPlayerSpawnPoint(Player);
        RenderTerrainArray(TerrainInfo.TerrainArray, TerrainTilemap);

        GameObject entryPoint = GameObject.Find("entrypointtest");

        if (entryPoint != null)
        {
            entryPoint.transform.position = new Vector3(TerrainInfo.EntryPoint1.x + 0.5f, TerrainInfo.EntryPoint1.y - 0.5f, 0);
        }
        else
        {
            entryPoint = Instantiate(PlayerSpawnPoint, new Vector3(TerrainInfo.EntryPoint1.x + 0.5f, TerrainInfo.EntryPoint1.y - 0.5f, 0), Quaternion.identity);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (TerrainInfo.TerrainLevel != GameInfo.CaveLevel)
            {
                TerrainInfo.Width += 15;
                TerrainInfo.Height += 30;
                TerrainInfo.TerrainLevel++;
            }

            GenerateBaseTerrainArray();
            RandomWalkCave(TerrainInfo.TerrainArray, TerrainInfo.Seed, TerrainInfo.PlayerMovingAreaPercent);
            GenerateCellularAutomata(TerrainInfo.TerrainArray, TerrainInfo.Seed, TerrainInfo.FillChance, TerrainInfo.WallEdges);
            SmoothMooreCellularAutomata(TerrainInfo.TerrainArray, TerrainInfo.WallEdges, TerrainInfo.SmoothCount);
            GeneratePlayerSpawnPoint(TerrainInfo.TerrainArray);
            PlayerSpawner(TerrainInfo.TerrainArray);
            SetPlayerSpawnPoint(Player);
            RenderTerrainArray(TerrainInfo.TerrainArray, TerrainTilemap);

            GameObject entryPoint = GameObject.Find("EntryPoint(Clone)");
            
            if (entryPoint != null)
            {
                entryPoint.transform.position = new Vector3(TerrainInfo.EntryPoint1.x + 0.5f, TerrainInfo.EntryPoint1.y - 0.5f, 0);
            }
            else
            {
                entryPoint = Instantiate(PlayerSpawnPoint, new Vector3(TerrainInfo.EntryPoint1.x + 0.5f, TerrainInfo.EntryPoint1.y - 0.5f, 0), Quaternion.identity);
            }

        }
    }

    public void GenerateBaseTerrainArray()
    {
        TerrainInfo.IsTerrainGenerated = false;

        int width = TerrainInfo.Width;
        int height = TerrainInfo.Height;

        // Create a new 2D array for the terrain
        TerrainInfo.TerrainArray = new int[width, height];

        // Fill the terrain array with the default value of 1
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TerrainInfo.TerrainArray[x, y] = 1;
            }
        }

        // Get basic info about the terrain
        TerrainInfo.Size = width * height; 
        TerrainInfo.BoundaryMaxPoint = new Vector2(width - 1, height - 1); // Calculate the maximum boundary point of the terrain
        TerrainInfo.BoundaryMinPoint = Vector2.zero; // The minimum boundary point is always (0, 0)


        TerrainInfo.ActualSize = TerrainInfo.Size; // will remove soon
    }

    public int[,] RandomWalkCave(int[,] emptyTerrainArray, float seed, int reqBlocksPercent)
    {
        TerrainInfo.Seed = Random.Range(-100000, 100000);
        UnityEngine.Random.InitState(seed.GetHashCode()); // Initialize random number generator

        int terrainWidth = emptyTerrainArray.GetLength(0);
        int terrainHeight = emptyTerrainArray.GetLength(1);

        int block_Xpos = UnityEngine.Random.Range(1, terrainWidth - 1);
        int block_Ypos = terrainHeight - 1;
        TerrainInfo.EntryPoint1 = new Vector2(block_Xpos, block_Ypos);

        // Set null tile amounts in the terrain
        int terrainSize = terrainHeight * terrainWidth;
        int playerMovingAreaCount = (terrainSize * reqBlocksPercent) / 100;

        int blockCurrentFilledCount = 0;
        emptyTerrainArray[block_Xpos, block_Ypos] = 0;
        blockCurrentFilledCount++;

        while (blockCurrentFilledCount < playerMovingAreaCount)
        {
            int randDir = UnityEngine.Random.Range(0, 4); // Randomly choose a direction

            switch (randDir)
            {
                // Up
                case 0:
                    if ((block_Ypos + 1) < (terrainHeight - 1))
                    {
                        block_Ypos++;

                        if (emptyTerrainArray[block_Xpos, block_Ypos] == 1)
                        {
                            emptyTerrainArray[block_Xpos, block_Ypos] = 0;
                            blockCurrentFilledCount++;
                        }
                    }
                    break;

                // Down
                case 1:
                    if ((block_Ypos - 1) > 1)
                    {
                        block_Ypos--;

                        if (emptyTerrainArray[block_Xpos, block_Ypos] == 1)
                        {
                            emptyTerrainArray[block_Xpos, block_Ypos] = 0;
                            blockCurrentFilledCount++;
                        }
                    }
                    break;

                // Right
                case 2:
                    if ((block_Xpos + 1) < (terrainWidth - 1))
                    {
                        block_Xpos++;

                        if (emptyTerrainArray[block_Xpos, block_Ypos] == 1)
                        {
                            emptyTerrainArray[block_Xpos, block_Ypos] = 0;
                            blockCurrentFilledCount++;
                        }
                    }
                    break;

                // Left
                case 3:
                    if ((block_Xpos - 1) > 1)
                    {
                        block_Xpos--;

                        if (emptyTerrainArray[block_Xpos, block_Ypos] == 1)
                        {
                            emptyTerrainArray[block_Xpos, block_Ypos] = 0;
                            blockCurrentFilledCount++;
                        }
                    }
                    break;
            }
        }

        // Return the updated map
        return emptyTerrainArray;
    }

    public int[,] GenerateCellularAutomata(int[,] terrainArray, float seed, int fillPercent, bool edgesAreWalls)
    {
        // Seed our random number generator
        System.Random rand = new System.Random(seed.GetHashCode());

        // Get the dimensions of the terrain array
        int terrainWidth = terrainArray.GetUpperBound(0);
        int terrainHeight = terrainArray.GetUpperBound(1);

        // Set the entrance point
        int entranceX = 0;
        int entranceY = terrainHeight / 2; ;
        TerrainInfo.EntryPoint2 = new Vector2(entranceX, entranceY);

        // Perform flood-fill algorithm from the entrance point
        FloodFill(terrainArray, entranceX, entranceY, fillPercent, rand);

        return terrainArray;
    }

    private void FloodFill(int[,] terrainArray, int x, int y, int fillPercent, System.Random rand)
    {
        int terrainWidth = terrainArray.GetUpperBound(0);
        int terrainHeight = terrainArray.GetUpperBound(1);

        // Check if the current cell is within the terrain bounds and is not already filled
        if (x >= 0 && x < terrainWidth && y >= 0 && y < terrainHeight && terrainArray[x, y] == 0)
        {
            // Randomly fill the current cell based on fillPercent chance
            terrainArray[x, y] = (rand.Next(0, 100) < fillPercent) ? 1 : 0;

            // Recursively flood-fill the neighboring cells
            FloodFill(terrainArray, x + 1, y, fillPercent, rand);
            FloodFill(terrainArray, x - 1, y, fillPercent, rand);
            FloodFill(terrainArray, x, y + 1, fillPercent, rand);
            FloodFill(terrainArray, x, y - 1, fillPercent, rand);
        }
    }

    public int GetNeighbourTilesCount(int[,] terrainArray, int mainTile_XPos, int mainTile_YPos, bool edgesAreWalls)
    {
        int terrainWidth = terrainArray.GetUpperBound(0);
        int terrainHeight = terrainArray.GetUpperBound(1);

        int tileCount = 0;

        // Iterate only over the valid neighbor tiles
        for (int x = mainTile_XPos - 1; x <= mainTile_XPos + 1; x++)
        {
            for (int y = mainTile_YPos - 1; y <= mainTile_YPos + 1; y++)
            {
                // Exclude the main tile itself and check if the tile is within the terrain boundaries
                if ((x != mainTile_XPos || y != mainTile_YPos) && 
                    x >= 0 && x < terrainWidth && y >= 0 && y < terrainHeight)
                {
                    tileCount += terrainArray[x, y];
                }
            }
        }

        return tileCount;
    }

    public int[,] SmoothMooreCellularAutomata(int[,] terrainArray, bool edgesAreWalls, int smoothCount)
    {
        int terrainWidth = terrainArray.GetUpperBound(0);
        int terrainHeight = terrainArray.GetUpperBound(1);

        // Perform the specified number of smoothing iterations
        for (int i = 0; i < smoothCount; i++)
        {
            // Iterate over each tile in the terrain array
            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainHeight; y++)
                {
                    // Count the number of surrounding tiles
                    int surroundingTiles = GetNeighbourTilesCount(terrainArray, x, y, edgesAreWalls);

                    // Check if the tile is on the edge and edges are treated as walls
                    bool isEdge = edgesAreWalls && (x == 0 || x == terrainWidth - 1 || y == 0);

                    // Set the tile value based on the number of surrounding tiles and edge condition
                    terrainArray[x, y] = 
                        (isEdge || surroundingTiles > 8) ? 1 : // 6/7/8 is the best
                        (surroundingTiles < 4) ? 0 :    // 4 is the best, less than it will be blocky
                        terrainArray[x, y];    
                }
            }
        }

        return terrainArray;
    }

    public int[,] GeneratePlayerSpawnPoint(int[,] terrainArray)
    {
        int terrainWidth = terrainArray.GetUpperBound(0);
        int terrainHeight = terrainArray.GetUpperBound(1);

        int x_LongestNullColumn = -1; // x pos with the longest y = 0 column
        int longestNullColumn = 0;    // length of the longest y = 0 column

        for (int x = 0; x <= terrainWidth; x++)
        {
            if (terrainArray[x, terrainHeight] == 0)
            {
                int nullColumns = 0;
                bool stopCounting = false;

                for (int y = terrainHeight; y >= 0; y--)
                {
                    // Count null columns and stop when encountering a non-null tile
                    nullColumns = (terrainArray[x, y] == 0 && !stopCounting) ? nullColumns + 1 : nullColumns;
                    stopCounting = (terrainArray[x, y] != 0) ? true : stopCounting;
                }

                // Update longest null column
                if (nullColumns > longestNullColumn)
                {
                    x_LongestNullColumn = x;
                    longestNullColumn = nullColumns;
                }
            }

            terrainArray[x, terrainHeight] = 0; // Set last row to 0
        }

        // Set tiles in the second-to-last row based on the longest null column
        for (int x = 0; x <= terrainWidth; x++)
        {
            terrainArray[x, terrainHeight - 1] = (x == x_LongestNullColumn) ? 0 : 1;
        }

        // Update TerrainInfo with the spawn point
        TerrainInfo.SpawnPoint.x = x_LongestNullColumn;
        TerrainInfo.SpawnPoint.y = terrainHeight;
        TerrainInfo.IsTerrainGenerated = true;

        return terrainArray;
    }


    public void RenderTerrainArray(int[,] terrainArray, Tilemap terrainTilemap)
    {
        terrainTilemap.ClearAllTiles(); // Clear all existing tiles in the tilemap

        int terrainWidth = terrainArray.GetLength(0); 
        int terrainHeight = terrainArray.GetLength(1); 

        for (int x = 0; x < terrainWidth; x++)
        {
            for (int y = 0; y < terrainHeight; y++)
            {
                if (terrainArray[x, y] == 0)
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0); // Create a tile position vector
                    terrainTilemap.SetTile(tilePosition, null); // Set the tile to NULL
                }
                else if (terrainArray[x, y] == 1)
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0); 
                    terrainTilemap.SetTile(tilePosition, CaveTile); // Set the tile to CaveTile
                }
            }
        }
    }

    public void PlayerSpawner(int[,] terrainArray)
    {
        PlayerSpawnPoint.transform.position = new Vector3(TerrainInfo.SpawnPoint.x, TerrainInfo.SpawnPoint.y - 2, 0);
        PlayerTeleportPoint.transform.position = new Vector3(TerrainInfo.SpawnPoint.x + 0.5f, TerrainInfo.SpawnPoint.y, 0);
    }


    public void SetPlayerSpawnPoint(GameObject player)
    {
        GameObject existedPlayer = GameObject.FindGameObjectWithTag("Player");

        if (existedPlayer == null)
        {
            GameObject playerInstance = Instantiate(player, PlayerSpawnPoint.transform.position, Quaternion.identity);
        }
        else
        {
            existedPlayer.transform.position = PlayerSpawnPoint.transform.position;
        }
    }
}
