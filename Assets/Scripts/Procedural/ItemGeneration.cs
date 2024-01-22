using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGeneration : MonoBehaviour
{
    [SerializeField]
    public ItemTypes[] itemList;

    [Header("Data Keeper")]
    [SerializeField] public TerrainInfo TerrainInfo;

    private bool hasGeneratedTerrain = false;

    private void Update()
    {
        if (!hasGeneratedTerrain)
        {
            StartCoroutine(getterrain());
            hasGeneratedTerrain = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            hasGeneratedTerrain = false;
        }
    }

    private IEnumerator getterrain()
    {
        foreach (ItemTypes ele in itemList)
        {
            RemoveItems($"{ele.ItemName}Parent");
            GameObject itemParent = ItemProcessMethods.CreateEmptyFolder($"{ele.ItemName}Parent");
            int maxItems = (int)(TerrainInfo.TerrainArray.GetUpperBound(0) * TerrainInfo.TerrainArray.GetUpperBound(1) * ele.FillPercent);
            int itemSpawned = 0;

            for (int x = 0; x < TerrainInfo.TerrainArray.GetUpperBound(0); x++)
            {
                for (int y = 0; y < TerrainInfo.TerrainArray.GetUpperBound(1); y++)
                {
                    if (TerrainInfo.TerrainArray[x, y] == 0)
                    {
                        bool isAtEdge = ItemProcessMethods.GetAtEdge(TerrainInfo.TerrainArray, x, y);
                        float neighItemsCount = ItemProcessMethods.GetNeighItemsCount(TerrainInfo.TerrainArray, x, y, ele.DetectRadius, ele.ArrayIndex);

                        if (isAtEdge && neighItemsCount == 0 && itemSpawned < maxItems)
                            if (UnityEngine.Random.value < ele.SpawnProbability)
                            {
                                Vector3 spawnPosition = new Vector3(x + ele.SpawnOffset.x, y + ele.SpawnOffset.y, ele.SpawnOffset.z = 0.5f);
                                GameObject newItem = ItemProcessMethods.GetRandomSelectedPrefab(ele.PrefabList);
                                newItem = Instantiate(newItem, spawnPosition, Quaternion.identity);

                                newItem.transform.SetParent(itemParent.transform);
                                itemSpawned++;
                                TerrainInfo.TerrainArray[x, y] = 2; // use 2 for easy item detection future
                            }
                    }
                }
            }
        }

        yield return null;
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
        else { return; }
    }
}
