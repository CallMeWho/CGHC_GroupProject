using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MoveProcess : MonoBehaviour
{
    [SerializeField] private InputController input = null;
    [SerializeField] public Rigidbody2D Body;
    [SerializeField] private GameObject Light;

    [Header("Data Keeper")]
    [SerializeField] public GameInfo GameInfo;

    private Vector2 direction;
    private Vector2 desiredVelocity;
    private Vector2 velocity;
    private float maxSpeedChange;
    private float acceleration;

    private void Awake()
    {
        if (GameInfo.CurrentSceneName != "Company")
        {
            return;
        }

        Body = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (Body == null)
        {
            Debug.Log("no rigidbody");
            return;
        }

        if (Body.gravityScale <= 0f)
        {
            Body.gravityScale = 1f;
        }
    }

    private void Update()
    {
        if (GameInfo.CurrentSceneName != "Company")
        {
            return;
        }

        PlayerMove();
        Light.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (GameInfo.CurrentSceneName != "Company")
        {
            return;
        }

        Accelerate();
    }

    private void PlayerMove()   //naming might be incorrect
    {
        direction.x = input.RetrieveHorizontalMoveInput();

        bool isMoving = direction.x != 0f;

        if (isMoving)
        {
            desiredVelocity = new Vector2(direction.x, 0f) * MathF.Max(GameInfo.MaxSpeed - GameInfo.GroundFriction, 0f);
            AnimationCall.PlayerAnimationInstance.ChangeAnimationState(AnimationCall.LAND_RUN);
            Flipping();
        }
        else
        {
            desiredVelocity = Vector2.zero;
            AnimationCall.PlayerAnimationInstance.ChangeAnimationState(AnimationCall.LAND_IDLE);
        }
    }

    private void Flipping()
    {
        if (Math.Abs(direction.x) > 0)
        {
            Vector3 newScale = transform.localScale;
            newScale.x = Mathf.Sign(input.RetrieveHorizontalMoveInput()) * Mathf.Abs(newScale.x);
            transform.localScale = newScale;
        }
    }

    private void Accelerate()   //this one attached then will just start moving
    {
        velocity = Body.velocity;
        //acceleration = GameInfo.IsOnGround ? GameInfo.MaxAcceleration : 0;  //got issue if collide with tile horizontal side of other tilemap
        acceleration = GameInfo.MaxAcceleration;

        maxSpeedChange = acceleration * Time.deltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);

        Body.velocity = velocity;
    }
}
