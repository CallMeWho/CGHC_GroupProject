using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using static UnityEditor.Progress;

public class ProcItemGenerate : MonoBehaviour
{
    [SerializeField] GameObject Item;
    public Transform itemParent;

    private ProceduralTerrainGeneration ptgScript;
    private int[,] terrainArray;
    private GameObject itemsParent;

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

    private IEnumerator GenerateItems() //generate items after terrain generation
    {
        while (!ptgScript.isTerrainGenerated)
        {
            yield return null;
        }

        terrainArray = ptgScript.GetTerrainArray();

        RemoveItems();
        SpawnItems(terrainArray);
    }
    
    private void SpawnItems(int[,] terrainArray)
    {
        if (terrainArray != null)
        {
            int width = terrainArray.GetUpperBound(0);
            int height = terrainArray.GetUpperBound(1);

            GameObject itemParent = GameObject.Find("ItemParent");
            if (itemParent == null)
            {
                itemParent = new GameObject("ItemParent");
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (terrainArray[x, y] == 0)
                    {
                        bool isAtEdge = GetAtEdge(terrainArray, x, y);

                        if (isAtEdge)
                        {
                            Vector3 spawnPosition = new Vector3(x + 0.5f, y + 0.3f, 0);
                            GameObject newItem = Instantiate(Item, spawnPosition, Quaternion.identity);

                            
                            newItem.transform.SetParent(itemParent.transform);
                        }
                    }
                }
            }

            itemParent = itemsParent;
        }
        else { return; }
    }

    private bool GetAtEdge(int[,] terrainArray, int x, int y)
    {
        bool isAtEdge = false;

        int width = terrainArray.GetUpperBound(0);
        int height = terrainArray.GetUpperBound(1);

        if (x > 0 && x < width && y > 0 && y < height && terrainArray[x, y] == 0)
        {
            if (terrainArray[x, y - 1] == 1 && terrainArray[x, y + 1] == 0 && (terrainArray[x + 1, y] == 1 || terrainArray[x - 1, y] == 1))
            {
                isAtEdge = true;
            }
        }

        return isAtEdge;
    }

    private void RemoveItems()
    {
        GameObject itemParent = GameObject.Find("ItemParent");
        if (itemParent != null)
        {
            Destroy(itemParent);
        }
    }
}
