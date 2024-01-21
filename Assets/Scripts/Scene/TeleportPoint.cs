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

    private FadeInOut Fade;

    private void Start()
    {
        Fade = FindAnyObjectByType<FadeInOut>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string sceneName = SceneNameSelection.ToString();

        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameInfo.CurrentSceneName == "Company")
            {
                if (sceneName == "Shop" && GameInfo.CanBuySkill && GameInfo.HasInteracted)
                {
                    StartCoroutine(FadeInLoadScene(sceneName));
                }

                if (sceneName == "Cave")
                {
                    StartCoroutine(FadeInLoadScene(sceneName));
                    GameInfo.CaveLevel += 1;
                }
            }

            else if (GameInfo.CurrentSceneName == "Cave")
            {
                if (sceneName == "Company" && GameInfo.HasMetQuota)
                {
                    StartCoroutine(FadeInLoadScene(sceneName));
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

    private IEnumerator FadeInLoadScene(string sceneName)
    {
        Fade.StartFadeIn();
        yield return new WaitForSeconds(1);
        GameScenesManager.GameScenesManagerInstance.LoadGameScene(sceneName);
    }
}
