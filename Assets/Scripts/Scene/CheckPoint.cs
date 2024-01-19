using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private SceneSelection SceneNameSelection;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string sceneName = SceneNameSelection.ToString();
        SceneElement[] sceneArray = GameScenesManager.GameScenesManagerInstance.SceneArray;

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("checked player");
            //GameScenesManager.GameScenesManagerInstance.LoadGameScene(sceneName);
            SceneManager.LoadScene(sceneName);
        }
    }
}
