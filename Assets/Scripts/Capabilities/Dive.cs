using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Dive : MonoBehaviour
{
    [SerializeField] private PlayerController input = null;
    [SerializeField, Range(0f, 100f)] private float maxSpeed = 4f;
    [SerializeField, Range(0f, 100f)] private float maxAcceleration = 35f;
    [SerializeField, Range(0f, 1080f)] private float rotationSpeed = 720f;

    private Vector2 direction;
    private Vector2 desiredVelocity;
    private Vector2 velocity;
    private Rigidbody2D body;
    private Spawn spawn;
    private PlayerInput playerInput;
    private BreatheOxygen oxygen;
    private WaterPressureBearing pressure;
    private Animator animator;

    private float maxSpeedChange;
    private float acceleration;
    private string sceneName;
    private bool isDead = false;
    private Quaternion initialRotation;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        spawn = GetComponent<Spawn>();
        playerInput = GetComponent<PlayerInput>();
        oxygen = GetComponent<BreatheOxygen>();
        pressure = GetComponent<WaterPressureBearing>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        initialRotation = transform.localRotation;
    }

    private void Update()
    {
        sceneName = spawn.GetSceneName();
        
        if (sceneName == "Cave")
        {
            if (oxygen.GetIsNoOxygen() || pressure.GetIsOverWaterPressure())    // player dead conditions
            { 
                desiredVelocity = Vector2.zero; 
                body.velocity = Vector2.zero;   //if dont have these two, player will still straight moving, even though we dont press any key
                isDead = true;
                return;
            }

            body.gravityScale = 0f;

            direction.x = input.RetrieveHorizontalMoveInput();
            //direction.x = input.RetrieveHorizontalMoveInput();
            direction.y = input.RetrieveVerticalMoveInput();
            //desiredVelocity = new Vector2(direction.x, direction.y) * MathF.Max(maxSpeed - ground.GetFriction(), 0f);
            bool isMoving = direction.x != 0f || direction.y != 0f;

            if (isMoving)
            {
                desiredVelocity = new Vector2(direction.x, direction.y) * MathF.Max(maxSpeed, 0f);
                AnimationCall.Instance.ChangeAnimationState(AnimationCall.CAVE_DIVE);
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

                AnimationCall.Instance.ChangeAnimationState(AnimationCall.CAVE_IDLE);
            }

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

    public bool GetIsDead()
    {
        return isDead;
    }

    /*
    IEnumerator RotateToInitialRotation()
    {
        Quaternion startRotation = transform.rotation;
        float rotationTime = 1f; // The duration of the rotation in seconds
        float elapsedTime = 0f;

        while (elapsedTime < rotationTime)
        {
            float t = elapsedTime / rotationTime;
            transform.rotation = Quaternion.Slerp(startRotation, initialRotation, t);
            elapsedTime += Time.deltaTime;
            yield return 0.1f;
        }

        transform.rotation = initialRotation;
    }*/
}
