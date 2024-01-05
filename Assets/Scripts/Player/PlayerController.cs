using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float fallMultiplier = 1.5f;

    [Header("Collisions")]
    [SerializeField] private LayerMask collideWith;
    [SerializeField] private int verticalRayAmount = 4;
    [SerializeField] private int horizontalRayAmount = 4;


    #region Properties

    public PlayerConditions Conditions => _conditions;  //Conditions: get _conditions all stuff in read-only
    public float Gravity => gravity;    // Gravity: get gravity read-only value
    public Vector2 Force => _force;     //Force: get _force read-only value
    public bool IsFacingRight { get; set; }   //if player currently facing right, get bool value

    #endregion


    #region Internal

    private BoxCollider2D _boxCollider2D;   //player box collider
    private PlayerConditions _conditions;   //player condtions

    private Vector2 _boundsTopLeft;         //box collider's top left point
    private Vector2 _boundsTopRight;        //... top right point
    private Vector2 _boundsBottomLeft;      //... bot left point
    private Vector2 _boundsBottomRight;     //... bot right point

    private float _boundsWidth;             //box collider's width
    private float _boundsHeight;            //... height

    private float _currentGravity;          //player current gravity
    private Vector2 _force;                 //player current force
    private Vector2 _movePosition;          //player current position

    private float _skin = 0.05f;            // ray cast line will longer than collider half length a bit, that's how _skin work here
    private float _internalFaceDirection = 1f;  //player initial face direction = right, as default face direction, and will be updated 
    private float _faceDirection;           //player current face direction

    #endregion


    private void Start()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _conditions = new PlayerConditions();   //_conditions: newly created player conditions
        _conditions.Reset();
    }

    private void Update()
    {
        ApplyGravity();
        StartMovement();
        SetRayOrigins();

        GetFaceDirection();
        if (IsFacingRight)
        {
            HorizontalCollision(1);
        }
        else
        {
            HorizontalCollision(-1);
        }

        CollisionBelow();
        CollisionAbove();
        transform.Translate(_movePosition, Space.Self);
        SetRayOrigins();
        CalculateMovement();
    }


    #region Collision

    #region Collision Below
    private void CollisionBelow()
    {
        // STEP 1 : CHECK PLAYER FALLING CONDITION

        if (_movePosition.y < -0.0001f) //if player is falling
        {
            _conditions.IsFalling = true;
        }
        else  //if player not falling
        {
            _conditions.IsFalling = false;
            _conditions.IsCollidingBelow = false;   //no collision happening now
            return;
        }

        // STEP 2: GET RAY LENGTH

        float rayLength = _boundsHeight / 2f + _skin;
        //ray length will now be longer a bit than the collider half height now because of _skin

        if (_movePosition.y < 0)
        {
            rayLength += Mathf.Abs(_movePosition.y);    //make rayLength will always be positive
        }

        // STEP 3: GET STARTING POINTS FOR SHOOTING RAY

        Vector2 leftOrigin = (_boundsBottomLeft + _boundsTopLeft) / 2f;     //left side mid point position = also as left ray point
        Vector2 rightOrigin = (_boundsBottomRight + _boundsTopRight) / 2f;  

        leftOrigin += (Vector2)(transform.up * _skin) + (Vector2)(transform.right * _movePosition.x);
        rightOrigin += (Vector2)(transform.up * _skin) + (Vector2)(transform.right * _movePosition.x);
        /* EXPLANATION
         * as player moving, his collider follows as well, 
         * so we have to let its raycast points not always in fix position but follow the collider,
         * 
         * (Vector2)(transform.up * _skin)              : is the vertical offset, or displacement,
         * (Vector2)(transform.right * _movePosition.x) : is the horizontal offset
         * 
         * when we add these 2 offset to the ray points, thn they become dynamic
         */

        // STEP 4: START SHOOTING RAY

        for (int i = 0; i < verticalRayAmount; i++)
        {
            float fraction = (float) i / (float) (verticalRayAmount - 1);
            Vector2 rayOrigin = Vector2.Lerp(leftOrigin, rightOrigin, fraction);    //get the ray point

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -transform.up, rayLength, collideWith); //start shooting ray from the point
            Debug.DrawRay(rayOrigin, -transform.up * rayLength, Color.green);

            if (hit)
            {
                if (_force.y > 0)   //if move upwards, like jumping
                {
                    _movePosition.y = _force.y * Time.deltaTime;
                    _conditions.IsCollidingBelow = false;   //no collision happening below
                }
                else // if standing (_force.y = 0) or move downwards
                {
                    _movePosition.y = -hit.distance + _boundsHeight / 2f + _skin;
                }

                _conditions.IsCollidingBelow = true;
                _conditions.IsFalling = false;

                if (Mathf.Abs(_movePosition.y) < 0.0001f)
                {
                    _movePosition.y = 0f;
                }
            }
        }
    }
    #endregion

    #region Collision Horizontal
    private void HorizontalCollision(int direction)
    {
        Vector2 rayHorizontalBottom = (_boundsBottomLeft + _boundsBottomRight) / 2f;
        Vector2 rayHorizontalTop = (_boundsTopLeft + _boundsTopRight) / 2f;
        rayHorizontalBottom += (Vector2)transform.up * _skin;
        rayHorizontalTop -= (Vector2)transform.up * _skin;
        float rayLenght = Mathf.Abs(_force.x * Time.deltaTime) + _boundsWidth / 2f + _skin * 2f;
        for (int i = 0; i < horizontalRayAmount; i++)
        {
            Vector2 rayOrigin = Vector2.Lerp(rayHorizontalBottom, rayHorizontalTop, (float)i / (horizontalRayAmount - 1));
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction * transform.right, rayLenght, collideWith);
            Debug.DrawRay(rayOrigin, transform.right * rayLenght * direction, Color.cyan);
            if (hit)
            {
                if (direction >= 0)
                {
                    _movePosition.x = hit.distance - _boundsWidth / 2f - _skin * 2f;
                    _conditions.IsCollidingRight = true;
                }
                else
                {
                    _movePosition.x = -hit.distance + _boundsWidth / 2f + _skin * 2f;
                    _conditions.IsCollidingLeft = true;
                }
                _force.x = 0f;
            }
        }
    }
    #endregion

    #region Collision Above
    private void CollisionAbove()
    {
        if (_movePosition.y < 0)
        {
            return;
        }
        // Set rayLenght
        float rayLenght = _movePosition.y + _boundsHeight / 2f;

        // Origin Points
        Vector2 rayTopLeft = (_boundsBottomLeft + _boundsTopLeft) / 2f;
        Vector2 rayTopRight = (_boundsBottomRight + _boundsTopRight) / 2f;
        rayTopLeft += (Vector2)transform.right * _movePosition.x;
        rayTopRight += (Vector2)transform.right * _movePosition.x;
        for (int i = 0; i < verticalRayAmount; i++)
        {
            Vector2 rayOrigin = Vector2.Lerp(rayTopLeft, rayTopRight, (float)i / (float)(verticalRayAmount - 1));
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, transform.up, rayLenght, collideWith);
            Debug.DrawRay(rayOrigin, transform.up * rayLenght, Color.red);
            if (hit)
            {
                _movePosition.y = hit.distance - _boundsHeight / 2f;
                _conditions.IsCollidingAbove = true;
            }
        }
    }
    #endregion
    #endregion

    #region Movement
    // Clamp our force applied
    private void CalculateMovement()
    {
        if (Time.deltaTime > 0)
        {
            _force = _movePosition / Time.deltaTime;
        }
    }
    // Initialize the movePosition
    private void StartMovement()
    {
        _movePosition = _force * Time.deltaTime;
        _conditions.Reset();
    }
    // Sets our new x movement
    public void SetHorizontalForce(float xForce)
    {
        _force.x = xForce;
    }
    public void SetVerticalForce(float yForce)
    {
        _force.y = yForce;
    }
    // Calculate the gravity to apply
    private void ApplyGravity()
    {
        _currentGravity = gravity;
        if (_force.y < 0)
        {
            _currentGravity *= fallMultiplier;
        }

        _force.y += _currentGravity * Time.deltaTime;
    }
    #endregion

    #region Direction
    private void GetFaceDirection()
    {
        _faceDirection = _internalFaceDirection;

        IsFacingRight = (_faceDirection == 1);
        /* EXPLANATION
         * _faceDirection = 1, means player currently facing to the right, 
         * then, IsFacingRight = true
         */

        if (_force.x > 0.0001f)  //move to the right
        {
            _faceDirection = 1f;    //face to the right
            IsFacingRight = true;
        }
        else if (_force.x < -0.0001f)
        {
            _faceDirection = -1f;
            IsFacingRight = false;
        }

        _internalFaceDirection = _faceDirection;    
        //_internalFaceDirection: will now be updated in either -1f or 1f (means facing left or right)
    }

    #endregion

    #region Ray Origins
    // Calculate ray based on our collider
    private void SetRayOrigins()
    {
        Bounds playerBounds = _boxCollider2D.bounds;

        _boundsBottomLeft = new Vector2(playerBounds.min.x, playerBounds.min.y);
        _boundsBottomRight = new Vector2(playerBounds.max.x, playerBounds.min.y);
        _boundsTopLeft = new Vector2(playerBounds.min.x, playerBounds.max.y);
        _boundsTopRight = new Vector2(playerBounds.max.x, playerBounds.max.y);
        _boundsHeight = Vector2.Distance(_boundsBottomLeft, _boundsTopLeft);
        _boundsWidth = Vector2.Distance(_boundsBottomLeft, _boundsBottomRight);
    }
    #endregion
}

