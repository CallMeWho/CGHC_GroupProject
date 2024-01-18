using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using static UnityEditor.Progress;

public class ProcItemGenerate : MonoBehaviour
{
    [Header("Chest")]
    [SerializeField] GameObject ChestPrefab;
    [SerializeField, Range(0, 100)] float ChestFillPercent = 0.05f;
    [SerializeField, Range(0, 100)] float ChestSpawnProbability = 0.05f;
    [SerializeField, Range(0, 100)] int ChestDetectRadius = 5;

    [Header("Diamond")]
    [SerializeField] GameObject Diamond;

    private ProceduralTerrainGeneration ptgScript;

    private int[,] terrainArray;

    private void Awake()
    {
        ptgScript = GetComponent<ProceduralTerrainGeneration>();
    }

    private void Start()
    {
        StartCoroutine(GenerateItems());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(GenerateItems());
        }
    }

    // generate items after terrain generation
    private IEnumerator GenerateItems() 
    {
        while (!ptgScript.isTerrainGenerated)
        {
            yield return null;
        }
        terrainArray = ptgScript.GetTerrainArray();
        RemoveItems();
        SpawnChest(terrainArray);
    }
    
    private void SpawnChest(int[,] terrainArray)
    {
        if (terrainArray != null)
        {
            GameObject itemParent = CreateEmptyFolder("ChestParent");

            int width = terrainArray.GetUpperBound(0);
            int height = terrainArray.GetUpperBound(1);

            int maxItems = (int) (width * height * ChestFillPercent);
            int itemSpawned = 0;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (terrainArray[x, y] == 0)
                    {
                        bool isAtEdge = GetAtEdge(terrainArray, x, y);
                        float neighItemsCount = GetNeighItemsCount(terrainArray, x, y, ChestDetectRadius);

                        if (isAtEdge && neighItemsCount == 0 && itemSpawned < maxItems)
                            if (Random.value < ChestSpawnProbability)
                            {
                                Vector3 spawnPosition = new Vector3(x + 0.5f, y + 0.3f, 0.5f);
                                GameObject newItem = Instantiate(ChestPrefab, spawnPosition, Quaternion.identity);

                                newItem.transform.SetParent(itemParent.transform);
                                itemSpawned++;
                                terrainArray[x, y] = 2; // use 2 for easy item detection future
                            }
                    }
                }
            }
        }
        else { return; }
    }

    #region Checks
    private bool GetAtEdge(int[,] terrainArray, int x, int y)
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

    private bool GetOnGround(int[,] terrainArray, int x, int y)
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

    private int GetNeighItemsCount(int[,] terrainArray, int x, int y, int neighRange)   //neighRange = radius
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
                        terrainArray[neigh_x, neigh_y] == 2)
                    {
                        neighItemsCount++;
                    }
                }
            }
        }

        return neighItemsCount;
    }
    #endregion

    public GameObject CreateEmptyFolder(string folderName)
    {
        GameObject itemParent = GameObject.Find(folderName);

        if (itemParent == null)
        {
            itemParent = new GameObject(folderName);
        }

        return itemParent;
    }

    private void RemoveItems()
    {
        GameObject itemParent = GameObject.Find("ChestParent");

        if (itemParent != null)
        {
            int childCount = itemParent.transform.childCount;

            for (int i = childCount - 1; i >= 0; i--)
            {
                Destroy(itemParent.transform.GetChild(i).gameObject);
            }
        }
    }
}
