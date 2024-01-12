using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] private InputController input = null;
    [SerializeField, Range(0f, 100f)] private float maxSpeed = 4f;
    [SerializeField, Range(0f, 100f)] private float maxAcceleration = 35f;

    private Vector2 direction;
    private Vector2 desiredVelocity;
    private Vector2 velocity;
    private Rigidbody2D body;
    private Ground ground;

    private float maxSpeedChange;
    private float acceleration;
    private bool onGround;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        ground = GetComponent<Ground>();
    }

    private void Update()
    {
        direction.x = input.RetrieveHorizontalMoveInput();
        desiredVelocity = new Vector2(direction.x, 0f) * MathF.Max(maxSpeed - ground.GetFriction(), 0f);

        // flipping
        if (Math.Abs(direction.x) > 0)
        {
            Vector3 newScale = transform.localScale;
            newScale.x = Mathf.Sign(input.RetrieveHorizontalMoveInput()) * Mathf.Abs(newScale.x);
            transform.localScale = newScale;
        }
    }

    private void FixedUpdate()
    {
        onGround = ground.GetOnGround();
        velocity = body.velocity;

        acceleration = onGround? maxAcceleration : 0;
        maxSpeedChange = acceleration * Time.deltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);

        body.velocity = velocity;
    }
}
