using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerController", menuName = "InputController/PlayerController")]
public class PlayerController : InputController
{
    public override float RetrieveHorizontalMoveInput()
    {
        return Input.GetAxisRaw("Horizontal");
    }

    public override float RetrieveVerticalMoveInput()
    {
        return Input.GetAxisRaw("Vertical");
    }
}
