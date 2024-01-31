using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCheck : MonoBehaviour
{
    public GameObject playerPrefab;

    private void Start()
    {
        DestroyExistingPlayer();
    }

    private void Update()
    {
        InstantiatePlayer();
    }

    // destroys any existing player object in the scene
    private void DestroyExistingPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Destroy(player);
        }
    }

    // instantiates a new player object in the scene
    private void InstantiatePlayer()
    {
        Instantiate(playerPrefab);
    }
}
