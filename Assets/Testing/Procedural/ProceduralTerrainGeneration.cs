using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TerrainUtils;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class ProceduralTerrainGeneration : MonoBehaviour
{
    [Header("Terrain")]
    [SerializeField] int Width;
    [SerializeField] int Height;
    [SerializeField] int Interval;

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

    void Start()
    {
        EmptyTerrainArray = GenerateEmptyTerrainArray();

        //TerrainArray = GenerateSmoothTerrainArray(EmptyTerrainArray, Interval);
        //TerrainArray = GenerateRandomWalkTopArray(EmptyTerrainArray, Seed);
        TerrainArray = RandomWalkTopSmoothed(EmptyTerrainArray, Seed, 10);

        //TerrainArray = PerlinNoiseCave(TerrainArray, 0.05f, true);
        //TerrainArray = RandomWalkCave(TerrainArray, Seed, 35);
        TerrainArray = GenerateCellularAutomata(TerrainArray, Seed, 50, true);
        TerrainArray = SmoothMooreCellularAutomata(TerrainArray, true, 20);


        RenderTerrainArray(EmptyTerrainArray, TerrainTilemap);

        Seed = 1;
        //Seed = Random.Range(-100000, 100000);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            
        }
    }

    #region GeneratingArray
    // Phase 1: Generate the Empty Array
    public int[,] GenerateEmptyTerrainArray()
    {
        int terrainWidth = Width; 
        int terrainHeight = Height;
        int[,] emptyTerrainArray = new int[terrainWidth, terrainHeight];

        for (int x = 0; x < terrainWidth; x++)
        {
            for (int y = 0; y < terrainHeight; y++)
            {
                emptyTerrainArray[x, y] = 0;
            }
        }

        return emptyTerrainArray;
    }

    // Phase 2(A): Generate the Random Height Array
    public int[,] GenerateTerrainArray(int[,] emptyTerrainArray)
    {
        int terrainWidth = emptyTerrainArray.GetUpperBound(0);
        int terrainHeight = emptyTerrainArray.GetUpperBound(1);
        float noiseReductionValue = 0.5f;

        for (int x = 0; x < terrainWidth; x++)
        {
            float noise = Mathf.PerlinNoise(x * 0.1f, Seed) - noiseReductionValue;  //this 0.1f is a MUST, or else no random num
            /* EXPLANATION
             * since noise value will be between 0 to 1, and because of noiseReductionValue 0.5f, it will finally be between -0.5 to 0.5
             * - means the terrain will be lower
            */

            int noiseHeight = Mathf.FloorToInt(noise * terrainHeight);
            /* EXPLANATION
             * since noice is between -0.5 to 0.5, you can think of it like the percentage be between -50% to 50%
             * so the noiseHeight can be negative
            */

            int averageHeight = terrainHeight / 2;
            int newHeight = averageHeight + noiseHeight;
            /* EXPLANATION
             * because of noiseHeight, newHeight will sometimes lower or higher than averageHeight,
             * and this newHeight, is part of our terrain
            */

            for (int y = newHeight; y >= 0; y--) 
            {
                emptyTerrainArray[x, y] = 1;
            }
        }

        int[,] terrainArray = emptyTerrainArray;
        return terrainArray;
    }

    public int[,] GenerateSmoothTerrainArray(int[,] emptyTerrainArray, int interval)
    {
        int terrainWidth = emptyTerrainArray.GetUpperBound(0);
        int terrainHeight = emptyTerrainArray.GetUpperBound(1);

        if (interval > 1) // means if got interval
        {
            float noiseReductionValue = 0.5f;

            List<int> listPointsX = new List<int>();
            List<int> listPointsY = new List<int>();

            for (int x = 0; x < terrainWidth; x += interval) 
            /* EXPLANATION
             * here, instead of make every x has random height, we set an interval between each 2 x, only every interval start point and 
             * end point will set with their random heights, the middle of them will not.
             * EXAMPLE
             * if interval = 3, at x = 0 and x = 3 have set their random height, but their middle which are x = 1, 2 no set random height
             */
            {
                float noise = Mathf.PerlinNoise(x * 0.1f, (Seed * noiseReductionValue));    
                int noiseHeight = Mathf.FloorToInt(noise * terrainHeight);

                listPointsY.Add(noiseHeight);
                listPointsX.Add(x);
            }

            int noiseY_Counts = listPointsY.Count;
            for (int i = 1; i < noiseY_Counts; i++)    //start at i = 1, not i = 0
            {
                /* EXPLANATION
                 * since we set interval start and end points have their random height, but the middle of them dont have yet, 
                 * and here we pick these two points and find the height difference between them,
                 * how much interval then divide how much times the height difference evenly => heightChange
                 */
                Vector2Int currentPos = new Vector2Int(listPointsX[i], listPointsY[i]);
                Vector2Int previousPos = new Vector2Int(listPointsX[i - 1], listPointsY[i - 1]);   //because of this
                Vector2 diff = currentPos - previousPos;

                int heightChange = Mathf.RoundToInt(diff.y / interval);
                int initialDistance = Mathf.RoundToInt(previousPos.x);
                int initialHeight = Mathf.RoundToInt(previousPos.y);
                int finalDistance = Mathf.RoundToInt(currentPos.x);
                int finalHeight = Mathf.RoundToInt(currentPos.y);

                for (int x = initialDistance; x < finalDistance; x++) 
                {
                    for (int y = finalHeight; y > 0; y--)
                    {
                        emptyTerrainArray[x, y] = 1;
                    }
                    initialHeight += heightChange;
                }
            }
        }
        else
        {
            emptyTerrainArray = GenerateTerrainArray(emptyTerrainArray);
        }

        int[,] terrainArray = emptyTerrainArray;
        return terrainArray;
    }

    // Phase 2(B): Generate the Random Height Array
    public int[,] GenerateRandomWalkTopArray(int[,] emptyTerrainArray, float seed)
    {
        int terrainWidth = emptyTerrainArray.GetUpperBound(0);
        int terrainHeight = emptyTerrainArray.GetUpperBound(1);

        System.Random rand = new System.Random(seed.GetHashCode());
        int lastHeight = Random.Range(0, terrainHeight);

        for (int x = 0; x < terrainWidth; x++)
        {
            int nextMove = rand.Next(2); //nextMove = 0 or 1, but not 2
            if (nextMove == 0 && lastHeight > 2) 
            {
                lastHeight--;
            }
            else if (nextMove == 1 && lastHeight < terrainHeight - 2)
            {
                lastHeight++;
            }

            for (int y = lastHeight; y >= 0; y--) 
            {
                emptyTerrainArray[x,y] = 1;
            }
        }

        int[,] terrainArray = emptyTerrainArray;
        return terrainArray;
    }
    
    public int[,] RandomWalkTopSmoothed(int[,] emptyTerrainArray, float seed, int minSectionWidth)
    {
        int terrainWidth = emptyTerrainArray.GetUpperBound(0);
        int terrainHeight = emptyTerrainArray.GetUpperBound(1);

        System.Random rand = new System.Random(seed.GetHashCode());
        int lastHeight = Random.Range(0, terrainHeight);
        int nextMove = 0;
        int sectionWidth = 0;

        for (int x = 0; x < terrainWidth; x++)
        {
            nextMove = rand.Next(2); //nextMove = 0 or 1, but not 2
            if (nextMove == 0 && lastHeight > 0 && sectionWidth > minSectionWidth)
            {
                lastHeight--;
                sectionWidth = 0;
            }
            else if (nextMove == 1 && lastHeight < terrainHeight && sectionWidth > minSectionWidth) 
            {
                lastHeight++;
                sectionWidth = 0;
            }
            sectionWidth++;

            for (int y = lastHeight; y >= 0; y--)
            {
                emptyTerrainArray[x, y] = 1;
            }
        }

        int[,] terrainArray = emptyTerrainArray;
        return terrainArray;
    }

    // Phase 3(A): Generate the Cave Array
    public int[,] PerlinNoiseCave(int[,] map, float modifier, bool edgesAreWalls)
    {
        int newPoint;
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {

                if (edgesAreWalls && (x == 0 || y == 0 || x == map.GetUpperBound(0) - 1 || y == map.GetUpperBound(1) - 1))
                {
                    map[x, y] = 1; //Keep the edges as walls
                }
                else
                {
                    //Generate a new point using Perlin noise, then round it to a value of either 0 or 1
                    newPoint = Mathf.RoundToInt(Mathf.PerlinNoise(x * modifier, y * modifier)); //modifier value between 0 to 0.5
                    map[x, y] = newPoint;
                }
            }
        }
        return map;
    }

    // Phase 3(B): Generate the Cave Array (Fixing)
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

    // Phase 3(C): Generate the Cave Array
    public int[,] RandomWalkCave(int[,] map, float seed, int requiredFloorPercent)
    {
        //Seed our random
        System.Random rand = new System.Random(seed.GetHashCode());

        //Define our start x position
        int floorX = rand.Next(1, map.GetUpperBound(0) - 1);
        //Define our start y position
        int floorY = rand.Next(1, map.GetUpperBound(1) - 1);
        //Determine our required floorAmount
        int reqFloorAmount = ((map.GetUpperBound(1) * map.GetUpperBound(0)) * requiredFloorPercent) / 100;
        //Used for our while loop, when this reaches our reqFloorAmount we will stop tunneling
        int floorCount = 0;

        //Set our start position to not be a tile (0 = no tile, 1 = tile)
        map[floorX, floorY] = 0;
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
                    if ((floorY + 1) < map.GetUpperBound(1) - 1)
                    {
                        //Move the y up one
                        floorY++;

                        //Check if that piece is currently still a tile
                        if (map[floorX, floorY] == 1)
                        {
                            //Change it to not a tile
                            map[floorX, floorY] = 0;
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
                        if (map[floorX, floorY] == 1)
                        {
                            //Change it to not a tile
                            map[floorX, floorY] = 0;
                            //Increase the floor count
                            floorCount++;
                        }
                    }
                    break;
                //Right
                case 2:
                    //Ensure that the edges are still tiles
                    if ((floorX + 1) < map.GetUpperBound(0) - 1)
                    {
                        //Move the x to the right
                        floorX++;
                        //Check if that piece is currently still a tile
                        if (map[floorX, floorY] == 1)
                        {
                            //Change it to not a tile
                            map[floorX, floorY] = 0;
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
                        if (map[floorX, floorY] == 1)
                        {
                            //Change it to not a tile
                            map[floorX, floorY] = 0;
                            //Increase the floor count
                            floorCount++;
                        }
                    }
                    break;
            }
        }
        //Return the updated map
        return map;
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
