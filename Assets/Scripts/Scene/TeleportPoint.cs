using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportPoint : MonoBehaviour
{
    [SerializeField] private SceneSelection SceneNameSelection;

    [Header("Data Keeper")]
    [SerializeField] public GameInfo GameInfo;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string sceneName = SceneNameSelection.ToString();

        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameInfo.CurrentSceneName == "Cave" && GameInfo.HasMetQuota ||
                GameInfo.CurrentSceneName == "Company")
            {
                GameScenesManager.GameScenesManagerInstance.LoadGameScene(sceneName);
            }
            else
            {
                // player will be pushed back
                Vector2 pushDirection = (collision.transform.position - transform.position).normalized;
                Vector2 pushVector = pushDirection * 2f;
                collision.transform.position += new Vector3(pushVector.x, pushVector.y, 0f);
            }
        }
    }
}
