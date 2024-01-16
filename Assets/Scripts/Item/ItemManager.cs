using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemManager : MonoBehaviour
{
    [SerializeField] 
    public ItemTypes[] itemList;

    private ProceduralTerrainGeneration ptgScript;
    private int[,] terrainArray;
    private int terrainWidth;
    private int terrainHeight;

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
            StartCoroutine(ReloadScene());
        }
    }

    private IEnumerator ReloadScene()
    {
        // Get the current scene
        Scene currentScene = SceneManager.GetActiveScene();

        yield return null;

        // Load the current scene again to reset it
        SceneManager.LoadScene(currentScene.name, LoadSceneMode.Single);
    }

    private IEnumerator GenerateItems()
    {
        while (!ptgScript.isTerrainGenerated)
        {
            yield return null;
        }

        terrainArray = ptgScript.GetTerrainArray();

        foreach (ItemTypes ele in itemList)
        {
            string folderName = $"{ele.ItemName}Parent";
            RemoveItems(folderName);

            ItemTypes item = GetItem(ele.ItemName); 
            SpawnItem(item);
        }
    }

        private ItemTypes GetItem(string itemName)
    {
        ItemTypes ele = Array.Find(itemList, x => 
        x.ItemName == itemName &&
        x.PrefabList != null &&
        x.SpawnProbability > 0 && 
        x.DetectRadius > 0 &&
        x.ArrayIndex > 1);

        return ele;
    }

    private void SpawnItem(ItemTypes ele)
    {
        if (ele != null)
        {
            GameObject itemParent = ItemProcessMethods.CreateEmptyFolder($"{ele.ItemName}Parent");

            terrainWidth = terrainArray.GetUpperBound(0);
            terrainHeight = terrainArray.GetUpperBound(1);

            int maxItems = (int)(terrainWidth * terrainHeight * ele.FillPercent);
            int itemSpawned = 0;

            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainHeight; y++)
                {
                    if (terrainArray[x, y] == 0)
                    {
                        bool isAtEdge = ItemProcessMethods.GetAtEdge(terrainArray, x, y);
                        float neighItemsCount = ItemProcessMethods.GetNeighItemsCount(terrainArray, x, y, ele.DetectRadius, ele.ArrayIndex);

                        if (isAtEdge && neighItemsCount == 0 && itemSpawned < maxItems)
                            if (UnityEngine.Random.value < ele.SpawnProbability)
                            {
                                Vector3 spawnPosition = new Vector3(x + 0.5f, y + 0.3f, 0);
                                GameObject newItem = ItemProcessMethods.GetRandomSelectedPrefab(ele.PrefabList);
                                newItem = Instantiate(newItem, spawnPosition, Quaternion.identity);

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
