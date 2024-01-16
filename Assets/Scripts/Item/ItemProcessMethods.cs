using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemProcessMethods
{
    public static GameObject CreateEmptyFolder(string folderName)
    {
        GameObject itemParent = GameObject.Find(folderName);

        if (itemParent == null)
        {
            itemParent = new GameObject(folderName);
        }

        return itemParent;
    }

    public static bool GetAtEdge(int[,] terrainArray, int x, int y)
    {
        bool isAtEdge = false;

        int width = terrainArray.GetUpperBound(0);
        int height = terrainArray.GetUpperBound(1);

        if (x > 0 && x < width && y > 0 && y < height && terrainArray[x, y] == 0)   // check if inside boundary & empty
        {
            if (terrainArray[x, y - 1] == 1 && terrainArray[x, y + 1] == 0 && (terrainArray[x + 1, y] == 1 || terrainArray[x - 1, y] == 1))
            {
                isAtEdge = true;
            }
        }

        return isAtEdge;
    }

    public static bool GetOnGround(int[,] terrainArray, int x, int y)
    {
        bool isOnGround = false;

        int width = terrainArray.GetUpperBound(0);
        int height = terrainArray.GetUpperBound(1);

        if (x > 0 && x < width && y > 0 && y < height && terrainArray[x, y] == 0)
        {
            if (terrainArray[x, y - 1] == 1 && terrainArray[x, y + 1] == 0)
            {
                isOnGround = true;
            }
        }

        return isOnGround;
    }

    public static int GetNeighItemsCount(int[,] terrainArray, int x, int y, int neighRange, int index)   //neighRange = radius
    {
        int neighItemsCount = 0;

        int width = terrainArray.GetUpperBound(0);
        int height = terrainArray.GetUpperBound(1);

        if (x > 0 && x < width && y > 0 && y < height && terrainArray[x, y] == 0)
        {
            for (int i = -neighRange; i <= neighRange; i++)
            {
                int neigh_x = x + i;

                for (int j = -neighRange; j <= neighRange; j++)
                {
                    int neigh_y = y + j;

                    if (neigh_x > 0 && neigh_x < width &&
                        neigh_y > 0 && neigh_y < height &&
                        terrainArray[neigh_x, neigh_y] == index)
                    {
                        neighItemsCount++;
                    }
                }
            }
        }

        return neighItemsCount;
    }

    public static GameObject GetRandomSelectedPrefab(GameObject[] prefabsList)
    {
        int randomIndex = UnityEngine.Random.Range(0, prefabsList.Length);
        GameObject selectedPrefab = prefabsList[randomIndex];

        return selectedPrefab;
    }
}
