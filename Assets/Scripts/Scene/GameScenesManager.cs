using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SceneElement
{
    public string SceneName;
    public SceneAsset Scene;
}

public enum SceneSelection { Company, Cave }

public class GameScenesManager : MonoBehaviour
{
    [SerializeField] private SceneSelection SceneNameSelection;
    [SerializeField] private GameObject PlayerSpawner;
    [SerializeField] private GameObject SceneLoader;
    [SerializeField] public SceneElement[] SceneArray;
    [SerializeField] public GameObject Player;

    [Header("Data Keeper")]
    [SerializeField] public GameInfo GameInfo;

    public static GameScenesManager GameScenesManagerInstance;

    private void Awake()
    {
        if (GameScenesManagerInstance == null)
        {
            GameScenesManagerInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SpawnPlayer(SceneNameSelection.ToString());
    }

    private void Update()
    {
        GameInfo.CurrentSceneName = SceneManager.GetActiveScene().name;
    }

    public void LoadGameScene(string sceneName)
    {
        if (SceneManager.GetActiveScene().name != sceneName && SceneArray.Length > 0)
        {
            foreach (SceneElement sceneEle in SceneArray)
            {
                if (sceneEle.SceneName != null && sceneEle.Scene != null &&
                     sceneEle.SceneName == sceneName)
                {
                    SceneManager.LoadScene(sceneEle.Scene.name);
                    break;
                }
            }
        }
        else { return; }
    }

    private void SpawnPlayer(string sceneName)
    {
        if (sceneName == "Company")
        {
            bool isSpawnerExist = GameObject.Find(PlayerSpawner.name);
            bool isPlayerExist = GameObject.Find(Player.name);

            if (isSpawnerExist)
            {
                if (!isPlayerExist)
                {
                    Instantiate(Player, PlayerSpawner.transform.position, Quaternion.identity);
                }

                Player.transform.position = PlayerSpawner.transform.position;
            }
        }
    }
}