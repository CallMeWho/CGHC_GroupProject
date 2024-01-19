using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveProcess : MonoBehaviour
{
    [SerializeField] private InputController input = null;
    [SerializeField] public Rigidbody2D Body;

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
        PlayerMove();
    }

    private void FixedUpdate()
    {
        Accelerate();
    }

    private void PlayerMove()
    {
        direction.x = input.RetrieveHorizontalMoveInput();
        desiredVelocity = new Vector2(direction.x, 0f) * MathF.Max(GameInfo.MaxSpeed - GameInfo.GroundFriction, 0f);

        Flipping();
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

    private void Accelerate()
    {
        velocity = Body.velocity;
        acceleration = GameInfo.IsOnGround ? GameInfo.MaxAcceleration : 0;

        maxSpeedChange = acceleration * Time.deltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);

        Body.velocity = velocity;
    }
}
