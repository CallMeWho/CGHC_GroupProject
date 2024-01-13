using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Mathematics;
using UnityEngine;

public class DiverMovement : MonoBehaviour
{
    [SerializeField] public GameObject SpawnPoint;
    [SerializeField] public float RotationSpeed;

    private DiverConditions m_diverConditions; 
    private Rigidbody2D m_rb;
    private BoxCollider2D m_boxCollider;
    
    private List<Vector2> m_rayPointsList = new List<Vector2>();

    // input vars
    private float horzInput;
    private float vertInput;

    // gravity vars
    [SerializeField] public float gravityModifier;
    protected Vector2 downfallVlc;  //Vlc = velocity
    private Vector2 displacement;
    private Vector2 vertMove;

    // movement vars
    private Vector2 moveDirection;

    private void OnEnable()
    {
        // get components
        m_diverConditions = GetComponent<DiverConditions>();
        m_rb = GetComponent<Rigidbody2D>();
        m_boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        // settings
        m_diverConditions.currentState = DiverState.Walking;

        m_rb.bodyType = RigidbodyType2D.Kinematic;
        m_rb.useFullKinematicContacts = true;
        
        // set spawn point
        Vector2 spawnPoint = SpawnPoint.transform.position;
        transform.position = spawnPoint;
    }

    private void Update()
    {
        GetInput();

        switch (m_diverConditions.currentState)
        {
            case DiverState.Walking:
                HandleWalking();
                break;

            case DiverState.Diving:
                HandleDiving();
                break;

            case DiverState.Dead:
                break;
        }
    }

    private void FixedUpdate()
    {
        Gravity();
    }

    private void GetInput()
    {
        horzInput = Input.GetAxis("Horizontal");
        vertInput = Input.GetAxis("Vertical");

        moveDirection = new Vector2(horzInput, vertInput);
    }

    private void Gravity()
    {
        downfallVlc += Physics2D.gravity * gravityModifier * Time.deltaTime;

        vertMove = Vector2.up * (downfallVlc.y * Time.deltaTime);

        m_rb.position = m_rb.position + vertMove;
    }

    private void HandleWalking()
    {
        m_rb.mass = 1.0f;
        m_rb.freezeRotation = true;

        if (m_diverConditions.CurrentMoveSpeed <= 0)
        {
            m_diverConditions.CurrentMoveSpeed = 10;
        }

        // moving
        float moveSpeed = m_diverConditions.CurrentMoveSpeed;
        //Vector2 displacement = new Vector2(horzInput, 0) * moveSpeed * Time.deltaTime;
        //transform.Translate(displacement);

        float inputMagnitude = Mathf.Clamp01(moveDirection.magnitude);
        moveDirection.Normalize();
        Vector2 displacement = moveDirection * moveSpeed * inputMagnitude * Time.deltaTime;
        m_rb.MovePosition(transform.position + (Vector3)displacement);

        // flipping horizontally
        if (Mathf.Abs(horzInput) > 0)
        {
            Vector3 newScale = transform.localScale;
            newScale.x = Mathf.Sign(horzInput) * Mathf.Abs(newScale.x);
            transform.localScale = newScale;
        }
    }

    private void HandleDiving()
    {
        m_rb.gravityScale = 0f;
        m_rb.freezeRotation = true;

        if (m_diverConditions.CurrentMoveSpeed <= 0)
        {
            m_diverConditions.CurrentMoveSpeed = 10;
        }

        Vector2 moveDirection = new Vector2(horzInput, vertInput);

        float inputMagnitude = Mathf.Clamp01(moveDirection.magnitude);
        moveDirection.Normalize();

        float moveSpeed = m_diverConditions.CurrentMoveSpeed;

        Vector2 displacement = moveDirection * moveSpeed * inputMagnitude * Time.deltaTime;
        m_rb.MovePosition(transform.position + (Vector3)displacement);
        
        //transform.Translate(displacement, Space.World);

        if (moveDirection != Vector2.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, RotationSpeed * Time.deltaTime);

            if (Mathf.Abs(horzInput) > 0)
            {
                // Flip the sprite horizontally
                Vector3 newScale = transform.localScale;
                newScale.x = Mathf.Sign(horzInput) * Mathf.Abs(newScale.x);
                transform.localScale = newScale;
            }
        }

        

        //botleft
        //Vector2 raycastOrigin = transform.position + transform.TransformVector(m_boxCollider.offset) - 
        //   transform.TransformVector(m_boxCollider.size) * 0.5f;

        //topleft
        Vector2 raycastOriginTL = transform.position + transform.TransformVector(m_boxCollider.offset) + 
            transform.TransformVector(new Vector2(-m_boxCollider.size.x, m_boxCollider.size.y)) * 0.5f;

        //topright
        Vector2 raycastOriginTR = transform.position + transform.TransformVector(m_boxCollider.offset) + 
            transform.TransformVector(new Vector2(m_boxCollider.size.x, m_boxCollider.size.y)) * 0.5f;

        RaycastHit2D hit = Physics2D.Raycast(
            raycastOriginTL, transform.up, 10f, LayerMask.GetMask("Map"));
        RaycastHit2D hit2 = Physics2D.Raycast(
            raycastOriginTR, transform.up, 10f, LayerMask.GetMask("Map"));
        Debug.DrawRay(raycastOriginTL, transform.up, UnityEngine.Color.green);
        Debug.DrawRay(raycastOriginTR, transform.up, UnityEngine.Color.green);

        if (hit)
        {
            Debug.Log("hit wall");
            m_rb.velocity = Vector2.zero;
        }
    }

    private void ShowRays()
    {
        m_rayPointsList.Clear();

        Vector2 rayPoint_TopRight = new Vector2(m_boxCollider.bounds.max.x, m_boxCollider.bounds.max.y);
        Vector2 rayPoint_BotRight = new Vector2(m_boxCollider.bounds.max.x, m_boxCollider.bounds.min.y);

        m_rayPointsList.Add(rayPoint_TopRight);
        m_rayPointsList.Add(rayPoint_BotRight);

        foreach (Vector2 rayPoint in m_rayPointsList)
        {
            Vector2 rotatedDirection = transform.rotation * Vector2.right;

            Debug.DrawRay(rayPoint, rotatedDirection, UnityEngine.Color.green);
        }
    }

    private void Raycasting(Vector2 displacement, float inputMagnitude)
    {
        
    }
}