using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveSourceControl : MonoBehaviour
{
    [SerializeField] private SceneSelection SceneNameSelection;

    [Header("Data Keeper")]
    [SerializeField] public GameInfo GameInfo;

    private void Start()
    {
        if (SceneNameSelection.ToString() == "Company")
        {
            CompanyMoveSourceControl();
        }

        if (SceneNameSelection.ToString() == "Cave")
        {
            CaveMoveSourceControl();
        }
    }

    private void Update()
    {
        
    }

    private void CompanyMoveSourceControl()
    {
        AudioManager.instance.PlaySound("WalkSound", AudioManager.instance.sfxSounds, AudioManager.instance.moveSource, false);
        AudioManager.instance.moveSource.gameObject.SetActive(false);
    }

    private void CaveMoveSourceControl()
    {
        AudioManager.instance.moveSource.gameObject.SetActive(true);
        AudioManager.instance.PlaySound("BreatheSound", AudioManager.instance.sfxSounds, AudioManager.instance.moveSource, false);
    }
}
