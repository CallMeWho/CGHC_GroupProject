using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.TerrainTools.PaintContext;

public class ItemGeneration : MonoBehaviour
{
    [SerializeField] private ItemTypes[] itemList;

    [Header("Data Keeper")]
    [SerializeField] private TerrainInfo terrainInfo;

    private bool hasGeneratedTerrain = false;

    private void Update()
    {
        if (!hasGeneratedTerrain)
        {
            StartCoroutine(GetTerrain());
            hasGeneratedTerrain = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            hasGeneratedTerrain = false;
        }
    }

    private IEnumerator GetTerrain()
    {
        try
        {
            foreach (ItemTypes item in itemList)
            {
                RemoveItems($"{item.ItemName}Parent");
                GameObject itemParent = ItemProcessMethods.CreateEmptyFolder($"{item.ItemName}Parent");

                GenerateItems(item, itemParent);
            }
        }
        catch (ArgumentNullException e)
        {
            Debug.LogError($"An argument null exception occurred: {e.Message}");
        }
        catch (InvalidOperationException e)
        {
            Debug.LogError($"An invalid operation exception occurred: {e.Message}");
        }
        catch (Exception e)
        {
            Debug.LogError($"An error occurred while generating items: {e.Message}");
        }

        yield return null;
    }

    private void GenerateItems(ItemTypes item, GameObject itemParent)
    {
        int maxX = terrainInfo.TerrainArray.GetUpperBound(0);
        int maxY = terrainInfo.TerrainArray.GetUpperBound(1);
        int maxItems = (int)(maxX * maxY * item.FillPercent);
        int itemSpawned = 0;

        for (int x = 0; x < maxX && itemSpawned < maxItems; x++)
        {
            for (int y = 0; y < maxY && itemSpawned < maxItems; y++)
            {
                if (terrainInfo.TerrainArray[x, y] == 0)
                {
                    bool isAtEdge = ItemProcessMethods.GetAtEdge(terrainInfo.TerrainArray, x, y);
                    float neighItemsCount = ItemProcessMethods.GetNeighItemsCount(terrainInfo.TerrainArray, x, y, item.DetectRadius, item.ArrayIndex);

                    if (isAtEdge && neighItemsCount == 0 && UnityEngine.Random.value < item.SpawnProbability)
                    {
                        Vector3 spawnPosition = new Vector3(x + item.SpawnOffset.x, y + item.SpawnOffset.y, item.SpawnOffset.z);
                        GameObject newItem = ItemProcessMethods.GetRandomSelectedPrefab(item.PrefabList);
                        Instantiate(newItem, spawnPosition, Quaternion.identity, itemParent.transform);

                        itemSpawned++;
                        terrainInfo.TerrainArray[x, y] = 2; // Use 2 for easy item detection in the future
                    }
                }
            }
        }
    }

    private void RemoveItems(string folderName)
    {
        GameObject itemParent = GameObject.Find(folderName);

        if (itemParent != null)
        {
            int childCount = itemParent.transform.childCount;

            for (int i = childCount - 1; i >= 0; i--)
            {
                Destroy(itemParent.transform.GetChild(i).gameObject);   //cannot move to IPMethods because of Destroy function
            }
        }
        else return;
    }
}
