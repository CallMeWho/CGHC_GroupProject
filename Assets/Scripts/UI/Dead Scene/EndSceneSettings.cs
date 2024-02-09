using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndSceneSettings : MonoBehaviour
{
    public static EndSceneSettings EndSceneSettingsInstance;
    private GameObject player;
    private GameInfo gameInfo;

    private void Start()
    {
        gameInfo = FindAnyObjectByType<GameScenesManager>().GameInfo;
        player = GameObject.FindWithTag("Player");
        if (gameInfo == null || player == null) return;

        // hide dead screen
        gameObject.SetActive(false);

        // set sorting layer
        int playerSortLayer = player.GetComponent<Renderer>().sortingOrder;
        gameObject.GetComponent<Canvas>().sortingOrder = playerSortLayer + 1;   // end scene is set at front player

        // set result text
        TextMeshProUGUI textResult = GetComponentInChildren<TextMeshProUGUI>();
        string days = (gameInfo.CaveLevel <= 1) ? "Day" : "Days";
        textResult.text = $"YOU ARE FIRED AFTER {gameInfo.CaveLevel} {days}";
    }

    // update not working in canvas here, dk why

    public static void BackToMenu()
    {
        AudioManager audioManager = AudioManager.instance;
        audioManager.musicSource.Stop();
        audioManager.moveSource.Stop();

        GameScenesManager gameScenesManager = GameScenesManager.GameScenesManagerInstance;
        gameScenesManager.ResetGame();
        gameScenesManager.LoadGameScene("GameStartScene");
    }
}
