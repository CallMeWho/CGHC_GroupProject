using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConditions : MonoBehaviour
{
    // Variables to track collision conditions
    public bool IsCollidingBelow { get; set; } // Is the player colliding with something below?
    public bool IsCollidingAbove { get; set; } // Is the player colliding with something above?
    public bool IsCollidingRight { get; set; } // Is the player colliding with something on the right?
    public bool IsCollidingLeft { get; set; } // Is the player colliding with something on the left?

    // Variables to track player movement conditions
    public bool IsFalling { get; set; } // Is the player currently falling?
    public bool IsWallClinging { get; set; } // Is the player currently clinging to a wall?
    public bool IsJetpacking { get; set; } // Is the player currently using a jetpack?
    public bool IsJumping { get; set; } // Is the player currently jumping?

    // Reset all the conditions to their initial state
    public void Reset()
    {
        IsCollidingBelow = false;
        IsCollidingLeft = false;
        IsCollidingRight = false;
        IsCollidingAbove = false;

        IsFalling = false;
    }
}
