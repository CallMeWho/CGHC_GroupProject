using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    [Range(0, 100)] [SerializeField] int BlockPercent;
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

    public delegate void ArrayGeneratedDelegate(int[,] generatedArray);
    public static event ArrayGeneratedDelegate OnArrayGenerated;

    void Start()
    {
        RenderMap renderMap = FindObjectOfType<RenderMap>();

        if (LevelType == LevelTypeOptions.TutorialLevel)
        {
            EmptyTerrainArray = GenerateEmptyTerrainArray(true);
            TerrainArray = RandomWalkTopSmoothed(EmptyTerrainArray, Seed, MinimumSectionWidth);
        }

        else if (LevelType == LevelTypeOptions.GameplayLevel)
        {
            EmptyTerrainArray = GenerateEmptyTerrainArray(false);
            TerrainArray = RandomWalkCave(EmptyTerrainArray, Seed, BlockPercent);
            TerrainArray = GenerateCellularAutomata(TerrainArray, Seed, FillPercent, EdgesAreWalls);
            TerrainArray = SmoothMooreCellularAutomata(TerrainArray, EdgesAreWalls, Smoothness);

            OnArrayGenerated?.Invoke(TerrainArray);

        }

        //RenderTerrainArray(EmptyTerrainArray, TerrainTilemap);
        
        if (renderMap != null)
        {
            renderMap.RenderTerrainArray(renderMap.TerrainArray, TerrainTilemap);
        }
        
        //Seed = 1;
        Seed = Random.Range(-100000, 100000);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RenderMap renderMap = FindObjectOfType<RenderMap>();

            if (LevelType == LevelTypeOptions.TutorialLevel)
            {
                EmptyTerrainArray = GenerateEmptyTerrainArray(true);
                TerrainArray = RandomWalkTopSmoothed(EmptyTerrainArray, Seed, MinimumSectionWidth);
            }

            else if (LevelType == LevelTypeOptions.GameplayLevel)
            {
                EmptyTerrainArray = GenerateEmptyTerrainArray(false);
                TerrainArray = RandomWalkCave(EmptyTerrainArray, Seed, BlockPercent);
                TerrainArray = GenerateCellularAutomata(TerrainArray, Seed, FillPercent, EdgesAreWalls);
                TerrainArray = SmoothMooreCellularAutomata(TerrainArray, EdgesAreWalls, Smoothness);

                OnArrayGenerated?.Invoke(TerrainArray);

            }

            //RenderTerrainArray(EmptyTerrainArray, TerrainTilemap);

            if (renderMap != null)
            {
                renderMap.RenderTerrainArray(renderMap.TerrainArray, TerrainTilemap);
            }

            //Seed = 1;
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
    public int[,] RandomWalkCave(int[,] emptyTerrainArray, float seed, int reqBlocksPercent)
    {
        int terrainWidth = emptyTerrainArray.GetUpperBound(0);
        int terrainHeight = emptyTerrainArray.GetUpperBound(1);

        System.Random rand = new System.Random(seed.GetHashCode()); //rand: a sequence random numbers

        /* IMPORTANT
         * block = 0, player can move area;
         * tiles = 1, wall or something;
         * 
         * or you wont clearly know what codes below mean
         */

        int block_Xpos = rand.Next(1, terrainWidth - 1);    //block_Xpos: random value between 1 to terrainWidth
        //int block_Ypos = rand.Next(1, terrainHeight - 1); (TEMPT)
        int block_Ypos = terrainHeight;
        /* EXPLANATION
         * Next(nI, nE): 
         * the first parameter, nI will be the smallest value and included
         * the second parameter, nE will be the largest value but excluded, so the largest value generated will be (nE - 1)
         */

        // STEP 1: SET BLOCKS AMOUNT IN OUR TERRAIN

        int terrainSize = terrainHeight * terrainWidth;
        int reqBlocksCount = (terrainSize * reqBlocksPercent) / 100;
        //reqBlocksCount: block counts required in our terrain map, also the player moving area counts.
        /* QUESTION STILL
         * int reqBlocksCount = terrainSize * (reqBlocksPercent / 100);
         * if formula be this, random block generate not working, idk why????
         */

        int blockCurrentFilledCount = 0;    
        //right now terrain has no moving area for the player, but will add soon below.

        emptyTerrainArray[block_Xpos, block_Ypos] = 0;  
        //so now, one random block is selected and it becomes player moving area, we call it main block.
        blockCurrentFilledCount++;

        while (blockCurrentFilledCount < reqBlocksCount)
        {
            int randDir = rand.Next(4); //randDir: random value between 0 to 3, used to determine next direction

            switch (randDir)
            {
                //Up
                case 0:
                    if ((block_Ypos + 1) < (terrainHeight - 1))
                    /* EXPLANATION
                     * block_Ypos: main block y pos, so (block_Ypos + 1) is its upper position
                     * (terrainHeight - 1): the position under 2 blocks away from highest boundary height 
                     * 
                     * so, it means that the main block upper part, must under 2 blocks away from hbh
                     */

                    {
                        block_Ypos++;    //now we set the main block's upper block y pos (call it neigh upper block),
                                         //and it now sets to main block 

                        if (emptyTerrainArray[block_Xpos, block_Ypos] == 1) //if the new main block is tile
                        {
                            emptyTerrainArray[block_Xpos, block_Ypos] = 0;  //then we remove it to become block instead
                            blockCurrentFilledCount++;  //player can move area increased
                        }
                    }
                    break;

                //Down
                case 1:
                    if ((block_Ypos - 1) > 1)
                    /* EXPLANATION
                     * (block_Ypos - 1): main block lower position
                     * since lowest boundary height = 0, so (> 1) means its position has to be above 2 blocks away from lbh
                     */

                    {
                        block_Ypos--;   //set neigh lower block be new main block
                        
                        if (emptyTerrainArray[block_Xpos, block_Ypos] == 1) //check new main block
                        {
                            emptyTerrainArray[block_Xpos, block_Ypos] = 0;
                            blockCurrentFilledCount++;
                        }
                    }
                    break;

                //Right
                case 2:
                    if ((block_Xpos + 1) < terrainWidth - 1)
                    {
                        block_Xpos++;

                        if (emptyTerrainArray[block_Xpos, block_Ypos] == 1)
                        {
                            emptyTerrainArray[block_Xpos, block_Ypos] = 0;
                            blockCurrentFilledCount++;
                        }
                    }
                    break;

                //Left
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

                    //if (edgesAreWalls && (x == 0 || x == (terrainWidth - 1) || y == 0 || y == (terrainHeight - 1)))
                    if (edgesAreWalls && (x == 0 || x == (terrainWidth - 1) || y == 0))
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
    /*
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
    */
}
