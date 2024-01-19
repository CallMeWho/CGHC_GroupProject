using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportPoint : MonoBehaviour
{
    [SerializeField] private SceneSelection SceneNameSelection;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string sceneName = SceneNameSelection.ToString();

        if (collision.gameObject.CompareTag("Player"))
        {
            GameScenesManager.GameScenesManagerInstance.LoadGameScene(sceneName);
        }
    }
}
