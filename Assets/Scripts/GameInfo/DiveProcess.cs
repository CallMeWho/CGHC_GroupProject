using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DiveProcess : MonoBehaviour
{
    [SerializeField] private InputController input = null;

    [Header("End Scene")]
    public GameObject EndSceneObj;

    // player components
    private Rigidbody2D body;
    private GameObject lightObj;

    // scriptable objs
    private GameInfo gameInfo;

    // movement
    private Vector2 direction;
    private Vector2 desiredVelocity;
    private Quaternion initialRotation;

    private void Awake()
    {
        InitializeCaveScene();
        body = GetComponent<Rigidbody2D>();
        lightObj = GetComponentInChildren<Light2D>(true)?.gameObject;
        gameInfo = FindAnyObjectByType<GameScenesManager>().GameInfo;
    }

    private void Start()
    {
        InitializeCaveScene();

        if (gameInfo.CurrentSceneName == "Company")
        {
            body.gravityScale = 1f;
            lightObj.SetActive(false);
        }
        else if (gameInfo.CurrentSceneName == "Cave")
        {
            body.gravityScale = 1f;
            lightObj.SetActive(true);
            initialRotation = transform.localRotation;
        }
        else return;
    }

    private void Update()
    {
        InitializeCaveScene();
        Moving();
    }

    private void FixedUpdate()
    {
        InitializeCaveScene();
        Accelerate();
    }

    // initialization
    private void InitializeCaveScene()
    {
        if (gameInfo == null || gameInfo.CurrentSceneName != "Company" || gameInfo.CurrentSceneName != "Cave" ||
            body == null || lightObj == null || input == null)
            return;
    }

    private void Moving()
    {
        switch (gameInfo.CurrentSceneName)
        {
            case "Company":
                direction.x = input.RetrieveHorizontalMoveInput();
                bool isMovingCompany = direction.x != 0f;

                AudioManager.instance.moveSource.gameObject.SetActive(isMovingCompany); // show/hide move audio source
                AnimationCall.PlayerAnimationInstance.ChangeAnimationState(isMovingCompany ? AnimationCall.LAND_RUN : AnimationCall.LAND_IDLE);

                desiredVelocity = new Vector2(direction.x, 0f).normalized * Mathf.Max(gameInfo.MaxSpeed - gameInfo.GroundFriction, 0f);
                Flipping();
                
                break;

            case "Cave":
                lightObj.SetActive(true);

                direction.x = input.RetrieveHorizontalMoveInput();
                direction.y = input.RetrieveVerticalMoveInput();
                bool isMovingCave = direction.x != 0f || direction.y != 0f;

                AnimationCall.PlayerAnimationInstance.ChangeAnimationState(isMovingCave ? AnimationCall.CAVE_DIVE : AnimationCall.CAVE_IDLE);

                desiredVelocity = isMovingCave ? direction.normalized * Mathf.Max(gameInfo.MaxSpeed, 0f) : Vector2.zero;
                
                if (!isMovingCave)
                {
                    float rotationTime = 1f; // The duration of the rotation in seconds
                    float elapsedTime = 0f;
                    Quaternion startRotation = transform.rotation;
                    Quaternion targetRotation = initialRotation;

                    while (elapsedTime < rotationTime)
                    {
                        float t = elapsedTime / rotationTime;
                        transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
                        elapsedTime += Time.deltaTime;
                    }

                    transform.rotation = targetRotation;
                }
                
                Flipping();
                StopMoving();
                break;

            default:
                return;
        }
    }

    private void StopMoving()
    {
        if (gameInfo.CurrentSceneName == "Cave" && (gameInfo.HasNoOxygen || gameInfo.HasOverWaterPressure))
        {
            //if dont have these two, player will still straight moving, even though we dont press any key
            desiredVelocity = Vector2.zero;
            body.velocity = desiredVelocity;

            // (play die animation)
            EndSceneObj.SetActive(true);
            return;
        }
    }

    private void Flipping()
    {
        switch (gameInfo.CurrentSceneName)
        {
            case "Company":
                if (Math.Abs(direction.x) > 0)
                {
                    // flip horizontally
                    Vector3 newScale = transform.localScale;
                    newScale.x = Mathf.Sign(input.RetrieveHorizontalMoveInput()) * Mathf.Abs(newScale.x);
                    transform.localScale = newScale;
                }
                break;

            case "Cave":
                if (desiredVelocity != Vector2.zero)
                {
                    // rotation moving
                    Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, desiredVelocity);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, gameInfo.RotationSpeed * Time.deltaTime);

                    if (Mathf.Abs(direction.x) > 0f)
                    {
                        Vector3 newScale = transform.localScale;
                        newScale.x = Mathf.Sign(direction.x) * Mathf.Abs(newScale.x);
                        transform.localScale = newScale;
                    }
                }
                break;

            default:
                break;
        }
    }

    private void Accelerate()
    {
        Vector2 velocity = body.velocity;
        float maxSpeedChange = gameInfo.MaxAcceleration * Time.deltaTime;

        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);

        switch (gameInfo.CurrentSceneName)
        {
            case "Company":
                body.velocity = velocity;
                break;

            case "Cave":
                velocity.y = Mathf.MoveTowards(velocity.y, desiredVelocity.y, maxSpeedChange);
                body.velocity = velocity;
                break;

            default:
                return;
        }
    }
}
