using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndSceneSettings : MonoBehaviour
{
    private GameObject player;
    private GameInfo gameInfo;

    private void Start()
    {
        gameInfo = FindAnyObjectByType<GameScenesManager>().GameInfo;
        player = GameObject.FindWithTag("Player");
        if (gameInfo == null || player == null) return;

        // hide dead screen
        gameObject.SetActive(true);

        // set sorting layer
        int playerSortLayer = player.GetComponent<Renderer>().sortingOrder;
        gameObject.GetComponent<Canvas>().sortingOrder = playerSortLayer + 1;   // end scene is set at front player
    }

    // update not working in canvas here, dk why

    public void BackToMenu()
    {
        AudioManager.instance.moveSource.Stop();
        GameScenesManager.GameScenesManagerInstance.LoadGameScene("GameStartScene");
    }
}
