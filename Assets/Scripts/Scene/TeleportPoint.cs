using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
                // Company -> Shop
                if (sceneName == "Shop" && GameInfo.CanBuySkill && GameInfo.HasInteracted)
                {
                    StartCoroutine(FadeInLoadScene(sceneName));
                }

                // Company -> Cave
                if (sceneName == "Cave")
                {
                    StartCoroutine(FadeInLoadScene(sceneName));
                    GameInfo.CaveLevel += 1;
                    AudioManager.instance.PlaySound("CaveBgm", AudioManager.instance.musicSounds, AudioManager.instance.musicSource, false);
                }
            }

            else if (GameInfo.CurrentSceneName == "Cave")
            {
                // Cave -> Company
                if (sceneName == "Company" && GameInfo.HasMetQuota)
                {
                    StartCoroutine(FadeInLoadScene(sceneName));
                    AudioManager.instance.PlaySound("CommonBgm", AudioManager.instance.musicSounds, AudioManager.instance.musicSource, false);
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

        // these lines below not working
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = transform.position;
    }
}
