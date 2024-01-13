using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "PlayerController", menuName = "InputController/PlayerController")]
public class PlayerController : InputController
{
    public NewInputSystem newInputSystem;

    private InputAction move;
    private InputAction interact;

    private Vector2 moveDirection;

    private void Awake()
    {
        //newInputSystem = new NewInputSystem();
    }

    private void OnEnable()
    {
        //move = newInputSystem.Player.Move;
        //interact = newInputSystem.Player.Interact;
    }

    private void OnDisable()
    {
        //move.Disable();
        //interact.Disable();
    }

    public override float RetrieveHorizontalMoveInput()
    {
        //moveDirection = move.ReadValue<Vector2>();

        //return moveDirection.x;
        return Input.GetAxisRaw("Horizontal");
    }

    public override float RetrieveVerticalMoveInput()
    {
        //moveDirection = move.ReadValue<Vector2>();

        //return moveDirection.y;
        return Input.GetAxisRaw("Vertical");
    }

    public override bool RetrieveInteractInput()
    {
        //return interact.ReadValue<bool>();
        return Input.GetKey(KeyCode.F);
        //return Input.GetButtonDown("F");
    }
}
