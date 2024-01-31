using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCall : MonoBehaviour
{
    public static AnimationCall PlayerAnimationInstance;

    public Animator Animator { get; private set; }
    public string CurrentState { get; private set; }

    private void Awake()
    {
        Animator = GetComponent<Animator>();

        if (PlayerAnimationInstance == null)
        {
            PlayerAnimationInstance = this;
        }
        else
        {
            Debug.LogWarning("Multiple instances of AnimationCall found. Destroying the duplicate.");
            Destroy(gameObject);
            return;
        }
    }

    // Animation states
    public const string LAND_IDLE = "RunIdle";
    public const string LAND_RUN = "Run";
    public const string CAVE_IDLE = "DiveIdle";
    public const string CAVE_DIVE = "Dive";

    // Change the animation state
    public void ChangeAnimationState(string newState)
    {
        try
        {
            if (CurrentState == newState) return;

            Animator.Play(newState);
            CurrentState = newState;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to change animation state to {newState}. Error: {e.Message}");
        }
    }
}
