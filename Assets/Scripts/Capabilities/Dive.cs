using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dive : MonoBehaviour
{
    [SerializeField] private InputController input = null;
    [SerializeField, Range(0f, 100f)] private float maxSpeed = 4f;
    [SerializeField, Range(0f, 100f)] private float maxAcceleration = 35f;
    [SerializeField, Range(0f, 1080f)] private float rotationSpeed = 720f;

    private Vector2 direction;
    private Vector2 desiredVelocity;
    private Vector2 velocity;
    private Rigidbody2D body;
    private Spawn spawn;

    private float maxSpeedChange;
    private float acceleration;
    private string sceneName;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        spawn = GetComponent<Spawn>();
    }

    private void Update()
    {
        sceneName = spawn.GetSceneName();
        
        if (sceneName == "Cave")
        {
            body.gravityScale = 0f;

            direction.x = input.RetrieveHorizontalMoveInput();
            direction.y = input.RetrieveVerticalMoveInput();
            //desiredVelocity = new Vector2(direction.x, direction.y) * MathF.Max(maxSpeed - ground.GetFriction(), 0f);
            desiredVelocity = new Vector2(direction.x, direction.y) * MathF.Max(maxSpeed, 0f);

            // flipping
            if (desiredVelocity != Vector2.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, desiredVelocity);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

                if (Mathf.Abs(direction.x) > 0)
                {
                    // Flip the sprite horizontally
                    Vector3 newScale = transform.localScale;
                    newScale.x = Mathf.Sign(direction.x) * Mathf.Abs(newScale.x);
                    transform.localScale = newScale;
                }
            }
        }
        else { return; }
    }

    private void FixedUpdate()
    {
        if (sceneName == "Cave")
        {
            //onGround = ground.GetOnGround();
            velocity = body.velocity;

            //acceleration = onGround ? maxAcceleration : 0;
            acceleration = maxAcceleration;
            maxSpeedChange = acceleration * Time.deltaTime;
            velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
            velocity.y = Mathf.MoveTowards(velocity.y, desiredVelocity.y, maxSpeedChange);

            body.velocity = velocity;
        }
        else { return; }
    }
}