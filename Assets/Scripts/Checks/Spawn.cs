using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spawn : MonoBehaviour
{
    [SerializeField] GameObject Spawner;

    private string sceneName;
    private bool isSpawned;
    private GameObject spawner;
    
    private void FixedUpdate()
    {
        Scene scene = SceneManager.GetActiveScene();

        if (scene != null)
        {
            switch (scene.name)
            {
                case "Cave":
                    sceneName = "Cave";
                    CheckPlayerExists("Player");
                    break;

                case "Company":
                    sceneName = "Company";
                    CheckPlayerExists("Player");
                    break;

                default:
                    return;
            }
        }
        else { return; }
    }

    private bool CheckPlayerExists(string tag)
    {
        GameObject player = GameObject.FindWithTag("Player");
        return isSpawned = player != null;
    }

    public string GetSceneName()
    {
        return sceneName;
    }

    public bool GetIsSpawned()
    {
        return isSpawned;
    }

    public GameObject GetSpawner()
    {
        return spawner;
    }
}
