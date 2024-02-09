using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    public override bool RetrieveInteractInput()
    {
        return Input.GetKey(KeyCode.J);
    }

    public override bool RetrievePauseInput()
    {
        return Input.GetKey(KeyCode.Escape);
    }
}
