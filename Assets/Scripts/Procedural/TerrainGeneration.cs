using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class TerrainGeneration : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField] private Tilemap terrainTilemap;

    [Header("Tiles")]
    [SerializeField] private TileBase caveTile;

    [Header("Check Points")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerSpawnPoint;
    [SerializeField] private GameObject playerTeleportPoint;

    [Header("Lights")]
    [SerializeField] private GameObject globalLightPrefab;

    [Header("Data Keeper")]
    [SerializeField] private GameInfo gameInfo;
    [SerializeField] private TerrainInfo terrainInfo;

    private new Light2D light;

    private void Awake()
    {
        light = globalLightPrefab.GetComponent<Light2D>();
        light.intensity = terrainInfo.TerrainLightIntensity;
    }

    private void Start()
    {
        GenerateTerrain();
        GeneratePlayerSpawnPoint();
        RenderTerrainArray();

        PlayerSpawner();
        SetPlayerSpawnPoint();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateTerrain();
            GeneratePlayerSpawnPoint();
            RenderTerrainArray();

            PlayerSpawner();
            SetPlayerSpawnPoint();
        }
    }

    private void GenerateTerrain()
    {
        IncreaseTerrainLevel();
        GenerateBaseTerrainArray();
        RandomWalkCave();
        GenerateCellularAutomata();
        SmoothMooreCellularAutomata();
    }

    private void IncreaseTerrainLevel()
    {
        if (terrainInfo.TerrainLevel != gameInfo.CaveLevel)
        {
            // Increase the terrain level by a customizable amount
            terrainInfo.TerrainLevel++;
            terrainInfo.Width += 15;
            terrainInfo.Height += 30;

            terrainInfo.TerrainLightIntensity -= 0.1f;
            terrainInfo.TerrainLightIntensity = Mathf.Max(terrainInfo.TerrainLightIntensity, 0);
        }
    }

    private void GenerateBaseTerrainArray()
    {
        int width = terrainInfo.Width;
        int height = terrainInfo.Height;

        terrainInfo.TerrainArray = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                terrainInfo.TerrainArray[x, y] = 1;
            }
        }

        terrainInfo.Size = width * height;
        terrainInfo.BoundaryMaxPoint = new Vector2(width - 1, height - 1);
        terrainInfo.BoundaryMinPoint = Vector2.zero;

        terrainInfo.ActualSize = terrainInfo.Size;
    }

    private void RandomWalkCave()
    {
        int terrainWidth = terrainInfo.TerrainArray.GetLength(0);
        int terrainHeight = terrainInfo.TerrainArray.GetLength(1);

        int currentX = UnityEngine.Random.Range(1, terrainWidth - 1);
        int currentY = terrainHeight - 1;
        terrainInfo.EntryPoint1 = new Vector2(currentX, currentY);

        int terrainSize = terrainHeight * terrainWidth;
        int playerMovingAreaCount = (terrainSize * terrainInfo.PlayerMovingAreaPercent) / 100;

        int blockCurrentFilledCount = 0;
        terrainInfo.TerrainArray[currentX, currentY] = 0;
        blockCurrentFilledCount++;

        while (blockCurrentFilledCount < playerMovingAreaCount)
        {
            int randDir = UnityEngine.Random.Range(0, 4);

            switch (randDir)
            {
                case 0:
                    if ((currentY + 1) < (terrainHeight - 1))
                    {
                        currentY++;

                        if (terrainInfo.TerrainArray[currentX, currentY] == 1)
                        {
                            terrainInfo.TerrainArray[currentX, currentY] = 0;
                            blockCurrentFilledCount++;
                        }
                    }
                    break;

                case 1:
                    if ((currentY - 1) > 1)
                    {
                        currentY--;

                        if (terrainInfo.TerrainArray[currentX, currentY] == 1)
                        {
                            terrainInfo.TerrainArray[currentX, currentY] = 0;
                            blockCurrentFilledCount++;
                        }
                    }
                    break;

                case 2:
                    if ((currentX + 1) < (terrainWidth - 1))
                    {
                        currentX++;

                        if (terrainInfo.TerrainArray[currentX, currentY] == 1)
                        {
                            terrainInfo.TerrainArray[currentX, currentY] = 0;
                            blockCurrentFilledCount++;
                        }
                    }
                    break;

                case 3:
                    if ((currentX - 1) > 1)
                    {
                        currentX--;

                        if (terrainInfo.TerrainArray[currentX, currentY] == 1)
                        {
                            terrainInfo.TerrainArray[currentX, currentY] = 0;
                            blockCurrentFilledCount++;
                        }
                    }
                    break;
            }
        }
    }

    private void GenerateCellularAutomata()
    {
        System.Random rand = new System.Random(terrainInfo.Seed.GetHashCode());

        int terrainWidth = terrainInfo.TerrainArray.GetUpperBound(0);
        int terrainHeight = terrainInfo.TerrainArray.GetUpperBound(1);

        int entranceX = 0;
        int entranceY = terrainHeight / 2;
        terrainInfo.EntryPoint2 = new Vector2(entranceX, entranceY);

        FloodFill(entranceX, entranceY, terrainInfo.FillChance, rand);
    }

    private void FloodFill(int currentX, int currentY, int fillPercent, System.Random rand)
    {
        int terrainWidth = terrainInfo.TerrainArray.GetUpperBound(0);
        int terrainHeight = terrainInfo.TerrainArray.GetUpperBound(1);

        if (currentX >= 0 && currentX < terrainWidth && currentY >= 0 && currentY < terrainHeight && terrainInfo.TerrainArray[currentX, currentY] == 0)
        {
            terrainInfo.TerrainArray[currentX, currentY] = (rand.Next(0, 100) < fillPercent) ? 1 : 0;

            FloodFill(currentX + 1, currentY, fillPercent, rand);
            FloodFill(currentX - 1, currentY, fillPercent, rand);
            FloodFill(currentX, currentY + 1, fillPercent, rand);
            FloodFill(currentX, currentY - 1, fillPercent, rand);
        }
    }

    private int GetNeighbourTilesCount(int mainTile_XPos, int mainTile_YPos)
    {
        int terrainWidth = terrainInfo.TerrainArray.GetLength(0);
        int terrainHeight = terrainInfo.TerrainArray.GetLength(1);

        int tileCount = 0;

        for (int x = mainTile_XPos - 1; x <= mainTile_XPos + 1; x++)
        {
            for (int y = mainTile_YPos - 1; y <= mainTile_YPos + 1; y++)
            {
                if ((x != mainTile_XPos || y != mainTile_YPos) &&
                    x >= 0 && x < terrainWidth && y >= 0 && y < terrainHeight)
                {
                    tileCount += terrainInfo.TerrainArray[x, y];
                }
            }
        }

        return tileCount;
    }

    private void SmoothMooreCellularAutomata()
    {
        int terrainWidth = terrainInfo.TerrainArray.GetUpperBound(0);
        int terrainHeight = terrainInfo.TerrainArray.GetUpperBound(1);

        for (int i = 0; i < terrainInfo.SmoothCount; i++)
        {
            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainHeight; y++)
                {
                    int surroundingTiles = GetNeighbourTilesCount(x, y);
                    bool isEdge = (x == 0 || x == terrainWidth - 1 || y == 0);

                    terrainInfo.TerrainArray[x, y] =
                        (isEdge || surroundingTiles > 8) ? 1 :
                        (surroundingTiles < 4) ? 0 :
                        terrainInfo.TerrainArray[x, y];
                }
            }
        }
    }

    private void GeneratePlayerSpawnPoint()
    {
        int terrainWidth = terrainInfo.TerrainArray.GetUpperBound(0);
        int terrainHeight = terrainInfo.TerrainArray.GetUpperBound(1);

        int x_LongestNullColumn = -1;
        int longestNullColumn = 0;

        for (int x = 0; x <= terrainWidth; x++)
        {
            if (terrainInfo.TerrainArray[x, terrainHeight] == 0)
            {
                int nullColumns = 0;
                bool stopCounting = false;

                for (int y = terrainHeight; y >= 0; y--)
                {
                    nullColumns = (terrainInfo.TerrainArray[x, y] == 0 && !stopCounting) ? nullColumns + 1 : nullColumns;
                    stopCounting = (terrainInfo.TerrainArray[x, y] != 0) ? true : stopCounting;
                }

                if (nullColumns > longestNullColumn)
                {
                    x_LongestNullColumn = x;
                    longestNullColumn = nullColumns;
                }
            }

            terrainInfo.TerrainArray[x, terrainHeight] = 0;
        }

        for (int x = 0; x <= terrainWidth; x++)
        {
            terrainInfo.TerrainArray[x, terrainHeight - 1] = (x == x_LongestNullColumn) ? 0 : 1;
        }

        terrainInfo.SpawnPoint.x = x_LongestNullColumn;
        terrainInfo.SpawnPoint.y = terrainHeight;
        terrainInfo.IsTerrainGenerated = true;
    }

    private void RenderTerrainArray()
    {
        terrainTilemap.ClearAllTiles();

        int terrainWidth = terrainInfo.TerrainArray.GetLength(0);
        int terrainHeight = terrainInfo.TerrainArray.GetLength(1);

        for (int x = 0; x < terrainWidth; x++)
        {
            for (int y = 0; y < terrainHeight; y++)
            {
                if (terrainInfo.TerrainArray[x, y] == 0)
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    terrainTilemap.SetTile(tilePosition, null);
                }
                else if (terrainInfo.TerrainArray[x, y] == 1)
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    terrainTilemap.SetTile(tilePosition, caveTile);
                }
            }
        }
    }

    private void PlayerSpawner()
    {
        playerSpawnPoint.transform.position = new Vector3(terrainInfo.SpawnPoint.x, terrainInfo.SpawnPoint.y - 2, 0);
        playerTeleportPoint.transform.position = new Vector3(terrainInfo.SpawnPoint.x + 0.5f, terrainInfo.SpawnPoint.y, 0);
    }

    private void SetPlayerSpawnPoint()
    {
        GameObject existedPlayer = GameObject.FindGameObjectWithTag("Player");

        if (existedPlayer == null)
        {
            GameObject playerInstance = Instantiate(player, playerSpawnPoint.transform.position, Quaternion.identity);
        }
        else
        {
            existedPlayer.transform.position = playerSpawnPoint.transform.position;
        }
    }
}