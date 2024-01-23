using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveSourceControl : MonoBehaviour
{
    [Header("Data Keeper")]
    [SerializeField] public GameInfo GameInfo;

    private void Start()
    {
        if (GameInfo.CurrentSceneName == "Company")
        {
            AudioManager.instance.moveSource.gameObject.SetActive(true);
            AudioManager.instance.PlaySound("WalkSound", AudioManager.instance.sfxSounds, AudioManager.instance.moveSource, false);
        }

        else if (GameInfo.CurrentSceneName == "Cave")
        {
            AudioManager.instance.moveSource.gameObject.SetActive(true);
            //AudioManager.instance.PlayMoveSound("BreatheSound");
        }
    }
}
