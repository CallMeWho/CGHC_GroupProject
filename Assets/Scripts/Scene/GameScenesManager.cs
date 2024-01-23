using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
//using UnityEditor;
using UnityEngine;
//using UnityEngine.InputSystem.iOS;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SceneElement
{
    public string SceneName;
    //public SceneAsset Scene;
}

public enum SceneSelection { Company, Cave, Shop, GameStartScene }

public class GameScenesManager : MonoBehaviour
{
    [SerializeField] private SceneSelection SceneNameSelection;
    [SerializeField] private GameObject PlayerSpawner;
    [SerializeField] private GameObject SceneLoader;
    [SerializeField] public SceneElement[] SceneArray;
    [SerializeField] public GameObject Player;

    [Header("Data Keeper")]
    [SerializeField] public GameInfo GameInfo;
    [SerializeField] public TerrainInfo TerrainInfo;

    public static GameScenesManager GameScenesManagerInstance;

    private bool hasStarted = false;
    private bool OpenMoveSource;

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
        if (!hasStarted && GameInfo.CurrentSceneName != "GameStartScene")
        {
            ResetGame();
            hasStarted = true;
        }

        SpawnPlayer(SceneNameSelection.ToString());

        //PlayBGM();
    }


    public void ResetGame()
    {
        GameInfo.CaveLevel = 0;
        GameInfo.MaxSpeed = 10;
        GameInfo.MaxAcceleration = 35;
        GameInfo.MaxOxygen = 100;
        GameInfo.OxygenRecoverRate = 100;
        GameInfo.OxygenConsumptionRate = 0.5f;
        GameInfo.MaxPressureCapacity = 100;
        GameInfo.PressureChangeRate = 0.5f;
        GameInfo.CurrentCredit = 0;
        GameInfo.Quota = 0;
        GameInfo.ShopCost = 100;
        GameInfo.LightIntensity = 2;
        GameInfo.LightInnerRadius = 3;
        GameInfo.LightOuterRadius = 5;
        GameInfo.HasInteracted = false;

        TerrainInfo.TerrainLevel = 0;
        TerrainInfo.Width = 100;
        TerrainInfo.Height = 100;
        TerrainInfo.TerrainLightIntensity = 1f;
    }

    private void Update()
    {
        GameInfo.CurrentSceneName = SceneManager.GetActiveScene().name;


        /*
        if (GameInfo.CurrentSceneName == "Company")
        {
            OpenMoveSource = false;
        }
        else if (GameInfo.CurrentSceneName == "Cave")
        {
            OpenMoveSource = true;
        }

        if (OpenMoveSource)
        {
            AudioManager.instance.moveSource.gameObject.SetActive(true);
            AudioManager.instance.PlayMoveSound("Walk");
        }
        else
        {
            AudioManager.instance.moveSource.gameObject.SetActive(false);
            AudioManager.instance.PlayMoveSound("BreatheSound");
        }
        */
    }

    public void LoadGameScene(string sceneName)
    {
        if (SceneManager.GetActiveScene().name != sceneName && SceneArray.Length > 0)
        {
            foreach (SceneElement sceneEle in SceneArray)
            {
                if (sceneEle.SceneName != null &&
                     sceneEle.SceneName == sceneName)
                {
                    SceneManager.LoadSceneAsync(sceneEle.SceneName);
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

        if (sceneName == "Shop")
        {
            return;
        }
    }
}