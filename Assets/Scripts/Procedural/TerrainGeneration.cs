using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainGeneration : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField] Tilemap TerrainTilemap;

    [Header("Tiles")]
    [SerializeField] TileBase CaveTile;

    [Header("Data Keeper")]
    [SerializeField] public TerrainInfo TerrainInfo;

    private void Start()
    {
        GenerateBaseTerrainArray();
        RandomWalkCave(TerrainInfo.TerrainArray, TerrainInfo.Seed, TerrainInfo.PlayerMovingAreaPercent);
        GenerateCellularAutomata(TerrainInfo.TerrainArray, TerrainInfo.Seed, TerrainInfo.FillChance, TerrainInfo.WallEdges);
        SmoothMooreCellularAutomata(TerrainInfo.TerrainArray, TerrainInfo.WallEdges, TerrainInfo.SmoothCount);
        RenderTerrainArray(TerrainInfo.TerrainArray, TerrainTilemap);
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateBaseTerrainArray();
            RandomWalkCave(TerrainInfo.TerrainArray, TerrainInfo.Seed, TerrainInfo.PlayerMovingAreaPercent);
            GenerateCellularAutomata(TerrainInfo.TerrainArray, TerrainInfo.Seed, TerrainInfo.FillChance, TerrainInfo.WallEdges);
            SmoothMooreCellularAutomata(TerrainInfo.TerrainArray, TerrainInfo.WallEdges, TerrainInfo.SmoothCount);
            RenderTerrainArray(TerrainInfo.TerrainArray, TerrainTilemap);
        }
    }

    public void GenerateBaseTerrainArray()
    {
        TerrainInfo.TerrainArray = new int[TerrainInfo.Width, TerrainInfo.Height];
        
        for (int x = 0; x < TerrainInfo.Width; x++)
        {
            for (int y = 0; y < TerrainInfo.Height; y++)
            {
                TerrainInfo.TerrainArray[x, y] = 1;
            }
        }

        // get basic info
        TerrainInfo.Size = TerrainInfo.Width * TerrainInfo.Height;
        TerrainInfo.BoundaryMaxPoint = new Vector2(TerrainInfo.TerrainArray.GetUpperBound(0), TerrainInfo.TerrainArray.GetUpperBound(1));
        TerrainInfo.BoundaryMinPoint = new Vector2(TerrainInfo.TerrainArray.GetLowerBound(0), TerrainInfo.TerrainArray.GetLowerBound(1));
        TerrainInfo.ActualSize = TerrainInfo.TerrainArray.GetUpperBound(0) * TerrainInfo.TerrainArray.GetUpperBound(1);
    }

    public int[,] RandomWalkCave(int[,] emptyTerrainArray, float seed, int reqBlocksPercent)
    {
        TerrainInfo.Seed = Random.Range(-100000, 100000);
        //TerrainInfo.Seed = 1;
        System.Random rand = new System.Random(seed.GetHashCode()); //rand: a sequence random numbers

        int terrainWidth = TerrainInfo.TerrainArray.GetUpperBound(0);
        int terrainHeight = TerrainInfo.TerrainArray.GetUpperBound(1);

        int block_Xpos = rand.Next(1, terrainWidth - 1);    
        int block_Ypos = terrainHeight;
        TerrainInfo.EntryPoint1 = new Vector2 (block_Xpos, block_Ypos);
        
        // STEP 1: SET BLOCKS AMOUNT IN OUR TERRAIN

        int terrainSize = terrainHeight * terrainWidth;
        TerrainInfo.PlayerMovingAreaCount = (terrainSize * reqBlocksPercent) / 100;

        int blockCurrentFilledCount = 0;
        emptyTerrainArray[block_Xpos, block_Ypos] = 0;
        blockCurrentFilledCount++;

        while (blockCurrentFilledCount < TerrainInfo.PlayerMovingAreaCount)
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
        //int entranceX = terrainWidth / 2;
        //int entranceY = 0;

        int entranceX = terrainWidth/2;
        int entranceY = 0; ;
        TerrainInfo.EntryPoint2 = new Vector2(entranceX, entranceY);

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
                    SpawnTile(CaveTile, x, y);
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
