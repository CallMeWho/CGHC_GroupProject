using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Animations;
using UnityEngine;

public class AnimationCall : MonoBehaviour
{
    public static AnimationCall PlayerAnimationInstance;
    public Animator Animator;
    public string CurrentState;

    private void Awake()
    {
        Animator = GetComponent<Animator>();

        if (PlayerAnimationInstance == null)
        {
            PlayerAnimationInstance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //animation states
    public const string LAND_IDLE = "Running Idle";
    public const string LAND_RUN = "Running";
    public const string CAVE_IDLE = "DiveIdle";
    public const string CAVE_DIVE = "Dive";

    public void ChangeAnimationState(string newState)
    {
        if (CurrentState != newState)
        {
            Animator.Play(newState);
            CurrentState = newState;
        }
        else { return; }
    }
}
