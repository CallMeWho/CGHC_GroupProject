using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TerrainUtils;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class ProceduralTerrainGeneration : MonoBehaviour
{
    [Header("Terrain Boundaries")]
    [SerializeField] int Width;
    [SerializeField] int Height;

    [Header("Terrain Types")]
    [SerializeField] LevelTypeOptions LevelType;

    [Header("Tutorial Terrain Settings")]
    [SerializeField] int SectionWidth = 0;
    [SerializeField] int MinimumSectionWidth;

    [Header("Gameplay Terrain Settings")]
    [Range(0, 100)] [SerializeField] int FloorPercent;
    [Range(0, 100)][SerializeField] int FillPercent;
    [SerializeField] bool EdgesAreWalls = true;
    [SerializeField] int Smoothness;

    /*
    [SerializeField] int MinInnerTileInterval;
    [SerializeField] int MaxInnerTileInterval;
    [Range(0,100)][SerializeField] float HeightValue, Smoothness;
    */
    private int[,] EmptyTerrainArray;
    public int[,] TerrainArray;
    //int RandomInnerTileHeight;

    [Header("Tilemaps")]
    [SerializeField] Tilemap TerrainTilemap;


    [Header("Tiles")]
    [SerializeField] TileBase UpperTile;
    [SerializeField] TileBase LowerTile;
    [SerializeField] TileBase GrassTile;

    [Header("Randomization")]
    [SerializeField] float Seed;

    public enum LevelTypeOptions
    {
        TutorialLevel = 1,
        GameplayLevel = 2,
    }

    void Start()
    {
        if (LevelType == LevelTypeOptions.TutorialLevel)
        {
            EmptyTerrainArray = GenerateEmptyTerrainArray(true);
            TerrainArray = RandomWalkTopSmoothed(EmptyTerrainArray, Seed, MinimumSectionWidth);
        }

        else if (LevelType == LevelTypeOptions.GameplayLevel)
        {
            EmptyTerrainArray = GenerateEmptyTerrainArray(false);
            TerrainArray = RandomWalkCave(EmptyTerrainArray, Seed, FloorPercent);
            TerrainArray = GenerateCellularAutomata(TerrainArray, Seed, FillPercent, EdgesAreWalls);
            TerrainArray = SmoothMooreCellularAutomata(TerrainArray, EdgesAreWalls, Smoothness);
        }

        RenderTerrainArray(EmptyTerrainArray, TerrainTilemap);
        Seed = 1;
        //Seed = Random.Range(-100000, 100000);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (LevelType == LevelTypeOptions.TutorialLevel)
            {
                EmptyTerrainArray = GenerateEmptyTerrainArray(true);
                TerrainArray = RandomWalkTopSmoothed(EmptyTerrainArray, Seed, MinimumSectionWidth);
            }

            else if (LevelType == LevelTypeOptions.GameplayLevel)
            {
                EmptyTerrainArray = GenerateEmptyTerrainArray(false);
                TerrainArray = RandomWalkCave(EmptyTerrainArray, Seed, FloorPercent);
                TerrainArray = GenerateCellularAutomata(TerrainArray, Seed, FillPercent, EdgesAreWalls);
                TerrainArray = SmoothMooreCellularAutomata(TerrainArray, EdgesAreWalls, Smoothness);
            }

            RenderTerrainArray(EmptyTerrainArray, TerrainTilemap);
            Seed = Random.Range(-100000, 100000);
        }
    }

    #region GeneratingArray
    // Phase 1: Generate the Empty Array
    public int[,] GenerateEmptyTerrainArray(bool isEmpty)
    {
        int terrainWidth = Width; 
        int terrainHeight = Height;
        int[,] emptyTerrainArray = new int[terrainWidth, terrainHeight];

        for (int x = 0; x < terrainWidth; x++)
        {
            for (int y = 0; y < terrainHeight; y++)
            {
                if (isEmpty)
                {
                    emptyTerrainArray[x, y] = 0;
                }
                else if (!isEmpty)
                {
                    emptyTerrainArray[x, y] = 1;
                }
            }
        }

        return emptyTerrainArray;
    }

    // Phase 2(A): Generate the Random Height Array
    // Tutorial Level
    public int[,] RandomWalkTopSmoothed(int[,] emptyTerrainArray, float seed, int minSectionWidth)
    {
        int terrainWidth = emptyTerrainArray.GetUpperBound(0);
        int terrainHeight = emptyTerrainArray.GetUpperBound(1);

        System.Random rand = new System.Random(seed.GetHashCode());
        int lastHeight = Random.Range(0, terrainHeight);
        int nextMove = 0;   //nextMove: to determine either next tile is to place either downwards or upwards

        for (int x = 0; x < terrainWidth; x++)
        {
            nextMove = rand.Next(2); //nextMove = 0 or 1, but not 2

            if (nextMove == 0 && lastHeight > 0 && SectionWidth > minSectionWidth)  //nextMove = 0, moving downwards
            {
                lastHeight--;
                SectionWidth = 0;
            }
            else if (nextMove == 1 && lastHeight < terrainHeight && SectionWidth > minSectionWidth) //nextMove = 1, moving upwards
            {
                lastHeight++;
                SectionWidth = 0;
            }
            SectionWidth++;

            for (int y = lastHeight; y >= 0; y--)
            {
                emptyTerrainArray[x, y] = 1;
            }
        }

        int[,] terrainArray = emptyTerrainArray;
        return terrainArray;
    }

    // Phase 2(B): Generate the Cave Array
    //Gameplay Level
    public int[,] RandomWalkCave(int[,] emptyTerrainArray, float seed, int requiredFloorPercent)
    {
        int terrainWidth = emptyTerrainArray.GetUpperBound(0);
        int terrainHeight = emptyTerrainArray.GetUpperBound(1);

        //Seed our random
        System.Random rand = new System.Random(seed.GetHashCode());

        //Define our start x position
        int floorX = rand.Next(1, terrainWidth - 1);
        //Define our start y position
        int floorY = rand.Next(1, terrainHeight - 1);
        //Determine our required floorAmount
        int reqFloorAmount = ((terrainHeight * terrainWidth) * requiredFloorPercent) / 100;
        //Used for our while loop, when this reaches our reqFloorAmount we will stop tunneling
        int floorCount = 0;

        //Set our start position to not be a tile (0 = no tile, 1 = tile)
        emptyTerrainArray[floorX, floorY] = 0;
        //Increase our floor count
        floorCount++;

        while (floorCount < reqFloorAmount)
        {
            //Determine our next direction
            int randDir = rand.Next(4);

            switch (randDir)
            {
                //Up
                case 0:
                    //Ensure that the edges are still tiles
                    if ((floorY + 1) < terrainHeight - 1)
                    {
                        //Move the y up one
                        floorY++;

                        //Check if that piece is currently still a tile
                        if (emptyTerrainArray[floorX, floorY] == 1)
                        {
                            //Change it to not a tile
                            emptyTerrainArray[floorX, floorY] = 0;
                            //Increase floor count
                            floorCount++;
                        }
                    }
                    break;

                //Down
                case 1:
                    //Ensure that the edges are still tiles
                    if ((floorY - 1) > 1)
                    {
                        //Move the y down one
                        floorY--;
                        //Check if that piece is currently still a tile
                        if (emptyTerrainArray[floorX, floorY] == 1)
                        {
                            //Change it to not a tile
                            emptyTerrainArray[floorX, floorY] = 0;
                            //Increase the floor count
                            floorCount++;
                        }
                    }
                    break;

                //Right
                case 2:
                    //Ensure that the edges are still tiles
                    if ((floorX + 1) < terrainWidth - 1)
                    {
                        //Move the x to the right
                        floorX++;
                        //Check if that piece is currently still a tile
                        if (emptyTerrainArray[floorX, floorY] == 1)
                        {
                            //Change it to not a tile
                            emptyTerrainArray[floorX, floorY] = 0;
                            //Increase the floor count
                            floorCount++;
                        }
                    }
                    break;

                //Left
                case 3:
                    //Ensure that the edges are still tiles
                    if ((floorX - 1) > 1)
                    {
                        //Move the x to the left
                        floorX--;
                        //Check if that piece is currently still a tile
                        if (emptyTerrainArray[floorX, floorY] == 1)
                        {
                            //Change it to not a tile
                            emptyTerrainArray[floorX, floorY] = 0;
                            //Increase the floor count
                            floorCount++;
                        }
                    }
                    break;
            }
        }

        //Return the updated map
        int[,] terrainArray = emptyTerrainArray;
        return terrainArray;
    }

    public int[,] GenerateCellularAutomata(int[,] terrainArray, float seed, int fillPercent, bool edgesAreWalls)
    {
        //Seed our random number generator
        System.Random rand = new System.Random(seed.GetHashCode());

        int terrainWidth = terrainArray.GetUpperBound(0);
        int terrainHeight = terrainArray.GetUpperBound(1);

        // Set the entrance and escape points
        int entranceX = terrainWidth / 2;
        int entranceY = 0;
        int escapeX = terrainWidth / 2;
        int escapeY = terrainHeight - 1;

        // Perform flood-fill algorithm from entrance point
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

    /*
    public int[,] GenerateCellularAutomata(int[,] terrainArray, float seed, int fillPercent, bool edgesAreWalls)
    {
        //Seed our random number generator
        System.Random rand = new System.Random(seed.GetHashCode());

        int terrainWidth = terrainArray.GetUpperBound(0);
        int terrainHeight = terrainArray.GetUpperBound(1);

        for (int x = 0; x < terrainWidth; x++)
        {
            for (int y = 0; y < terrainHeight; y++)
            {
                //If we have the edges set to be walls, ensure the cell is set to on (1)
                if (edgesAreWalls && (x == 0 || x == terrainWidth - 1 || y == 0 || y == terrainHeight - 1))
                {
                    terrainArray[x, y] = 1;
                }
                else
                {
                    //Randomly generate the grid
                    terrainArray[x, y] = (rand.Next(0, 100) < fillPercent) ? 1 : 0;
                }
            }
        }
        return terrainArray;
    }
    */

    public int GetNeighbourTilesCount(int[,] terrainArray, int mainTile_XPos, int mainTile_YPos, bool edgesAreWalls)
    {
        /* Moore Neighbourhood looks like this ('T' is the main tile, 'N' is the neighbour tiles)
         *
         * N N N
         * N T N
         * N N N
         *
         */

        int terrainWidth = terrainArray.GetUpperBound(0);
        int terrainHeight = terrainArray.GetUpperBound(1);
        
        int mX = mainTile_XPos;
        int mY = mainTile_YPos;
        // nX = neighTile_XPos = neighbourhood tile of X position
        // nY = neighTile_YPos = neighbourhood tile of Y position
        int tileCount = 0;

        for (int nX = mX - 1; nX <= mX + 1; nX++) // tiles at (x-1), (x), (x+1)
        {
            for (int nY = mY - 1; nY <= mY + 1; nY++)
            {
                if (nX >= 0 && nX < terrainWidth && nY >= 0 && nY < terrainHeight)  //tiles within boundaries
                {
                    if (nX != mX || nY != mY) //  tiles at only (x-1), (x+1)
                    {
                        tileCount += terrainArray[nX, nY];
                    }
                }
            }
        }
        return tileCount;
    }

    public int[,] SmoothMooreCellularAutomata(int[,] terrainArray, bool edgesAreWalls, int smoothCount)
    {
        int terrainWidth = terrainArray.GetUpperBound(0);
        int terrainHeight = terrainArray.GetUpperBound(1);

        for (int i = 0; i < smoothCount; i++)
        {
            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainHeight; y++)
                {
                    int surroundingTiles = GetNeighbourTilesCount(terrainArray, x, y, edgesAreWalls);

                    if (edgesAreWalls && (x == 0 || x == (terrainWidth - 1) || y == 0 || y == (terrainHeight - 1)))
                    {
                        //Set the edge to be a wall if we have edgesAreWalls to be true
                        terrainArray[x, y] = 1;
                    }
                    //The default moore rule requires more than 4 neighbours
                    else if (surroundingTiles > 4)
                    {
                        terrainArray[x, y] = 1;
                    }
                    else if (surroundingTiles < 4)
                    {
                        terrainArray[x, y] = 0;
                    }
                }
            }
        }
        //Return the modified map
        return terrainArray;
    }
    #endregion

    #region RenderingArray
    public void RenderTerrainArray(int[,] terrainArray, Tilemap terrainTilemap)
    {
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

                if (terrainArray[x, y] == 2)
                {
                    SpawnTile(UpperTile, x, y);
                }

                if (terrainArray[x, y] == 3)
                {
                    SpawnTile(GrassTile, x, y);
                }
            }
        }
    }

    void SpawnTile(TileBase tile, int xPos, int yPos)
    {
        Vector3Int tilePosition = new Vector3Int(xPos, yPos, 0);
        TerrainTilemap.SetTile(tilePosition, tile);
    }
    #endregion
}
