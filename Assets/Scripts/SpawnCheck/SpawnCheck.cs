using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCheck : MonoBehaviour
{
    public GameObject playerPrefab;

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Destroy(player);
        }
        
        GameObject spawner = GameObject.Find("PlayerSpawner");
        //Instantiate(playerPrefab, spawner.transform.position, Quaternion.identity);
        Instantiate(playerPrefab);
    }

    private void Update()
    {
        for (int i = 0; i < 1; i++)
        {
            Instantiate(playerPrefab);
        }
    }
}
