using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiveProcess : MonoBehaviour
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
    private Quaternion initialRotation;

    private void Awake()
    {
        if (GameInfo.CurrentSceneName != "Cave")
        {
            return;
        }

        Body = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Body.gravityScale = 1f;
        initialRotation = transform.localRotation;
    }

    private void Update()
    {
        if (GameInfo.CurrentSceneName != "Cave")
        {
            return;
        }

        Diving();
    }

    private void FixedUpdate()
    {
        if (GameInfo.CurrentSceneName != "Cave")
        {
            return;
        }

        Accelerate();
    }

    private void Diving()
    {
        direction.x = input.RetrieveHorizontalMoveInput();
        direction.y = input.RetrieveVerticalMoveInput();

        //desiredVelocity = new Vector2(direction.x, direction.y) * MathF.Max(maxSpeed - ground.GetFriction(), 0f);
        bool isMoving = direction.x != 0f || direction.y != 0f;

        if (isMoving)
        {
            desiredVelocity = new Vector2(direction.x, direction.y) * MathF.Max(GameInfo.MaxSpeed, 0f);
            AnimationCall.PlayerAnimationInstance.ChangeAnimationState(AnimationCall.CAVE_DIVE);
        }
        else
        {
            desiredVelocity = Vector2.zero;
            //StartCoroutine(RotateToInitialRotation());


            //STILL NEED TO MODIFY
            Quaternion startRotation = transform.rotation;
            float rotationTime = 1f; // The duration of the rotation in seconds
            float elapsedTime = 0f;

            while (elapsedTime < rotationTime)
            {
                float t = elapsedTime / rotationTime;
                transform.rotation = Quaternion.Slerp(startRotation, initialRotation, t);
                elapsedTime += Time.deltaTime;

            }

            transform.rotation = initialRotation;

            AnimationCall.PlayerAnimationInstance.ChangeAnimationState(AnimationCall.CAVE_IDLE);
        }

        Flipping();
        StopMoving();
    }

    private void StopMoving()
    {
        if (GameInfo.HasNoOxygen || GameInfo.HasOverWaterPressure)    
        {
            desiredVelocity = Vector2.zero;
            Body.velocity = Vector2.zero;   //if dont have these two, player will still straight moving, even though we dont press any key
            return;
        }
    }

    private void Flipping()
    {
        if (desiredVelocity != Vector2.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, desiredVelocity);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, GameInfo.RotationSpeed * Time.deltaTime);

            if (Mathf.Abs(direction.x) > 0)
            {
                // Flip the sprite horizontally
                Vector3 newScale = transform.localScale;
                newScale.x = Mathf.Sign(direction.x) * Mathf.Abs(newScale.x);
                transform.localScale = newScale;
            }
        }
    }

    private void Accelerate()
    {
        velocity = Body.velocity;

        //acceleration = onGround ? maxAcceleration : 0;
        acceleration = GameInfo.MaxAcceleration;
        maxSpeedChange = acceleration * Time.deltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.y = Mathf.MoveTowards(velocity.y, desiredVelocity.y, maxSpeedChange);

        Body.velocity = velocity;
    }
}
